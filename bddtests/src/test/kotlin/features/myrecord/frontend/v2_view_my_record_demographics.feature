@my-record
Feature: Demographics Frontend - Medical Record v2

  Scenario Outline: A <GP System> user sees their demographics - Medical Record v2
    Given I am a <GP System> user setup to use medical record version 2
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    And I see header text is Your GP medical record
    Then I see the expected demographics information - Medical Record v2
    And I see my record button on the nav bar is highlighted

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |
