@authentication
@authentication-crsfTokenValidation
Feature: CRSF Token Validation
    Tests authentication failure when the CRSF token is not sent correctly.

  @backend
  Scenario: API fails with no csrf token
    Given I have upcoming appointments for EMIS
    And I have logged into EMIS and have a valid session cookie
    Then the "Emis" API call fails with csrf token of ""


  @backend
  Scenario: API fails with invalid csrf token
    Given I have upcoming appointments for EMIS
    And I have logged into EMIS and have a valid session cookie
    Then the "Emis" API call fails with csrf token of "fghliarehgdfknbflk"