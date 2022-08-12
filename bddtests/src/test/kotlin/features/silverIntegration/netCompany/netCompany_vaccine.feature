@silverIntegration
@netCompany
Feature: Net Company Vaccine

  Scenario: A user without access to Net Company Vaccine Record cannot see the menu item 'NHS COVID Pass'
    Given I am a user with Vaccine Records from Net Company disabled
    And I am logged in
    Then I can't see the Get your NHS COVID Pass menu link
    When I navigate to the health record hub page
    Then I can't see the NHS COVID Pass menu link

  Scenario: A user with access to Net Company Vaccine Record can navigate to Covid Status via the home page
    Given I am a user with Vaccine Records from Net Company enabled
    And Net Company responds to requests to share Covid Status
    And I am logged in
    When I click the Get your NHS COVID Pass menu link
    Then a new tab has been opened by the link

  Scenario: A user with access to Net Company Vaccine Record can navigate to Covid Status via the health hub
    Given I am a user with Vaccine Records from Net Company enabled
    And Net Company responds to requests to share Covid Status
    And I am logged in
    When I navigate to the health record hub page
    And I click the NHS COVID Pass menu link
    Then a new tab has been opened by the link
