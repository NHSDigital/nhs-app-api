@silverIntegration
@nhsd
Feature: NHSD Vaccine

  Scenario: A user without access to NHSD Vaccine Record cannot see the menu item 'Share your COVID-19 status for events'
    Given I am a user with Vaccine Records from NHSD disabled
    And I am logged in
    Then I can't see the Check your COVID-19 vaccine record menu link
    When I navigate to the health record hub page
    Then I can't see the Share your COVID-19 status for events in England menu link

  Scenario: A user with access to NHSD Vaccine Record can navigate to Covid vaccine record via the home page
    Given I am a user with Vaccine Records from NHSD enabled
    And NHSD responds to requests to check your Covid vaccine record
    And I am logged in
    When I click the Check your COVID-19 vaccine record menu link
    Then a new tab has been opened by the link

  Scenario: A user with access to NHSD Vaccine Record can navigate to Covid vaccine record via the health hub
    Given I am a user with Vaccine Records from NHSD enabled
    And NHSD responds to requests to check your Covid vaccine record
    And I am logged in
    When I navigate to the health record hub page
    And I click the Check your COVID-19 vaccine record menu link
    Then a new tab has been opened by the link
