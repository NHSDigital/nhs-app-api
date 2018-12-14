@nativepending @NHSO-2964
Feature: Switching Apps on Mobile Devices
  If a user switches apps whilst using their device, the app maintains integrity in a variety of different ways

  @manual
  Scenario Outline: A user on any page within the app can switch apps, switch back, and still be on the same page
    Given I am logged in as a <GP System> user
    Then I see the home page
    Examples:
      | GP System |
      | EMIS      |

  @manual
  Scenario: When a user on any secure page switches apps, and switches back after session timeout, they are shown the login page
    #For instance, Appointments and Prescription pages

  @manual
  Scenario: When a user on any non secure page switches apps, and switches back after session timeout, they are shown the same page
    #For instance, Symptoms

  @manual
  Scenario: When a user on any secure page switches apps, and switches back after session timeout and internet connection loss, they are shown the login page
    #For instance, Appointments and Prescription pages

  @manual
  Scenario: When a user on any non secure page switches apps, and switches back after session timeout and internet connection loss, they are shown the same page
    #For instance, Symptoms