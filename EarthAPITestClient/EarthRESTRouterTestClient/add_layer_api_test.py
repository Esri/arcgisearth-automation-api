import requests
import json
import unittest
import config as cf


def CreateRestLayerJsonDescription(urls, type, target):
    descripton = {}
    descripton[cf.urlsField] = urls
    descripton[cf.layerTypeField] = type
    descripton[cf.addToField] = target
    return json.dumps(descripton)


class AgeRestApiAddLayerTest(unittest.TestCase):

    _url = cf.base_address + cf.addLayersEndpoint

    def normal_template(self, url, type, target):
        urls  = []
        urls.append(url)
        data = CreateRestLayerJsonDescription(urls, type, target)
        headers = {"content-Type": "application/json"}
        r = requests.post(self._url, data=data, headers=headers)
        return r

    def test_add_feature_service(self):
        layer_path = 'https://gis.internationalmedicalcorps.org/arcgis/rest/services/Nepal/Nepal_Earthquake_2015_data/FeatureServer/14'
        urls  = []
        urls.append(layer_path)
        data = CreateRestLayerJsonDescription(urls, 'FeatureService', 'operational_layer')
        headers = {"content-Type": "application/json"}
        r = requests.post(self._url, data=data, headers=headers)
        self.assertEqual(r.status_code, 200)

    def test_add_kml(self):
        layer_path = "//vm-3d-data/storage/ArcGISEarth/Data/KML/demo_kmls/Black-tailed prairie dog.kmz"
        r = self.normal_template(layer_path, 'KML', 'operational_layer')
        self.assertEqual(r.status_code, 200)


if __name__ == '__main__':
    unittest.main(verbosity=2)