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
    Given I am a user who can view Wayfinder from Appointments and has referrals
    And I am logged in
    When I retrieve the 'Wayfinder' page directly
    Then the Wayfinder_Desktop_Referrals page is saved to disk
