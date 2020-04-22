@my-record
@backend
Feature: Get Allergies data Backend
  A user can get their patient allergy information

  Scenario Outline: GP practice for <GP System> has enabled allergies functionality
    Given I am a <GP System> user setup to use medical record version 2
    And the GP Practice has enabled allergies functionality and the patient has "3" allergies
    And I have logged in and have a valid session cookie
    When I request my record data
    Then I receive "3" allergies as part of the my record object
    And the flag informing that the patient has access to the allergy data is set to "True"
    And the flag informing that there was an error retrieving the allergy data is set to "False"
    And the field indicating supplier is set

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: GP practice for <GP System> has disabled allergies functionality
    Given I am a <GP System> user setup to use medical record version 2
    And the GP Practice has disabled allergies functionality
    And I have logged in and have a valid session cookie
    When I request my record data
    Then I receive "0" allergies as part of the my record object
    And the flag informing that the patient has access to the allergy data is set to "False"
    And the flag informing that there was an error retrieving the allergy data is set to "False"
    And the field indicating supplier is set

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
