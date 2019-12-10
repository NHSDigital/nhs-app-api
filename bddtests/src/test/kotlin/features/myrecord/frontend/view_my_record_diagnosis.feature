@my-record
@vision-diagnosis
Feature: View My Medical Record Information - Diagnosis Frontend

  Scenario: A VISION user has no access to diagnosis section
    Given the my record wiremocks are initialised for VISION
    And the GP Practice has enabled demographics functionality
    And I do not have access to diagnosis
    And I am on my record information page
    When I click the diagnosis section
    Then I see a message indicating that I have no access to view Diagnosis on My Record

  Scenario: An error occurs when trying to retrieve diagnosis data from VISION
    Given the my record wiremocks are initialised for VISION
    And the GP Practice has enabled demographics functionality
    And an error occurred retrieving the diagnosis
    And I am on my record information page
    When I click the diagnosis section
    Then I see an error occurred message with Diagnosis on My Record

  Scenario: A VISION user navigates directly to the diagnosis details page
    Given the my record wiremocks are initialised for VISION
    And the GP Practice has enabled demographics functionality
    And the GP Practice has multiple diagnosis
    And I am on my record information page
    When I enter url address for diagnosis detail directly into the url
    Then I am redirected to the 'My Record' page
    And I see the your medical record page
    And I see the top of my medical record page
