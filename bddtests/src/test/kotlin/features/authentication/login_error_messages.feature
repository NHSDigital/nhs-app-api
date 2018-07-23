Feature: Login error messages

  The user is shown appropriate error messages if something goes wrong during login

  @NHSO-1625
  Scenario: CitizenID provides invalid data after successful login
    Given I am logged into Citizen ID but am receiving invalid data
    Then I see the appropriate error message for a login error
    When I click on the navigation button
    Then I see the login page

  @NHSO-1625
  Scenario: CitizenID login is successful but GP System authentication fails
    Given I am logged into Citizen ID but GP System authentication fails
    Then I see the appropriate error message for a login error
    When I click on the navigation button
    Then I see the login page

  @NHSO-1625
  Scenario: CitizenID login is successful but GP System session cannot be established
    Given I am logged into Citizen ID but GP System session cannot be established
    Then I see the appropriate error message for a login error
    When I click on the navigation button
    Then I see the login page

  @pending
  Scenario: User is inactive for 20 mins and tries to use feature that requires GP System
