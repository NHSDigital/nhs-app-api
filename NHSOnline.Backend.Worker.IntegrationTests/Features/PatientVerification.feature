Feature: Patient Verification
	In order to verify patient data
	As a Citizen ID User
	I want to be able to get an IM1 connection token and an array of NHS Numbers
	As a Citizen ID developer
	I want to verify patient data and receive patient NHS Numbers

@wip
Scenario: Verifying patient data with valid credentials returns a valid response with single NHS Number
   Given I have valid credentials for a patient with one NHS Number
   When I verify patient data
   Then I receive the expected NHS Number


