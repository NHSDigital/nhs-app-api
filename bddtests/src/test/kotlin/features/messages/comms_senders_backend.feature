@comms-senders
@backend
Feature: Comms Senders Backend

  Scenario: An api user can get sender details from senders container
    Given I am an api user wishing to get the sender details existing in database
    When I get sender details based on an sender id
    Then I receive an "OK" success code
    And I receive a sender data from senders endpoint

  Scenario: An api user that has no details stored getting sender details on the senders endpoint will receive a 404
    Given I am an api user without stored details wishing to get sender details
    When I get sender details for non-existing sender id
    Then I receive a "Not Found" error

  Scenario: An api user getting sender details on the senders endpoint without the api key will receive a 401
    Given I am an api user wishing to get the sender details existing in database
    When I get sender details based on sender id without the api key
    Then I receive a "Unauthorized" error

  Scenario: An api user can post sender details to senders container
    Given I am an api user wishing to submit sender details to the senders endpoint
    When I post to the senders endpoint with valid body
    Then I receive an "Created" success code
    And I receive a created sender details response from senders endpoint

  Scenario: An api user can post sender details to the senders endpoint without an api key will receive a 401
    Given I am an api user wishing to submit sender details to the senders endpoint
    When I post to the senders endpoint without an api key
    Then I receive an "Unauthorized" error

  Scenario: An api user can post sender details to the senders endpoint without an name receive a 400
    Given I am an api user wishing to submit sender details to the senders endpoint
    When I post to the senders endpoint without name
    Then I receive an "Bad Request" error

  Scenario: An api user can get stale senders from senders container
    Given I am an api user wishing to get senders existing in database whose details have not been recently updated
    When I get sender details based on when they were last updated
    Then I receive an "OK" success code
    And I receive ids of senders not recently updated from senders endpoint

  Scenario: An api user with no stale senders getting stale senders on the senders endpoint will receive a 200
    Given I am an api user without stale senders stored details wishing to get stale senders
    When I get sender details based on when they were last updated
    Then I receive an "OK" success code
    And I receive an empty list of sender ids not recently updated from senders endpoint

  Scenario: An api user getting stale senders on the senders endpoint without the api key will receive a 401
    Given I am an api user wishing to get senders existing in database whose details have not been recently updated
    When I get sender details based on when they were last updated without the api key
    Then I receive a "Unauthorized" error
