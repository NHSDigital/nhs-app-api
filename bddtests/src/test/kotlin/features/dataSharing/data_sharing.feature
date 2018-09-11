@data-sharing
Feature: Data Sharing

  A user can access Data Sharing

  Background:
    Given a patient from EMIS is defined
    And I am logged in
    And I navigate to more
    When I choose to set my data sharing preferences

  @NHSO-2437
  Scenario Outline: A user navigates through Data Sharing Preferences pages
    Given I am on the Data Sharing <StartingPage> page
    When I click the Next button <Clicks> times
    Then I am on the Data Sharing <TargetPage> page
    When I click the Previous button <Clicks> times
    Then I am on the Data Sharing <StartingPage> page
  Examples:
  |StartingPage|Clicks|TargetPage                 |
  |Overview    |1     |Benefits                   |
  |Overview    |2     |Data Use                   |
  |Overview    |3     |Where Opt Out Doesn't Apply|
  |Overview    |4     |Manage Your Choice         |

  @NHSO-2437
  Scenario: A user navigates to Data Sharing Manage Your Choice page directly from Overview
    Given I am on the Data Sharing Overview page
    When I click the Manage Your Choice direct link
    Then I am on the Data Sharing Manage Your Choice page

  @NHSO-2437
  Scenario: A user can navigate to the NHS website to find out more information on Data Sharing
    Given I am on the Data Sharing Overview page
    When I click the Data Sharing More Info link
    Then a new tab opens https://www.nhs.uk/your-nhs-data-matters/

  @NHSO-2363
  @smoketest
  Scenario: A user chooses to manage their Data Sharing preferences
    Given I am on the Data Sharing Overview page
    And I click the Next button 4 times
    And I am on the Data Sharing Manage Your Choice page
    When I click the Start Now button
    Then I am on the Ndop website
