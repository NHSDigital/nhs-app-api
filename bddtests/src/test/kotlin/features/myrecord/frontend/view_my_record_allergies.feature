Feature: View My Medical Record Information - Allergies


  @NHSO-677
  @NHSO-1081
  Scenario Outline: A <Service> user can view allergies and adverse reactions section
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And I am on my record information page
    Then I see the Allergies and Adverse Reactions heading
    And I see the Allergies and Adverse Reactions section collapsed

    Examples:
      |Service|
      |EMIS|
      |TPP|


  @NHSO-677
  @NHSO-1081
  Scenario Outline: A <Service> user does not have Summary Care Record access who is disabled at practice-level or patient level
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    But the GP Practice has disabled allergies functionality for <Service>
    And I am on my record information page
    Then I see Service not offered by GP or to specific user or access revoked warning message

    Examples:
      |Service|
      |EMIS|
      |TPP|

  @NHSO-677
  @NHSO-1081
  Scenario Outline: A <Service> user has no allergies on their record
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has enabled allergies functionality and the patient has "0" allergies for <Service>
    And I am on my record information page
    When I click the Allergies and Adverse Reactions section
    Then I see a message indicating I have no allergies

    Examples:
      |Service|
      |EMIS|
      |TPP|

  @NHSO-677
  @NHSO-1081
  Scenario Outline: A <Service> user has one or more allergy records
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has enabled allergies functionality and the patient has "2" allergies for <Service>
    And I am on my record information page
    When I click the Allergies and Adverse Reactions section
    And I see one or more drug type allergies record displayed

    Examples:
      |Service|
      |EMIS|
      |TPP|

  @NHSO-677
  @NHSO-1081
  Scenario Outline: A <Service> user has multiple allergies with different date display formats
    Given the my record wiremocks are initialised for <Service>
    And the GP Practice has enabled demographics functionality for <Service>
    And the GP Practice has enabled allergies functionality and has 5 different allergies with different date formats for <Service>
    And I am on my record information page
    When I click the Allergies and Adverse Reactions section
    And I see 5 allergies with different date formats

    Examples:
      |Service|
      |EMIS|