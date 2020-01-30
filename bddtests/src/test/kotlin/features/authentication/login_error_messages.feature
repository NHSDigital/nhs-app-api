@authentication
@authentication-login
Feature: Login error messages

  The user is shown appropriate error messages if something goes wrong during login
#400
  @nativesmoketest
  Scenario: CitizenID provides invalid data after successful login
    Given I am logged into Citizen ID but am receiving invalid data
    Then In the error message I see the service reference number prefix with "3a"
    And Contact us link is appended with the error code as a query parameter
    When I click on the navigation action
    Then I see the login page

#403
  @nativesmoketest
  Scenario: CitizenID login is successful but TPP GP System authentication fails
    Given I am logged into Citizen ID but GP System authentication fails
    Then In the error message I see the service reference number prefix with "3c"
    And Contact us link is appended with the error code as a query parameter
    When I click on the navigation action
    Then I see the login page

#403
  Scenario: Cannot log in as an EMIS user with no userPatientLinkToken
    Given I attempt to log in as an EMIS user with no userPatientLinkToken
    Then In the error message I see the service reference number prefix with "3c"
    And Contact us link is appended with the error code as a query parameter

#502
  @nativesmoketest
  Scenario: CitizenID login is successful but EMIS session cannot be established
    Given I am logged into Citizen ID but EMIS session cannot be established
    Then In the error message I see the service reference number prefix with "3e"
    And Contact us link is appended with the error code as a query parameter
    When I click on the navigation action
    Then I see the login page

  #465
  Scenario Outline: Cannot log in as a <GP System> user with no Date of Birth
    Given I attempt to log in as a <GP System> user without a date of birth
    Then I see an error message informing me I cannot log in as I am under the minimum age
    Examples:
      | GP System |
      | TPP       |

  @nativesmoketest
    Examples:
      | GP System |
      | EMIS      |

    #465
  Scenario Outline: Cannot log in as a <GP System> user with an age under 13
    Given I attempt to log in as a <GP System> user with an age under 13
    Then I see an error message informing me I cannot log in as I am under the minimum age
    Examples:
      | GP System |
      | TPP       |

    #464
  Scenario Outline: Cannot log in as a <GP System> user with no NHS Number
    Given I attempt to log in as a <GP System> user without an NHS Number
    Then In the error message I see the service reference number prefix with "3f"
    Examples:
      | GP System |
      | TPP       |

  @nativesmoketest
    Examples:
      | GP System |
      | EMIS      |

  #464
  @nativesmoketest
  Scenario Outline: Cannot log in as a <GP System> user with invalid ODS Code
    Given I attempt to log in as a <GP System> user with invalid ODS Code
    Then In the error message I see the service reference number prefix with "3f"
    And Contact us link is appended with the error code as a query parameter
    Examples:
      | GP System |
      | EMIS      |

#504 timeout
  Scenario: Cannot log in as a <GP System> when the request timeout I see error code with "zn" prefix
    Given I attempt to log in as an EMIS and the CID request timeout
    Then In the error message I see the service reference number prefix with "zn"
    And Contact us link is appended with the error code as a query parameter
