@accessibility
@wayfinder-accessibility
Feature: Wayfinder accessibility

  Scenario: The 'Wayfinder with no appointments' native page is captured
    Given I am using the native app user agent
    And I am a user whose surgery has enabled Wayfinder
    And I have no referrals or appointments
    And I am logged in
    When I retrieve the 'Wayfinder' page directly
    Then the Wayfinder_Native page is saved to disk

  Scenario: The 'Wayfinder with no appointments' desktop page is captured
    Given I am a user whose surgery has enabled Wayfinder
    And I have no referrals or appointments
    And I am logged in
    When I retrieve the 'Wayfinder' page directly
    Then the Wayfinder_Desktop page is saved to disk

  Scenario: The 'Wayfinder with appointments and referrals' desktop page is captured
    Given I am a user whose surgery has enabled Wayfinder
    And I have referrals and upcoming appointments
    And I am logged in
    When I retrieve the 'Wayfinder' page directly
    Then the Wayfinder_Desktop page is saved to disk

  Scenario Outline: The 'Wayfinder API <An Error>' desktop page is captured
    Given I am a user whose surgery has enabled Wayfinder
    And I have no referrals or appointments
    And the Wayfinder Aggregator API is <An Error>
    And I am logged in
    When I retrieve the 'Wayfinder' page directly
    Then I see a helpful message indicating unavailable secondary care services with a <Prefix> service desk reference
    And the <FileName> page is saved to disk
    Examples:
      | An Error              | Prefix | FileName                           |
      | timing out            | zu     | Wayfinder_Desktop_Timeout_Error    |
      | encountering an issue | 4u     | Wayfinder_Desktop_BadGateway_Error |

  Scenario: The 'Wayfinder help' desktop page is captured
    Given I am a user whose surgery has enabled Wayfinder
    And I have no referrals or appointments
    And I am logged in
    When I retrieve the 'Wayfinder Help' page directly
    Then I am navigated to the Wayfinder help page
    When I click the 'Missing referrals' expander link on a Wayfinder Help page
    Then I can see the missing referrals expander elements displayed
    When I click the 'Incorrect or cancelled referrals' expander link on a Wayfinder Help page
    Then I can see the incorrect or cancelled referrals expander elements displayed
    When I click the 'Missing appointments' expander link on a Wayfinder Help page
    Then I can see the missing appointments expander expander elements displayed
    When I click the 'Incorrect, changed or cancelled appointments' expander link on a Wayfinder Help page
    Then I can see the incorrect changed cancelled appointments expander elements displayed
    And the Wayfinder_help_Desktop page is saved to disk

  Scenario: The 'Wayfinder Wait times' desktop page is captured
    Given I am a user whose surgery has enabled Wayfinder
    And I have referrals and upcoming appointments
    And I have one wait time available
    And I am logged in
    When I retrieve the 'Wayfinder Wait Times' page directly
    Then the Wayfinder_WaitTimes_Desktop page is saved to disk

  Scenario: The 'Wayfinder Wait Times Error' desktop page is captured
    Given I am a user whose surgery has enabled Wayfinder
    And I have no referrals or appointments
    And I get a wait times error
    And I am logged in
    When I retrieve the 'Wayfinder Wait Times' page directly
    Then I am navigated to the Wayfinder wait times error page
    And I see the correct error page elements
    And the Wayfinder_WaitTimes_Desktop_Error page is saved to disk
