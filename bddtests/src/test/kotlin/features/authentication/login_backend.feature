@authentication
@authentication-login
@backend
Feature: Login backend

  Tests Im1 Connection Token caching

  Scenario: Logging in with a cached Im1 Connection Token for an EMIS user will remove that token from the cache
    Given I have valid EMIS linkage details and it's the first time a linkage key has been created for my nhs number
    And no IM1 Connection Token is currently cached
    And I call the Linkage POST endpoint
    And I POST to IM1 Connection to register the user
    When I have logged in with the user associated with the IM1 Connection Token
    Then the IM1 Connection Token is no longer in the cache

  Scenario: Logging in with a cached Im1 Connection Token for a TPP user will remove that token from the cache
    Given I have valid TPP linkage details for posting
    And no IM1 Connection Token is currently cached
    And I call the Linkage POST endpoint
    And I POST to IM1 Connection to register the user
    When I have logged in with the user associated with the IM1 Connection Token
    Then the IM1 Connection Token is no longer in the cache

  Scenario: Logging in as a different EMIS user after an Im1 Connection Token is cached won't remove that token
    Given another EMIS user has a new linkage key created for them
    And no IM1 Connection Token is currently cached
    And I call the Linkage POST endpoint
    And the IM1 Connection Token is in the cache
    When I have logged in and have a valid session cookie
    Then the IM1 Connection Token is in the cache

  Scenario: Logging in as a different TPP user after an Im1 Connection Token is cached won't remove that token
    Given another user has valid TPP linkage details
    And no IM1 Connection Token is currently cached
    And I call the Linkage POST endpoint
    And the IM1 Connection Token is in the cache
    When I have logged in and have a valid session cookie
    Then the IM1 Connection Token is in the cache
