@logged-out
Feature: Logged out home page content

  Scenario: The correct information about using the service is displayed when on logged-out home page
    Given I am on the login logged-out page
    And I see a list of other services I can use without logging in
    Then I see the link called 'Go to the NHS website' with a url of 'https://www.nhs.uk/'
    And I see the link called 'Find out more about who can have an NHS account' with a url of 'https://www.nhs.uk/nhs-app/nhs-app-help-and-support/getting-started-with-the-nhs-app/who-can-use-the-nhs-app/'

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


