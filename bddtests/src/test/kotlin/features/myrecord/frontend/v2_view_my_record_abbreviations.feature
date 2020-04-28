@my-record
Feature: Clinical Abbreviations Frontend - Medical Record v2

  Scenario: A patient can navigate to the clinical abbreviations page - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has enabled allergies functionality and has a drug and non drug allergy record for VISION
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then I see the Medical Record Warning page
    When I click continue
    When I click the link called 'Help with abbreviations' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/abbreviations/'
    Then a new tab has been opened by the link
