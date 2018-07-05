Feature: Get My Record data

  A user can get their patient data information

  @NHSO-690
  @backend
  Scenario Outline: GP practice has enabled allergies functionality for <Service>
    Given the my record wiremocks are initialised for <Service>
    And I have logged in and have a valid session cookie for <Service>
    And the GP Practice has enabled allergies functionality and the patient has "3" allergies for <Service>
    When I get the users my record data
    Then I receive "3" allergies as part of the my record object
    And the flag informing that the patient has access to the allergy data is set to "True"
    And the flag informing that there was an error retrieving the allergy data is set to "False"

    Examples:
      |Service|
      |EMIS|
      |TPP|

  @NHSO-690
  @backend
  Scenario Outline: GP practice has disabled allergies functionality for <Service>
    Given the my record wiremocks are initialised for <Service>
    And I have logged in and have a valid session cookie for <Service>
    And the GP Practice has disabled allergies functionality for <Service>
    When I get the users my record data
    Then I receive "0" allergies as part of the my record object
    And the flag informing that the patient has access to the allergy data is set to "False"
    And the flag informing that there was an error retrieving the allergy data is set to "False"

    Examples:
      |Service|
      |EMIS|
      |TPP|
