@prescription
Feature: View prescriptions
  A user can view information about their prescriptions after logging in

  #This test covers navigation via buttons/links

  @nativesmoketest
  Scenario Outline: <GP System> patient selects the prescriptions menu button
    Given I am patient using the <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And I am logged in
    And I navigate to prescriptions
    Then I see prescriptions page loaded
    And the prescriptions menu button is highlighted on mobile
    Examples:
      | GP System |
      | EMIS      |

  # These tests navigate directly to the pages where the features are to be tested, to save time.

  @nativesmoketest
  Scenario Outline: <GP System> patient with no past repeat prescriptions
    Given I am patient using the <GP System> GP System
    And I have 0 past repeat prescriptions
    And each repeat prescription contains 0 courses of which 0 are repeats
    And I am logged in
    When I retrieve the 'My Prescriptions' page directly
    Then I see no prescriptions
    And I see a message indicating that I have no repeat prescriptions
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: <GP System> patient who has prescriptions totalling more than one hundred courses
    Given I am patient using the <GP System> GP System
    And I have 110 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And I am logged in
    When I retrieve the 'My Prescriptions' page directly
    Then I see 100 prescriptions
    Examples:
      | GP System |
      | EMIS      |

  @smoketest
  @nativesmoketest
  Scenario Outline: <GP System> patient who has multiple prescription each containing one course
    Given I am patient using the <GP System> GP System
    And I have 3 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And I am logged in
    When I retrieve the 'My Prescriptions' page directly
    Then I see 3 prescriptions
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: <GP System> patient who has multiple prescription each containing the same repeat prescription
    Given I am patient using the <GP System> GP System
    And I have 3 past repeat prescriptions
    And each repeat prescription shares the same course
    And I am logged in
    When I retrieve the 'My Prescriptions' page directly
    Then I see 3 prescriptions
    Examples:
      | GP System |
      | TPP       |

  Scenario Outline: <GP System> patient who has only one prescription containing multiple courses
    Given I am patient using the <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 3 courses of which 3 are repeats
    And I am logged in
    When I retrieve the 'My Prescriptions' page directly
    Then I see 3 prescriptions
    Examples:
      | GP System |
      | EMIS      |

  Scenario: EMIS patient who has acute prescriptions
    Given I am patient using the EMIS GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 3 courses of which 2 are repeats
    And I am logged in
    When I retrieve the 'My Prescriptions' page directly
    Then I see 2 prescriptions

  @nativesmoketest
  Scenario Outline: The <GP System> User clicks on the Prescriptions button and the service is disabled at a GP Practice level
    Given I am patient using the <GP System> GP System
    Given prescriptions is disabled at a GP Practice level
    And I am logged in
    When I retrieve the 'My Prescriptions' page directly
    Then I see a message informing me that I don't currently have access to this service
    Examples:
      | GP System |
      | TPP       |

  Scenario Outline: A <GP System> user with historic prescriptions with missing quantity info
    Given I am patient using the <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And each course has only dosage info
    And I am logged in
    When I retrieve the 'My Prescriptions' page directly
    Then I see 1 prescriptions
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: A <GP System> user with historic prescriptions with missing dosage info
    Given I am patient using the <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And each course has only quantity info
    And I am logged in
    When I retrieve the 'My Prescriptions' page directly
    Then I see 1 prescriptions
    Examples:
      | GP System |
      | EMIS      |

  Scenario: VISION user with historic prescriptions with missing dosage and quantity info
    Given I am patient using the VISION GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And each course has no info
    And I am logged in
    When I retrieve the 'My Prescriptions' page directly
    Then I see 1 prescriptions

  Scenario: A user who has multiple prescriptions but medication status should not be displayed
    Given I am patient using the EMIS GP System
    And I have 6 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And courses have status
      | Issued              |
      | Requested           |
      | ForwardedForSigning |
      | Rejected            |
      | Unknown             |
      | Cancelled           |
    And I am logged in
    When I retrieve the 'My Prescriptions' page directly
    Then I see 4 prescriptions

