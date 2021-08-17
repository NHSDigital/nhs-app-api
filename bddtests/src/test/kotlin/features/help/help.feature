@help
Feature: Help pages

  Scenario: A user can get help about appointments
    Given I am an EMIS patient
    And I am logged in
    And I navigate to appointments
    And I click help and support
    Then I see the appointments help page

  Scenario: A user can get help about prescriptions
    Given I am an EMIS patient
    And I am logged in
    And I navigate to prescriptions
    And I click help and support
    Then I see the prescriptions help page

  Scenario: A user can get help about messaging
    Given I am an EMIS patient
    And I am logged in
    And I navigate to messages
    And I click help and support
    Then I see the messaging help page

  Scenario: A user can get help about your health
    Given I am an EMIS patient
    And I am logged in
    And I navigate to your_health
    And I click help and support
    Then I see the your health help page

