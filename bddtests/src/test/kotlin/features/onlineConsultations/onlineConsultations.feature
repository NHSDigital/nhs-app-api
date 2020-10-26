@onlineconsultations
Feature: Online Consultations

Scenario: A user can go through the online consultations gp advice journey and it is an emergency
  Given I am logged in as a EMIS user with no linked profiles
  And I have access to online consultations gp advice journey and it is an emergency
  When I navigate to Advice
  Then the Advice page is displayed
  When I click Ask your GP for Advice
  And I accept demographics and terms and conditions question
  And I click on a condition
  And I select my gender and click continue
  And I am submitting the questionnaire for myself
  And I am in an emergency and I agree to end my consultation
  Then I see advice on what to do next

Scenario: A user going through the online consultations journey accidentally navigates away can use the modal to stay on the page and then complete their journey
  Given I am logged in as a EMIS user with no linked profiles
  And I have access to online consultations gp advice journey and it is an emergency
  When I navigate to Advice
  Then the Advice page is displayed
  When I click Ask your GP for Advice
  And I accept demographics and terms and conditions question
  And I click on a condition
  And I select my gender and click continue
  And I am submitting the questionnaire for myself
  And I navigate to Appointments
  Then I see the page leave warning
  And I click stay on page on the popup
  And I am in an emergency and I agree to end my consultation
  And I see advice on what to do next

Scenario: A user going through the online consultations journey navigates away and can use the modal to leave the page
  Given I am logged in as a EMIS user with no linked profiles
  And I have access to online consultations gp advice journey and it is an emergency
  When I navigate to Advice
  Then the Advice page is displayed
  When I click Ask your GP for Advice
  And I accept demographics and terms and conditions question
  And I click on a condition
  And I select my gender and click continue
  And I am submitting the questionnaire for myself
  And I click the home icon
  And I see the page leave warning
  Then I click leave the page on the popup
  And I see the home page

Scenario: A user can go through the online consultations gp advice journey and it is not an emergency
  Given I am logged in as a EMIS user with no linked profiles
  And I have access to online consultations gp advice journey and it is not an emergency
  When I navigate to Advice
  Then the Advice page is displayed
  When I click Ask your GP for Advice
  And I accept demographics and terms and conditions question
  And I click on a condition
  And I select my gender and click continue
  And I am submitting the questionnaire for myself
  And I am not in an emergency
  And I insert my symptoms and click continue
  And I insert my date of birth
  And I select how much alcohol I drink weekly
  And I click the origin of the pain on the image
  And I insert how long I have felt the pain
  Then I see a care plan

  Scenario: A user can go through the online consultations gp advice journey and it is not an emergency with no GP session
    Given I am an EMIS patient whose GP system is unavailable
    And I have access to online consultations gp advice journey and it is not an emergency with no GP session
    And I am logged in
    When I navigate to Advice
    Then the Advice page is displayed
    When I click Ask your GP for Advice
    And I accept demographics and terms and conditions question
    And I click on a condition
    And I select my gender and click continue
    And I am submitting the questionnaire for myself
    And I am not in an emergency
    And I insert my symptoms and click continue
    And I insert my date of birth
    And I select how much alcohol I drink weekly
    And I click the origin of the pain on the image
    And I insert how long I have felt the pain
    Then I see a care plan

Scenario: A user can end their online consultation journey and go back to the home page
  Given I am logged in as a EMIS user with no linked profiles
  And I have access to online consultations gp advice journey and it is an emergency
  When I navigate to Advice
  Then the Advice page is displayed
  When I click Ask your GP for Advice
  And I accept demographics and terms and conditions question
  And I click the end my consultation button
  Then I see the home page

Scenario: A user sees an appropriate error when online consultations are switched off by the practice
  Given I am logged in as a EMIS user with no linked profiles
  And I have access to online consultations but they are switched off by the practice
  When I navigate to Advice
  Then the Advice page is displayed
  When I click Ask your GP for Advice
  Then I see the online consultations unavailable message for gp advice
  When I navigate to Appointments
  And I click the Additional GP services link on the Appointments Hub
  Then I see the online consultations unavailable message for admin help
