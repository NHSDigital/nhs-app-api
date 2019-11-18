@service-journey-rules
Feature: Service Journey Rules Frontend
  #This endpoint is GPSystem agnostic

  Scenario: A user with appointments configured to Im1 navigates directly to Informatica appointments page and is redirected to the Im1 Appointments page
    Given I am a EMIS user where the journey configurations are:
      | Journey            | Value   |
      | appointments       | im1     |
    And I am logged in
    When I retrieve the 'Informatica Appointments' page directly
    Then I am redirected to the 'Your Appointments' page

  Scenario: A user with appointments configured to Im1 navigates directly to GP at Hand Appointments page and is redirected to the Im1 Appointments page
    Given I am a EMIS user where the journey configurations are:
      | Journey            | Value   |
      | appointments       | im1     |
    And I am logged in
    When I retrieve the 'GP at Hand Appointments' page directly
    Then I am redirected to the 'Your Appointments' page

  Scenario: A user with appointments configured to Informatica navigates directly to Im1 Appointments page and is redirected to the Informatica Appointments page
    Given I am a EMIS user where the journey configurations are:
      | Journey            | Value       |
      | appointments       | informatica |
    And I am logged in
    When I retrieve the 'Your Appointments' page directly
    Then I am redirected to the 'Informatica Appointments' page

  Scenario: A user with appointments configured to Informatica navigates directly to GP at Hand Appointments page and is redirected to the Informatica Appointments page
    Given I am a EMIS user where the journey configurations are:
      | Journey            | Value       |
      | appointments       | informatica |
    And I am logged in
    When I retrieve the 'GP at Hand Appointments' page directly
    Then I am redirected to the 'Informatica Appointments' page

  Scenario: A user with appointments configured to GP at Hand navigates directly to Im1 Appointments page and is redirected to the GP at Hand Appointments page
    Given I am a EMIS user where the journey configurations are:
      | Journey            | Value     |
      | appointments       | gpAtHand  |
    And I am logged in
    When I retrieve the 'Your Appointments' page directly
    Then I am redirected to the 'GP at Hand Appointments' page

  Scenario: A user with appointments configured to GP at Hand navigates directly to Informatica Appointments page and is redirected to the GP at Hand Appointments page
    Given I am a EMIS user where the journey configurations are:
      | Journey            | Value     |
      | appointments       | gpAtHand  |
    And I am logged in
    When I retrieve the 'Informatica Appointments' page directly
    Then I am redirected to the 'GP at Hand Appointments' page

  Scenario: A user with medical record configured to Im1 navigates directly to GP at Hand Medical Record page and is redirected to the Im1 Medical Record page
    Given I am a EMIS user where the journey configurations are:
      | Journey            | Value     |
      | medical record     | im1       |
    And I am logged in
    When I retrieve the 'GP at Hand My Record' page directly
    Then I am redirected to the 'My Record' page

  Scenario: A user with medical record configured to medical records v2 navigates from the navigation panel.
    Given I am a EMIS user where the journey configurations are:
      | Journey            | Value     |
      | medical record version    | 2       |
    And I am logged in
    When I retrieve the 'My Record' page directly
    Then I am redirected to the 'My GP medical record' page

  Scenario: A user with medical record configured to GP at Hand navigates directly to Im1 Medical Record page and is redirected to the GP at Hand Medical Record page
    Given I am a EMIS user where the journey configurations are:
      | Journey            | Value     |
      | medical record     | gpAtHand  |
    And I am logged in
    When I retrieve the 'My Record' page directly
    Then I am redirected to the 'GP at Hand My Record' page

  Scenario: A user with prescriptions configured to Im1 navigates directly to GP at Hand Prescriptions page and is redirected to the Im1 Prescriptions page
    Given I am a EMIS user where the journey configurations are:
      | Journey            | Value     |
      | prescriptions      | im1       |
    And I am logged in
    When I retrieve the 'Gp at Hand Prescriptions' page directly
    Then I am redirected to the 'Your Prescriptions' page

  Scenario: A user with prescriptions configured to GP at Hand navigates directly to Im1 Prescriptions page and is redirected to the GP at Hand Prescriptions page
    Given I am a EMIS user where the journey configurations are:
      | Journey            | Value     |
      | prescriptions      | gpAtHand  |
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    Then I am redirected to the 'GP at Hand Prescriptions' page
