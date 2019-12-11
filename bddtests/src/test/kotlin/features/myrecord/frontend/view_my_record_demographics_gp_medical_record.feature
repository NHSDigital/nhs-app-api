@my-record
Feature: Demographics Frontend - Medical Record v2

  Scenario Outline: A <GP System> user sees their demographics - GP Medical Record
    Given I am a <GP System> user setup to use medical record version 2
    And I am on the record warning page
    Then I click continue
    And I see header text is Your GP medical record
    Then I see the expected demographics information - GP Medical Record
    And I see my record button on the nav bar is highlighted

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |