Feature: Medical record hub page

  Scenario: Redirect to GP medical record page if I don't have access to any third party care plans or health tracker
    Given I am an EMIS patient with no access to any Third Party Health Record Hub Features
    And I am logged in
    When I retrieve the 'health record hub' page directly
    Then I am redirected to the 'health record hub' page

  Scenario: A user can access the health record hub page if PKB Care Plans is available
    Given I am an EMIS patient and I have access to Patients Know Best Care Plans
    And I am logged in
    When I navigate to the health record hub page
    Then I see the health records hub page
    And I see the Third Party menu item 'Care plans'

  Scenario: A user can access the health record hub page if CIE Care Plans is available
    Given I am an EMIS patient and I have access to Care Information Exchange Care Plans
    And I am logged in
    When I navigate to the health record hub page
    Then I see the health records hub page
    And I see the Third Party menu item 'Care plans'

  Scenario: A user can access the medical record hub page if PKB Health Tracker is available
    Given I am using the native app user agent
    And I am an EMIS patient and I have access to Patients Know Best Health Tracker
    And I am logged in
    When I navigate to the health record hub page
    Then I see the health records hub page
    And I see the Third Party menu item 'Track your health'

  Scenario: A user can access the medical record hub page if CIE Health Tracker is available
    Given I am using the native app user agent
    And I am an EMIS patient and I have access to Care Information Exchange Health Tracker
    And I am logged in
    When I navigate to the health record hub page
    Then I see the health records hub page
    And I see the Third Party menu item 'Track your health'

  Scenario: The GP medical record button redirects to GP medical record page
    Given I am an EMIS patient and I have access to Patients Know Best Care Plans
    And I am logged in
    When I navigate to the health record hub page
    Then I see the health records hub page
    And I click the menu item 'GP medical record'
    And I see the 'GP medical record' page

  Scenario: Proxy users are redirected to the sensitive information acceptance screen
    Given I am an EMIS user with linked profiles
    And I am logged in
    When I select the linked profiles link from the home page
    And I select a linked profile
    And I click the Switch to this profile button for the proxy user
    And I navigate to MY_RECORD
    Then I see 'Your GP medical record' page
