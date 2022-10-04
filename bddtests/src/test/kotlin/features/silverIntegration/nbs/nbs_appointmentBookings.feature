@silverIntegration
@nbs
Feature: National Booking Service Appointment Bookings

  Scenario: A user without access to NBS cannot see the menu item 'Book or manage a coronavirus (COVID-19) vaccination' on the appointments hub
    Given I am a user who cannot view Book Coronavirus Vaccinations from NBS
    And I am logged in
    And I navigate to the Appointment hub page
    And the Appointments Hub page is displayed
    And the NBS link to Book Coronavirus Vaccinations is not available on the Appointments Hub

  Scenario: A user with access to NBS Appointment Bookings can navigate to third party provider via the appointments hub
    Given I am a user who can view Book Coronavirus Vaccinations from NBS
    And NBS responds to requests for Appointment Bookings
    And I am logged in
    When I navigate to the Appointment Hub page
    Then the Appointments Hub page is displayed
    When I click the Book or manage a coronavirus (COVID-19) vaccination menu link
    Then a new tab has been opened by the link
