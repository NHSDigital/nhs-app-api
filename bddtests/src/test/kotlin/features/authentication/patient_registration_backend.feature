@authentication
@registration
@backend
Feature: Patient Registration Backend
  A user can create a new NHS account from the login page, allowing them to access the app

  Scenario Outline: Patient registers for a <GP System> account with NHS Numbers of <NHS Numbers>
    Given I have a new <GP System> patient with Nhs Numbers of <NHS Numbers>
    And no IM1 Connection Token is currently cached
    When I register the user's IM1 credentials
    Then I receive a "Created" success code
    And the response has the expected connection token
    And the response has the expected NHS numbers
    And the IM1 Connection Token is in the cache

    Examples:
      | GP System | NHS Numbers |
      | EMIS      |             |
      | EMIS      | "one"       |
      | EMIS      | "one","two" |
      | TPP       | "one"       |

  Scenario: Patient registers for a Vision account with NHS Number of "one"
    Given I have a new VISION patient with Nhs Numbers of "one"
    When I register the user's IM1 credentials
    Then I receive a "Created" success code
    And the response has the expected connection token
    And the response has the expected NHS numbers

  Scenario: Registering with linkage details for a Microtest practice, who don't support linkage keys, results in a Linkage Not Supported response
    Given I have a new MICROTEST patient with Nhs Numbers of "5785445875"
    When I register the user's IM1 credentials
    Then I receive a "Linkage Not Supported" code

  Scenario Outline: <GP System> Account ID doesn't match a user
    Given I have data for a <GP System> patient that does not exist
    And no IM1 Connection Token is currently cached
    When I register the user's IM1 credentials
    Then I receive a "Not found" error

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: Incorrect <GP System> Linkage Key
    Given I have data for a <GP System> patient with incorrect linkage key
    When I register the user's IM1 credentials
    Then I receive a "Not found" error

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: <GP System> - Incorrect Surname
    Given I have data for a <GP System> patient with incorrect surname
    And no IM1 Connection Token is currently cached
    When I register the user's IM1 credentials
    Then I receive a "Not found" error

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: <GP System> - Incorrect Date of Birth
    Given I have data for a <GP System> patient with incorrect date of birth
    And no IM1 Connection Token is currently cached
    When I register the user's IM1 credentials
    Then I receive a "Not found" error

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario: Registering an EMIS patient who is already registered creates a Conflict response
    Given I have data for an EMIS patient that has already been associated with the application in the GP system
    When I register the user's IM1 credentials
    Then I receive a "Conflict" error

  Scenario: Registering a VISION patient who is already registered creates a Conflict response
    Given I have data for a Vision patient that has already been associated with the application in the GP system
    When I register the user's IM1 credentials
    Then I receive a "Conflict" error

  Scenario: Registering a patient which has been locked by VISION creates a Bad Gateway response
    Given I have data for a Vision patient with a locked account as the account is opened in the Vision application
    When I register the user's IM1 credentials
    Then I receive a "forbidden" error

  Scenario: ODS Code not in the expected format
    Given I have a user's IM1 credentials with an ODS Code not in the expected format
    When I register the user's IM1 credentials
    Then I receive a "Bad Request" error

  Scenario Outline: <GP System> - Surname not in the expected format
    Given I have a <GP System> user's IM1 credentials with a Surname not in the expected format
    And no IM1 Connection Token is currently cached
    When I register the user's IM1 credentials
    Then I receive a "<Response>" error

    Examples:
      | GP System | Response    |
      | EMIS      | Bad Request |
      | TPP       | Not Found   |
      | VISION    | Not Found   |

  Scenario Outline: <GP System> - Account ID not in the expected format
    Given I have a <GP System> user's IM1 credentials with an Account ID not in the expected format
    When I register the user's IM1 credentials
    Then I receive a "<Response>" error

    Examples:
      | GP System | Response    |
      | EMIS      | Bad Request |
      | TPP       | Not Found   |
      | VISION    | Bad Request |

  Scenario Outline: <GP System> - Linkage Key not in the expected format
    Given I have a <GP System> user's IM1 credentials with a Linkage Key not in the expected format
    When I register the user's IM1 credentials
    Then I receive a "<Response>" error

    Examples:
      | GP System | Response    |
      | EMIS      | Bad Request |
      | TPP       | Not Found   |
      | VISION    | Bad Request |

  Scenario Outline: <GP System> - Date Of Birth not in the expected format
    Given I have a <GP System> user's IM1 credentials with a Date Of Birth not in the expected format
    When I register the user's IM1 credentials
    Then I receive a "<Response>" error

    Examples:
      | GP System | Response    |
      | EMIS      | Bad Request |
      | TPP       | Bad Request |
      | VISION    | Bad Request |

  Scenario: Missing ODS Code
    Given I have a user's IM1 credentials with missing ODS Code
    When I register the user's IM1 credentials
    Then I receive a "Bad Request" error

  Scenario Outline: <GP System> - Missing Surname
    Given I have a <GP System> user's IM1 credentials with missing Surname
    When I register the user's IM1 credentials
    Then I receive a "<Response>" error

    Examples:
      | GP System | Response    |
      | EMIS      | Bad Request |
      | TPP       | Bad Request |
      | VISION    | Bad Request |

  Scenario Outline: <GP System> - Missing Account ID
    Given I have a <GP System> user's IM1 credentials with missing Account ID
    When I register the user's IM1 credentials
    Then I receive a "<Response>" error

    Examples:
      | GP System | Response    |
      | EMIS      | Bad Request |
      | TPP       | Bad Request |
      | VISION    | Bad Request |
      | MICROTEST | Bad Request |

  Scenario Outline: <GP System> - Missing Linkage Key
    Given I have a <GP System> user's IM1 credentials with missing Linkage Key
    When I register the user's IM1 credentials
    Then I receive a "<Response>" error

    Examples:
      | GP System | Response    |
      | EMIS      | Bad Request |
      | TPP       | Bad Request |
      | VISION    | Bad Request |
      | MICROTEST | Bad Request |

  Scenario: Targeting the IM1 endpoint does not expose the Patient Facing Services endpoint
    Given I target the IM1 endpoint
    And I have an IM1 request and a Patient Facing Request
    Then I receive a response from the IM1 request
    And I receive a Not Found response from the Patient Facing request


  Scenario: Targeting the Patient Facing Services endpoint does not expose the IM1 endpoint
    Given I target the Patient Facing Services endpoint
    And I have an IM1 request and a Patient Facing Request
    Then I receive a response from the Patient Facing request
    And I receive a Not Found response from the IM1 request
