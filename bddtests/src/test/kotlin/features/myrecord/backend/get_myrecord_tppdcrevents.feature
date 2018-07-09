Feature: Get Immunisations Data

  A user can get their Tpp Dcr Event information

  @backend
  Scenario: Requesting multiple tpp dcr events returns multiple tpp dcr events data for TPP
    Given the my record wiremocks are initialised for TPP
    And I have logged in and have a valid session cookie for TPP
    And the GP Practice has multiple dcr events for TPP
    When I get the users dcr event data
    Then I receive "2" dcr events as part of the my record object
    And the flag informing that the patient has access to the dcr event data is set to "True"
    And the field indicating supplier is set to TPP

  @backend
  Scenario: GP practice has disabled tpp dcr events functionality for TPP
    Given the my record wiremocks are initialised for TPP
    And I have logged in and have a valid session cookie for TPP
    And the GP Practice has disabled dcr events functionality for TPP
    When I get the users dcr event data
    Then I receive "0" dcr events as part of the my record object
    And the flag informing that the patient has access to the dcr event data is set to "False"
    And the field indicating supplier is set to TPP

  @backend
  Scenario: Error occurs getting tpp dcr events for TPP
    Given the my record wiremocks are initialised for TPP
    And I have logged in and have a valid session cookie for TPP
    And an error occurred retrieving the dcr events from TPP
    When I get the users dcr event data
    Then I receive "0" dcr events as part of the my record object
    And the flag informing that there was an error retrieving the dcr event data is set to "True"
    And the field indicating supplier is set to TPP

