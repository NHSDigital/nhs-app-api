@my-record
Feature: User Not Enabled Frontend - GP Medical Record

  Scenario: A user without access to their medical record receives an error message - GP Medical Record
    Given I am a TPP user setup to use medical record version 2
    And the GP Practice has disabled summary care record functionality
    And the GP Practice has disabled dcr events functionality for TPP
    And I am logged in
    And I am on my record information page - GP Medical Record
    Then I see Service not offered by GP or to specific user or access revoked warning message

  Scenario: A user with partial access to their medical record can see some of their record - GP Medical Record
    Given I am a TPP user setup to use medical record version 2
    And the GP Practice has disabled summary care record functionality
    And the GP Practice has multiple dcr events for TPP
    And I am logged in
    And I am on my record information page - GP Medical Record
    Then I see the Consultations section link on my record - GP Medical Record

  Scenario: A MICROTEST user who does not have medical record access will get a 403 returned - GP Medical Record
    Given the my record wiremocks return a 403 for MICROTEST
    And I am logged in
    And I am on my record information page - GP Medical Record
    Then I see Service not offered by GP or to specific user or access revoked warning message
