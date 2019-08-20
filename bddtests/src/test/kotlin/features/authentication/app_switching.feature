@nativepending
@NHSO-2964
@tech-debt
Feature: Switching Apps on Mobile Devices
  If a user switches apps whilst using their device, the app maintains integrity in a variety of different ways

  # below tests are covered in Manual Regression Test pack
  @NHSO-4063
  Scenario: An EMIS user on any page within the app can switch apps, switch back, and still be on the same page
    Given I am logged in as a EMIS user
    Then I see the home page

  @NHSO-4064
  Scenario: When a user on any secure page switches apps, and switches back after session timeout, they are shown the login page
    #For instance, Appointments and Prescription pages

  @NHSO-4064
  Scenario: When a user on any non secure page switches apps, and switches back after session timeout, they are shown the same page
    #For instance, Symptoms

  @NHSO-4064
  Scenario: When a user on any secure page switches apps, and switches back after session timeout and internet connection loss, they are shown the login page
    #For instance, Appointments and Prescription pages

  @NHSO-4064
  Scenario: When a user on any non secure page switches apps, and switches back after session timeout and internet connection loss, they are shown the same page
    #For instance, Symptoms