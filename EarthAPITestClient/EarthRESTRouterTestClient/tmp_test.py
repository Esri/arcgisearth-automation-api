import config as cf
import requests

flyto_info = "{ \"camera\": { \"mapPoint\": { \"x\": 113.59647525051167, \"y\": 32.464715999412107, \"z\": 2213290.0751730204, \"spatialReference\": { \"wkid\": 4326 } }, \"heading\": 354.04823651174161, \"pitch\": 19.96239543740441},\"duration\" : 20}"
url = cf.base_address + "/flyto/" + flyto_info
r = requests.put(url)
