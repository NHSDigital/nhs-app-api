@my-record
@backend
Feature: Get medications data Backend
  A user can get their medication information

  Scenario Outline: A <GP System> user can get medications data when GP Practice has enabled medications functionality
    Given I am a <GP System> user setup to use medical record version 2
    And I have logged in and have a valid session cookie
    And the GP Practice has enabled medications functionality
    When I request my record data
    Then I receive "<Number of Acute>" acute medications as part of the my record object
    And I receive "<Number of Current Repeat>" current repeat medications as part of the my record object
    And I receive "<Number of Discontinued Repeat>" discontinued repeat medications as part of the my record object
    And the field indicating supplier is set

    Examples:
      | GP System | Number of Acute | Number of Current Repeat | Number of Discontinued Repeat |
      | EMIS      | 2               | 5                        | 3                             |
      | TPP       | 2               | 3                        | 2                             |

  Scenario Outline: A <GP System> user cannot get medications data when GP Practice has disabled medications functionality
    Given I am a <GP System> user setup to use medical record version 2
    And I have logged in and have a valid session cookie
    But the GP Practice has disabled medications functionality
    When I request my record data
    Then the flag informing that the patient has access to the medications data is set to "False"
    And the flag informing that there was an error retrieving the medications data is set to "False"
    And the field indicating supplier is set

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
