@silverIntegration
@nbs
Feature: National Booking Service Homepage Appointment Bookings

  Scenario: A user without access to NBS cannot see the menu item 'Book or manage a coronavirus (COVID-19) vaccination' on the homepage
    Given I am a user who cannot view Book Coronavirus Vaccinations from NBS
    And I am logged in
    And the NBS link to Book Coronavirus Vaccinations is not available on the Appointments Hub

  Scenario: A user with access to NBS Appointment Bookings can navigate to third party provider via the homepage
    Given I am a user who can view Book Coronavirus Vaccinations from NBS
    And NBS responds to requests for Appointment Bookings
    And I am logged in
    When I click the Book or manage a coronavirus (COVID-19) vaccination menu link
    Then a new tab has been opened by the link

  Scenario: A user with P5 permission cannot see the NBS menu item 'Book or manage a coronavirus (COVID-19) vaccination' on the homepage
    Given I am a P5 user who cannot view Book Coronavirus Vaccinations from NBS
    And I am logged in
    And the NBS link to Book Coronavirus Vaccinations is not available on the Appointments Hub
