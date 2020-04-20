Feature: Services available with P5 Access

  Scenario: P5 User can access their account settings
    Given I am a patient with proof level 5
    And I am logged in
    When I navigate to the account page
    Then the Account page is displayed

  Scenario: P5 user can access 111 Online
    Given I am a patient with proof level 5
    And I am logged in
    When I navigate to the symptoms page
    And I click the link called 'Use NHS 111 online' with a url of 'https://111.nhs.uk/'
    Then a new tab has been opened by the link

  Scenario: P5 user can access Health A-Z
    Given I am a patient with proof level 5
    And I am logged in
    When I navigate to the symptoms page
    And I click the link called 'Search conditions and treatments' with a url of 'https://www.nhs.uk/conditions/'
    Then a new tab has been opened by the link
