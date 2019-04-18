@authentication
Feature: Registration
  A user can create a new NHS account from the login page, allowing them to access the app

  @backend
  Scenario Outline: Patient registers for a <GP System> account with NHS Numbers of <NHS Numbers>
    Given I have a new <GP System> patient with Nhs Numbers of <NHS Numbers>
    And no IM1 Connection Token is currently cached
    When I register the user's IM1 credentials
    Then I receive a response
    And the response has the expected connection token
    And the response has the expected NHS numbers
    And the IM1 Connection Token is in the cache

    Examples:
      | GP System | NHS Numbers |
      | EMIS      |             |
      | EMIS      | "one"       |
      | EMIS      | "one","two" |
      | TPP       | "one"       |

  @backend
  Scenario: Patient registers for a Vision account with NHS Number of "one"
    Given I have a new VISION patient with Nhs Numbers of "one"
    When I register the user's IM1 credentials
    Then I receive a response
    And the response has the expected connection token
    And the response has the expected NHS numbers

  @backend
  Scenario Outline: Patient registers for a Microtest account with NHS Number of "one"
    Given I have a new <GP System> patient with Nhs Numbers of <NHS Numbers>
    When I register the user's IM1 credentials
    Then I receive a response
    And the response has the expected connection token
    And the response has the expected NHS numbers
    Examples:
      | GP System | NHS Numbers |
      | MICROTEST |             |

  @backend
  Scenario Outline: <GP System> Account ID doesn't match a user
    Given I have data for a <GP System> patient that does not exist
    And no IM1 Connection Token is currently cached
    When I register the user's IM1 credentials
    Then I get a "Not found" error

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  @backend
  Scenario Outline: Incorrect <GP System> Linkage Key
    Given I have data for a <GP System> patient with incorrect linkage key
    When I register the user's IM1 credentials
    Then I get a "Not found" error

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  @backend
  Scenario Outline: <GP System> - Incorrect Surname
    Given I have data for a <GP System> patient with incorrect surname
    And no IM1 Connection Token is currently cached
    When I register the user's IM1 credentials
    Then I get a "Not found" error

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  @backend
  Scenario Outline: <GP System> - Incorrect Date of Birth
    Given I have data for a <GP System> patient with incorrect date of birth
    And no IM1 Connection Token is currently cached
    When I register the user's IM1 credentials
    Then I get a "Not found" error

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  @backend
  Scenario: Registering an EMIS patient who is already registered creates a Conflict response
    Given I have data for an EMIS patient that has already been associated with the application in the GP system
    When I register the user's IM1 credentials
    Then I get a "Conflict" error

  @backend
  Scenario: Registering a VISION patient who is already registered creates a Conflict response
    Given I have data for a Vision patient that has already been associated with the application in the GP system
    When I register the user's IM1 credentials
    Then I get a "Conflict" error

  @backend
  Scenario: Registering a patient which has been locked by VISION creates a Bad Gateway response
    Given I have data for a Vision patient with a locked account as the account is opened in the Vision application
    When I register the user's IM1 credentials
    Then I get a "bad gateway" error


  @backend
  Scenario: ODS Code not in the expected format
    Given I have a user's IM1 credentials with an ODS Code not in the expected format
    When I register the user's IM1 credentials
    Then I get a "Bad Request" error

  @backend
  Scenario Outline: <GP System> - Surname not in the expected format
    Given I have a <GP System> user's IM1 credentials with a Surname not in the expected format
    And no IM1 Connection Token is currently cached
    When I register the user's IM1 credentials
    Then I get a "<Response>" error

    Examples:
      | GP System | Response    |
      | EMIS      | Bad Request |
      | TPP       | Not Found   |
      | VISION    | Not Found   |

  @backend
  Scenario Outline: <GP System> - Account ID not in the expected format
    Given I have a <GP System> user's IM1 credentials with an Account ID not in the expected format
    When I register the user's IM1 credentials
    Then I get a "<Response>" error

    Examples:
      | GP System | Response    |
      | EMIS      | Bad Request |
      | TPP       | Not Found   |
      | VISION    | Bad Request |

  @backend
  Scenario Outline: <GP System> - Linkage Key not in the expected format
    Given I have a <GP System> user's IM1 credentials with a Linkage Key not in the expected format
    When I register the user's IM1 credentials
    Then I get a "<Response>" error

    Examples:
      | GP System | Response    |
      | EMIS      | Bad Request |
      | TPP       | Not Found   |
      | VISION    | Bad Request |

  @backend
  Scenario Outline: <GP System> - Date Of Birth not in the expected format
    Given I have a <GP System> user's IM1 credentials with a Date Of Birth not in the expected format
    When I register the user's IM1 credentials
    Then I get a "<Response>" error

    Examples:
      | GP System | Response    |
      | EMIS      | Bad Request |
      | TPP       | Bad Request |
      | VISION    | Bad Request |

  @backend
  Scenario: Missing ODS Code
    Given I have a user's IM1 credentials with missing ODS Code
    When I register the user's IM1 credentials
    Then I get a "Bad Request" error

  @backend
  Scenario Outline: <GP System> - Missing Surname
    Given I have a <GP System> user's IM1 credentials with missing Surname
    When I register the user's IM1 credentials
    Then I get a "<Response>" error

    Examples:
      | GP System | Response    |
      | EMIS      | Bad Request |
      | TPP       | Bad Request |
      | VISION    | Bad Request |

  @backend
  Scenario Outline: <GP System> - Missing Account ID
    Given I have a <GP System> user's IM1 credentials with missing Account ID
    When I register the user's IM1 credentials
    Then I get a "<Response>" error

    Examples:
      | GP System | Response    |
      | EMIS      | Bad Request |
      | TPP       | Bad Request |
      | VISION    | Bad Request |
      | MICROTEST | Bad Request |

  @backend
  Scenario Outline: <GP System> - Missing Linkage Key
    Given I have a <GP System> user's IM1 credentials with missing Linkage Key
    When I register the user's IM1 credentials
    Then I get a "<Response>" error

    Examples:
      | GP System | Response    |
      | EMIS      | Bad Request |
      | TPP       | Bad Request |
      | VISION    | Bad Request |
      | MICROTEST | Bad Request |

  @backend
  Scenario: Targeting the IM1 endpoint does not expose the Patient Facing Services endpoint
    Given I target the IM1 endpoint
    And I have an IM1 request and a Patient Facing Request
    Then I receive a response from the IM1 request
    And I receive a Not Found response from the Patient Facing request


  @backend
  Scenario: Targeting the Patient Facing Services endpoint does not expose the IM1 endpoint
    Given I target the Patient Facing Services endpoint
    And I have an IM1 request and a Patient Facing Request
    Then I receive a response from the Patient Facing request
    And I receive a Not Found response from the IM1 request

  @nativepending @NHSO-2974
  Scenario Outline: <GP System> User launches the create account CitizenID journey
    Given I want to register for a <GP System> account
    When I select to create an account
    Then I am redirected to the CID create an account page

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  @ios
  Scenario Outline: <GP System> User launches and completes account creation from web
    Given I have completed <GP System> account creation
    Then I am redirected to the signed in home page
    And I see a welcome message
    And I see the navigation menu
    And I see the home page header

    Examples:
      | GP System |
      | TPP       |
      | VISION    |
  @smoketest
  @native-smoketest
    Examples:
      | GP System |
      | EMIS      |
