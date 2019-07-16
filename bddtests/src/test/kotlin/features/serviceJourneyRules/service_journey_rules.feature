@service-journey-rules
Feature: Service Journey Rules
  #This endpoint is GPSystem agnostic

  @backend
  Scenario: A user fetching the service journey rules for an ODS code not configured will receive a 404
    Given I am a user whose ODS Code does not have specific journey configuration set up
    When I login but service journey rules has no configuration for my GP practice
    Then I receive a "Not Found" error

  @long-running
  @backend
  Scenario: A user fetching the service journey rules but the session has expired, will receive Unauthorized
    Given I am a user whose ODS Code has a specific journey configuration set up
    And I have logged in and have a valid session cookie
    And My session has expired
    When I request the service journey rules for my ODS Code
    Then I receive a "Unauthorized" error

  Scenario: A user with appointments configured to Im1 navigates directly to Informatica appointments page and is
  redirected to the Im1 Appointments page
    Given I am a user where the journey configurations are:
      | Journey            | Value   |
      | appointments       | im1     |
    And I am logged in
    When I retrieve the 'Informatica Appointments' page directly
    Then I am redirected to the 'My Appointments' page

  @tech-debt @NHSO-4503
  Scenario: A user with appointments configured to Im1 navigates directly to GP at Hand Appointments page and is
  redirected to the Im1 Appointments page
    Given I am a user where the journey configurations are:
      | Journey            | Value   |
      | appointments       | im1     |
    And I am logged in
    When I retrieve the 'GP at Hand Appointments' page directly
    Then I am redirected to the 'My Appointments' page

  Scenario: A user with appointments configured to Informatica navigates directly to Im1 Appointments page and is
  redirected to the Informatica Appointments page
    Given I am a user where the journey configurations are:
      | Journey            | Value       |
      | appointments       | informatica |
    And I am logged in
    When I retrieve the 'My Appointments' page directly
    Then I am redirected to the 'Informatica Appointments' page

  @tech-debt @NHSO-4503
  Scenario: A user with appointments configured to Informatica navigates directly to GP at Hand Appointments page and
  is redirected to the Informatica Appointments page
    Given I am a user where the journey configurations are:
      | Journey            | Value       |
      | appointments       | informatica |
    And I am logged in
    When I retrieve the 'GP at Hand Appointments' page directly
    Then I am redirected to the 'Informatica Appointments' page

  @tech-debt @NHSO-4503
  Scenario: A user with appointments configured to GP at Hand navigates directly to Im1 Appointments page and is
  redirected to the GP at Hand Appointments page
    Given I am a user where the journey configurations are:
      | Journey            | Value     |
      | appointments       | gpAtHand  |
    And I am logged in
    When I retrieve the 'My Appointments' page directly
    Then I am redirected to the 'GP at Hand Appointments' page

  @tech-debt @NHSO-4503
  Scenario: A user with appointments configured to GP at Hand navigates directly to Informatica Appointments page and
  is redirected to the GP at Hand Appointments page
    Given I am a user where the journey configurations are:
      | Journey            | Value     |
      | appointments       | gpAtHand  |
    And I am logged in
    When I retrieve the 'Informatica Appointments' page directly
    Then I am redirected to the 'GP at Hand Appointments' page

  Scenario: A user with medical record configured to Im1 navigates directly to GP at Hand Medical Record page and is
  redirected to the Im1 Medical Record page
    Given I am a user where the journey configurations are:
      | Journey            | Value     |
      | medical record     | im1       |
    And I am logged in
    When I retrieve the 'GP at Hand My Record' page directly
    Then I am redirected to the 'My Record' page

  Scenario: A user with medical record configured to GP at Hand navigates directly to Im1 Medical Record page and is
  redirected to the GP at Hand Medical Record page
    Given I am a user where the journey configurations are:
      | Journey            | Value     |
      | medical record     | gpAtHAnd  |
    And I am logged in
    When I retrieve the 'My Record' page directly
    Then I am redirected to the 'GP at Hand My Record' page

  Scenario: A user with prescriptions configured to Im1 navigates directly to GP at Hand Prescriptions page and is
  redirected to the Im1 Prescriptions page
    Given I am a user where the journey configurations are:
      | Journey            | Value     |
      | prescriptions      | im1       |
    And I am logged in
    When I retrieve the 'Gp at Hand Prescriptions' page directly
    Then I am redirected to the 'My Prescriptions' page

  Scenario: A user with prescriptions configured to GP at Hand navigates directly to Im1 Prescriptions page and is
  redirected to the GP at Hand Prescriptions page
    Given I am a user where the journey configurations are:
      | Journey            | Value     |
      | prescriptions      | gpAtHand  |
    And I am logged in
    When I retrieve the 'My Prescriptions' page directly
    Then I am redirected to the 'GP at Hand Prescriptions' page
