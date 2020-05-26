@userinfo
@backend
Feature: User Info User Research Backend

  Scenario: An api user can post their user research preference of Opt In to the user info endpoint
    Given I am an api user wishing to post my user research preference 'OptIn'
    And I have logged in and have a valid session cookie
    When I post my user research preference to the user info endpoint
    Then I receive a "No Content" success code

  Scenario: An api user can post their user research preference of Opt Out to the user info endpoint
    Given I am an api user wishing to post my user research preference 'OptOut'
    And I have logged in and have a valid session cookie
    When I post my user research preference to the user info endpoint
    Then I receive a "No Content" success code

  Scenario: An api user posting their user research preferences without an access token will receive a 401
    Given I am an api user wishing to post my user research preference 'OptIn'
    And I have logged in and have a valid session cookie
    When I post my user research preference to the user info endpoint without an access token
    Then I receive an "Unauthorized" error

  Scenario: An api user posting their user research preferences with an invalid access token will receive a 401
    Given I am an api user wishing to post my user research preference 'OptIn'
    And I have logged in and have a valid session cookie
    Then posting user research preferences with an invalid access token will return an Unauthorised error

  Scenario: An api user posting their user research preferences when qualtrics returns an error will receive a 502
    Given I am an api user wishing to post my user research preference 'OptIn' but qualtrics will return an error
    And I have logged in and have a valid session cookie
    When I post my user research preference to the user info endpoint
    Then I receive a "Bad Gateway" error

  Scenario: An api user posting their user research preferences when no email can be found will receive a 502
    Given I am an api user wishing to post my user research preference 'OptIn' but without an email
    And I have logged in and have a valid session cookie
    When I post my user research preference to the user info endpoint
    Then I receive a "Bad Gateway" error