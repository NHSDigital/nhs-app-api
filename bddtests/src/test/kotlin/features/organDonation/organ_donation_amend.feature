@organ-donation
Feature: Organ Donation - Amend

  Scenario Outline: A <GP System> user who opted to donate all their organs can amend it to not donate
    Given I am a <GP System> user registered as opt-in who then amends their decision to opt-out
    And I am logged in
    When I navigate to the internal Organ Donation Page
    Then the Organ Donation page is displayed with my existing decision to opt-in
    When I choose to amend my Organ Donation decision
    Then the internal Organ Donation Choice Page is displayed
    When I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And the choice of not wishing to donate organs is displayed on the Organ Donation Check Details page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'No I do not want to be a donor' button
    Then the Organ Donation Confirmation page is displayed
    And the decision to opt out of organ donation has been successfully created
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user who opted to donate all their organs can amend it to only donate some
    Given I am a <GP System> user registered as opt-in who then amends their decision to some
    And I am logged in
    And I navigate to the internal Organ Donation Page
    Then the Organ Donation page is displayed with my existing decision to opt-in
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
    And I click the 'Yes I want to be a donor' button
    Then the Organ Donation Confirmation page is displayed
    And the decision to opt in to organ donation with some organs has been successfully created
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user who opted to donate all their organs can amend faith and beliefs
    Given I am a <GP System> user registered as opt-in who then amends their faith and beliefs
    And I am logged in
    And I navigate to the internal Organ Donation Page
    Then the Organ Donation page is displayed with my existing decision to opt-in
    When I choose to amend my Organ Donation decision
    Then the internal Organ Donation Choice Page is displayed
    When I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    And the all organs option is selected
    And I click the 'Continue' button
    Then the Organ Donation Faith And Beliefs page is displayed
    And the previous option on the Organ Donation Faith And Beliefs page is selected
    When I select the option 'Yes' to share my organ donation faith and beliefs
    And I click the 'Continue' button
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And the choice of wishing to donate organs is displayed on the Organ Donation Check Details page
    And my choice of 'Yes' to share my faith and beliefs is displayed on the Organ Donation Check Details page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Yes I want to be a donor' button
    Then the Organ Donation Confirmation page is displayed
    And the decision to opt in to organ donation has been successfully created
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user opted to donate some of their organs can amend it to not donate
    Given I am a <GP System> user registered as some who then amends their decision to opt-out
    And I am logged in
    When I navigate to the internal Organ Donation Page
    Then the Organ Donation page is displayed with my existing decision of some
    When I choose to amend my Organ Donation decision
    Then the internal Organ Donation Choice Page is displayed
    When I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And the choice of not wishing to donate organs is displayed on the Organ Donation Check Details page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'No I do not want to be a donor' button
    Then the Organ Donation Confirmation page is displayed
    And the decision to opt out of organ donation has been successfully created
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user opted to donate some of their organs can amend to donate all
    Given I am a <GP System> user registered as some who then amends their decision to opt-in
    And I am logged in
    When I navigate to the internal Organ Donation Page
    Then the Organ Donation page is displayed with my existing decision of some
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
    And I click the 'Yes I want to be a donor' button
    Then the Organ Donation Confirmation page is displayed
    And the decision to opt in to organ donation has been successfully created
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user opted to donate some of their organs can amend the selected organs
    Given I am a <GP System> user registered as some who then amends their selected organs
    And I am logged in
    When I navigate to the internal Organ Donation Page
    Then the Organ Donation page is displayed with my existing decision of some
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
    And I click the 'Yes I want to be a donor' button
    Then the Organ Donation Confirmation page is displayed
    And the decision to opt in to organ donation with some organs has been successfully created
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user opted to not donate their organs can amend it to donate all
    Given I am a <GP System> user registered as opt-out who then amends their decision to opt-in
    And I am logged in
    When I navigate to the internal Organ Donation Page
    Then the Organ Donation page is displayed with my existing decision to opt-out
    When I choose to amend my Organ Donation decision
    Then the internal Organ Donation Choice Page is displayed
    And I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
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
    And I click the 'Yes I want to be a donor' button
    Then the Organ Donation Confirmation page is displayed
    And the decision to opt in to organ donation has been successfully created
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user opted to not donate their organs can amend it to donate some
    Given I am a <GP System> user registered as opt-out who then amends their decision to some
    And I am logged in
    When I navigate to the internal Organ Donation Page
    Then the Organ Donation page is displayed with my existing decision to opt-out
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
    And I click the 'Continue' button
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And my specific organ donation choices are displayed on the Organ Donation Check Details page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'Yes I want to be a donor' button
    Then the Organ Donation Confirmation page is displayed
    And the decision to opt in to organ donation with some organs has been successfully created
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario: A user can navigate back through the amend journey
    Given I am a EMIS user registered as opt-in with organ donation, who wishes to amend
    And I am logged in
    When I navigate to the internal Organ Donation Page
    Then the Organ Donation page is displayed with my existing decision to opt-in
    When I choose to amend my Organ Donation decision
    Then the internal Organ Donation Choice Page is displayed
    When I click the 'Back' button
    Then the Organ Donation page is displayed with my existing decision to opt-in

  Scenario: A user is informed when an amended registration is in conflicted state
    Given I am a EMIS user registered as opt-in with organ donation, who wishes to opt-out but will cause a conflict
    And I am logged in
    When I navigate to the internal Organ Donation Page
    Then the Organ Donation page is displayed with my existing decision to opt-in
    When I choose to amend my Organ Donation decision
    Then the internal Organ Donation Choice Page is displayed
    When I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    And the choice of not wishing to donate organs is displayed on the Organ Donation Check Details page
    When I confirm that my details are accurate, and accept the privacy statement for organ donation
    And I click the 'No I do not want to be a donor' button
    Then the organ donation decision has been submitted and is to be processed

  Scenario: A user can find out more about organ donation when amending their decision
    Given I am a EMIS user registered as opt-in with organ donation, who wishes to amend
    And I am logged in
    Then I navigate to the internal Organ Donation Page
    When I choose to amend my Organ Donation decision
    Then the internal Organ Donation Choice Page is displayed
    When I select the Find Out More About Organ Donation link
    Then a new tab opens https://www.organdonation.nhs.uk/faq/
