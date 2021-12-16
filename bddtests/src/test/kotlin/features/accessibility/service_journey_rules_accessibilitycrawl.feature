@accessibility
@service-journey-rules-accessibility
Feature: service journey rules accessibility

  Scenario: Informatica no appointment booking availability is captured
    Given I am a EMIS user where the journey configurations are:
      | Journey      | Value       |
      | appointments | informatica |
    And  'Informatica' responds to requests for '/andover-medical-practice-appointments-online-co-uk'
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    Then I see an appropriate message informing me that my GP surgery uses Appointments Online
    And the SJR02_Informatica_NoAppointments page is saved to disk

  Scenario: Babylon GP at hand no appointment booking availability is captured
    Given I am a EMIS user where the journey configurations are:
      | Journey            | Value     |
      | appointments       | gpAtHAnd  |
    And 'NHS UK' responds to requests for '/gpathand-download-app'
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    Then I see an appropriate message informing me that my GP surgery uses the Babylon App for appointments
    And the SJR03_BabylonGpAtHand_NoAppointments page is saved to disk

  Scenario: Babylon GP at hand no prescription ordering availability is captured
    Given I am a EMIS user where the journey configurations are:
      | Journey            | Value     |
      | prescriptions      | gpAtHAnd  |
    And 'NHS UK' responds to requests for '/gpathand-download-app'
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    Then I see an appropriate message informing me that my GP surgery uses the Babylon App for prescriptions
    And the SJR13_BabylonGpAtHand_NoPrescriptions page is saved to disk

  Scenario: Babylon GP at hand medical record availability is captured
    Given I am a EMIS user where the journey configurations are:
      | Journey            | Value     |
      | medical record     | gpAtHAnd  |
    And 'NHS UK' responds to requests for '/gpathand-download-app'
    And I am logged in
    When I retrieve the 'Gp Medical Record' page directly
    Then I see an appropriate message informing me that my GP surgery uses the Babylon App for my medical record
    And the SJR23_BabylonGpAtHand_NoMedicalRecord page is saved to disk
