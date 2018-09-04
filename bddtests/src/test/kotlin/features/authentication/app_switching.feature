@native
Feature: Switching Apps on Mobile Devices
  If a user switches apps whilst using their device, the app maintains integrity in a variety of different ways

  @NHSO-893
  @manual
  Scenario: A user on any page within the app can switch apps, switch back, and still be on the same page

  @NHSO-893
  @manual
  Scenario: When a user on any secure page switches apps, and switches back after session timeout, they are shown the login page
    #For instance, Appointments and Prescription pages

  @NHSO-893
  @manual
  Scenario: When a user on any non secure page switches apps, and switches back after session timeout, they are shown the same page
    #For instance, Symptoms

  @NHSO-893
  @manual
  Scenario: When a user on any secure page switches apps, and switches back after session timeout and internet connection loss, they are shown the login page
    #For instance, Appointments and Prescription pages

  @NHSO-893
  @manual
  Scenario: When a user on any non secure page switches apps, and switches back after session timeout and internet connection loss, they are shown the same page
    #For instance, Symptoms