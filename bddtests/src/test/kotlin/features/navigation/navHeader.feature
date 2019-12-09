@navigation-bar
Feature: Use the navigation header bar

  Background:
    Given I am a EMIS patient

  @pending
  Scenario: A patient can access the help and support page by clicking the help icon
    Given I am logged in
    And I see the header
    When I click the help icon
    Then a new tab has been opened by the link

  @nativesmoketest
  Scenario: A patient can access the my account page by clicking the my account icon
    Given I am logged in
    And I see the header
    When I click the my account icon
    Then the Account page is displayed

  @nativesmoketest
  Scenario: A patient can access the home page by clicking the home icon
    Given I am logged in
    And I see the header
    And I navigate away from the home page
    When I click the home icon
    Then I see the home page

  @nativesmoketest
  Scenario: Dynamic back link leads to the correct pages
    Given I am using the native app user agent
    And I am logged in
    And there are EMIS appointments available to book with a reason
    And a booked appointment can be cancelled
    When I follow the Appointments link from the home page
    And I select "Book an appointment" button
    And I select "Book an appointment" button
    And I have selected an appointment slot to book
    Then the Appointment Slot page is displayed
    When I click the 'Back' breadcrumb
    Then the Available Appointments page is displayed
    When I click the 'Back' breadcrumb
    Then I am on the Appointments Guidance page
    When I click the 'Back' breadcrumb
    Then the Your Appointments page is displayed
    And the breadcrumb bar is not visible

  Scenario: A user can navigate through organ donation with the dynamic back button
    Given I am using the native app user agent
    And I am logged in
    And I am a EMIS user not registered with organ donation, who wishes to register and opt in
    And I navigate to the internal Organ Donation Choice Page
    And I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    When I select the option to donate all my organs
    And I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Faith And Beliefs page is displayed
    And no options on the Organ Donation Faith And Beliefs page are selected
    When I select an option in sharing my organ donation faith and beliefs
    And I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Check Details page is displayed
    And the choice of wishing to donate organs is displayed on the Organ Donation Check Details page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Back' breadcrumb
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Back' breadcrumb
    Then the Organ Donation Faith And Beliefs page is displayed
    When I click the 'Back' breadcrumb
    Then the Organ Donation Your Choice page is displayed
    When I click the 'Back' breadcrumb
    And I click the 'Back' breadcrumb
    Then I am on the More Page
