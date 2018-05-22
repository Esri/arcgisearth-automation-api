import requests
import json
import os
import unittest
import hmac
import hashlib
import base64
import json

# API Example

# Get camera
# http://localhost:80/Temporary_Listen_Addresses/arcgisearth/api/v1/camera

# ScreenSnap
# http://localhost:80/Temporary_Listen_Addresses/arcgisearth/api/v1/snapshot

# Get all operational layer
# http://localhost:80/Temporary_Listen_Addresses/arcgisearth/api/v1/operationallayers

# Set camera info (PUT)
# http://localhost:80/Temporary_Listen_Addresses/arcgisearth/api/v1/camera/{ "mapPoint": { "x": 116, "y": 39, "z": 11000000, "spatialReference": { "wkid": 4326 } }, "heading": 0.0, "pitch": 0.099999999996554886}

# Delete all operational layer (DEL)
# http://localhost:80/Temporary_Listen_Addresses/arcgisearth/api/v1/operationallayers

# Add layer (POST)
# http://localhost:80/Temporary_Listen_Addresses/arcgisearth/api/v1/layer

# Settting
#base_address = "http://localhost:80/Temporary_Listen_Addresses/arcgisearth/api/v1"
base_address = "http://localhost:50066/arcgisearth"
camera_info = "{ \"mapPoint\": { \"x\": 113.59647525051167, \"y\": 32.464715999412107, \"z\": 2213290.0751730204, \"spatialReference\": { \"wkid\": 4326 } }, \"heading\": 354.04823651174161, \"pitch\": 19.96239543740441}"

fly_to_info = "{\"camera\":{\"mapPoint\":{\"x\":-92,\"y\":41,\"z\":11000000,\"spatialReference\":{\"wkid\":4326}},\"heading\":0.0,\"pitch\":0.099999999996554886},\"duration\":2}"
layer_path = "//vm-3d-data/storage/ArcGISEarth/Data/KML/demo_kmls/Black-tailed prairie dog.kmz"
add_layer_info = "{\"type\":\"MapService\",\"URI\":\"https://services.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer\",\"target\":\"OperationalLayers\"}"



# layer_path = "C:/Users/ben7276/Desktop/dog.kmz"

class AgeRestApiTest(unittest.TestCase):
#    def test_get_camera_info(self):
#        url = base_address + "/camera"
#        r = requests.get(url)
#        self.assertEqual(r.status_code, 200)
#
#    def test_set_camera_info(self):
#        url = base_address + "/camera/" + camera_info
#        r = requests.put(url)
#        self.assertEqual(r.status_code, 200)
#        if r.status_code == 200:
#            content = r.content
#            str_camera = content.decode('utf-8')
#            json_camera = json.loads(str_camera)
#            result = json_camera['UpdateCameraResult']
#            self.assertEqual('Success', result)
#
#    def test_fly_to(self):
#        url = base_address + "/flyto/" + fly_to_info
#        r = requests.put(url)
#        self.assertEqual(r.status_code, 200)
#        if r.status_code == 200:
#            content = r.content
#            str_camera = content.decode('utf-8')
#            json_camera = json.loads(str_camera)
#            result = json_camera['FlyToResult']
#            self.assertEqual('Success', result)

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

        # get layer information test
        if layer_id is not None:
            url = base_address + "/layer/" + layer_id
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

    def test_layers_operation(self):
        # get layers
        url = base_address + "/layers"
        r = requests.get(url)
        layers_json = None
        content = None
        self.assertEqual(r.status_code, 200)
        if r.status_code == 200:
            content = r.content.decode('utf-8')
            print(content)
            layers_json = eval(content)
            print(layers_json)
            results = json.loads(layers_json)
            print(results)

        # remove layers

        url = base_address + "/layers" + "/AllLayers"
        r = requests.delete(url)
        self.assertEqual(r.status_code, 200)

        # import layers
        url = base_address + "/layers/" + content
        #data = layers_json
        #headers = {"content-Type": "application/json"}
        #r = requests.post(url, data=data, headers=headers)
        r = requests.post(url)
        self.assertEqual(r.status_code, 200)


#    def test_screensnap(self):
#        url = base_address + "/snapshot"
#        r = requests.get(url, stream=True)
#        self.assertEqual(r.status_code, 200)
#        if r.status_code == 200:
#            path = "./snaps.jpg"
#            if os.path.exists(path):
#                os.remove(path)
#            with open(path, 'wb') as f:
#                for chunk in r:
#                    f.write(chunk)
#            #self.assertTrue(os.path.exists(path))
#
#    def test_remove_all_operational_layer(self):
#        url = base_address + "/operationallayers"
#        r = requests.delete(url)
#        self.assertEqual(r.status_code, 200)
#        if r.status_code == 200:
#            str = json.loads(r.content.decode('utf-8'))['RemoveOperationalLayersResult']
#            #self.assertEqual(str, "Operational layers removed")

#    def test_get_all_operational_layers(self):
#        url = base_address + "/operationallayers"
#        r = requests.get(url)
#        #self.assertEqual(r.status_code, 200)


if __name__ == '__main__':
    unittest.main(verbosity=2)





