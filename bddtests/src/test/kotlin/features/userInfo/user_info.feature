@userinfo
Feature: User Info

  Scenario: When enabled in sjr, user info details will be saved in the repository upon login into the app
    Given I am a user where the journey configurations are:
      | Journey   | Value   |
      | user info | enabled |
    And the user info repository has been emptied
    And I am logged in
    Then my details are available in the user info repository

  Scenario: When not enabled in sjr, user info details will not be saved in the repository upon login into the app
    Given I am a user where the journey configurations are:
      | Journey   | Value    |
      | user info | disabled |
    And the user info repository has been emptied
    And I am logged in
    Then there are no details available in the user info repository