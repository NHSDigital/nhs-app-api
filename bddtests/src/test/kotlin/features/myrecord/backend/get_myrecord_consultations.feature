@my-record
@backend
Feature: Get Consultations Data Backend
  A user can get their consultation information

  Scenario: Requesting multiple consultations results returns multiple consultations data
    Given I am a EMIS user setup to use medical record version 2
    And I have logged in and have a valid session cookie
    And the GP Practice has multiple consultations
    When I get the users consultations
    Then I receive "2" consultations as part of the my record object

  Scenario: Patient has no consultations
    Given I am a EMIS user setup to use medical record version 2
    And I have logged in and have a valid session cookie
    And an error occurred retrieving the consultations
    When I get the users consultations
    Then I receive "0" consultations as part of the my record object

  Scenario: Patient does not have access to consultations
    Given I am a EMIS user setup to use medical record version 2
    And I have logged in and have a valid session cookie
    And the Patient has no access to consultations
    When I get the users consultations
    Then I receive consultations object with hasAccess flag set to "False"

  Scenario: Error occurs getting consultations
    Given I am a EMIS user setup to use medical record version 2
    And I have logged in and have a valid session cookie
    And an error occurred retrieving the consultations
    When I get the users consultations
    Then the consultations object with hasErrored flag set to "True"
