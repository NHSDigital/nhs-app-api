@appointmentsHub
Feature: Hospital Appointments

  Scenario: A user without the secondary appointments permission will not be able to manage hospital appointments
    Given I am a user without the permission to manage their hospital appointments
    And I am logged in
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    And the 'Hospital and other services' link is not available on the Appointments Hub
    #TODO @NHSO-8715
    #And I browse to the pages at the following urls I see the relevant page
    #| /appointments/hospital-appointments  | /appointments |

  Scenario: A user can navigate back from the hospital appointments page to the appointments hub using the breadcrumb
    Given I am a user who can manage their hospital appointments
    And I am logged in
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    When I click the 'Hospital and other services' link on the Appointments Hub
    Then the Hospital Appointments page is displayed
    When I click the 'Appointments' breadcrumb
    Then the Appointments Hub page is displayed

  Scenario: A proxy user cannot see the hospital appointments page
    Given I am a user who can manage their hospital appointments and has linked profiles
    And I am logged in
    And I have switched to a linked profile
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    And the 'Hospital and other services' link is not available on the Appointments Hub
    When I click the proxy warning
    And I click the Switch to my profile button for the main user
    And I navigate to Appointments
    Then the Appointments Hub page is displayed
    When I click the 'Hospital and other services' link on the Appointments Hub
    Then the Hospital Appointments page is displayed
