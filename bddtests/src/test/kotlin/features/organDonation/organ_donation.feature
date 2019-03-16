@organ-donation
Feature: Organ Donation

  Scenario Outline: A <GP System> user can opt to donate all their organs
    Given I am a <GP System> user not registered with organ donation, who wishes to register and opt in
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    And I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    When I select the option to donate all my organs
    And I click the 'Continue' button
    Then the Organ Donation Faith And Beliefs page is displayed
    And no options on the Organ Donation Faith And Beliefs page are selected
    When I select the option 'Yes' to share my organ donation faith and beliefs
    And I click the 'Continue' button
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And the choice of wishing to donate organs is displayed on the Organ Donation Check Details page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button
    Then the Organ Donation View Registration page is displayed
    And the decision to opt in to organ donation has been successfully created
    @smoketest
    Examples:
      | GP System |
      | EMIS      |
    Examples:
      | GP System |
      | TPP       |
      | VISION    |

  Scenario Outline: A user can select '<Option>' for faith and belief sharing when opting to donate all their organs
    Given I am a EMIS user not registered with organ donation, who wishes to register and opt in
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    And I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    When I select the option to donate all my organs
    And I click the 'Continue' button
    Then the Organ Donation Faith And Beliefs page is displayed
    And no options on the Organ Donation Faith And Beliefs page are selected
    When I select the option '<Option>' to share my organ donation faith and beliefs
    And I click the 'Continue' button
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And the choice of wishing to donate organs is displayed on the Organ Donation Check Details page
    And my choice of '<Option>' to share my faith and beliefs is displayed on the Organ Donation Check Details page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button
    Then the Organ Donation View Registration page is displayed
    And the decision to opt in to organ donation has been successfully created
    Examples:
      | Option            |
      | Yes               |
      | No                |
      | Prefer not to say |

  Scenario Outline: A <GP System> user can opt to not donate their organs
    Given I am a <GP System> user not registered with organ donation, who wishes to register and opt out
    And I am logged in
    When I navigate to the internal Organ Donation Choice Page
    When I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And the choice of not wishing to donate organs is displayed on the Organ Donation Check Details page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button
    Then the Organ Donation View Registration page is displayed
    And the decision to opt out of organ donation has been successfully created

    @smoketest
    Examples:
      | GP System |
      | VISION    |
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: A <GP System> user can opt to donate some of their organs
    Given I am a <GP System> user not registered with organ donation, who wishes to register and donate some organs
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    And I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    When I select the option to donate some of my organs
    And I click the 'Continue' button
    Then the Organ Donation Specific Organ Choice page is displayed
    When I choose which organs to donate
    And I click the 'Continue' button
    Then the Organ Donation Faith And Beliefs page is displayed
    And no options on the Organ Donation Faith And Beliefs page are selected
    When I select the option 'No' to share my organ donation faith and beliefs
    And I click the 'Continue' button
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And my specific organ donation choices are displayed on the Organ Donation Check Details page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button
    Then the Organ Donation View Registration page is displayed
    And the decision to opt in to organ donation with some organs has been successfully created
    @smoketest
    Examples:
      | GP System |
      | TPP       |
    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  Scenario: A user can choose to record their ethnicity when opting out of organ donation
    Given I am a EMIS user not registered with organ donation, who wishes to register and opt out
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    When I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    When I select an ethnicity to record for organ donation
    And I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And the choice of not wishing to donate organs is displayed on the Organ Donation Check Details page
    And my ethnicity is recorded on the Organ Donation Check Details page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button
    Then the Organ Donation View Registration page is displayed
    And the decision to opt out of organ donation has been successfully created

  Scenario: A user can choose not to record their ethnicity when opting out of organ donation
    Given I am a EMIS user not registered with organ donation, who wishes to register and opt out
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    When I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And the choice of not wishing to donate organs is displayed on the Organ Donation Check Details page
    And my ethnicity is recorded as not chosen on the Organ Donation Check Details page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button
    Then the Organ Donation View Registration page is displayed
    And the decision to opt out of organ donation has been successfully created

  Scenario: A user can choose to record their religion when opting out of organ donation
    Given I am a EMIS user not registered with organ donation, who wishes to register and opt out
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    When I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    When I select a religion to record for organ donation
    And I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And my religion is recorded on the Organ Donation Check Details page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button
    Then the Organ Donation View Registration page is displayed
    And the decision to opt out of organ donation has been successfully created

  Scenario: A user can choose not to record their religion when opting out of organ donation
    Given I am a EMIS user not registered with organ donation, who wishes to register and opt out
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    When I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    And I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And my religion is recorded as not chosen on the Organ Donation Check Details page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button
    Then the Organ Donation View Registration page is displayed
    And the decision to opt out of organ donation has been successfully created

  Scenario: A user can view the privacy statement on the organ donation Check Details page
    Given I am a EMIS user not registered with organ donation, who wishes to register and opt out
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    When I follow the opt-out journey to the 'Check Details' page
    And I select the privacy statement link on the Organ Donation Check Details page
    Then a new tab opens https://www.nhsbt.nhs.uk/privacy/

  Scenario: A user can register to be a blood donor on the organ donation View Registration page
    Given I am a EMIS user not registered with organ donation, who wishes to register and opt out
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    When I follow the opt-out journey to the 'Confirmation' page
    When I select the 'Register to be a blood donor' link on the Organ Donation View Registration page
    Then a new tab opens https://my.blood.co.uk/preregister

  Scenario: A user can navigate back through the opt out journey
    Given I am a EMIS user not registered with organ donation, who wishes to register and opt out
    And I am logged in
    When I navigate to the internal Organ Donation Choice Page
    When I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And the choice of not wishing to donate organs is displayed on the Organ Donation Check Details page
    When I click the 'Back' button
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Back' button
    Then the internal Organ Donation Choice Page is displayed

  Scenario: A user can navigate back through the opt in journey
    Given I am a EMIS user not registered with organ donation, who wishes to register and opt in
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    And I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    When I select the option to donate all my organs
    And I click the 'Continue' button
    Then the Organ Donation Faith And Beliefs page is displayed
    And no options on the Organ Donation Faith And Beliefs page are selected
    When I select the option 'Yes' to share my organ donation faith and beliefs
    And I click the 'Continue' button
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And the choice of wishing to donate organs is displayed on the Organ Donation Check Details page
    When I click the 'Back' button
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Back' button
    Then the Organ Donation Faith And Beliefs page is displayed
    When I click the 'Back' button
    Then the Organ Donation Your Choice page is displayed
    When I click the 'Back' button
    Then the internal Organ Donation Choice Page is displayed

  Scenario: A user can navigate back through the opt in with some organs journey
    Given I am a EMIS user not registered with organ donation, who wishes to register and donate some organs
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    And I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    When I select the option to donate some of my organs
    And I click the 'Continue' button
    Then the Organ Donation Specific Organ Choice page is displayed
    When I choose which organs to donate
    And I click the 'Continue' button
    Then the Organ Donation Faith And Beliefs page is displayed
    And no options on the Organ Donation Faith And Beliefs page are selected
    When I select the option 'Yes' to share my organ donation faith and beliefs
    And I click the 'Continue' button
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    When I click the 'Back' button
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Back' button
    Then the Organ Donation Faith And Beliefs page is displayed
    When I click the 'Back' button
    Then the Organ Donation Specific Organ Choice page is displayed
    And my previous decisions are displayed on the Organ Donation Specific Organ Choice page
    When I click the 'Back' button
    Then the Organ Donation Your Choice page is displayed
    When I click the 'Back' button
    Then the internal Organ Donation Choice Page is displayed

  Scenario: A user is shown a validation message if they submit their decision without ticking both the required checkboxes
    Given I am a EMIS user not registered with organ donation, who wishes to register and opt out
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    When I follow the opt-out journey to the 'Check Details' page
    And the choice of not wishing to donate organs is displayed on the Organ Donation Check Details page
    Then a validation message is shown if both or either of the required conditions for organ donation are not checked

  Scenario: A user is shown a validation message if they submit faith and beliefs without selecting an option
    Given I am a EMIS user not registered with organ donation, who wishes to register and opt in
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    And I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    When I select the option to donate all my organs
    And I click the 'Continue' button
    Then the Organ Donation Faith And Beliefs page is displayed
    Then a validation message is shown if a user attempts to continue without selecting a faith and belief option

  Scenario: A user is shown a validation message if they attempt to continue opting in without selecting 'All' or 'Some'
    Given I am a EMIS user not registered with organ donation, who wishes to register and opt in
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    And I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    And a validation message is shown if a user attempts to continue without choosing to donate all or some organs

  Scenario: A user is shown a validation error if they submit their decision without choosing an option for each organ
    Given I am a EMIS user not registered with organ donation, who wishes to register and donate some organs
    And I am logged in
    When I navigate to the internal Organ Donation Choice Page
    And I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    When I select the option to donate some of my organs
    And I click the 'Continue' button
    Then the Organ Donation Specific Organ Choice page is displayed
    And a validation message is shown if a user attempts to continue without selecting a decision for all organs

  Scenario: A user is shown a validation error if they select 'no' for all specific organ and tissue donation choices
    Given I am a EMIS user not registered with organ donation, who wishes to register
    And I am logged in
    When I navigate to the internal Organ Donation Choice Page
    And I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    When I select the option to donate some of my organs
    And I click the 'Continue' button
    Then the Organ Donation Specific Organ Choice page is displayed
    And a validation message is shown if a user attempts to continue with all specific organ options set to no

  Scenario: When a user has no organ donation registration, they may follow a link to check their registration
    Given I am a EMIS user not registered with organ donation, who wishes to register
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    When I select the Think You're Already Registered Organ Donation link
    Then a new tab opens https://www.organdonation.nhs.uk/register-to-donate/check-registration/

  Scenario: A user can find out more about organ donation when registering
    Given I am a EMIS user not registered with organ donation, who wishes to register
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    When I select the Find Out More About Organ Donation link
    Then a new tab opens https://www.organdonation.nhs.uk/faq/

  Scenario Outline: A user opting out, where OD returns a <Error Code> recoverable error is shown an error message
  and can retry
    Given I am a EMIS user who wishes to register as opt out, but OD returns recoverable <Error Code> error
    And I am logged in
    When I navigate to the internal Organ Donation Choice Page
    When I follow the opt-out journey to the 'Check Details' page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button
    And I see an appropriate Organ Donation error message with a retry option
    When I click the 'Try again' button
    And the Organ Donation View Registration page is displayed
    Examples:
      | Error Code |
      | 429        |

  Scenario Outline: An user opting out, where OD returns a <Error Code> non-recoverable error, is shown an error
  message and can't retry
    Given I am a EMIS user who wishes to register as opt out, but OD returns non-recoverable <Error Code> error
    And I am logged in
    When I navigate to the internal Organ Donation Choice Page
    When I follow the opt-out journey to the 'Check Details' page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button
    And I see an appropriate Organ Donation error message without a retry option
    Examples:
      | Error Code |
      | 400        |
