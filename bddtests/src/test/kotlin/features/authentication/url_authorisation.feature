@mobile
@desktop
Feature: Authorisation occurs during each URL visit

  Access to the secure area is only granted to authenticated users.  This feature ensures this occurs when directly navigating using URLs too.

  Background:
    Given wiremock is initialised

  @NHSO-906
  @NHSO-1012
  @smoketest
  Scenario Outline: User has never logged in and attempts to navigate to <URL>
    Given I am not logged in
    When I browse to the page at <URL>
    Then I see the login page

    Examples:
    | URL                           |
    | /                             |
    | /appointments                 |
    | /prescriptions                |
    | /prescriptions/repeat-courses |
    | /more                         |
    | /account                      |


  @NHSO-906
  @NHSO-1012
  Scenario Outline: User has just logged out and attempts to navigate to <URL>
    Given I have just logged out
    And I see the login page
    When I browse to the page at <URL>
    Then I see the login page

    Examples:
    | URL                           |
    | /                             |
    | /appointments                 |
    | /prescriptions                |
    | /prescriptions/repeat-courses |
    | /more                         |
    | /account                      |

  @manual @NHSO-906
  @NHSO-1012
  Scenario Outline: User session has expired and attempts to navigate to <URL>
    Given my session has expired
    When I browse to the page at <URL>
    Then I see the login page

    Examples:
    | URL                           |
    | /                             |
    | /appointments                 |
    | /prescriptions                |
    | /prescriptions/repeat-courses |
    | /more                         |
    | /account                      |

  @NHSO-906
  @NHSO-1012
  @smoketest
  Scenario Outline: User browses to <URL> when logged in
    Given I am logged in
    And I see the home page
    When I browse to the page at <URL>
    Then I see the relevant page

    Examples:
    | URL                           |
    | /                             |
    | /appointments                 |
    | /prescriptions                |
    | /prescriptions/repeat-courses |
    | /more                         |
    | /account                      |

  @manual @native @NHSO-907
  @NHSO-1012
  Scenario Outline: Mobile Web User switches app then browses to <URL>
    Given I am logged in
    And I switch apps
    When I switch to the NHS App
    And I browse to the page at <URL>
    Then I see the relevant page

    Examples:
    | URL                           |
    | /                             |
    | /appointments                 |
    | /prescriptions                |
    | /prescriptions/repeat-courses |
    | /more                         |
    | /account                      |
