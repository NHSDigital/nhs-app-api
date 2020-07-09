@my-record
Feature: User Not Enabled Frontend - Medical Record v1

  Scenario: A user without access to their medical record receives an error message - Medical Record v1
    Given I am a TPP user setup to use medical record version 1
    And the GP Practice has disabled summary care record functionality
    And the GP Practice has disabled dcr events functionality for TPP
    And I am logged in
    When I retrieve the 'my record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    Then I see Service not offered by GP or to specific user or access revoked warning message

  Scenario: A user with partial access to their medical record can see some of their record - Medical Record v1
    Given I am a TPP user setup to use medical record version 1
    And the GP Practice has disabled summary care record functionality
    And the GP Practice has multiple dcr events for TPP
    And I am logged in
    When I retrieve the 'my record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    Then I see the Consultations heading on My Record - Medical Record v1

  Scenario: A MICROTEST user who does not have medical record access will get a 403 returned - Medical Record v1
    Given I am a MICROTEST user setup to use medical record version 1
    And the my record wiremocks return a 403
    And I am logged in
    When I retrieve the 'my record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    Then I see Service not offered by GP or to specific user or access revoked warning message
