@symptoms
@noJs
@pending
Feature: Access Symptoms Checker with no javascript

  A user can navigate to the 111 service after logging in while Javascript is disabled

  Scenario: A user with javascript disabled can use the symptoms checker to navigate to advice about coronavirus
    Given I have disabled javascript
    And I am a user who wishes to view advice about coronavirus
    And I am logged in
    When I navigate to Symptoms
    Then the Symptoms page is displayed
    When I click Advice About Coronavirus
    Then the advice about coronavirus page has been opened in a new tab

  Scenario: A user with javascript disabled can use the symptoms checker to navigate to the Health A to Z
    Given I have disabled javascript
    And I am a user who wishes to view the Health A to Z
    And I am logged in
    When I navigate to Symptoms
    Then the Symptoms page is displayed
    When I click Search Conditions and Treatments
    Then the health A to Z page has been opened in a new tab

  Scenario: A user with javascript disabled can use the symptoms checker to navigate to 111 online
    Given I have disabled javascript
    And I am a user who wishes to view 111 online
    And I am logged in
    When I navigate to Symptoms
    Then the Symptoms page is displayed
    When I click Use NHS 111 online
    Then the 111 online page has been opened in a new tab
