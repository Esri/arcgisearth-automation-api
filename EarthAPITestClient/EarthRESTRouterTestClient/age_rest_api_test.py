import requests
import unittest
import json
import time
import os

base_address = "http://localhost:80/Temporary_Listen_Addresses/arcgisearth/api/v1"
camera_info = "{ \"mapPoint\": { \"x\": 113.59647525051167, \"y\": 32.464715999412107, \"z\": 2213290.0751730204, \"spatialReference\": { \"wkid\": 4326 } }, \"heading\": 354.04823651174161, \"pitch\": 19.96239543740441}"
fly_to_info = "{\"camera\":{\"mapPoint\":{\"x\":-92,\"y\":41,\"z\":11000000,\"spatialReference\":{\"wkid\":4326}},\"heading\":0.0,\"pitch\":0.099999999996554886},\"duration\":2}"
add_layer_info = "{\"type\":\"MapService\",\"URI\":\"https://services.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer\",\"target\":\"OperationalLayers\"}"


class AgeRestApiTest(unittest.TestCase):
    def test_get_camera_info(self):
        url = base_address + "/camera"
        r = requests.get(url)
        self.assertEqual(r.status_code, 200)

    def test_set_camera_info(self):
        url = base_address + "/camera/" + camera_info
        r = requests.put(url)
        self.assertEqual(r.status_code, 200)
        if r.status_code == 200:
            content = r.content
            str_camera = content.decode('utf-8')
            json_camera = json.loads(str_camera)
            result = json_camera['SetCameraResult']
            self.assertEqual('Success', result)

    def test_fly_to(self):
        url = base_address + "/flyto/" + fly_to_info
        r = requests.put(url)
        self.assertEqual(r.status_code, 200)
        if r.status_code == 200:
            content = r.content
            str_camera = content.decode('utf-8')
            json_camera = json.loads(str_camera)
            result = json_camera['FlyToResult']
            self.assertEqual('Success', result)

    def test_layer_operation(self):
        # add layer test
        url = base_address + '/layer'
        data = add_layer_info
        headers = {"content-Type": "application/json"}
        r = requests.post(url, data=data, headers=headers)
        self.assertEqual(r.status_code, 200)
        layer_id = None
        if r.status_code == 200:
            content = r.content.decode('utf-8')
            print(content)
            results = eval(content)
            print(results)
            results = json.loads(results)
            layer_id = results["id"]

        time.sleep(2)

        # get layer information test
        if layer_id is not None:
            url = base_address + "/layer/" + layer_id + "/load_status"
            r = requests.get(url)
            self.assertEqual(r.status_code, 200)
            if r.status_code == 200:
                content = r.content.decode('utf-8')
                results = eval(content)
                print(results)
                results = json.loads(results)
                print(results)

        # remove layer test
        if layer_id is not None:
            url = base_address + "/layer/" + layer_id
            r = requests.delete(url)
            self.assertEqual(r.status_code, 200)

    def test_workspace_operation(self):
        # get layers
        url = base_address + "/workspace"
        r = requests.get(url)
        layers_json = None
        content = None
        self.assertEqual(r.status_code, 200)
        if r.status_code == 200:
            content = r.content.decode('utf-8')
            print(content)
            layers_json = eval(content)
            print(layers_json)
            layers_json = json.loads(layers_json)
            print(layers_json)

        # remove layers
        url = base_address + "/layers" + "/AllLayers"
        r = requests.delete(url)
        self.assertEqual(r.status_code, 200)

        # import layers
        from requests import Session
        s = Session()
        url = base_address + "/workspace"
        data = json.dumps(layers_json)
        r = requests.put(url, data=data, stream=True)
        self.assertEqual(r.status_code, 200)

    def test_screensnap(self):
        url = base_address + "/snapshot"
        r = requests.get(url, stream=True)
        self.assertEqual(r.status_code, 200)
        if r.status_code == 200:
            path = "./snaps.jpg"
            if os.path.exists(path):
                os.remove(path)
            with open(path, 'wb') as f:
                for chunk in r:
                    f.write(chunk)
            self.assertTrue(os.path.exists(path))


if __name__ == '__main__':
    unittest.main(verbosity=2)





