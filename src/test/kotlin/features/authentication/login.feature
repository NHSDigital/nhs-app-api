Feature: Login

  Logging into the service is handled via the CitizenID service.  A user will be shown personalised welcome messages upon successful login.

  Background:
    Given wiremock is initialised

  @manual
  Scenario: User logs in using CitizenID
    Given I am not logged in
    When I log in
    Then I am redirected to 'http://citizenidaddresshere'

  @smoketest
  Scenario: User sees the home page
    Given I am logged in
    Then I see a welcome message for Montel Frye
    And I see the header
    And I see the navigation menu

  Scenario: The spinner is shown while loading
    Given I am not logged in
    And sign in verification is slow
    When I log in
    Then the spinner appears