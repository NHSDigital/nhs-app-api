@my-record
@smoketest
  @target
Feature: View My Medical Record Information

  Scenario Outline: A <Service> user can view allergies, consultations, demographics and test results
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has enabled allergies functionality and the patient has "2" allergies for <Service>
    And the GP Practice has multiple consultations for <Service>
    And the GP Practice has six test results for <Service>

    And I am on the record warning page
    Then I see the my record warning page

    When I click agree and continue
    Then I see header text is My medical record
    And I see heading My details
    And I see the patient information details
    And I see my record button on the nav bar is highlighted

    When I click the Allergies and Adverse Reactions section
    Then I see one or more drug type allergies record displayed

    When I click the Consultations section <Service>
    Then I see Consultations records displayed <Service>

    When I click the test result section
    Then I see test result information

    Examples:
      |Service|
      |EMIS|
      |TPP|


  Scenario Outline: A <Service> user can view acute, current and discontinued medications
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has enabled medications functionality for <Service>
    And I am on my record information page
    And I see heading Acute medications
    When I click acute medications
    Then I see acute medication information
    When I click current repeat medications
    Then I see current repeat medication information
    When I click discontinued repeat medications
    Then I see discontinued repeat medication information

    Examples:
      | Service |
      | EMIS    |
      | TPP     |


  Scenario Outline: A <Service> user can view immunisations and problems
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has enabled immunisations functionality and multiple immunisation records exist for <Service>
    And the GP Practice has enabled problems functionality for <Service>
    And I am on my record information page
    And I see heading Immunisations
    When I click the Immunisations section
    Then I see immunisation records displayed
    And I see heading Problems
    When I click the Problems section
    Then I see Problems records displayed

    Examples:
      | Service |
      | EMIS    |
