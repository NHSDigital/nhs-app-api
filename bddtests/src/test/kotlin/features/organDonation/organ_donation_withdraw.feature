@organ-donation
Feature: Organ Donation Withdraw

# These tests navigate directly to the pages where the features are to be tested, to save time.

  #All combinations of GP System users and Decisions have been covered in backend tests
  Scenario Outline: As a <GP System> user, I can withdraw previously registered <Decision> organ donation decision
    Given I am a <GP System> user registered with organ donation with a decision to <Decision> who wishes to withdraw
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    Then the Organ Donation View Registration page is displayed with my existing decision to <Decision>
    When I choose to withdraw my organ donation decision
    Then the Organ Donation Withdraw Decision page is displayed
    And I select an organ donation withdrawal reason from the list
    When I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Check Details page is displayed
    And my decision to withdraw is recorded on the Organ Donation Check Details page
    And I confirm that my details are accurate, and accept the privacy statement for organ donation
    When I click the 'Submit my decision' button on an Organ Donation page
    Then the Organ Donation View Registration page is displayed with my decision to withdraw
    Examples:
      | Decision                 | GP System |
      | opt-in                   | EMIS      |
      | opt-in-some              | TPP       |
      | opt-out                  | VISION    |
      | appoint-a-representative | EMIS      |

  Scenario: A user sees an error message without retry option when trying to withdraw a previously registered organ donation decision and access is denied
    Given I am a TPP user registered with organ donation with a decision to opt-out who wishes to withdraw but OD returns non-recoverable 502 error
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    Then the Organ Donation View Registration page is displayed with my existing decision to opt-out
    When I choose to withdraw my organ donation decision
    Then the Organ Donation Withdraw Decision page is displayed
    And the organ donation withdraw decision reasons are shown sorted alphabetically
    And I select an organ donation withdrawal reason from the list
    When I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Check Details page is displayed
    And my decision to withdraw is recorded on the Organ Donation Check Details page
    And I confirm that my details are accurate, and accept the privacy statement for organ donation
    When I click the 'Submit my decision' button on an Organ Donation page
    Then I see an appropriate Organ Donation error message without a retry option

  Scenario: A user sees an error message with a retry option when trying to withdraw a previously registered organ donation decision and timeout occurs
    Given I am a VISION user registered with organ donation with a decision to opt-out who wishes to withdraw but OD returns recoverable 503 error
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    Then the Organ Donation View Registration page is displayed with my existing decision to opt-out
    When I choose to withdraw my organ donation decision
    Then the Organ Donation Withdraw Decision page is displayed
    And I select an organ donation withdrawal reason from the list
    When I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Check Details page is displayed
    And my decision to withdraw is recorded on the Organ Donation Check Details page
    And I confirm that my details are accurate, and accept the privacy statement for organ donation
    When I click the 'Submit my decision' button on an Organ Donation page
    Then I see an appropriate Organ Donation error message with a retry option
    When I click the 'Try again' button on an Organ Donation page
    Then the Organ Donation View Registration page is displayed with my decision to withdraw

  Scenario: A user is shown a validation error if an organ donation withdrawal reason is not selected
    Given I am a EMIS user registered with organ donation with a decision to opt-in who wishes to withdraw
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    Then the Organ Donation View Registration page is displayed with my existing decision to opt-in
    When I choose to withdraw my organ donation decision
    Then the Organ Donation Withdraw Decision page is displayed
    When I click the 'Continue' button on an Organ Donation page
    Then I am shown validation error on the page advising me to choose an organ donation withdrawal reason
    When I select an organ donation withdrawal reason from the list
    And I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Check Details page is displayed
    And my decision to withdraw is recorded on the Organ Donation Check Details page

  Scenario: A user can navigate back through the withdraw journey
    Given I am a EMIS user registered with organ donation with a decision to opt-in who wishes to withdraw
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    Then the Organ Donation View Registration page is displayed with my existing decision to opt-in
    When I choose to withdraw my organ donation decision
    Then the Organ Donation Withdraw Decision page is displayed
    When I click the 'Back' button on an Organ Donation page
    Then the Organ Donation View Registration page is displayed with my existing decision to opt-in
    When I choose to withdraw my organ donation decision
    Then the Organ Donation Withdraw Decision page is displayed
    And I select an organ donation withdrawal reason from the list
    When I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Check Details page is displayed
    And my decision to withdraw is recorded on the Organ Donation Check Details page

