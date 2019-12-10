@my-record

Feature: View My Medical Record Information - Combined Frontend

  Scenario Outline: A <GP System> user can view allergies, consultations, demographics and test results
    Given the my record wiremocks are initialised for <GP System>
    And the GP Practice has enabled demographics functionality
    And the GP Practice has enabled allergies functionality and the patient has "2" allergies
    And the GP Practice has multiple consultations
    And the GP Practice has six test results
    When I am on the record warning page
    Then I see the my record warning page
    When I click continue
    Then I see the your medical record page
    When I click the Allergies and adverse reactions section on My Record
    Then I see one or more drug type allergies record displayed
    When I click the Consultations section on My Record
    Then I see Consultations records displayed
    When I click the test result section
    Then I see test result information

    Examples:
      | GP System |
      | EMIS      |
  @smoketest
    Examples:
      | GP System |
      | TPP       |

  Scenario Outline: A <GP System> user can view acute, current and discontinued medications
    Given the my record wiremocks are initialised for <GP System>
    And the GP Practice has enabled demographics functionality
    And the GP Practice has enabled medications functionality
    And I am on my record information page
    When I click the Acute (short-term) medications section on My Record
    Then I see acute medication information
    When I click the Repeat medications: current section on My Record
    Then I see current repeat medication information
    When I click the Repeat medications: discontinued section on My Record
    Then I see discontinued repeat medication information
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
  @smoketest
    Examples:
      | GP System |
      | VISION    |

  Scenario Outline: A <GP System> user can view immunisations and problems
    Given the my record wiremocks are initialised for <GP System>
    And the GP Practice has enabled demographics functionality
    And the GP Practice has enabled immunisations functionality and multiple immunisation records exist
    And the GP Practice has enabled problems functionality
    And I am on my record information page
    When I click the Immunisations section on My Record
    Then I see immunisation records displayed
    When I click the Health conditions section on My Record
    Then I see health condition records displayed
    Examples:
      | GP System |
      | EMIS      |
  @smoketest
    Examples:
      | GP System |
      | VISION    |

  @smoketest
  Scenario: A VISION user can view allergies and demographics
    Given the my record wiremocks are initialised for VISION
    And the GP Practice has enabled demographics functionality
    And the GP Practice has enabled allergies functionality and has a drug and non drug allergy record for VISION
    When I am on the record warning page
    Then I see the my record warning page
    When I click continue
    Then I see the your medical record page
    When I click the Allergies and adverse reactions section on My Record
    Then I see a drug and non drug allergy record from VISION
