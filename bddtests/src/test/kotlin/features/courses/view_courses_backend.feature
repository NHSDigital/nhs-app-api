@courses
@prescription
@backend
Feature: View Courses Backend
  In order to view courses associated with a user
  As a logged in user
  I want to see a list of repeat courses that I can order

  Scenario Outline: A <GP System> user requesting courses with correct data returns a list of repeat courses that can be requested
    Given I am a <GP System> patient
    And I have historic prescriptions
    And I have logged in and have a valid session cookie
    And I have 10 assigned prescriptions
    And 5 of my prescriptions are of type repeat
    And 2 of my prescriptions can be requested
    When I get the users courses with a valid cookie
    Then I receive a list of 2 repeating prescriptions that can be requested
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
