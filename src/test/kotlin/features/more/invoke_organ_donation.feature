Feature: Invoke Organ Donation

  A user can access Organ Donation preferences

  Background:
    Given wiremock is initialised
    And I am logged in
    And I navigate to more

  @pending
  @smoketest
  Scenario: User clicks on the 'Set organ donation preferences'
    Given I am on the More Page
    When I choose to set my organ donation preferences
    Then a new tab opens https://www.organdonation.nhs.uk/
    And the app remains on the More Page
    


