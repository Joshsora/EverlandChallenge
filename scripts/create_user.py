import config
import json
import urllib.request
import ssl
from errors import handle_error_response

print('This Script allows you to create users on this Everland API demonstration.')

request_data = json.dumps({
	'username': input('Enter a username: '),
	'password': input('Enter a password: '),
	'email': input('Enter an email address: ')
}).encode('utf8')

try:
	response = urllib.request.urlopen(
		urllib.request.Request(
			config.api_base_url + 'accounts',
			data = request_data,
			headers = {
				'Content-Type': 'application/json'
			}
		),
		context=ssl._create_unverified_context()
	)
	json_response = json.loads(response.read().decode('utf8'))
	if json_response['success']:
		data = json_response['data']
		print('Created account with ID: %s' % data['id'])
	else:
		handle_error_response(json_response)
except urllib.error.HTTPError as http_error:
	json_response = json.loads(http_error.read().decode('utf8'))
	handle_error_response(json_response)
