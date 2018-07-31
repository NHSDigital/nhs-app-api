Feature: View courses

  In order to view courses associated with a user
  As a logged in user
  I want to see a list of repeat courses that I can order

  @backend
  Scenario Outline: Requesting courses with correct data returns a list of repeat courses that can be requested
    Given <GP System> is initialised
    And I have historic prescriptions
    And I have logged into <GP System> and have a valid session cookie
    And I have 10 <GP System> assigned prescriptions
    And 5 of my prescriptions are of type repeat
    And 2 of my prescriptions can be requested
    When I get the users courses with a valid cookie
    Then I receive a list of 2 repeating prescriptions that can be requested
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
