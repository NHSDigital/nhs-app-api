Feature: Registration

  A user can create a new NHS account from the login page, allowing them to access the app

  @backend
  @bug @NHSO-922
  Scenario: Patient has no NHS Numbers
    Given I have valid patient data to register new account
    When I register an EMIS user's IM1 credentials
    Then I receive the expected connection token and single NHS Number

  @backend
  @bug @NHSO-922
  Scenario: Patient has NHS Numbers
    Given I have valid patient data with multiple nhs numbers to register new account
    When I register an EMIS user's IM1 credentials
    Then I receive the expected connection token and multiple NHS Numbers

  @backend
  @bug @NHSO-922
  Scenario: NHS Number does not exist
    Given I have valid data for a patient with no NHS Number
    When I register an EMIS user's IM1 credentials
    Then I receive the expected connection token without NHS Numbers

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
    Then I receive a "Bad Request" error


  @bug @NHSO-661 @NHSO-922
  @backend
  Scenario: Surname not in the expected format
    Given I have an EMIS user's IM1 credentials with a Surname not in the expected format
    When I register an EMIS user's IM1 credentials
    Then I receive a "Bad Request" error


  @bug @NHSO-661 @NHSO-922
  @backend
  Scenario: Account ID not in the expected format
    Given I have an EMIS user's IM1 credentials with an Account ID not in the expected format
    When I register an EMIS user's IM1 credentials
    Then I receive a "Bad Request" error


  @NHSO-661
  @backend
  Scenario: Linkage Key not in the expected format
    Given I have an EMIS user's IM1 credentials with a Linkage Key not in the expected format
    When I register an EMIS user's IM1 credentials
    Then I receive a "Bad Request" error

  @backend
  Scenario: Date Of Birth not in the expected format
    Given I have an EMIS user's IM1 credentials with a Date Of Birth not in the expected format
    When I register an EMIS user's IM1 credentials
    Then I receive a "Bad Request" error

  @backend
  Scenario: Missing ODS Code
    Given I have an EMIS user's IM1 credentials with missing ODS Code
    When I register an EMIS user's IM1 credentials
    Then I receive a "Bad Request" error

  @backend
  Scenario: Missing Surname
    Given I have an EMIS user's IM1 credentials with missing Surname
    When I register an EMIS user's IM1 credentials
    Then I receive a "Bad Request" error

  @backend
  Scenario: Missing Account ID
    Given I have an EMIS user's IM1 credentials with missing Account ID
    When I register an EMIS user's IM1 credentials
    Then I receive a "Bad Request" error

  @backend
  Scenario: Missing Linkage Key
    Given I have an EMIS user's IM1 credentials with missing Linkage Key
    When I register an EMIS user's IM1 credentials
    Then I receive a "Bad Request" error

  @NHSO-313
  Scenario: A user who is not signed in sees create account button
    Given wiremock is initialised
    Given I am not logged in
    When I am on the home page
    Then I see create account button

  @NHSO-313 
  Scenario: User launches the create account CitizenID journey
    Given wiremock is initialised
    Given I am not logged in
    When I select to create an account
    Then I am redirected to the CID create an account page

  @NHSO-313
  @smoketest
  Scenario: User launches and completes account creation from web
    Given wiremock is initialised
    Given I am not logged in
    And I have completed account creation
    Then I am redirected to the signed in home page
    And I see a welcome message for Montel Frye
    And I see the navigation menu
    And I see the header

  @manual
  @NHSO-313
  Scenario: User launches and completes account creation from native app
    Given wiremock is initialised
    Given I am not logged in
    And I have completed account creation
    Then I am redirected to the app to the signed in home page
    And I see a welcome message for Montel Frye
    #And I see the navigation menu
    #And I see the header

  @NHSO-313
  Scenario: While a new user who has creared an account waits to be logged in a spinner is shown
    Given wiremock is initialised
    Given I am not logged in
    And sign in verification is slow
    When I have completed account creation
    Then the spinner appears