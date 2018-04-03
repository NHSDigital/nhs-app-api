Feature: EMIS Patient Registration
  In order to allow users login to NHS online app
  As a Citizen ID User
  I want to be able to register an EMIS user's IM1 credentials

Scenario: Registering an EMIS user's IM1 credentials returns a valid response with single NHS Number and connection token
	Given I have valid patient data to register new account
	When I register an EMIS user's IM1 credentials
	Then I receive the expected connection token and single NHS Number

Scenario: Registering an EMIS user's IM1 credentials returns a valid response with multiple NHS Numbers and connection token
	Given I have valid patient data with multiple nhs numbers to register new account
	When I register an EMIS user's IM1 credentials
	Then I receive the expected connection token and multipe NHS Numbers

Scenario: Registering an EMIS user's IM1 credentials returns only connection token if NHS Number does not exists
	Given I have valid data for a patient with no NHS Number
	When I register an EMIS user's IM1 credentials
	Then I receive the expected connection token without NHS Numbers

@bug
@wip
Scenario: Registering an EMIS user's IM1 credentials returns "Not found" response if no user was found matching the supplied details
	Given I have data for a patient that does not exist
	When I register an EMIS user's IM1 credentials
	Then I receive a "Not found" error

Scenario: Registering an EMIS user's IM1 credentials returns "Conflict" response if user's account has already been associated with the application
	Given I have data for a patient that has already been associated with the application in the GP system
	When I register an EMIS user's IM1 credentials
	Then I receive a "Conflict" error

@bug
@wip
Scenario: Registering an EMIS user's IM1 credentials returns "Forbidden" resposne if the EMIS Demographics endpoint is disable
	Given I have valid patient data to register new account
    And EMIS Demographics endpoint is disable
	When I register an EMIS user's IM1 credentials
	Then I receive a "Forbidden" error
