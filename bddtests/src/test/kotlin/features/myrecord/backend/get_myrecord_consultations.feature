Feature: Get Consultations Data

  A user can get their consultation information

  @backend
  Scenario Outline: Requesting multiple consultations results returns multiple consultations data
    Given the my record wiremocks are initialised for <Service>
    And I have logged in and have a valid session cookie for <Service>
    And the GP Practice has multiple consultations for <Service>
    When I get the users consultations
    Then I receive "2" consultations as part of the my record object

    Examples:
      |Service|
      |EMIS|

  @backend
  Scenario Outline: Patient has no consultations
    Given the my record wiremocks are initialised for <Service>
    And I have logged in and have a valid session cookie for <Service>
    And an error occurred retrieving the consultations for <Service>
    When I get the users consultations
    Then I receive "0" consultations as part of the my record object

    Examples:
      |Service|
      |EMIS|

  @backend
  Scenario Outline: Patient does not have access to consultations
    Given the my record wiremocks are initialised for <Service>
    And I have logged in and have a valid session cookie for <Service>
    And the Patient has no access to consultations for <Service>
    When I get the users consultations
    Then I receive consultations object with hasAccess flag set to False

    Examples:
      |Service|
      |EMIS   |


  @backend
  Scenario Outline: Error occurs getting consultations
    Given the my record wiremocks are initialised for <Service>
    And I have logged in and have a valid session cookie for <Service>
    And an error occurred retrieving the consultations for <Service>
    When I get the users consultations
    Then I receive consultations object with hasErrored flag set to True

    Examples:
      |Service|
      |EMIS|


