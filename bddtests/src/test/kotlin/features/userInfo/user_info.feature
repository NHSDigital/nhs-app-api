@userinfo
Feature: User Info

  Scenario: When enabled in sjr, user info details will be saved in the repository upon login into the app
    Given I am a user who has user info enabled in SJR, and has not registered before
    And I am logged in
    Then a user info record has been created
    And the user info record will have my NHS Login ID
    And the user info record will have my ODS Code
    And the user info record will have my NHS Number

  Scenario: When not enabled in sjr, user info details will not be saved in the repository upon login into the app
    Given I am a user who does not have user info enabled in SJR, and has not registered before
    And I am logged in
    Then there are no details available in the user info repository