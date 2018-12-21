@data-sharing
@more
Feature: Data Sharing
  A user can access Data Sharing

  Background:
    Given a patient from EMIS is defined
    And I am logged in
    And I navigate to more
    When I choose to set my data sharing preferences

  Scenario Outline: A user navigates through Data Sharing Preferences pages via the Next/Previous buttons
    Given I am on the Data Sharing <StartingPage> page
    When I click the Next button <Clicks> times
    Then I am on the Data Sharing <TargetPage> page
    When I click the Previous button <Clicks> times
    Then I am on the Data Sharing <StartingPage> page
  Examples:
  |StartingPage|Clicks|TargetPage                                     |
  |Overview    |1     |Where confidential patient information is used |
  |Overview    |2     |Where your choice does not apply               |

  Scenario Outline: A use navigates through the Data Sharing Preferences pages via the Contents links
    Given I am on the Data Sharing <StartingPage> page
    When I click the <ContentsLink> contents link
    Then I am on the Data Sharing <TargetPage> page
  Examples:
  |StartingPage|ContentsLink                                   |TargetPage                                     |
  |Overview    |Overview                                       |Overview                                       |
  |Overview    |Where confidential patient information is used |Where confidential patient information is used |
  |Overview    |Where your choice does not apply               |Where your choice does not apply               |

  Scenario: A user can navigate to the NHS website to find out more information on Data Sharing
    Given I am on the Data Sharing Overview page
    When I click the Data Sharing More Info link
    Then a new tab opens https://www.nhs.uk/your-nhs-data-matters/

  @smoketest
  Scenario: A user chooses to manage their Data Sharing preferences
    Given I am on the Data Sharing Overview page
    And I click the Next button 2 times
    And I am on the Data Sharing Where your choice does not apply page
    When I click the Start Now button
    Then I am on the Ndop website
