@organ-donation
Feature: Organ Donation Frontend

#  This test covers navigation via buttons/links
  Scenario Outline: A <GP System> user can opt to donate all their organs
    Given I am using the native app user agent
    And I am a <GP System> user not registered with organ donation, who wishes to register and opt in
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    And I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    When I select the option to donate all my organs
    And I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Faith And Beliefs page is displayed
    And no options on the Organ Donation Faith And Beliefs page are selected
    When I select an option in sharing my organ donation faith and beliefs
    And I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Check Details page is displayed
    And the choice of wishing to donate organs is displayed on the Organ Donation Check Details page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button on an Organ Donation page
    Then the Organ Donation View Registration page is displayed
    And the decision to opt in to organ donation has been successfully created
  @smoketest
  @nativesmoketest
    Examples:
      | GP System |
      | EMIS      |
    Examples:
      | GP System |
      | TPP       |
      | VISION    |
      | MICROTEST |

# These tests navigate directly to the pages where the features are to be tested, to save time.
  Scenario Outline: A user can select '<Option>' for faith and belief sharing when opting to donate all their organs
    Given I am using the native app user agent
    And I am a EMIS user not registered with organ donation, who wishes to opt in with '<Option>' faith decision
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    And I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    When I select the option to donate all my organs
    And I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Faith And Beliefs page is displayed
    And no options on the Organ Donation Faith And Beliefs page are selected
    When I select the option '<Option>' to share my organ donation faith and beliefs
    And I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Check Details page is displayed
    And the choice of wishing to donate organs is displayed on the Organ Donation Check Details page
    And my choice of '<Option>' to share my faith and beliefs is displayed on the Organ Donation Check Details page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button on an Organ Donation page
    Then the Organ Donation View Registration page is displayed
    And the decision to opt in to organ donation has been successfully created
    And the faith and beliefs decision of '<Option>' is displayed on the Organ Donation View Registration page
    Examples:
      | Option                            |
      | Yes - this is applicable to me    |
      | No - this is not applicable to me |
      | Prefer not to say                 |

  Scenario Outline: A <GP System> user can opt to not donate their organs
    Given I am using the native app user agent
    And I am a <GP System> user not registered with organ donation, who wishes to register and opt out
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    And I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Check Details page is displayed
    And the choice of not wishing to donate organs is displayed on the Organ Donation Check Details page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button on an Organ Donation page
    Then the Organ Donation View Registration page is displayed
    And the decision to opt out of organ donation has been successfully created
  @nativesmoketest
    Examples:
      | GP System |
      | VISION    |
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | MICROTEST |

  Scenario Outline: A <GP System> user can opt to donate some of their organs
    Given I am using the native app user agent
    And I am a <GP System> user not registered with organ donation, who wishes to register and donate some organs
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    And I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    When I select the option to donate some of my organs
    And I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Specific Organ Choice page is displayed
    When I choose which organs to donate
    And I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Faith And Beliefs page is displayed
    And no options on the Organ Donation Faith And Beliefs page are selected
    When I select an option in sharing my organ donation faith and beliefs
    And I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Check Details page is displayed
    And my specific organ donation choices are displayed on the Organ Donation Check Details page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button on an Organ Donation page
    Then the Organ Donation View Registration page is displayed
    And the decision to opt in to organ donation with some organs has been successfully created
    Examples:
      | GP System |
      | TPP       |
      | EMIS      |
      | VISION    |
      | MICROTEST |

  Scenario: A user can choose to record their ethnicity and religion when opting out of organ donation
    Given I am using the native app user agent
    And I am a EMIS user not registered with organ donation, who wishes to register and opt out
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    When I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    When I select an ethnicity to record for organ donation
    And I select a religion to record for organ donation
    And I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Check Details page is displayed
    And the choice of not wishing to donate organs is displayed on the Organ Donation Check Details page
    And my ethnicity and religion are recorded on the Organ Donation Check Details page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button on an Organ Donation page
    Then the Organ Donation View Registration page is displayed
    And the decision to opt out of organ donation has been successfully created

  Scenario: A user can choose not to record their ethnicity or religion when opting out of organ donation
    Given I am using the native app user agent
    And I am a EMIS user not registered with organ donation, who wishes to register and opt out
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    When I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Check Details page is displayed
    And the choice of not wishing to donate organs is displayed on the Organ Donation Check Details page
    And my ethnicity and religion are recorded as not chosen on the Organ Donation Check Details page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button on an Organ Donation page
    Then the Organ Donation View Registration page is displayed
    And the decision to opt out of organ donation has been successfully created

  Scenario: A user can view the privacy statement on the organ donation Check Details page
    Given I am using the native app user agent
    And I am a EMIS user not registered with organ donation, who wishes to register and opt out
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    And I follow the opt-out journey to the 'Check Details' page
    When I click the link called 'privacy statement' with a url of 'https://www.organdonation.nhs.uk/app/app-privacy/'
    Then a new tab has been opened by the link

  Scenario: A user can register to be a blood donor on the organ donation View Registration page
    Given I am using the native app user agent
    And I am a EMIS user not registered with organ donation, who wishes to register and opt out
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    And I follow the opt-out journey to the 'Confirmation' page
    When I click the link called 'Register to be a blood donor' with a url of 'https://www.blood.co.uk/'
    Then a new tab has been opened by the link

  Scenario: A user can navigate back through the opt out journey
    Given I am using the native app user agent
    And I am a EMIS user not registered with organ donation, who wishes to register and opt out
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    And I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Check Details page is displayed
    And the choice of not wishing to donate organs is displayed on the Organ Donation Check Details page
    When I click the 'Back' breadcrumb
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Back' breadcrumb
    Then the internal Organ Donation Choice Page is displayed

  Scenario: A user can navigate back through the opt in journey
    Given I am using the native app user agent
    And I am a EMIS user not registered with organ donation, who wishes to register and opt in
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    And I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    When I select the option to donate all my organs
    And I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Faith And Beliefs page is displayed
    And no options on the Organ Donation Faith And Beliefs page are selected
    When I select the option 'Yes - this is applicable to me' to share my organ donation faith and beliefs
    And I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Check Details page is displayed
    And the choice of wishing to donate organs is displayed on the Organ Donation Check Details page
    When I click the 'Back' breadcrumb
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Back' breadcrumb
    Then the Organ Donation Faith And Beliefs page is displayed
    When I click the 'Back' breadcrumb
    Then the Organ Donation Your Choice page is displayed
    When I click the 'Back' breadcrumb
    Then the internal Organ Donation Choice Page is displayed

  Scenario: A user can navigate back through the opt in with some organs journey
    Given I am using the native app user agent
    And I am a EMIS user not registered with organ donation, who wishes to register and donate some organs
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    And I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    When I select the option to donate some of my organs
    And I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Specific Organ Choice page is displayed
    When I choose which organs to donate
    And I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Faith And Beliefs page is displayed
    And no options on the Organ Donation Faith And Beliefs page are selected
    When I select the option 'Yes - this is applicable to me' to share my organ donation faith and beliefs
    And I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Check Details page is displayed
    When I click the 'Back' breadcrumb
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Back' breadcrumb
    Then the Organ Donation Faith And Beliefs page is displayed
    When I click the 'Back' breadcrumb
    Then the Organ Donation Specific Organ Choice page is displayed
    And my previous decisions are displayed on the Organ Donation Specific Organ Choice page
    When I click the 'Back' breadcrumb
    Then the Organ Donation Your Choice page is displayed
    When I click the 'Back' breadcrumb
    Then the internal Organ Donation Choice Page is displayed

  Scenario: A user is shown a validation message if they submit their decision without ticking both the required checkboxes
    Given I am using the native app user agent
    And I am a EMIS user not registered with organ donation, who wishes to register and opt out
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    And I follow the opt-out journey to the 'Check Details' page
    And the choice of not wishing to donate organs is displayed on the Organ Donation Check Details page
    Then a validation message is shown if both or either of the required conditions for organ donation are not checked

  Scenario: A user is shown a validation message if they submit faith and beliefs without selecting an option
    Given I am using the native app user agent
    And I am a EMIS user not registered with organ donation, who wishes to register and opt in
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    And I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    When I select the option to donate all my organs
    And I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Faith And Beliefs page is displayed
    Then a validation message is shown if a user attempts to continue without selecting a faith and belief option

  Scenario: A user is shown a validation message if they attempt to continue opting in without selecting 'All' or 'Some'
    Given I am using the native app user agent
    And I am a EMIS user not registered with organ donation, who wishes to register and opt in
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    And I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    And a validation message is shown if a user attempts to continue without choosing to donate all or some organs

  Scenario: A user is shown a validation error if they submit their decision without choosing an option for each organ
    Given I am using the native app user agent
    And I am a EMIS user not registered with organ donation, who wishes to register and donate some organs
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    And I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    When I select the option to donate some of my organs
    And I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Specific Organ Choice page is displayed
    And a validation message is shown if a user attempts to continue without selecting a decision for all organs

  Scenario: A user is shown a validation error if they select 'no' for all specific organ and tissue donation choices
    Given I am using the native app user agent
    And I am a EMIS user not registered with organ donation, who wishes to register
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    And I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    When I select the option to donate some of my organs
    And I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Specific Organ Choice page is displayed
    And a validation message is shown if a user attempts to continue with all specific organ options set to no

  Scenario: When a user has no organ donation registration, they may follow a link to check their registration
    Given I am using the native app user agent
    And I am a EMIS user not registered with organ donation, who wishes to register
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    And I click the link called 'Think you have registered already?' with a url of 'https://www.organdonation.nhs.uk/app/app-check/'
    Then a new tab has been opened by the link

  Scenario: A user can find out more about organ donation when registering
    Given I am using the native app user agent
    And I am a EMIS user not registered with organ donation, who wishes to register
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    And I click the link called 'Find out more about organ donation' with a url of 'https://www.organdonation.nhs.uk/app/app-donation/'
    Then a new tab has been opened by the link

  Scenario: A user can find out more about examples of of end of life wishes
    Given I am using the native app user agent
    And I am a EMIS user not registered with organ donation, who wishes to register and opt in
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    And I follow the opt-in journey to the 'Faith And Beliefs' page
    Then the Organ Donation Faith And Beliefs page is displayed
    And the Organ Donation 'Examples of end of life wishes' is collapsed, and can be expanded

  Scenario: A user can view more information about the specific organs and tissue when opting to donate some organs
    Given I am using the native app user agent
    And I am a EMIS user not registered with organ donation, who wishes to register and donate some organs
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    And I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    When I select the option to donate some of my organs
    And I click the 'Continue' button on an Organ Donation page
    Then the Organ Donation Specific Organ Choice page is displayed
    When I click the Find out more about organs and tissue link
    Then the Find Out More About Organs And Tissue Page is displayed
    When I click the 'Back' breadcrumb
    Then the Organ Donation Specific Organ Choice page is displayed

  Scenario Outline: A user opting out, where OD returns a <Error Code> recoverable error is shown an error message and can retry
    Given I am using the native app user agent
    And I am a EMIS user who wishes to register as opt out, but OD returns recoverable <Error Code> error
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    When I follow the opt-out journey to the 'Check Details' page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button on an Organ Donation page
    And I see an appropriate Organ Donation error message with a retry option
    When I click the 'Try again' button on an Organ Donation page
    And the Organ Donation View Registration page is displayed
    Examples:
      | Error Code |
      | 429        |

  Scenario Outline: A user opting out, where OD returns a <Error Code> non-recoverable error, is shown an error message and can't retry
    Given I am using the native app user agent
    And I am a EMIS user who wishes to register as opt out, but OD returns non-recoverable <Error Code> error
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    And I follow the opt-out journey to the 'Check Details' page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Submit my decision' button on an Organ Donation page
    And I see an appropriate Organ Donation error message without a retry option
    Examples:
      | Error Code |
      | 400        |

    Scenario: A user registering, where OD takes too long to respond, is shown decision pending page
      Given I am using the native app user agent
      And I am a EMIS user who wishes to register as opt out, but OD takes too long to respond
      And I am logged in
      When I retrieve the 'Organ Donation' page directly
      And I follow the opt-out journey to the 'Check Details' page
      When I confirm that my details are accurate, and accept the privacy statement for organ donation
      And I click the 'Submit my decision' button on an Organ Donation page
      And I wait for 15 seconds
      And I see an appropriate Organ Donation decision processing message without a retry option

    Scenario: A user cannot submit another registration request while waiting for the first to complete
      Given I am using the native app user agent
      And I am a EMIS user who wishes to register as opt out, but OD takes too long to respond
      And I am logged in
      When I retrieve the 'Organ Donation' page directly
      And I follow the opt-out journey to the 'Check Details' page
      When I confirm that my details are accurate, and accept the privacy statement for organ donation
      And I click the 'Submit my decision' button on an Organ Donation page
      Then the 'Submit my decision' button has the 'disabled' attribute

  Scenario: A user can find out more about changes to the organ donation laws when registering
    Given I am using the native app user agent
    And I am a EMIS user not registered with organ donation, who wishes to register
    And I am logged in
    When I retrieve the 'Organ Donation' page directly
    And I click the link called 'changes to the law around organ donation may affect you' with a url of 'https://www.organdonation.nhs.uk/app/app-donation/#law'
    Then a new tab has been opened by the link