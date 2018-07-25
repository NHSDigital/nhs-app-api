Feature: Invoke Organ Donation and Data Sharing

  A user can access Organ Donation and Data Sharing preferences

  Background:
    Given wiremock is initialised
    And I am logged in
    And I navigate to more

  @smoketest
  @tech-debt @NHSO-1937
  Scenario: User clicks on the 'Set organ donation preferences'
    Given I am on the More Page
    When I choose to set my organ donation preferences
    Then a new tab opens https://www.organdonation.nhs.uk/

  @smoketest
  Scenario: User clicks on the 'Set health data sharing preferences'
    Given I am on the More Page
    When I choose to set my data sharing preferences
    Then a new tab opens https://www.nhs.uk/your-nhs-data-matters/benefits-of-data-sharing/

