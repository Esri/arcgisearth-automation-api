import requests
import json
import os
import unittest
import hmac
import hashlib
import base64

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
base_address = "http://localhost:80/Temporary_Listen_Addresses/arcgisearth/api/v1"
camera_info = "{ \"mapPoint\": { \"x\": 113.59647525051167, \"y\": 32.464715999412107, \"z\": 2213290.0751730204, \"spatialReference\": { \"wkid\": 4326 } }, \"heading\": 354.04823651174161, \"pitch\": 19.96239543740441}"
layer_path = "//vm-3d-data/storage/ArcGISEarth/Data/KML/demo_kmls/Black-tailed prairie dog.kmz"


# layer_path = "C:/Users/ben7276/Desktop/dog.kmz"

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
            current_camera_info = json_camera['UpdateCameraResult']
            self.assertEqual(current_camera_info, camera_info)

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

    def test_remove_all_operational_layer(self):
        url = base_address + "/operationallayers"
        r = requests.delete(url)
        self.assertEqual(r.status_code, 200)
        if r.status_code == 200:
            str = json.loads(r.content.decode('utf-8'))['RemoveOperationalLayersResult']
            self.assertEqual(str, "Operational layers removed")

#    def test_add_layer(self):
#        url = base_address + '/layer'
#        data = "{\"url\" :\"" + layer_path + "\"}"
#        headers = {"content-Type": "application/json"}
#        r = requests.post(url, data=data, headers=headers)
#        self.assertEqual(r.status_code, 200)
#        if r.status_code == 200:
#            content = r.content.decode('utf-8')
#            self.assertEqual(r.content.decode('utf-8'),
#                              '"\\/\\/vm-3d-data\\/storage\\/ArcGISEarth\\/Data\\/KML\\/demo_kmls\\/Black-tailed prairie dog.kmz"')

    def test_flyto(self):
        flyto_info = "{ \"camera\": { \"mapPoint\": { \"x\": 113.59647525051167, \"y\": 32.464715999412107, \"z\": 2213290.0751730204, \"spatialReference\": { \"wkid\": 4326 } }, \"heading\": 354.04823651174161, \"pitch\": 19.96239543740441},\"duration\" : 10}"
        url = base_address + "/flyto/" + flyto_info
        r = requests.put(url)
        self.assertEqual(r.status_code, 200)

    def test_get_all_operational_layers(self):
        url = base_address + "/operationallayers"
        r = requests.get(url)
        self.assertEqual(r.status_code, 200)


    # Userid userkey is from config of the plugin
    # In this example the userid is 'ArcGISEarth' and user key is 'B0065327-EC31-43A0-ACA8-926077ED1D92'
    # Client will send request with authorization header (in this header the url is convert to a hash by userkey)
    # Server side will search the user id in its database then decode the header and compare the string.
    # More details please see https://stackoverflow.com/a/8366526
    def test_authentication(self):
        DEFAULT_HASHEDPASSWORD = "B0065327-EC31-43A0-ACA8-926077ED1D92"
        DEFAULT_USERID = "ArcGISEarth"
        url = base_address + "/testauth"
        hash = hmac.new(DEFAULT_HASHEDPASSWORD.encode('utf-8'), url.encode('utf-8'), digestmod=hashlib.md5).hexdigest()
        auth_info = DEFAULT_USERID + ":" + hash
        r = requests.get(url, headers={'Authorization': auth_info})
        self.assertEqual(r.status_code, 200)



if __name__ == '__main__':
    unittest.main(verbosity=2)





