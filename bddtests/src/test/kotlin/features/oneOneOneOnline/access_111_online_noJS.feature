@111Online
@noJs
Feature: Access 111 Online with no javascript

  A user can navigate to the 111 service after logging in while Javascript is disabled

  Scenario: The User is logged in and can view the symptoms links with javascript disabled and can click to review their conditions
    Given I have disabled javascript
    And a patient from EMIS is defined
    And I am logged in
    When I navigate to Symptoms
    Then the Check My Symptoms page is displayed

    When I press the A-Z symptoms header
    Then a new tab opens https://www.nhs.uk/conditions/


  Scenario: The User is logged in and can view the symptoms links with javascript disabled and can click the urgent help header
    Given I have disabled javascript
    And a patient from EMIS is defined
    And I am logged in
    When I navigate to Symptoms
    Then the Check My Symptoms page is displayed

    When I press the urgent help header
    Then a new tab opens https://111.nhs.uk/