@onlineconsultations
Feature: Online Consultations

Scenario: A user can go through the online consultations gp advice journey and it is an emergency
  Given I am logged in as a EMIS user with no linked profiles
  And I have access to online consultations gp advice journey and it is an emergency
  And I have no booked appointments for EMIS
  And I click on the Appointments link on the header
  And I click the GP Appointments link
  When I select "Book an appointment" button
  Then I am on the Appointments Guidance page
  When I select the GP Advice menu item on the guidance page
  And I accept demographics and terms and conditions question
  And I click on a condition
  And I select my gender and click continue
  And I am submitting the questionnaire for myself
  And I am in an emergency and I agree to end my consultation
  Then I see advice on what to do next

Scenario: A user can go through the online consultations gp advice journey and it is not an emergency
  Given I am logged in as a EMIS user with no linked profiles
  And I have access to online consultations gp advice journey and it is not an emergency
  And I have no booked appointments for EMIS
  And I click on the Appointments link on the header
  And I click the GP Appointments link
  When I select "Book an appointment" button
  Then I am on the Appointments Guidance page
  When I select the GP Advice menu item on the guidance page
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
  And I have no booked appointments for EMIS
  And I click on the Appointments link on the header
  And I click the GP Appointments link
  When I select "Book an appointment" button
  Then I am on the Appointments Guidance page
  When I select the GP Advice menu item on the guidance page
  And I accept demographics and terms and conditions question
  And I click the end my consultation button
  Then I see the home page

