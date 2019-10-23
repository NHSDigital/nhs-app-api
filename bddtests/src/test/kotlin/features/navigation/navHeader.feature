@navigation-bar
Feature: Use the navigation header bar

  Background:
    Given I am a EMIS patient
    And I am logged in

  @pending
  Scenario: A patient can access the help and support page by clicking the help icon
    Given I see the header
    When I click the help icon
    Then a new tab has been opened by the link

  @nativesmoketest
  Scenario: A patient can access the my account page by clicking the my account icon
    Given I see the header
    When I click the my account icon
    Then the Account page is displayed

  @nativesmoketest
  Scenario: A patient can access the home page by clicking the home icon
    Given I see the header
    And I navigate away from the home page
    When I click the home icon
    Then I see the home page

  @native
  Scenario: Dynamic back link leads to the correct pages
    Given there are EMIS appointments available to book with a reason
    And a booked appointment can be cancelled
    When I try to progress to the available appointments page
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I click the back link
    Then the Available Appointments page is displayed
    When I click the back link
    Then I am on the Appointments Guidance page
    When I click the back link
    Then the My Appointments page is displayed
    And the back bar is not visible
