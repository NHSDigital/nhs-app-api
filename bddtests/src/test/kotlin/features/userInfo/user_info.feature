@userinfo
Feature: User Info

  Scenario: When enabled in sjr, user info details will be saved in the repository upon login into the app
    Given I am a user where the journey configurations are:
      | Journey   | Value   |
      | user info | enabled |
    And the user info repository has been emptied
    And I am logged in
    Then a user info record is created
    And the user info record will have my NHS Login ID
    And the user info record will have my ODS Code
    And the user info record will have my NHS Number

  Scenario: When not enabled in sjr, user info details will not be saved in the repository upon login into the app
    Given I am a user where the journey configurations are:
      | Journey   | Value    |
      | user info | disabled |
    And the user info repository has been emptied
    And I am logged in
    Then there are no details available in the user info repository