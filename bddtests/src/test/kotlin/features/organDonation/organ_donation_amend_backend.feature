@organ-donation
@backend
Feature: Organ Donation Amend Backend

  Scenario Outline: When amending an organ donation decision to opt in, a registered <GP System> user receives
  a 200 response and a registration id
    Given I am a <GP System> api user registered as opt-out who amends their decision to opt-in
    And I have logged in and have a valid session cookie
    When I submit my updated decision to organ donation
    Then I receive an "Ok" success code
    And I receive my registration id from organ donation
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: When amending an organ donation decision to some organs, a registered <GP System> user receives
  a 200 response and a registration id
    Given I am a <GP System> api user registered as opt-in who amends their decision to some organs
    And I have logged in and have a valid session cookie
    When I submit my updated decision to organ donation
    Then I receive an "Ok" success code
    And I receive my registration id from organ donation
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: When amending an organ donation decision to opt out, a registered <GP System> user receives
  a 200 response and a registration id
    Given I am a <GP System> api user registered as some organs who amend their decision to opt-out
    And I have logged in and have a valid session cookie
    When I submit my updated decision to organ donation
    Then I receive an "Ok" success code
    And I receive my registration id from organ donation
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: When amending an organ donation decision which will be conflicted, an unregistered <GP System>
  user receives a 200 response with state value of "conflicted" and a registration id
    Given I am a <GP System> api user who wants amend their decision, but will cause a conflict
    And I have logged in and have a valid session cookie
    When I submit my updated decision to organ donation
    Then I receive a "Ok" success code
    And I receive my registration id from organ donation
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: When amending an organ donation decision, an OD response of <Error Code> will prompt a 500
  response and no retry option
    Given I am a EMIS api user amending my decision, but OD returns '<Error Code>' error
    And I have logged in and have a valid session cookie
    When I submit my updated decision to organ donation
    Then I receive a "Internal Server Error" error
    And the Internal Server Error response does not include a retry option
    Examples:
      | Error Code |
      | 400        |
      | 401        |
      | 403        |
      | 405        |
      | 415        |
      | 422        |

  Scenario Outline: When amending an organ donation decision, an OD response of <Error Code> will prompt a 502
  response and a retry option
    Given I am a EMIS api user amending my decision, but OD returns '<Error Code>' error
    And I have logged in and have a valid session cookie
    When I submit my updated decision to organ donation
    Then I receive a "Bad Gateway" error
    And the Bad Gateway error response includes a retry option
    Examples:
      | Error Code |
      | 429        |
      | 503        |
      | 504        |

  Scenario Outline: When amending an organ donation decision, an OD response of <Error Code> will prompt a 502
  response and no retry option
    Given I am a EMIS api user amending my decision, but OD returns '<Error Code>' error
    And I have logged in and have a valid session cookie
    When I submit my updated decision to organ donation
    Then I receive a "Bad Gateway" error
    And the Bad Gateway error response does not include a retry option
    Examples:
      | Error Code |
      | 404        |
      | 409        |
      | 500        |
      | 502        |
