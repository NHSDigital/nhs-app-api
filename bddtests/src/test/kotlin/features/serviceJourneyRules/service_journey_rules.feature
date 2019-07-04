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

  @backend
  Scenario: API call for SJR will successfully return a response with appointments, medical record and prescriptions
  configured to IM1 and with nominated pharmacy enabled
    Given I am a user where the journey configurations are:
      | Journey            | Value   |
      | appointments       | im1     |
      | medical record     | im1     |
      | prescriptions      | im1     |
      | nominated pharmacy | enabled |
    And I have logged in and have a valid session cookie
    When I request the service journey rules for my ODS Code
    Then I receive an "Ok" success code
    And the service journey rules response will have appointments set to im1
    And the service journey rules response will have nominated pharmacy enabled
    And the service journey rules response will have medical record set to im1
    And the service journey rules response will have prescriptions set to im1

  @backend
  Scenario: API call for SJR will successfully return a response with appointments configured to Informatica Frontdesk
  and with nominated pharmacy disabled
    Given I am a user where the journey configurations are:
      | Journey            | Value       |
      | appointments       | informatica |
      | nominated pharmacy | disabled    |
    And I have logged in and have a valid session cookie
    When I request the service journey rules for my ODS Code
    Then I receive an "Ok" success code
    And the service journey rules response will have appointments set to informatica
    And the service journey rules response will have nominated pharmacy disabled

  @backend
  Scenario: API call for SJR will successfully return a response with appointments, medical record and prescriptions
  configured to GP at Hand
    Given I am a user where the journey configurations are:
      | Journey            | Value       |
      | appointments       | gpAtHand    |
      | medical record     | gpAtHand    |
      | prescriptions      | gpAtHand    |
    And I have logged in and have a valid session cookie
    When I request the service journey rules for my ODS Code
    Then I receive an "Ok" success code
    And the service journey rules response will have appointments set to gpAtHand
    And the service journey rules response will have medical record set to gpAtHand
    And the service journey rules response will have prescriptions set to gpAtHand

  Scenario: A user with appointments configured to Im1 navigates directly to Informatica appointments page and is
  redirected to the Im1 Appointments page
    Given I am a user where the journey configurations are:
      | Journey            | Value       |
      | appointments       | im1         |
    And I am logged in
    When I retrieve the 'Informatica Appointments' page directly
    Then I am redirected to the 'My Appointments' page

  @tech-debt @NHSO-4503
  Scenario: A user with appointments configured to Im1 navigates directly to GP at Hand Appointments page and is
  redirected to the Im1 Appointments page
    Given I am a user where the journey configurations are:
      | Journey            | Value       |
      | appointments       | im1         |
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
      | Journey            | Value       |
      | appointments       | gpAtHand    |
    And I am logged in
    When I retrieve the 'My Appointments' page directly
    Then I am redirected to the 'GP at Hand Appointments' page

  @tech-debt @NHSO-4503
  Scenario: A user with appointments configured to GP at Hand navigates directly to Informatica Appointments page and
  is redirected to the GP at Hand Appointments page
    Given I am a user where the journey configurations are:
      | Journey            | Value       |
      | appointments       | gpAtHand    |
    And I am logged in
    When I retrieve the 'Informatica Appointments' page directly
    Then I am redirected to the 'GP at Hand Appointments' page

  Scenario: A user with prescriptions configured to Im1 navigates directly to GP at Hand Prescriptions page and is
  redirected to the Im1 Prescriptions page
    Given I am a user where the journey configurations are:
      | Journey            | Value       |
      | prescriptions      | im1         |
    And I am logged in
    When I retrieve the 'Gp at Hand Prescriptions' page directly
    Then I am redirected to the 'My Prescriptions' page

  Scenario: A user with prescriptions configured to GP at Hand navigates directly to Im1 Prescriptions page and is
  redirected to the GP at Hand Prescriptions page
    Given I am a user where the journey configurations are:
      | Journey            | Value       |
      | prescriptions      | gpAtHand    |
    And I am logged in
    When I retrieve the 'My Prescriptions' page directly
    Then I am redirected to the 'GP at Hand Prescriptions' page

  @tech-debt @NHSO-4505
  Scenario: A user with medical record configured to Im1 navigates directly to GP at Hand Medical Record page and is
  redirected to the Im1 Medical Record page
    Given I am a user where the journey configurations are:
      | Journey            | Value       |
      | medical record     | im1         |
    And I am logged in
    When I retrieve the 'GP at Hand Medical Record' page directly
    Then I am redirected to the 'My Record' page

  @tech-debt @NHSO-4505
  Scenario: A user with medical record configured to GP at Hand navigates directly to Im1 Medical Record page and is
  redirected to the GP at Hand Medical Record page
    Given I am a user where the journey configurations are:
      | Journey            | Value       |
      | medical record     | gpAtHand         |
    And I am logged in
    When I retrieve the 'My Record' page directly
    Then I am redirected to the 'GP at Hand Medical Record' page
