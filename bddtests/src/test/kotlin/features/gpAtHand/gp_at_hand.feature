@gpathand
Feature: GP at Hand Frontend

  @appointments
  Scenario: A user sees an appropriate message when the appointments journey configuration is set to Gp at Hand
    Given I am a EMIS user where the journey configurations are:
      | Journey            | Value     |
      | appointments       | gpAtHAnd  |
    And I am logged in
    When I retrieve the 'Your GP Appointments' page directly
    Then I see an appropriate message informing me that my GP surgery uses the Babylon App for appointments
    When I click the link called 'use the Babylon app' with a url of 'https://www.gpathand.nhs.uk/download-app'
    Then a new tab has been opened by the link

  @my-record
  Scenario: A user sees an appropriate message when the medical record journey configuration is set to GP at Hand
    Given I am a EMIS user where the journey configurations are:
      | Journey            | Value     |
      | medical record     | gpAtHAnd  |
    And I am logged in
    When I retrieve the 'Gp Medical Record' page directly
    Then I see an appropriate message informing me that my GP surgery uses the Babylon App for my medical record
    When I click the link called 'use the Babylon app' with a url of 'https://www.gpathand.nhs.uk/download-app'
    Then a new tab has been opened by the link

  @prescription
  Scenario: A user sees an appropriate message when the prescriptions journey configuration is set to GP at Hand
    Given I am a EMIS user where the journey configurations are:
      | Journey            | Value     |
      | prescriptions      | gpAtHAnd  |
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    Then I see an appropriate message informing me that my GP surgery uses the Babylon App for prescriptions
    When I click the link called 'use the Babylon app' with a url of 'https://www.gpathand.nhs.uk/download-app'
    Then a new tab has been opened by the link
