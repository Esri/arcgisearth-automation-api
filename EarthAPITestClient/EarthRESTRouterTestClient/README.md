# age-rest-test
Use it to test the rest api for ArcGIS Earth


## Introduction

- list the current api and codes about how to use it
- Tested with python3.5
- find the docs about api from here: \\dory\share\Projects\ArcGISEarth\DESIGN\UX\AE_V1.6\API_doc

## How to use it
- Use the latest version of earth(1.6)
- Make sure you have ArcGISEarth.RestServicePlugin.dll in bin\plugins\
- `pip install -r requirements.txt` to install the depends ( use pipreqs to generate requirements)
- Start the service with the last icon in toolbar (It start when earth starting by default)
- Run it: `python age_rest_api_test.py`

The result will be:
```
test_add_layer (__main__.AgeRestApiTest) ... ok
test_get_all_operational_layers (__main__.AgeRestApiTest) ... ok
test_get_camera_info (__main__.AgeRestApiTest) ... ok
test_remove_all_operational_layer (__main__.AgeRestApiTest) ... ok
test_screensnap (__main__.AgeRestApiTest) ... ok
test_set_camera_info (__main__.AgeRestApiTest) ... ok

----------------------------------------------------------------------
Ran 6 tests in 0.425s

OK
```

# config

More details please see [here](https://devtopia.esri.com/ben7276/age-center/wiki/config-of-arcgis-earth-rest-api-plugin)

## API details

- The self-host WCF plugin named "RestServicePlugin" will handle the requests.
- It's not final version of api.
- More example please find in the python file.
- All [post] methods can be used in web browser.
- You can use postman for [put][post][del] methods.(adding layer need send data of layerinfo)







