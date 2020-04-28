@my-record
Feature: Health Conditions Frontend - Medical Record V2

  Scenario Outline: A <GP System> user has multiple health conditions on their record - Medical Record v2
    Given I am a <GP System> user setup to use medical record version 2
    And the GP Practice has enabled problems functionality
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    When I click the Health conditions link on my record - Medical Record v2
    Then I see the expected health conditions - Medical Record v2

    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  Scenario Outline: A <GP System> user has no health conditions on their record - Medical Record v2
    Given I am a <GP System> user setup to use medical record version 2
    And no Problems records exist for the patient
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    When I click the Health conditions link on my record - Medical Record v2
    Then I see a message that I have no information recorded for a specific record - Medical Record v2

    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  Scenario Outline: A <GP System> user does not have access to their health conditions record - Medical Record v2
    Given I am a <GP System> user setup to use medical record version 2
    And the GP Practice has disabled problems functionality
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    When I click the Health conditions link on my record - Medical Record v2
    Then I see a message indicating that I have no access to view this section on My Record - Medical Record v2

    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  Scenario Outline: A <GP System> user has an error accessing their health conditions - Medical Record v2
    Given I am a <GP System> user setup to use medical record version 2
    And there is an error retrieving Problems data
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    When I click the Health conditions link on my record - Medical Record v2
    Then I see an error occurred message on My Record - Medical Record v2

    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  Scenario Outline: A <GP System> user receiving bad problems data accessing their health conditions sees an error - Medical Record v2
    Given I am a <GP System> user setup to use medical record version 2
    And the GP practice responds with bad problems data
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    When I click the Health conditions link on my record - Medical Record v2
    Then I see an error occurred message on My Record - Medical Record v2

    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  Scenario: A MICROTEST user can view health conditions - Medical Record v2
    Given I am a MICROTEST user setup to use medical record version 2
    And the my record wiremocks are populated
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    When I click the Health conditions link on my record - Medical Record v2
    Then I see the expected health conditions - Medical Record v2

  Scenario: A MICROTEST user has no health conditions on their record - Medical Record v2
    Given I am a MICROTEST user setup to use medical record version 2
    And I have 0 Problems
    And the my record wiremocks are populated
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    When I click the Health conditions link on my record - Medical Record v2
    Then I see a message that this information isn't available through the NHS App - Medical Record v2

  Scenario: An EMIS user has a health condition result with an unknown date - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And the EMIS GP Practice has three problem results where the second record has no date
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    When I click the Health conditions link on my record - Medical Record v2
    Then I see the problem with unknown date displayed last - Medical Record v2
