Feature: P5 uplift shutter screens

  Scenario Outline: P5 user attempts to view a P9 <Page> screen
    Given I am patient using the EMIS with proof level 5 access
    And I am logged in
    When I retrieve the '<Page>' page directly
    Then the page title is '<Page Title>'
    And I am told to finish setting up my NHS login
    Examples:
      | Page               | Page Title                |
      | appointment hub    | Appointments              |
      | gp medical record  | Your GP medical record    |
      | more               | More                      |
      | organ donation     | More                      |
      | your prescriptions | Your repeat prescriptions |
