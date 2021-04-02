@logged-out
Feature: Logged out home page content

  Scenario: The correct information about using the service is displayed when on logged-out home page
    Given I am on the login logged-out page
    And 'NHS UK' responds to requests for '/healthAtoZ'
    And I see a list of other services I can use without logging in
    Then I click the link called 'Search conditions and treatments' with a url of 'http://stubs.local.bitraft.io:8080/external/healthAtoZ'
    And a new tab has been opened by the link

  Scenario: The correct content appears when on logged-out home page
    Given I am on the login logged-out page
    Then I see a list of other services I can use without logging in
    And I see Before you start information
    And I see the Download app panel
    And I see the desktop specific information displayed

  Scenario: The desktop specific information does not appear when on logged-out home page in native
    Given I am a EMIS patient using the native app
    And I am on the login logged-out page
    Then I do not see desktop specific information displayed


