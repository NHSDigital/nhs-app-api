@organdonation
@backend
Feature: Organ Donation Withdraw Backend

  Scenario Outline: As a <GP System> user, I am able to withdraw a previously registered Organ Donation decision of <Decision>
    Given I am a <GP System> api user registered with an organ donation decision to <Decision> and wish to withdraw my decision
    And I have logged in and have a valid session cookie
    When I submit my organ donation withdraw decision
    Then I receive a "ok" success code
    Examples:
      | Decision                 | GP System   |
      | opt-in                   | EMIS        |
      | opt-in-some              | EMIS        |
      | opt-out                  | EMIS        |
      | appoint-a-representative | EMIS        |
      | opt-in                   | TPP         |
      | opt-in-some              | TPP         |
      | opt-out                  | TPP         |
      | appoint-a-representative | TPP         |
      | opt-in                   | VISION      |
      | opt-in-some              | VISION      |
      | opt-out                  | VISION      |
      | appoint-a-representative | VISION      |

  Scenario Outline: As a user, when I attempt to withdraw organ donation decision an OD response of <OD Response Code> will prompt a 502 response in the backend with a retry option
    Given I am a EMIS user registered with OD, but on attempting to withdraw decision OD returns <OD Response Code> error
    And I have logged in and have a valid session cookie
    When I submit my organ donation withdraw decision
    Then I receive a "bad gateway" error
    And the Bad Gateway error response includes a retry option
    Examples:
      | OD Response Code |
      | 429              |
      | 503              |
      | 504              |

  Scenario Outline: When attempting to withdraw organ donation decision an OD response of <OD Response Code> will prompt a 502 response in the backend with no retry option
    Given I am a EMIS user registered with OD, but on attempting to withdraw decision OD returns <OD Response Code> error
    And I have logged in and have a valid session cookie
    When I submit my organ donation withdraw decision
    Then I receive a "bad gateway" error
    And the Bad Gateway error response does not include a retry option
    Examples:
      | OD Response Code |
      | 500              |
      | 502              |

  Scenario Outline: When attempting to withdraw organ donation decision an OD response of <OD Response Code> will prompt a 500 response in the backend with no retry option
    Given I am a EMIS user registered with OD, but on attempting to withdraw decision OD returns <OD Response Code> error
    And I have logged in and have a valid session cookie
    When I submit my organ donation withdraw decision
    Then I receive a "internal server error" error
    And the Internal Server Error response does not include a retry option
    Examples:
      | OD Response Code |
      | 400              |
      | 403              |
      | 405              |
      | 415              |
      | 422              |
      | 401              |