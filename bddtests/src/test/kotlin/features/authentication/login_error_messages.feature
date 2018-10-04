@authentication
@authentication-login
Feature: Login error messages

  The user is shown appropriate error messages if something goes wrong during login

  Scenario: CitizenID provides invalid data after successful login
    Given I am logged into Citizen ID but am receiving invalid data
    Then I see the appropriate error message for a login error
    When I click on the navigation button
    Then I see the login page

  Scenario: CitizenID login is successful but GP System authentication fails
    Given I am logged into Citizen ID but GP System authentication fails
    Then I see the appropriate error message for a login error
    When I click on the navigation button
    Then I see the login page

  Scenario: CitizenID login is successful but GP System session cannot be established
    Given I am logged into Citizen ID but GP System session cannot be established
    Then I see the appropriate error message for a login error
    When I click on the navigation button
    Then I see the login page

  Scenario Outline: Cannot log in as a <GP System> user with no Date of Birth
    Given I attempt to log in as a <GP System> user without a date of birth
    Then I see an error message informing me I cannot log in as I am under 16
  Examples:
  | GP System   |
  | EMIS        |
  | TPP         |

  Scenario Outline: Cannot log in as a <GP System> user with no NHS Number
    Given I attempt to log in as a <GP System> user without an NHS Number
    Then I see an error message informing me I cannot log in
  Examples:
  | GP System   |
  | EMIS        |
  | TPP         |

  Scenario Outline: Cannot log in as a <GP System> user with invalid ODS Code
    Given I attempt to log in as a <GP System> user with invalid ODS Code
    Then I see an error message informing me I cannot log in
    Examples:
      | GP System   |
      | EMIS        |