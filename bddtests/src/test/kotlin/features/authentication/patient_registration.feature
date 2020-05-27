@authentication
Feature: Patient Registration Frontend
  A user can create a new NHS account from the login page, allowing them to access the app

  Scenario Outline: <GP System> User launches the create account CitizenID journey
    Given I want to register for a <GP System> account
    When I select to create an account
    Then I see the CID create an account page

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  @ios
  Scenario Outline: <GP System> User launches and completes account creation from web
    Given I have completed <GP System> account creation
    Then the Terms and Conditions page is displayed
    When I check the agree to terms and conditions checkbox
    And I click the continue button on Terms and Conditions
    Then the User Research page is displayed
    When I click the 'No' radio button
    And I click the 'Continue' button
    Then I see the signed in home page
    And I see a welcome message
    And I see the navigation menu
    And I see the home page header

    Examples:
      | GP System |
      | TPP       |
      | VISION    |
  @nativesmoketest
    Examples:
      | GP System |
      | EMIS      |
