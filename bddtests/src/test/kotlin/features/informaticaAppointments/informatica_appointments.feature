@appointments
@informatica
Feature: Informatica Frontdesk Appointments

  Scenario: A user sees an appropriate message when the appointments journey configuration is set to
  Informatica Frontdesk
    Given I am a EMIS user where the journey configurations are:
      | Journey      | Value       |
      | appointments | informatica |
    And  'Informatica' responds to requests for '/andover-medical-practice.appointments-online.co.uk'
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    Then I see an appropriate message informing me that my GP surgery uses Appointments Online
    When I click the link called 'log in to Appointments Online' with a url of 'http://stubs.local.bitraft.io:8080/external/andover-medical-practice.appointments-online.co.uk/'
    Then a new tab has been opened by the link
