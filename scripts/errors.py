from enum import Enum

class ApiErrorCode(Enum):
	InvalidRequest = 1
	UnhandledException = 2
	DatabaseError = 3
	AuthenticationFailed = 4
	NotFound = 5
	AccountCreationFailed = 6
	AccountUsernameInUse = 7
	AccountEmailInUse = 8

def default_handler(error):
	print(error['reason'])

def handle_validation_error(error):
	print(error['reason'])
	print('\t- # of invalid fields: %d' % len(error['invalidFields']))
	for field in error['invalidFields']:
		print('\t%s:' % field['name'])
		for reason in field['reasons']:
			print('\t\t' + reason)

handlers_by_code = {
	ApiErrorCode.InvalidRequest: handle_validation_error
}

def handle_error_response(response):
	print('Got unsuccessful response.')
	print('- # of errors: %d' % len(response['errors']))
	for error in response['errors']:
		error_code = ApiErrorCode(error['errorCode'])
		try:
			handlers_by_code[error_code](error)
		except:
			default_handler(error)