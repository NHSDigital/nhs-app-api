Feature: Services available with P5 Access

  Scenario: P5 User can access their account settings
    Given I am a patient with proof level 5
    And I am logged in
    When I navigate to the account page
    Then the Account page is displayed

  Scenario: P5 user can access 111 Online
    Given I am a patient with proof level 5 who wishes to view 111 online
    And I am logged in
    When I navigate to the symptoms page
    And I click the link called 'Use NHS 111 online' with a url of 'http://stubs.local.bitraft.io:8080/external/111'
    Then the 111 online page has been opened in a new tab

  Scenario: P5 user can access Health A-Z
    Given I am a patient with proof level 5 who wishes to view the Health A to Z
    And I am logged in
    When I navigate to the symptoms page
    And I click the link called 'Search conditions and treatments' with a url of 'http://stubs.local.bitraft.io:8080/external/healthAtoZ'
    Then the health A to Z page has been opened in a new tab
