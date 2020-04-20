Feature: Proof level 5 shutter screens

  Scenario Outline: A user with proof level 5 is asked to prove identity when they attempt to view <Page Title>
    Given I am a patient with proof level 5
    And I am logged in
    When I retrieve the '<Page>' page directly
    Then the page title is '<Page Title>'
    And I am asked to prove my identity
    Examples:
      | Page               | Page Title                |
      | appointment hub    | Appointments              |
      | gp medical record  | Your GP medical record    |
      | more               | More                      |
      | organ donation     | More                      |
      | your prescriptions | Your repeat prescriptions |
