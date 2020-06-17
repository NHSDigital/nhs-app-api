@my-record
Feature: Combined Frontend - Medical Record v2

  Scenario Outline: A <GP System> user can view medicines, consultations, and test results - Medical Record v2
    Given I am a <GP System> user setup to use medical record version 2
    And the GP Practice has enabled all medical records for the patient
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    Then I see the medical record v2 page
    When I click the Medicines link on my record - Medical Record v2
    Then I see the medical record v2 medicines page
    When I click the Acute medicines link - Medical Record v2
    Then I see the expected acute medicines - Medical Record v2
    When I click the Back link
    Then I see the medical record v2 medicines page
    When I click the Back link
    And I click the Consultations and events link on my record - Medical Record v2
    Then I see the expected consultations and events - Medical Record v2
    When I click the Back link
    And I click the Test results link on my record - Medical Record v2
    Then I see the correct number of test results for current the supplier - Medical Record v2
    Examples:
      | GP System |
      | EMIS      |
  @smoketest
    Examples:
      | GP System |
      | TPP       |

  Scenario Outline: A <GP System> user can view their med record with bad medicines and consultations data and OK test results data - Medical Record v2
    Given I am a <GP System> user setup to use medical record version 2
    And The GP practice responds with bad medications data
    And the GP Practice has six test results
    And the GP practice returns bad consultations data
    And I am logged in
    And I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    Then I see the medical record v2 page
    When I click the Medicines link on my record - Medical Record v2
    Then I see the medical record v2 medicines page
    When I click the Acute medicines link - Medical Record v2
    Then I see an error occurred message on My Record - Medical Record v2
    When I click the Back link
    Then I see the medical record v2 medicines page
    When I click the Back link
    And I click the Consultations and events link on my record - Medical Record v2
    Then I see an error occurred message on My Record - Medical Record v2
    When I click the Back link
    And I click the Test results link on my record - Medical Record v2
    Then I see 6 test results - Medical Record v2
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: A <GP System> user can view immunisations and problems - Medical Record v2
    Given I am a <GP System> user setup to use medical record version 2
    And the GP Practice has enabled all medical records for the patient
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Immunisations link on my record - Medical Record v2
    Then I see the expected immunisations - Medical Record v2
    When I click the Back link
    And I click the Health conditions link on my record - Medical Record v2
    Then I see the expected health conditions - Medical Record v2
    Examples:
      | GP System |
      | EMIS      |
    @smoketest
    Examples:
      | GP System |
      | VISION    |

  Scenario: A TPP user cannot view their record before accepting the warning - Medical Record v2
    Given I am a TPP user setup to use medical record version 2
    And the GP Practice has enabled all medical records for the patient
    And I am logged in
    Then retrieving the Medical Record pages directly displays the Medical Record Warning page
      |/health-records/gp-medical-record/allergies-and-reactions                 |
      |/health-records/gp-medical-record/medicines                               |
      |/health-records/gp-medical-record/medicines/acute-medicines               |
      |/health-records/gp-medical-record/medicines/current-medicines             |
      |/health-records/gp-medical-record/medicines/discontinued-medicines        |
      |/health-records/gp-medical-record/immunisations                           |
      |/health-records/gp-medical-record/health-conditions                       |
      |/health-records/gp-medical-record/test-results                            |
      |/health-records/gp-medical-record/events                                  |
      |/health-records/gp-medical-record/documents                               |
      |/health-records/gp-medical-record/documents/detail                        |

  Scenario: A EMIS user cannot view their record before accepting the warning - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has enabled all medical records for the patient
    And I am logged in
    Then retrieving the Medical Record pages directly displays the Medical Record Warning page
      |/health-records/gp-medical-record/allergies-and-reactions                 |
      |/health-records/gp-medical-record/medicines                               |
      |/health-records/gp-medical-record/medicines/acute-medicines               |
      |/health-records/gp-medical-record/medicines/current-medicines             |
      |/health-records/gp-medical-record/medicines/discontinued-medicines        |
      |/health-records/gp-medical-record/immunisations                           |
      |/health-records/gp-medical-record/health-conditions                       |
      |/health-records/gp-medical-record/test-results                            |
      |/health-records/gp-medical-record/consultations                           |
      |/health-records/gp-medical-record/documents                               |
      |/health-records/gp-medical-record/documents/detail                        |

  Scenario: A VISION user cannot view their record before accepting the warning - Medical Record v2
    Given I am a VISION user setup to use medical record version 2
    And the GP Practice has enabled all medical records for the patient
    And I am logged in
    Then retrieving the Medical Record pages directly displays the Medical Record Warning page
      |/health-records/gp-medical-record/allergies-and-reactions                 |
      |/health-records/gp-medical-record/medicines                               |
      |/health-records/gp-medical-record/medicines/acute-medicines               |
      |/health-records/gp-medical-record/medicines/current-medicines             |
      |/health-records/gp-medical-record/medicines/discontinued-medicines        |
      |/health-records/gp-medical-record/immunisations                           |
      |/health-records/gp-medical-record/health-conditions                       |
      |/health-records/gp-medical-record/test-results                            |
      |/health-records/gp-medical-record/diagnosis                               |
      |/health-records/gp-medical-record/examinations                            |
      |/health-records/gp-medical-record/procedures                              |

  Scenario: A MICROTEST user cannot view their record before accepting the warning - Medical Record v2
    Given I am a MICROTEST user setup to use medical record version 2
    And the GP Practice has enabled all medical records for the patient
    And I am logged in
    Then retrieving the Medical Record pages directly displays the Medical Record Warning page
      |/health-records/gp-medical-record/allergies-and-reactions                 |
      |/health-records/gp-medical-record/medicines                               |
      |/health-records/gp-medical-record/medicines/acute-medicines               |
      |/health-records/gp-medical-record/medicines/current-medicines             |
      |/health-records/gp-medical-record/medicines/discontinued-medicines        |
      |/health-records/gp-medical-record/immunisations                           |
      |/health-records/gp-medical-record/health-conditions                       |
      |/health-records/gp-medical-record/test-results                            |
      |/health-records/gp-medical-record/medical-history                         |
      |/health-records/gp-medical-record/recalls                                 |
      |/health-records/gp-medical-record/encounters                              |
      |/health-records/gp-medical-record/referrals                               |

  Scenario: A EMIS user attempting to view pages not provided by their supplier will be directed to the Medical Record main page - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has enabled all medical records for the patient
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    Then retrieving the Medical Record pages directly displays the Medical Record main page
      |/health-records/gp-medical-record/medical-history                         |
      |/health-records/gp-medical-record/recalls                                 |
      |/health-records/gp-medical-record/encounters                              |
      |/health-records/gp-medical-record/referrals                               |
      |/health-records/gp-medical-record/diagnosis                               |
      |/health-records/gp-medical-record/examinations                            |
      |/health-records/gp-medical-record/procedures                              |
      |/health-records/gp-medical-record/events                                  |

  Scenario: A TPP user attempting to view pages not provided by their supplier will be directed to the Medical Record main page - Medical Record v2
    Given I am a TPP user setup to use medical record version 2
    And the GP Practice has enabled all medical records for the patient
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    Then retrieving the Medical Record pages directly displays the Medical Record main page
      |/health-records/gp-medical-record/immunisations                           |
      |/health-records/gp-medical-record/health-conditions                       |
      |/health-records/gp-medical-record/consultations                           |
      |/health-records/gp-medical-record/medical-history                         |
      |/health-records/gp-medical-record/recalls                                 |
      |/health-records/gp-medical-record/encounters                              |
      |/health-records/gp-medical-record/referrals                               |
      |/health-records/gp-medical-record/diagnosis                               |
      |/health-records/gp-medical-record/examinations                            |
      |/health-records/gp-medical-record/procedures                              |

  Scenario: A VISION user attempting to view pages not provided by their supplier will be directed to the Medical Record main page - Medical Record v2
    Given I am a VISION user setup to use medical record version 2
    And the GP Practice has enabled all medical records for the patient
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    Then retrieving the Medical Record pages directly displays the Medical Record main page
      |/health-records/gp-medical-record/consultations                           |
      |/health-records/gp-medical-record/medical-history                         |
      |/health-records/gp-medical-record/recalls                                 |
      |/health-records/gp-medical-record/encounters                              |
      |/health-records/gp-medical-record/referrals                               |
      |/health-records/gp-medical-record/events                                  |
      |/health-records/gp-medical-record/documents                               |

  Scenario: A MICROTEST user attempting to view pages not provided by their supplier will be directed to the Medical Record main page - Medical Record v2
    Given I am a MICROTEST user setup to use medical record version 2
    And the GP Practice has enabled all medical records for the patient
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    Then retrieving the Medical Record pages directly displays the Medical Record main page
      |/health-records/gp-medical-record/consultations                           |
      |/health-records/gp-medical-record/diagnosis                               |
      |/health-records/gp-medical-record/examinations                            |
      |/health-records/gp-medical-record/procedures                              |
      |/health-records/gp-medical-record/events                                  |
      |/health-records/gp-medical-record/documents                               |
