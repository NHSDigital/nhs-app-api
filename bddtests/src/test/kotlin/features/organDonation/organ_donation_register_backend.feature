@organ-donation
@backend
Feature: Organ Donation Register Backend

  Scenario Outline: A <GP System> user chooses to donate their organs and submits their Faiths And Beliefs and a POST request is made containing user's choices
    Given I am a <GP System> api user who wants to opt-in to organ donation
    And I have logged in and have a valid session cookie
    When I submit a request to set my organ donation preferences with all organs and my faiths and beliefs decision
    Then I receive my registration id from organ donation
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: When submitting an organ donation opt out decision, an unregistered <GP System> user receives a 201 response and a registration id
    Given I am a <GP System> api user who wants to opt-out of organ donation
    And I have logged in and have a valid session cookie
    When I submit my decision to organ donation
    Then I receive a "Created" success code
    And I receive my registration id from organ donation
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: When submitting an organ donation opt in decision, an unregistered <GP System> user receives a 201 response and a registration id
    Given I am a <GP System> api user who wants to opt-in to organ donation
    And I have logged in and have a valid session cookie
    When I submit my decision to organ donation
    Then I receive a "Created" success code
    And I receive my registration id from organ donation
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: When submitting an organ donation some organs decision, an unregistered <GP System> user receives a 201 response and a registration id
    Given I am a <GP System> api user who wants to donate some but not all organs
    And I have logged in and have a valid session cookie
    When I submit my decision to organ donation
    Then I receive a "Created" success code
    And I receive my registration id from organ donation
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: When submitting an organ donation decision which will be conflicted, an unregistered <GP System> user receives a 201 response with state value of "conflicted" and a registration id
    Given I am a <GP System> api user who wants to opt-in to organ donation but will cause a conflict
    And I have logged in and have a valid session cookie
    When I submit my decision to organ donation
    Then I receive a "Created" success code
    And I receive my registration id from organ donation
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: On submitting an organ donation decision, an OD response of <Error Code> will prompt a 500 response and no retry option
    Given I am a EMIS api user who wants to opt-out, but OD returns '<Error Code>' error
    And I have logged in and have a valid session cookie
    When I submit my decision to organ donation
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

  Scenario Outline: On submitting an organ donation decision, an OD response of <Error Code> will prompt a 502 response and a retry option
    Given I am a EMIS api user who wants to opt-out, but OD returns '<Error Code>' error
    And I have logged in and have a valid session cookie
    When I submit my decision to organ donation
    Then I receive a "Bad Gateway" error
    And the Bad Gateway error response includes a retry option
    Examples:
      | Error Code |
      | 429        |
      | 503        |
      | 504        |

  Scenario Outline: On submitting an organ donation decision, an OD response of <Error Code> will prompt a 502 response and no retry option
    Given I am a EMIS api user who wants to opt-out, but OD returns '<Error Code>' error
    And I have logged in and have a valid session cookie
    When I submit my decision to organ donation
    Then I receive a "Bad Gateway" error
    And the Bad Gateway error response does not include a retry option
    Examples:
      | Error Code |
      | 500        |
      | 502        |