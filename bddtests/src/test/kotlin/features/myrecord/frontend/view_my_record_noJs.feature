@my-record
@noJs
Feature: View My Medical Record Information - No Javascript

  Scenario Outline: A <Service> user can view allergies, consultations, demographics and test results
    Given I have disabled javascript
    And the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has enabled allergies functionality and the patient has "2" allergies for <Service>
    And the GP Practice has multiple consultations for <Service>
    And the GP Practice has six test results for <Service>

    When I am on the record warning page
    Then I see the my record warning page
    When I click agree and continue
    Then I see the my medical record page

    Then I see one or more drug type allergies record displayed
    And I see Consultations records displayed
    And I see test result information

    Examples:
      | Service |
      | EMIS    |
      | TPP     |


  Scenario Outline: A <Service> user can view acute, current and discontinued medications
    Given I have disabled javascript
    And the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has enabled medications functionality for <Service>
    And I am on my record information page

    Then I see acute medication information
    And I see current repeat medication information
    And I see discontinued repeat medication information

    Examples:
      | Service |
      | EMIS    |
      | TPP     |
      | VISION  |

  Scenario Outline: A <Service> user can view immunisations and problems
    Given I have disabled javascript
    And the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has enabled immunisations functionality and multiple immunisation records exist for <Service>
    And the GP Practice has enabled problems functionality for <Service>
    And I am on my record information page

    Then I see immunisation records displayed
    And I see Problems records displayed

    Examples:
      | Service |
      | EMIS    |
      | VISION  |

  Scenario: A VISION user can view allergies and demographics
    Given I have disabled javascript
    And the my record wiremocks are initialised for VISION
    And the GP Practice has enabled demographics functionality for VISION
    And the GP Practice has enabled allergies functionality and has a drug and non drug allergy record for VISION
    When I am on the record warning page
    Then I see the my record warning page
    When I click agree and continue
    Then I see the my medical record page
    And I see a drug and non drug allergy record from VISION
