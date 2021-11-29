@accessibility
@online-consultation-errors-accessibility
Feature: online consultation errors accessibility

  Scenario: Service unavailable error is captured
    Given I am logged in as a EMIS user with no linked profiles
    And I have access to online consultations but they are switched off by the practice
    When I navigate to Advice
    Then the Advice page is displayed
    When I click Ask your GP for Advice
    Then I see the online consultations unavailable message for gp advice
    And the Errors_OLC31b_ServiceUnavailable page is saved to disk

  Scenario: leave page warning is captured
    Given I am logged in as a EMIS user with no linked profiles
    And I have access to online consultations gp advice journey and it is an emergency
    When I navigate to Advice
    Then the Advice page is displayed
    When I click Ask your GP for Advice
    And I accept demographics and terms and conditions question
    And I am submitting the questionnaire for myself
    And I see a condition list for myself
    And I click on a condition
    And I click the home icon
    And I see the page leave warning
    And the Errors_OLC32_LeavePageWarning page is saved to disk
