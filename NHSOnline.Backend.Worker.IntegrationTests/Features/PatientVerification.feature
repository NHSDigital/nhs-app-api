Feature: Patient Verification
	In order to verify patient data
	As a Citizen ID developer
	I want to verify patient data and receive patient NHS Numbers

@wip
Scenario: Verifying patient data with valid credentials returns a valid response with single NHS Number
   Given I have valid credentials for a patient with one NHS Number
   When I verify patient data
   Then I receive the expected NHS Number
   
@wip
Scenario: Verifying patient data with valid credentials returns a valid response with multiple NHS Numbers
	Given I have valid credentials for a patient with multiple NHS Numbers
	When I verify patient data
	Then I receive the expected NHS Numbers

@wip
@bug
Scenario: Verify patient data with valid credentials does not return NHS Number if does not exists
	Given I have valid credentials for a patient with no NHS Number
	When I verify patient data
	Then I receive no NHS Number

@wip
@bug
Scenario: Verifying patient data with an IM1 Connection Token that does not exist returns a "Not Found" error
	Given I have an IM1 Connection Token that does not exist
	When I verify patient data
	Then I receive a "Not Found" error

@wip
@bug
Scenario: Verifying patient data with an IM1 Connection Token not in the expected format returns a "Bad Request" error
	Given I have an IM1 Connection Token that is in an invalid format
	When I verify patient data
	Then I receive a "Bad Request" error

@wip
Scenario: Verifying patient data with a missing IM1 Connection Token returns a "Bad Request" error
	Given I have no IM1 Connection Token
	When I verify patient data
	Then I receive a "Bad Request" error

@wip
Scenario: Verifying patient data with an ODS Code that does not exist returns a "Not Found" error
	Given I have an ODS Code that does not exists
	When I verify patient data
	Then I receive a "Not Found" error

@wip
Scenario: Verifying patient data with an ODS Code not in the expected format returns a "Bad Request" error
	Given I have an ODS Code not in expected format
	When I verify patient data
	Then I receive a "Bad Request" error

@wip
Scenario: Verifying patient data with a missing ODS Code returns a "Bad Request" error
	Given I have no ODS Code
	When I verify patient data
	Then I receive a "Bad Request" error

@wip
@bug
Scenario: Verifying patient data when EMIS is unavailable returns an "Internal Server Error" error
	Given EMIS is unavailable
	When I verify patient data
	Then I receive an "Internal Server Error" error


