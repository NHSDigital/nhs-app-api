@accessibility
@online-consultations-accessibility
Feature: online consultations accessibility

  Scenario: The Online Consultation Components are captured
    Given I am logged in as a EMIS user with no linked profiles
    And I have access to online consultations gp advice journey and it is not an emergency
    When I navigate to Advice
    Then the Advice page is displayed
    When I click Ask your GP for Advice
   # Checkbox
    And the OLC_CheckBox page is saved to disk
    And I accept demographics and terms and conditions question
   # RadioGroup
    And the OLC_RadioGroups page is saved to disk
    And I am submitting the questionnaire for myself
    And I see a condition list for myself
    And I click on a condition
    And I click continue without selecting an option
    # error & summary
    And the OLC_ErrorMessage_and_Summary page is saved to disk
    And I am not in an emergency
    And I select my gender and click continue
    # Text Area
    And the OLC_TextArea page is saved to disk
    And I insert my symptoms and click continue
    # Date Input
    And the OLC_DateInput page is saved to disk
    And I insert my date of birth
    And I select how much alcohol I drink weekly
    # Hint Text, Text Input and Select
    And the OLC_TextInput_HintText_And_Select page is saved to disk
    And I insert how long I have felt the pain
    Then I see a care plan for myself
