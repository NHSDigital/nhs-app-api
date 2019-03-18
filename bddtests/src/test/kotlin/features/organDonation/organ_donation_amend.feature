@organ-donation
Feature: Organ Donation - Amend

  Scenario: A user who opted to donate all their organs can amend it to not donate
    Given I am a EMIS user registered as opt-in who then amends their decision to opt-out
    And I am logged in
    When I navigate to the internal Organ Donation Page
    Then the Organ Donation View Registration page is displayed with my existing decision to opt-in
    When I choose to amend my Organ Donation decision
    Then the internal Organ Donation Choice Page is displayed
    When I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And the choice of not wishing to donate organs is displayed on the Organ Donation Check Details page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button
    Then the Organ Donation View Registration page is displayed
    And the decision to opt out of organ donation has been successfully created

  Scenario: A user who opted to donate all their organs can amend it to only donate some
    Given I am a TPP user registered as opt-in who then amends their decision to some
    And I am logged in
    And I navigate to the internal Organ Donation Page
    Then the Organ Donation View Registration page is displayed with my existing decision to opt-in
    When I choose to amend my Organ Donation decision
    Then the internal Organ Donation Choice Page is displayed
    When I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    And the all organs option is selected
    When I select the option to donate some of my organs
    And I click the 'Continue' button
    Then the Organ Donation Specific Organ Choice page is displayed
    When I choose which organs to donate
    And I click the 'Continue' button
    Then the Organ Donation Faith And Beliefs page is displayed
    And the previous option on the Organ Donation Faith And Beliefs page is selected
    And I click the 'Continue' button
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And my specific organ donation choices are displayed on the Organ Donation Check Details page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button
    Then the Organ Donation View Registration page is displayed
    And the decision to opt in to organ donation with some organs has been successfully created

  Scenario: A user who opted to donate all their organs can amend faith and beliefs
    Given I am a VISION user registered as opt-in who then amends their faith and beliefs
    And I am logged in
    And I navigate to the internal Organ Donation Page
    Then the Organ Donation View Registration page is displayed with my existing decision to opt-in
    When I choose to amend my Organ Donation decision
    Then the internal Organ Donation Choice Page is displayed
    When I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    And the all organs option is selected
    And I click the 'Continue' button
    Then the Organ Donation Faith And Beliefs page is displayed
    And the previous option on the Organ Donation Faith And Beliefs page is selected
    When I select the option 'Yes - this is applicable to me' to share my organ donation faith and beliefs
    And I click the 'Continue' button
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And the choice of wishing to donate organs is displayed on the Organ Donation Check Details page
    And my choice of 'Yes' to share my faith and beliefs is displayed on the Organ Donation Check Details page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button
    Then the Organ Donation View Registration page is displayed
    And the decision to opt in to organ donation has been successfully created

  Scenario: A user opted to donate some of their organs can amend it to not donate
    Given I am a EMIS user registered as some who then amends their decision to opt-out
    And I am logged in
    When I navigate to the internal Organ Donation Page
    Then the Organ Donation View Registration page is displayed with my existing decision to opt-in-some
    When I choose to amend my Organ Donation decision
    Then the internal Organ Donation Choice Page is displayed
    When I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And the choice of not wishing to donate organs is displayed on the Organ Donation Check Details page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button
    Then the Organ Donation View Registration page is displayed
    And the decision to opt out of organ donation has been successfully created

  Scenario: A user opted to donate some of their organs can amend to donate all
    Given I am a TPP user registered as some who then amends their decision to opt-in
    And I am logged in
    When I navigate to the internal Organ Donation Page
    Then the Organ Donation View Registration page is displayed with my existing decision to opt-in-some
    When I choose to amend my Organ Donation decision
    Then the internal Organ Donation Choice Page is displayed
    And I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    And the some organs option is selected
    When I select the option to donate all my organs
    And I click the 'Continue' button
    Then the Organ Donation Faith And Beliefs page is displayed
    And the previous option on the Organ Donation Faith And Beliefs page is selected
    And I click the 'Continue' button
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And the choice of wishing to donate organs is displayed on the Organ Donation Check Details page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button
    Then the Organ Donation View Registration page is displayed
    And the decision to opt in to organ donation has been successfully created

  Scenario: A user opted to donate some of their organs can amend the selected organs
    Given I am a VISION user registered as some who then amends their selected organs
    And I am logged in
    When I navigate to the internal Organ Donation Page
    Then the Organ Donation View Registration page is displayed with my existing decision to opt-in-some
    When I choose to amend my Organ Donation decision
    Then the internal Organ Donation Choice Page is displayed
    And I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    And the some organs option is selected
    And I click the 'Continue' button
    Then the Organ Donation Specific Organ Choice page is displayed
    And my previous decisions are displayed on the Organ Donation Specific Organ Choice page
    When I choose which organs to donate
    And I click the 'Continue' button
    Then the Organ Donation Faith And Beliefs page is displayed
    And the previous option on the Organ Donation Faith And Beliefs page is selected
    And I click the 'Continue' button
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And my specific organ donation choices are displayed on the Organ Donation Check Details page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button
    Then the Organ Donation View Registration page is displayed
    And the decision to opt in to organ donation with some organs has been successfully created

  Scenario: A user who opted to donate some organs but did not make a decision for all of them can amend their selection
    Given I am a EMIS user registered to donate some organs, with some undecided, who amends their decision
    And I am logged in
    When I navigate to the internal Organ Donation Page
    Then the Organ Donation View Registration page is displayed with my existing decision to opt-in-some
    When I choose to amend my Organ Donation decision
    Then the internal Organ Donation Choice Page is displayed
    And I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    And the some organs option is selected
    And I click the 'Continue' button
    Then the Organ Donation Specific Organ Choice page is displayed
    And my previous decisions are displayed on the Organ Donation Specific Organ Choice page
    When I choose which organs to donate
    And I click the 'Continue' button
    Then the Organ Donation Faith And Beliefs page is displayed
    And the previous option on the Organ Donation Faith And Beliefs page is selected
    And I click the 'Continue' button
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And my specific organ donation choices are displayed on the Organ Donation Check Details page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button
    Then the Organ Donation View Registration page is displayed
    And the decision to opt in to organ donation with some organs has been successfully created
    
  Scenario: A user opted to not donate their organs can amend it to donate all
    Given I am a EMIS user registered as opt-out who then amends their decision to opt-in
    And I am logged in
    When I navigate to the internal Organ Donation Page
    Then the Organ Donation View Registration page is displayed with my existing decision to opt-out
    When I choose to amend my Organ Donation decision
    Then the internal Organ Donation Choice Page is displayed
    And I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    When I select the option to donate all my organs
    And I click the 'Continue' button
    Then the Organ Donation Faith And Beliefs page is displayed
    And no options on the Organ Donation Faith And Beliefs page are selected
    When I select an option in sharing my organ donation faith and beliefs
    And I click the 'Continue' button
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And the choice of wishing to donate organs is displayed on the Organ Donation Check Details page
    And my choice of whether to share my faith and beliefs is displayed on the Organ Donation Check Details page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button
    Then the Organ Donation View Registration page is displayed
    And the decision to opt in to organ donation has been successfully created

  Scenario: A user who opted to not donate their organs can amend it to donate some
    Given I am a TPP user registered as opt-out who then amends their decision to some
    And I am logged in
    When I navigate to the internal Organ Donation Page
    Then the Organ Donation View Registration page is displayed with my existing decision to opt-out
    When I choose to amend my Organ Donation decision
    Then the internal Organ Donation Choice Page is displayed
    And I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    When I select the option to donate some of my organs
    And I click the 'Continue' button
    Then the Organ Donation Specific Organ Choice page is displayed
    And no options on the Organ Donation Specific Organ Choice page are selected
    When I choose which organs to donate
    And I click the 'Continue' button
    Then the Organ Donation Faith And Beliefs page is displayed
    And no options on the Organ Donation Faith And Beliefs page are selected
    When I select an option in sharing my organ donation faith and beliefs
    And I click the 'Continue' button
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And my specific organ donation choices are displayed on the Organ Donation Check Details page
    And my choice of whether to share my faith and beliefs is displayed on the Organ Donation Check Details page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button
    Then the Organ Donation View Registration page is displayed
    And the decision to opt in to organ donation with some organs has been successfully created

  Scenario: A user can navigate back through the amend journey
    Given I am a EMIS user registered as opt-in with organ donation, who wishes to amend
    And I am logged in
    When I navigate to the internal Organ Donation Page
    Then the Organ Donation View Registration page is displayed with my existing decision to opt-in
    When I choose to amend my Organ Donation decision
    Then the internal Organ Donation Choice Page is displayed
    When I click the 'Back' button
    Then the Organ Donation View Registration page is displayed with my existing decision to opt-in

  Scenario: A user is informed when an amended registration is in conflicted state
    Given I am a EMIS user registered as opt-in with organ donation, who wishes to opt-out but will cause a conflict
    And I am logged in
    When I navigate to the internal Organ Donation Page
    Then the Organ Donation View Registration page is displayed with my existing decision to opt-in
    When I choose to amend my Organ Donation decision
    Then the internal Organ Donation Choice Page is displayed
    When I follow the opt-out journey to the 'Confirmation' page
    Then the organ donation decision has been submitted and is to be processed

  Scenario: A user can find out more about organ donation when amending their decision
    Given I am a EMIS user registered as opt-in with organ donation, who wishes to amend
    And I am logged in
    Then I navigate to the internal Organ Donation Page
    When I choose to amend my Organ Donation decision
    Then the internal Organ Donation Choice Page is displayed
    When I select the Find Out More About Organ Donation link
    Then a new tab opens https://www.organdonation.nhs.uk/faq/

  Scenario Outline: A user amending a decision, where OD returns a <Error Code> recoverable error, is shown an
  error message and can retry
    Given I am a EMIS user registered as opt-in amends to opt-out, but OD returns recoverable <Error Code> error
    And I am logged in
    When I navigate to the internal Organ Donation Page
    Then the Organ Donation View Registration page is displayed with my existing decision to opt-in
    When I choose to amend my Organ Donation decision
    Then the internal Organ Donation Choice Page is displayed
    When I follow the opt-out journey to the 'Check Details' page
    And I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button
    And I see an appropriate Organ Donation error message with a retry option
    When I click the 'Try again' button
    And the Organ Donation View Registration page is displayed
    Examples:
      | Error Code |
      | 503        |

  Scenario Outline: A user amending a decision, where OD returns a <Error Code> recoverable error, is shown an
  error message and can't retry
    Given I am a EMIS user registered as opt-in amends to opt-out, but OD returns non-recoverable <Error Code> error
    And I am logged in
    When I navigate to the internal Organ Donation Page
    Then the Organ Donation View Registration page is displayed with my existing decision to opt-in
    When I choose to amend my Organ Donation decision
    Then the internal Organ Donation Choice Page is displayed
    When I follow the opt-out journey to the 'Check Details' page
    And I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button
    And I see an appropriate Organ Donation error message without a retry option
    Examples:
      | Error Code |
      | 401        |
