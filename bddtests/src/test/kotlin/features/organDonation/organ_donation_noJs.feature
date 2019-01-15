@organ-donation
@noJs
Feature: Organ Donation Without Javascript

  Scenario: A user can opt to not donate their organs with javascript disabled
    Given I have disabled javascript
    And I am a EMIS user not registered with organ donation, who wishes to register
    And I am logged in
    When I navigate to the internal Organ Donation Choice Page
    When I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    And I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    #Check can submit in future task

  Scenario: A user can navigate back to the Organ Donation Decision page from Additional Details after opting to not
  donate their organs, with javascript disabled
    Given I have disabled javascript
    And I am a EMIS user not registered with organ donation, who wishes to register
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    When I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    When I click the 'Back' button
    Then the internal Organ Donation page is displayed

  Scenario: A user can opt to donate all their organs with javascript disabled
    Given I have disabled javascript
    And I am a EMIS user not registered with organ donation, who wishes to register
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    And I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    When I select the option to donate all my organs
    #Check can submit in future task

  Scenario: A user can navigate back to the internal Organ Donation Choice Page page from the All or Some Organs page with javascript disabled
    Given I have disabled javascript
    And I am a EMIS user not registered with organ donation, who wishes to register
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    And I choose to donate my organs
    Then the Organ Donation Your Choice page is displayed
    When I click the 'Back' button
    Then the internal Organ Donation Choice Page is displayed

  Scenario: A user can navigate back to the Additional Details page from Check Details page after opting to not donate
  their organs, with javascript disabled
    Given I have disabled javascript
    And I am a EMIS user not registered with organ donation, who wishes to register
    And I am logged in
    And I navigate to the internal Organ Donation Choice Page
    When I choose to not donate my organs
    Then the Organ Donation Decision Additional Details page is displayed
    And I click the 'Continue' button
    Then the Organ Donation Check Details page is displayed
    When I click the 'Back' button
    Then the Organ Donation Decision Additional Details page is displayed

