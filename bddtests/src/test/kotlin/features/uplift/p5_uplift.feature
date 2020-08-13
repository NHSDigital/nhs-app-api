Feature: Proof level 5 shutter screens

  Scenario Outline: A user with proof level 5 is asked to prove identity when they attempt to view <Page Title>
    Given I am a patient with proof level 5
    And I am logged in
    When I retrieve the '<Page>' page directly
    Then the page title is '<Page Title>'
    And I am asked to prove my identity to access '<Page>'
    When I click the 'Continue' button
    Then the uplift journey starts
    Examples:
      | Page               | Page Title                |
      | appointment hub    | Appointments              |
      | gp medical record  | Your GP health record     |
      | more               | More                      |
      | organ donation     | More                      |
      | your prescriptions | Prescriptions             |

  @native
  Scenario Outline: P5 user accesses <Page Title> shutter page and can use the navigation bar
    Given I am a patient logging in natively with proof level 5
    And I am logged in
    When I retrieve the '<Page>' page directly
    Then the page title is '<Page Title>'
    And I am asked to prove my identity to access '<Page>'
    And the navbar is working
    Examples:
    | Page               | Page Title                |
    | appointment hub    | Appointments              |
    | gp medical record  | Your GP health record     |
    | more               | More                      |
    | organ donation     | More                      |
    | your prescriptions | Prescriptions             |
