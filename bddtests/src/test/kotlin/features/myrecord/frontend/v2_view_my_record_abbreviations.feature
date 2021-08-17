@my-record
Feature: Clinical Abbreviations Frontend - Medical Record v2

  Scenario: A patient can navigate to the clinical abbreviations page - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has enabled allergies functionality and has a drug and non drug allergy record for VISION
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the link called 'Help with abbreviations' with a url of 'http://stubs.local.bitraft.io:8080/help/health-records-in-the-nhs-app/abbreviations-commonly-found-in-medical-records/'
    And a new tab has been opened by the link
    Then I see the abbreviations help page
