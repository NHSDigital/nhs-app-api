@prescription
Feature: View prescriptions Frontend
  A user can view information about their prescriptions after logging in

  # These tests navigate directly to the pages where the features are to be tested, to save time.

  Scenario Outline: <GP System> patient with no past repeat prescriptions
    Given I am a patient using the <GP System> GP System
    And I have 0 past repeat prescriptions
    And each repeat prescription contains 0 courses of which 0 are repeats
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And I click the View Orders link
    Then I see no prescriptions
    And I see a message indicating that I have no repeat prescriptions
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: <GP System> patient who has prescriptions totalling more than one hundred courses
    Given I am a patient using the <GP System> GP System
    And I have 110 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And I click the View Orders link
    Then I see 100 prescriptions
    Examples:
      | GP System |
      | EMIS      |

  @smoketest
  Scenario Outline: <GP System> patient who has multiple prescription each containing one course
    Given I am a patient using the <GP System> GP System
    And I have 3 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And I click the View Orders link
    Then I see 3 prescriptions
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: <GP System> patient who has multiple prescription each containing the same repeat prescription
    Given I am a patient using the <GP System> GP System
    And I have 3 past repeat prescriptions
    And each repeat prescription shares the same course
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And I click the View Orders link
    Then I see 3 prescriptions
    Examples:
      | GP System |
      | TPP       |

  Scenario: The Ordered by label does exist if the prescription was ordered by proxy
    Given I am a patient using the EMIS GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 3 courses of which 2 are repeats
    And the prescription was ordered by proxy user
    And I am logged in
    And I see the home page
    When I navigate to Prescriptions
    And I click the View Orders link
    Then I see the name of the proxy user who ordered the prescription

  Scenario: The Ordered by label does not exist if the prescription was not ordered by proxy
    Given I am a patient using the EMIS GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 3 courses of which 2 are repeats
    And I am logged in
    And I see the home page
    When I navigate to Prescriptions
    And I click the View Orders link
    Then I do not see the name of the proxy user who ordered the prescription

  Scenario Outline: <GP System> patient who has only one prescription containing multiple courses
    Given I am a patient using the <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 3 courses of which 3 are repeats
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And I click the View Orders link
    Then I see 3 prescriptions
    Examples:
      | GP System |
      | EMIS      |

  Scenario: EMIS patient who has acute prescriptions
    Given I am a patient using the EMIS GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 3 courses of which 2 are repeats
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And I click the View Orders link
    Then I see 2 prescriptions

  Scenario: The TPP User clicks on the Prescriptions button and the service is disabled at a GP Practice level
    Given I am a patient using the TPP GP System
    And prescriptions is disabled at a GP Practice level
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And I click 'Order a prescription'
    Then I see a message informing me that I don't currently have access to this service

  Scenario Outline: A <GP System> user with historic prescriptions with missing quantity info
    Given I am a patient using the <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And each course has only dosage info
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And I click the View Orders link
    Then I see 1 prescriptions
    Examples:
      | GP System |
      | EMIS      |

  Scenario Outline: A <GP System> user with historic prescriptions with missing dosage info
    Given I am a patient using the <GP System> GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And each course has only quantity info
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And I click the View Orders link
    Then I see 1 prescriptions
    Examples:
      | GP System |
      | EMIS      |

  Scenario: VISION user with historic prescriptions with missing dosage and quantity info
    Given I am a patient using the VISION GP System
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And each course has no info
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And I click the View Orders link
    Then I see 1 prescriptions

  Scenario: A user who has multiple prescriptions but medication status should not be displayed
    Given I am a patient using the EMIS GP System
    And I have 6 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And courses have status
      | Issued              |
      | Requested           |
      | ForwardedForSigning |
      | Rejected            |
      | Unknown             |
      | Cancelled           |
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And I click the View Orders link
    Then I see 4 prescriptions

  Scenario Outline: An <GP System> user without a GP session is able to recover their session on try again
    Given I am an <GP System> patient whose GP system is unavailable
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And The <GP System> GP system is still unavailable
    And I click the View Orders link
    Then I see appropriate try again shutter screen for prescriptions when there is no GP session
    And The <GP System> GP system becomes available
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And each course has only quantity info
    When I click the 'Try again' button
    Then I see 1 prescriptions
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: A <GP System> user GP session eventually becomes available when viewing orders
    Given I am an <GP System> patient whose GP system is unavailable
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And The <GP System> GP system is still unavailable
    And I click the View Orders link
    Then I see appropriate try again shutter screen for prescriptions when there is no GP session
    When I click the 'Try again' button
    Then I see what I can do next with a repeat prescriptions error message and reference code '3p'
    And I click the session error back link
    And I click the View Orders link
    And I see what I can do next with a repeat prescriptions error message and reference code '3p'
    And I click the session error back link
    And the Prescriptions Hub page is displayed
    And The <GP System> GP system becomes available
    And I have 1 past repeat prescriptions
    And each repeat prescription contains 1 courses of which 1 are repeats
    And each course has only quantity info
    When I click the View Orders link
    Then I see 1 prescriptions
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario: The user can see the Contact your GP screen and follow an external link
    Given the scenario is submit prescription
    And '111' responds to requests for '/emergency-prescription'
    And I am using TPP GP System to submit my prescription
    And I have 1 historic prescriptions in this scenario
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    Then the Prescriptions Hub page is displayed
    And the Prescriptions_Hub page is saved to disk
    When I click the Order a prescription button
    Then the Type of Prescriptions page is displayed
    And the Type_Of_Prescriptions page is saved to disk
    When I select the option to order a non repeat prescription
    Then the Contact Your GP page is displayed
    When I click the link called '111.nhs.uk/emergency-prescription' with a url of 'http://stubs.local.bitraft.io:8080/external/111/emergency-prescription'
    Then a new tab has been opened by the link
