@appointmentsHub
Feature: Hospital Appointments

  Scenario: A user with correct permissions can navigate to the ers hospital appointments page
    Given I am a user who can manage their hospital appointments
    And I am logged in
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    When I click the 'Hospital and other services' link on the Appointments Hub
    Then the Hospital Appointments page is displayed
    And the Hospital Appointments links are displayed
    When I click the link called 'Book or cancel your referral appointment' with a url of 'http://web.local.bitraft.io:3000/redirector?redirect_to=http%3A%2F%2Fsilver.local.bitraft.io%3A5000%2Fnhslogin'
    Then a new tab has been opened by the link

  Scenario: A user without the secondary appointments permission will not be able to manage hospital appointments
    Given I am a user without the permission to manage their hospital appointments
    And I am logged in
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    And the 'Hospital and other services' link is not available on the Appointments Hub
    And I browse to the pages at the following urls I see the relevant page
    | /appointments/hospital-appointments  | /appointments |

  Scenario: A user with cie access only can navigate to the hospital appointments page
    Given I am a user who can manage their cie hospital appointments only
    And I am logged in
    When I retrieve the 'appointment hub' page directly
    Then the Appointments Hub page is displayed
    When I click the 'Hospital and other services' link on the Appointments Hub
    Then the Hospital Appointments page is displayed
    And the Hospital Appointments links are displayed

  Scenario: A user with pkb access only can navigate to the hospital appointments page
    Given I am a user who can manage their pkb hospital appointments only
    And I am logged in
    When I retrieve the 'appointment hub' page directly
    Then the Appointments Hub page is displayed
    When I click the 'Hospital and other services' link on the Appointments Hub
    Then the Hospital Appointments page is displayed
    And the Hospital Appointments links are displayed

  Scenario: A user with ers access only can navigate to the hospital appointments page
    Given I am a user who can manage their ers hospital appointments only
    And I am logged in
    When I retrieve the 'appointment hub' page directly
    Then the Appointments Hub page is displayed
    When I click the 'Hospital and other services' link on the Appointments Hub
    Then the Hospital Appointments page is displayed
    And the Hospital Appointments links are displayed

  Scenario: A user can navigate back from the hospital appointments page to the appointments hub using the breadcrumb
    Given I am a user who can manage their hospital appointments
    And I am logged in
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    When I click the 'Hospital and other services' link on the Appointments Hub
    Then the Hospital Appointments page is displayed
    And the Hospital Appointments links are displayed
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
    And the Hospital Appointments links are displayed
