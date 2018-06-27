Feature: Get My Record data

  A user can get their patient data information

  Background:
    Given wiremock is initialised
    And the my record wiremocks are initialised

  @NHSO-690
  @backend
  Scenario: GP practice has enabled allergies functionality
    Given I have logged in and have a valid session cookie
    Given the GP Practice has enabled allergies functionality and the patient has "2" allergies
    When I get the users my record data
    Then I receive "2" allergies as part of the my record object
    And the flag informing that the patient has access to the allergy data is set to "True"
    And the flag informing that there was an error retrieving the allergy data is set to "False"

  @NHSO-690
  @backend
  Scenario: GP practice has disabled allergies functionality
    Given I have logged in and have a valid session cookie
    But the GP Practice has disabled allergies functionality
    When I get the users my record data
    Then I receive "0" allergies as part of the my record object
    And the flag informing that the patient has access to the allergy data is set to "False"
    And the flag informing that there was an error retrieving the allergy data is set to "False"
