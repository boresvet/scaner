import requests

r = requests.post("http://127.0.0.0:9000/", data={'day': '56', 'hour': '12', 'minutes': '20', 'second':'345'})
