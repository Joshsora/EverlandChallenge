import config
import json
import urllib.request
import ssl
import base64
from errors import handle_error_response

print('This Script allows you to get users from this Everland API demonstration.')
	
def get_by_username():
	username = input('Enter username: ')
	return urllib.request.Request(
		config.api_base_url + ('accounts/username/%s' % username),
		headers = {
			'Content-Type': 'application/json',
			'X-Api-Key': config.api_key
		}
	)
	
def get_by_id():
	id = input('Enter id: ')
	return urllib.request.Request(
		config.api_base_url + ('accounts/%s' % id),
		headers = {
			'Content-Type': 'application/json',
			'X-Api-Key': config.api_key
		}
	)

def get_by_authentication():
	username = input('Enter username: ')
	password = input('Enter password: ')	
	authorization_value = 'Basic %s' % base64.b64encode(
		('%s:%s' % (username, password)).encode('utf8')
	).decode('utf8')
	return urllib.request.Request(
		config.api_base_url + 'accounts',
		headers = {
			'Content-Type': 'application/json',
			'Authorization': authorization_value
		}
	)

mode_handler = {
	'u': get_by_username,
	'i': get_by_id,
	'a': get_by_authentication
}

mode = None
while mode not in ['u', 'i', 'a']:
	print('Enter a letter corresponding to search type.')
	mode = input('Get by (u)sername, (i)d, or (a)uthenticate: ').lower()

try:
	response = urllib.request.urlopen(
		mode_handler[mode](),
		context=ssl._create_unverified_context()
	)
	json_response = json.loads(response.read().decode('utf8'))
	if json_response['success']:
		data = json_response['data']
		print('Account details:')
		print('\tid: %s' % data['id'])
		print('\tusername: %s' % data['username'])
		print('\temail: %s' % data['email'])
	else:
		handle_error_response(json_response)
except urllib.error.HTTPError as http_error:
	json_response = json.loads(http_error.read().decode('utf8'))
	handle_error_response(json_response)
