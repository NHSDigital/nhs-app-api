Feature: EMIS Session Registration
	In order to verify user session
	As a Citizen ID developer
	I want to receive user session with family name and given name

@wip
Scenario: Creating user session returns a valid response with session guid token tls-only and http-only cookie and given name and family name in body

@wip
Scenario: Creating user session returns "Bad Request" response if OAuth details are incomplete

@wip
Scenario: Creating user session returns "Bad Request" response if OAuth details are invalid

@wip
Scenario: Creating user session returns "Server Error" response when EMIS is unavailable

@wip
Scenario: Creating user session returns "Server Error" response when CID tokens endpoint fails to process the request

@wip
Scenario: Creating user session returns "Server Error" response when CID user profile endpoint fails to process the request

@wip
Scenario: Creating user session returns "Server Error" response when EMIS end user session fails to create 

@wip
Scenario: Creating user session returns "Server Error" response when EMIS session fails to create

@wip
Scenario: Creating user session returns "Server Error" response when EMIS session fails to be saved in cache

@wip
Scenario: Creating user session returns "Bad Gateway" response when EMIS is unavailable

@wip
Scenario: Creating user session returns "Forbidden" response when CID connection token fails to authenticate with EMIS

@wip
Scenario: Creating user session returns "Gateway Timeout" when EMIS fails to respond in 30 seconds