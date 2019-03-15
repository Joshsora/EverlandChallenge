import config
import json
import urllib.request
import ssl
import base64
from errors import handle_error_response

print('This Script allows you to update users from this Everland API demonstration.')
	
def get_update_data():
	data = {}
	email = input('Enter new email (or, press enter to leave unchanged): ') or None
	if email is not None:
		data['email'] = email
	password = input('Enter new password (or, press enter to leave unchanged): ') or None
	if password is not None:
		data['password'] = password
	
	return json.dumps(data).encode('utf8')
	
def update_by_username():
	username = input('Enter username: ')
	return urllib.request.Request(
		config.api_base_url + ('accounts/username/%s' % username),
		data = get_update_data(),
		headers = {
			'Content-Type': 'application/json',
			'X-Api-Key': config.api_key
		},
		method = 'PUT'
	)
	
def update_by_id():
	id = input('Enter id: ')
	return urllib.request.Request(
		config.api_base_url + ('accounts/%s' % id),
		data = get_update_data(),
		headers = {
			'Content-Type': 'application/json',
			'X-Api-Key': config.api_key
		},
		method = 'PUT'
	)

def update_by_authentication():
	username = input('Enter username: ')
	password = input('Enter password: ')
	authorization_value = 'Basic %s' % base64.b64encode(
		('%s:%s' % (username, password)).encode('utf8')
	).decode('utf8')
	return urllib.request.Request(
		config.api_base_url + 'accounts',
		data = get_update_data(),
		headers = {
			'Content-Type': 'application/json',
			'Authorization': authorization_value
		},
		method = 'PUT'
	)

mode_handler = {
	'u': update_by_username,
	'i': update_by_id,
	'a': update_by_authentication
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
		print('Successfully updated.')
	else:
		handle_error_response(json_response)
except urllib.error.HTTPError as http_error:
	json_response = json.loads(http_error.read().decode('utf8'))
	handle_error_response(json_response)
