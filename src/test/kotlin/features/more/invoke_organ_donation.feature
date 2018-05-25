Feature: Invoke Organ Donation

  A user can access Organ Donation preferences

  Background:
    Given I am logged in
    And I navigate to more

  Scenario: User clicks on the 'Set organ donation preferences'
    When I choose to set my organ donation preferences
    Then a new tab opens https://www.organdonation.nhs.uk/
    And the app remains on the More Page
    


