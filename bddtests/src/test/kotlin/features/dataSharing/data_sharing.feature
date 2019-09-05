@data-sharing
@more
@nativesmoketest
Feature: Data Sharing Frontend
  A user can access Data Sharing

  Background:
    Given I am a EMIS patient
    And I am logged in
    And I retrieve the 'More' page directly
    When I choose to set my data sharing preferences

  Scenario Outline: A user navigates through Data Sharing Preferences pages via the Next/Previous buttons to page with heading <TargetPage>
    Given I am on the Data Sharing <StartingPage> page
    When I click the Next button <Clicks> times
    Then I am on the Data Sharing <TargetPage> page
    When I click the Previous button <Clicks> times
    Then I am on the Data Sharing <StartingPage> page
  Examples:
  |StartingPage|Clicks|TargetPage                                     |
  |Overview    |1     |Where confidential patient information is used |
  |Overview    |2     |Where your choice does not apply               |
  |Overview    |3     |Make your choice                               |

  Scenario Outline: A use navigates through the Data Sharing Preferences pages via the Contents links to page with heading <TargetPage>
    Given I am on the Data Sharing <StartingPage> page
    When I click the <ContentsLink> contents link
    Then I am on the Data Sharing <TargetPage> page
  Examples:
  |StartingPage|ContentsLink                                   |TargetPage                                     |
  |Overview    |Overview                                       |Overview                                       |
  |Overview    |Where confidential patient information is used |Where confidential patient information is used |
  |Overview    |Where your choice does not apply               |Where your choice does not apply               |
  |Overview    |Make your choice                               |Make your choice                               |

  @android
  @nativepending
  @nhso-6689
  Scenario: A user can navigate to the NHS website to find out more information on Data Sharing
    Given I am on the Data Sharing Overview page
    When I click the link called 'Visit the NHS.UK website' with a url of 'https://www.nhs.uk/your-nhs-data-matters/'
    Then a new tab has been opened by the link

  Scenario: A user chooses to manage their Data Sharing preferences
    Given I am on the Data Sharing Overview page
    And I click the Next button 3 times
    And I am on the Data Sharing Make your choice page
    When I click the Start Now button
    Then I am on the Ndop website