@data-sharing
Feature: Data Sharing

  A user can access Data Sharing

  Background:
    Given a patient from EMIS is defined
    And I am logged in
    And I navigate to more
    When I choose to set my data sharing preferences

  @NHSO-2184
  Scenario: A user navigates to Data Sharing Preferences Manage Your Choice from Overview via contents
    Given I am on the Data Sharing Overview page
    When I click the Manage Your Choice contents link
    Then I am taken to Data Sharing Manage Your Choice page

  @NHSO-2184
  Scenario: A user navigates to Data Sharing Preferences Manage Your Choice from Overview via the Next button
    Given I am on the Data Sharing Overview page
    When I click the Next button
    Then I am taken to Data Sharing Manage Your Choice page

  @NHSO-2184
  Scenario: A user navigates to Data Sharing Preferences Overview from Manage Your Choice via contents
    Given I click the Next button
    And I am on the Data Sharing Manage Your Choice page
    When I click the Overview contents link
    Then I am taken to Data Sharing Overview page

  @NHSO-2184
  Scenario: A user navigates to Data Sharing Preferences Overview from Manage Your Choice via the Previous button
    Given I click the Next button
    And I am on the Data Sharing Manage Your Choice page
    When I click the Previous button
    Then I am taken to Data Sharing Overview page
