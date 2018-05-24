Feature: Authorisation occurs during each URL visit

  Access to the secure area is only granted to authenticated users.  This feature ensures this occurs when directly navigating using URLs too.

  @NHSO-906
  @tech-debt @NHSO-1012
  @pending
  Scenario: User has never logged in and attempts to navigate to secure routes
    Given I am not logged in
    When I browse to the page at
    | URL                          |
    | /                            |
    | /appointments                |
    | /prescriptions               |
    | /repeat-prescription-courses |
    | /more                        |
    | /account                     |
    | /myrecord                    |
    Then I see the login page

  @NHSO-906
  @pending
  Scenario: User has just logged out and attempts to navigate to secure routes
    Given I have just logged out
    When I browse to the page at
      | URL                          |
      | /                            |
      | /appointments                |
      | /prescriptions               |
      | /repeat-prescription-courses |
      | /more                        |
      | /account                     |
      | /myrecord                    |
    Then I see the login page

  @manual @NHSO-906
  Scenario: User session has expired and attempts to navigate to secure routes
    Given my session has expired
    When I browse to the page at
      | URL                          |
      | /                            |
      | /appointments                |
      | /prescriptions               |
      | /repeat-prescription-courses |
      | /more                        |
      | /account                     |
      | /myrecord                    |
    Then I see the login page

  @NHSO-906
  @pending
  Scenario: User browses to deep link when logged in
    Given I am logged in
    When I browse to the page at
      | URL                          |
      | /                            |
      | /appointments                |
      | /prescriptions               |
      | /repeat-prescription-courses |
      | /more                        |
      | /account                     |
      | /myrecord                    |
    Then I see the relevant page

  @manual @native @NHSO-907
  Scenario: Mobile Web User switches app then browses to a specific URL
    Given I am logged in
    And I switch apps
    When I switch to the NHS App
    And I browse to the page at
      | URL                          |
      | /                            |
      | /appointments                |
      | /prescriptions               |
      | /repeat-prescription-courses |
      | /more                        |
      | /account                     |
      | /myrecord                    |
    Then I see the relevant page