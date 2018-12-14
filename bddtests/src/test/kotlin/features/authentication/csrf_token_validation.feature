@authentication
@authentication-crsfTokenValidation
@backend
Feature: CRSF Token Validation
    Tests authentication failure when the CRSF token is not sent correctly.

  Scenario: API fails with no csrf token
    Given I have upcoming appointments before cutoff time for EMIS
    And I have logged into EMIS and have a valid session cookie
    Then the "Emis" API call fails with csrf token of ""


  Scenario: API fails with invalid csrf token
    Given I have upcoming appointments before cutoff time for EMIS
    And I have logged into EMIS and have a valid session cookie
    Then the "Emis" API call fails with csrf token of "fghliarehgdfknbflk"