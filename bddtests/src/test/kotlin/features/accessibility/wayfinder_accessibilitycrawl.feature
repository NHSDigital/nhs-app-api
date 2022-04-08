@accessibility
@wayfinder-accessibility
Feature: Wayfinder accessibility

  Scenario: The 'Wayfinder with no appointments' native page is captured
    Given I am using the native app user agent
    And I am a user who can view Wayfinder from Appointments
    And I am logged in
    When I retrieve the 'Wayfinder' page directly
    Then the Wayfinder_Native page is saved to disk

  Scenario: The 'Wayfinder with no appointments' desktop page is captured
    Given I am a user who can view Wayfinder from Appointments
    And I am logged in
    When I retrieve the 'Wayfinder' page directly
    Then the Wayfinder_Desktop page is saved to disk

  Scenario: The 'Wayfinder with in review referral' desktop page is captured
    Given I am a user who can view Wayfinder from Appointments and has referrals and upcoming appointments
    And I am logged in
    When I retrieve the 'Wayfinder' page directly
    Then the Wayfinder_Desktop_Referrals page is saved to disk

  Scenario Outline: The 'Wayfinder API <An Error>' desktop page is captured
    Given I am a user who can view Wayfinder from Appointments
    And the Wayfinder Aggregator API is <An Error>
    And I am logged in
    When I retrieve the 'Wayfinder' page directly
    Then I see a helpful message indicating unavailable secondary care services with a <Prefix> service desk reference
    And the <FileName> page is saved to disk
    Examples:
      | An Error              | Prefix | FileName                           |
      | timing out            | zu     | Wayfinder_Desktop_Timeout_Error    |
      | encountering an issue | 4u     | Wayfinder_Desktop_BadGateway_Error |
