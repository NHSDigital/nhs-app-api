@silverIntegration
@netCompanyP5
Feature: Net Company P5 Vaccine

  Scenario: A proof level 5 user without access to Net Company Vaccine Record cannot see the menu item 'Get your NHS COVID Pass' on the Home page
    Given I am a proof level 5 user with P5 Vaccine Records from Net Company disabled
    And I am logged in
    Then I can't see the Get your NHS COVID Pass menu link
    When I navigate to the health record hub page
    Then I can't see the Get your NHS COVID Pass menu link

  Scenario: A proof level 5 user with access to Net Company Vaccine Record can navigate to Covid Status for events
    Given I am a proof level 5 user with P5 Vaccine Records from Net Company enabled
    And Net Company responds to requests to share Covid Status P5
    And I am logged in
    When I click the Get your NHS COVID Pass menu link
    Then a new tab has been opened by the link
