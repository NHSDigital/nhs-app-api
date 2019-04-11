@my-record
Feature: My Record - User Not Enabled

  Scenario: A user without access to their medical record receives an error message
    Given the my record wiremocks are initialised for TPP
    And the GP Practice has enabled demographics functionality
    And the GP Practice has disabled summary care record functionality
    And the GP Practice has disabled dcr events functionality for TPP
    And I am on my record information page
    Then I see Service not offered by GP or to specific user or access revoked warning message

  Scenario: A user with partial access to their medical record can see some of their record
    Given the my record wiremocks are initialised for TPP
    And the GP Practice has enabled demographics functionality
    And the GP Practice has disabled summary care record functionality
    And the GP Practice has multiple dcr events for TPP
    And I am on my record information page
    Then I see the Consultations heading on My Record