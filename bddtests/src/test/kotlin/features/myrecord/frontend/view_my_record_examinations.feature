@my-record
@vision-examinations
Feature: View My Medical Record Information - Examinations

  Scenario: A VISION user has no access to examinations section
    Given the my record wiremocks are initialised for VISION
    And the GP Practice has enabled demographics functionality
    And I do not have access to examinations
    And I am on my record information page
    When I click the examinations section
    Then I see a message indicating that I have no access to view Examinations on My Record

  Scenario: An error occurs when trying to retrieve examinations data from VISION
    Given the my record wiremocks are initialised for VISION
    And the GP Practice has enabled demographics functionality
    And an error occurred retrieving the examinations
    And I am on my record information page
    When I click the examinations section
    Then I see an error occurred message with Examinations on My Record

  Scenario: A VISION user navigates directly to the examinations details page
    Given the my record wiremocks are initialised for VISION
    And the GP Practice has enabled demographics functionality
    And the GP Practice has multiple examinations
    And I am on my record information page
    When I enter url address for examinations detail directly into the url
    Then I am redirected to the 'My Record' page
    And I see the my record warning page
