@more
Feature: View More Page

  @nativesmoketest
  Scenario: A patient can navigate to More page
    Given I am a EMIS patient
    And I am logged in
    When I click the more icon
    Then the More page is displayed
    And none of the menu buttons are highlighted

  @nativesmoketest
  Scenario: The app version is on the More Page
    Given I am a EMIS patient
    And I am logged in
    When I click the more icon
    Then the More page is displayed
    And I see the current app version

  Scenario Outline: A patient can see the linked accounts, account & settings and Help & support links
    Given I am a <Gp System> patient
    And I am logged in
    When I click the more icon
    Then the More page is displayed
    And the Linked Profiles link is displayed
    And the Account and Settings link is displayed
    And the Help and Support link is displayed
    Examples:
      | Gp System |
      | EMIS      |
      | TPP       |

  Scenario Outline: A patient can navigate to More page and can not see the linked account link
    Given I am logged in as a <Gp System> user
    When I click the more icon
    Then the More page is displayed
    And the Linked Profiles link is not displayed
    And the Account and Settings link is displayed
    And the Help and Support link is displayed
    Examples:
      | Gp System |
      | VISION    |

    Scenario: A patient with proof level 5 navigates to More page
      Given I am a patient with proof level 5
      And I am logged in
      When I click the more icon
      Then the More page is displayed
      And the Linked Profiles link is not displayed
