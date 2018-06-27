Feature: Registration

  A user can create a new NHS account from the login page, allowing them to access the app

  @backend
  Scenario Outline: Patient registers for an EMIS account with NHS Numbers of <NHS Numbers>
    Given I have a new patient with Nhs Numbers of <NHS Numbers>
    When I register an EMIS user's IM1 credentials
    Then I receive a response
    And the response has the expected connection token
    And the response has the expected NHS numbers

    Examples:
    |NHS Numbers|
    |           |
    |"one"      |
    |"one","two"|

  @bug
  @NHSO-666
  @pending
  @backend
  Scenario: Account ID doesn't match a user
    Given I have data for a patient that does not exist
    When I register an EMIS user's IM1 credentials
    Then I receive a "Not found" error

  @bug
  @NHSO-666
  @pending
  @backend
  Scenario: Incorrect Linkage Key
    Given I have data for a patient
    But an incorrect linkage key
    When I register an EMIS user's IM1 credentials
    Then I receive a "Not found" error

  @bug
  @NHSO-666
  @pending
  @backend
  Scenario: Incorrect Surname
    Given I have data for a patient
    But an incorrect surname
    When I register an EMIS user's IM1 credentials
    Then I receive a "Not found" error

  @bug
  @NHSO-666
  @pending
  @backend
  Scenario: Incorrect Date of Birth
    Given I have data for a patient
    But an incorrect DoB
    When I register an EMIS user's IM1 credentials
    Then I receive a "Not found" error


  @pending
  @backend
  Scenario: User's account has already been associated with the application
    Given I have data for a patient that has already been associated with the application in the GP system
    When I register an EMIS user's IM1 credentials
    Then I receive a "Conflict" error

  @pending
  @backend
  Scenario: EMIS Demographics endpoint is disabled
    Given I have valid patient data to register new account
    And EMIS Demographics endpoint is disable
    When I register an EMIS user's IM1 credentials
    Then I receive a "Forbidden" error


  @backend
  Scenario: ODS Code not in the expected format
    Given I have an EMIS user's IM1 credentials with an ODS Code not in the expected format
    When I register an EMIS user's IM1 credentials
    Then I get a "Bad Request" error


  @bug @NHSO-661
  @backend
  Scenario: Surname not in the expected format
    Given I have an EMIS user's IM1 credentials with a Surname not in the expected format
    When I register an EMIS user's IM1 credentials
    Then I get a "Bad Request" error


  @bug @NHSO-661
  @backend
  Scenario: Account ID not in the expected format
    Given I have an EMIS user's IM1 credentials with an Account ID not in the expected format
    When I register an EMIS user's IM1 credentials
    Then I receive a "Bad Request" error


  @bug @NHSO-661
  @backend
  Scenario: Linkage Key not in the expected format
    Given I have an EMIS user's IM1 credentials with a Linkage Key not in the expected format
    When I register an EMIS user's IM1 credentials
    Then I get a "Bad Request" error

  @backend
  Scenario: Date Of Birth not in the expected format
    Given I have an EMIS user's IM1 credentials with a Date Of Birth not in the expected format
    When I register an EMIS user's IM1 credentials
    Then I get a "Bad Request" error

  @backend
  Scenario: Missing ODS Code
    Given I have an EMIS user's IM1 credentials with missing ODS Code
    When I register an EMIS user's IM1 credentials
    Then I get a "Bad Request" error

  @backend
  Scenario: Missing Surname
    Given I have an EMIS user's IM1 credentials with missing Surname
    When I register an EMIS user's IM1 credentials
    Then I get a "Bad Request" error

  @backend
  Scenario: Missing Account ID
    Given I have an EMIS user's IM1 credentials with missing Account ID
    When I register an EMIS user's IM1 credentials
    Then I get a "Bad Request" error

  @backend
  Scenario: Missing Linkage Key
    Given I have an EMIS user's IM1 credentials with missing Linkage Key
    When I register an EMIS user's IM1 credentials
    Then I get a "Bad Request" error

  @NHSO-313 
  Scenario: User launches the create account CitizenID journey
    Given I want to register for an account
    When I select to create an account
    Then I am redirected to the CID create an account page

  @NHSO-313
  @smoketest
  Scenario: User launches and completes account creation from web
    Given I have completed account creation
    Then I am redirected to the signed in home page
    And I see a welcome message
    And I see the navigation menu
    And I see the header

  @manual
  @native
  @NHSO-313
  Scenario: User launches and completes account creation from native app
    Given I have completed account creation
    Then I am redirected to the app to the signed in home page
    And I see a welcome message
    And I see the navigation menu
    And I see the header

  @NHSO-313
  Scenario: While a new user who has creared an account waits to be logged in a spinner is shown
    Given I want to register for an account
    And sign in verification is slow
    When I complete the account registration
    Then the spinner appears