Feature: EMIS Session Registration
	In order to verify user session
	As a Citizen ID developer
	I want to receive user session with family name and given name

	Scenario: Creating user session returns a valid response with session guid token tls-only and http-only cookie and given name and family name in body
		   Given I have a valid authCode and codeVerifier for a patient
		   When I create a user session with valid details
		   Then I receive a session id, given name and family name

	Scenario: Creating user session returns "Bad Request" response if OAuth details are incomplete
		   Given I have incomplete OAuth details
		   When I create a user session with incomplete details
		   Then I receive a "Bad Request" error

	Scenario: Creating user session returns "Bad Request" response if OAuth details are invalid
		   Given I have invalid OAuth details
		   When I create a user session with invalid details
		   Then I receive a "Bad Request" error

	Scenario: Creating user session returns "Server Error" response when CID tokens endpoint fails to process the request
		   Given I have valid OAuth details and the CID tokens endpoint fails to process the request
		   When I create a user session with valid details
		   Then I receive a "Internal Server Error" error

	Scenario: Creating user session returns "Server Error" response when CID user profile endpoint fails to process the request
		   Given I have valid OAuth details and the CID user profile endpoint fails to process the request
		   When I create a user session with valid details
		   Then I receive a "Internal Server Error" error

	Scenario: Creating user session returns "Server Error" response when EMIS end user session fails to create 
		   Given I have valid OAuth details and the EMIS end user session endpoint fails to create
		   When I create a user session with valid details
		   Then I receive a "Bad Gateway" error

	Scenario: Creating user session returns "Server Error" response when EMIS session fails to create
		   Given I have valid OAuth details and the EMIS session endpoint fails to create
		   When I create a user session with valid details
		   Then I receive a "Bad Gateway" error

	Scenario: Creating user session returns "Bad Gateway" response when EMIS is unavailable
		   Given I have valid OAuth details and the EMIS is unavailable
		   When I create a user session with valid details
		   Then I receive a "Bad Gateway" error

	Scenario: Creating user session returns "Forbidden" response when CID connection token fails to authenticate with EMIS
		   Given I have invalid OAuth details and CID connection token fails to authenticate with emis
		   When I create a user session with valid details
		   Then I receive a "Forbidden" error

	Scenario: Creating user session returns "Gateway Timeout" when EMIS fails to respond in 30 seconds
		   Given I have valid OAuth details and emis fails to respond in 30 seconds
		   When I create a user session with valid details
		   Then I receive a "Gateway Timeout" error

	@Manual
	Scenario: Creating user session returns "Server Error" response when EMIS session fails to be saved in cache 
		   Given I have valid OAuth details and the EMIS session fails to be saved in cache
		   When I create a user session with valid details
		   Then I receive a "Server Error" error