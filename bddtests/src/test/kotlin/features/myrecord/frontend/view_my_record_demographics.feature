@my-record
Feature: View My Medical Record Information - Demographics

  Scenario Outline: A <Service> user with access navigates to the patient record information page
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality
    And I am on the record warning page
    When I click agree and continue
    Then the my record information screen is loaded

    Examples:
      | Service   |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: A <Service> user navigates to patient information page
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality
    And I am on the record warning page
    Then I click agree and continue
    And I see header text is My medical record
    Then I see the My details heading on My Record
    And I see the patient information details
    And I see my record button on the nav bar is highlighted

    Examples:
      | Service   |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: A <Service> user collapses the patient details section
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality
    And I am on my record information page
    When I click My details heading
    Then I do not see patient information details

    Examples:
      | Service   |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |
