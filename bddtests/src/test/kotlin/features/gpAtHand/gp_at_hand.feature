@gpathand
Feature: GP at Hand

  @tech-debt @NHSO-4503
  @appointments
  Scenario: A user sees an appropriate message when the appointments journey configuration is set to Gp at Hand
    Given I am a user where the journey configurations are:
      | Journey            | Value       |
      | appointments       | gpathand    |
    And I am logged in
    When I retrieve the 'My Appointments' page directly
    Then I see an appropriate message informing me that my GP surgery uses the Babylon App for appointments
    When I click the link called 'use the Babylon app' with a url of 'https://www.gpathand.nhs.uk/download-app'
    Then a new tab has been opened by the link

  @tech-debt @NHSO-4505
  @my-record
  Scenario: A user sees an appropriate message when the medical record journey configuration is set to GP at Hand
    Given I am a user where the journey configurations are:
      | Journey            | Value       |
      | medical record     | gpathand    |
    And I am logged in
    When I retrieve the 'My medical record' page directly
    Then I see an appropriate message informing me that my GP surgery uses the Babylon App for my medical record
    When I click the link called 'use the Babylon app' with a url of 'https://www.gpathand.nhs.uk/download-app'
    Then a new tab has been opened by the link

  @prescription
  Scenario: A user sees an appropriate message when the prescriptions journey configuration is set to GP at Hand
    Given I am a user where the journey configurations are:
      | Journey            | Value       |
      | prescriptions      | gpathand    |
    And I am logged in
    When I retrieve the 'My Prescriptions' page directly
    Then I see an appropriate message informing me that my GP surgery uses the Babylon App for prescriptions
    When I click the link called 'use the Babylon app' with a url of 'https://www.gpathand.nhs.uk/download-app'
    Then a new tab has been opened by the link
