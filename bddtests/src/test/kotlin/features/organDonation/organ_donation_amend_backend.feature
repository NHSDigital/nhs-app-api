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

  Scenario Outline: When amending an organ donation decision but the system times out, the <GP System>
  user receives a 504 response
    Given I am a <GP System> api user who wants to amend their decision, but system times out
    And I have logged in and have a valid session cookie
    When I submit my updated decision to organ donation
    Then I receive a "gateway timeout" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: When amending an organ donation decision but the system returns internal error, the <GP System>
  user receives a 502 response
    Given I am a <GP System> api user who wants to amend their decision, but OD will return an internal error
    And I have logged in and have a valid session cookie
    When I submit my updated decision to organ donation
    Then I receive a "bad gateway" error
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

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
