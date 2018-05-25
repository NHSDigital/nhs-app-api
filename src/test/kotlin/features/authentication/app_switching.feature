@native
Feature: Switching Apps on Mobile Devices

  If a user switches apps whilst using their device, the app maintains integrity in a variety of different ways

  @NHSO-893  
  @pending
  Scenario Outline: User is on a page and switches apps
    Given I am on the <start page> page
    And I switch apps
    When I switch to the NHS App
    Then I see the <end page> page

    Examples:
    | start page    | end page      |
    | symptoms      | symptoms      |
    | appointments  | appointments  |
    | prescriptions | prescriptions |
    | my record     | my record     |
    | more          | more          |


  @NHSO-893
  @manual
  Scenario Outline: User switches back to app after session times out
    Given I am on the <start page> page
    And I switch apps
    And I wait for 20 minutes
    When I switch to the NHS App
    Then I see the <end page> page

    Examples:
      | start page    | end page      |
      | symptoms      | symptoms      |
      | appointments  | login         |
      | prescriptions | login         |
      | my record     | login         |
      | more          | login         |

  @NHSO-893
  @manual
  Scenario: User switches back to app after session times out without an internet connection
    Given I am on the appointments page
    And I switch apps
    And I wait for 20 minutes
    And I lose connection
    When I switch to the NHS App
    Then I see the session expired screen