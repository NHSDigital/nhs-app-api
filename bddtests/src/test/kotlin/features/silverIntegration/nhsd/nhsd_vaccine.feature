@silverIntegration
@nhsd
Feature: NHSD Vaccine

  Scenario: A user without access to NHSD Vaccine Record cannot see the menu item 'Check your COVID-19 vaccine record'
    Given I am a user with Vaccine Records from NHSD disabled
    And I am logged in
    When I navigate to the health record hub page
    Then I can't see the Check your COVID-19 vaccine record menu link

  Scenario: A user with access to NHSD Vaccine Record can navigate to Covid vaccine record
    Given I am a user with Vaccine Records from NHSD enabled
    And NHSD responds to requests to check your Covid vaccine record
    And I am logged in
    When I navigate to the health record hub page
    And I click the Check your COVID-19 vaccine record menu link
    Then a new tab has been opened by the link
