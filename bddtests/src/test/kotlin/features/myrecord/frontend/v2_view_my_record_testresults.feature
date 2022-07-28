@my-record
@test-results
Feature: Test Results Frontend - Medical Record v2

  Scenario: A VISION user has no access to test result section - Medical Record v2
    Given I am a VISION user setup to use medical record version 2
    And I do not have access to test results
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Test results link on my record - Medical Record v2
    Then I see a message indicating that I have no access to view this section on My Record - Medical Record v2

  Scenario: A VISION user has no test results - Medical Record v2
    Given I am a VISION user setup to use medical record version 2
    And I have no test results
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Test results link on my record - Medical Record v2
    Then I see a message that I have no information available for a specific record - Medical Record v2

  Scenario: An EMIS user has one test result with one value - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has a single test result with single child values with no ranges for EMIS
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I see the medical records with count of 1 for Test results
    And I click the Test results link on my record - Medical Record v2
    Then I see one test result with one value - Medical Record v2

  Scenario: An EMIS user receiving a corrupt response for test results sees an error - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And there is a corrupted test results response returned
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I see the medical records without any count for Test results
    And I click the Test results link on my record - Medical Record v2
    Then I see an error occurred message on My Record - Medical Record v2

  Scenario: An EMIS user has one test result with one value and a range - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has a single test result with single child value with A range for EMIS
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Test results link on my record - Medical Record v2
    Then I see one test result with one value and a range - Medical Record v2

  Scenario: An EMIS user has one test result with multiple child values - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has a single test result with multiple child values with no ranges for EMIS
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Test results link on my record - Medical Record v2
    Then I see one test result with multiple child values - Medical Record v2

  Scenario: An EMIS user has test results with multiple child values which have ranges - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And the GP Practice has a single test result with multiple child values with ranges for EMIS
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Test results link on my record - Medical Record v2
    Then I see test results with multiple child values some of which have ranges - Medical Record v2
    And I see a message that no further information is available for this section in GP Medical Record

  Scenario: An EMIS user has a test result with an unknown date - Medical Record v2
    Given I am a EMIS user setup to use medical record version 2
    And the EMIS GP Practice has three test results where the second record has no date
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Test results link on my record - Medical Record v2
    Then I see 3 test results - Medical Record v2
    And The third test result record has an unknown date - Medical Record v2

  Scenario: A TPP user has multiple test results - Medical Record v2
    Given I am a TPP user setup to use medical record version 2
    And the GP Practice has 6 test results
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Test results link on my record - Medical Record v2
    Then I see 6 test results - Medical Record v2
    And I see a message that no further information is available for this section in GP Medical Record

  Scenario: A TPP user will see a error screen when viewing an invalid individual test result - Medical Record v2
    Given I am a TPP user setup to use medical record version 2
    And '111' responds to requests for '/home'
    And the GP Practice has 6 test results
    And an error occurs retrieving the test result detail
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Test results link on my record - Medical Record v2
    And I click a test result - Medical Record v2
    Then I see the appropriate error message for retrieving test result detail
    When I click the error '111.nhs.uk' link with a url of 'http://stubs.local.bitraft.io:8080/external/111/home'
    Then a new tab has been opened by the link

  Scenario: A TPP user can navigate to an individual test result - Medical Record v2
    Given I am a TPP user setup to use medical record version 2
    And the GP Practice has 6 test results
    And the GP Practice has test result details
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Test results link on my record - Medical Record v2
    And I click a test result - Medical Record v2
    And I see the test results content - Medical Record v2

  Scenario: A TPP user can navigate to a test result containing HTML entities - Medical Record v2
    Given I am a TPP user setup to use medical record version 2
    And the GP Practice has 6 test results
    And the GP Practice has test result details with HTML entities
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Test results link on my record - Medical Record v2
    And I click a test result - Medical Record v2
    And there are no wrongly displayed HTML entities - Medical Record v2

  Scenario Outline: A TPP user can navigate to the choose test results page displaying the previous 5 years
    Given I am a TPP user setup to use medical record version 2
    And the GP Practice has <Test Results> test results
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Test results link on my record - Medical Record v2
    Then I see <Test Results> test results - Medical Record v2
    And I click the menu item 'View all test results'
    And I see the previous 5 years
    Examples:
      | Test Results |
      | 6            |

  Scenario Outline: A TPP user can navigate to next and previous choose test results pages
    Given I am a TPP user setup to use medical record version 2
    And the GP Practice has <Test Results> test results
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Test results link on my record - Medical Record v2
    Then I see <Test Results> test results - Medical Record v2
    And I click the menu item 'View all test results'
    And I see the previous 5 years
    And I can see the next pagination link
    When I click the next pagination link
    Then I see further previous test results
    And I can see the previous pagination link
    And I click the previous pagination link
    And I see the previous 5 years
    Examples:
      | Test Results |
      | 6            |

  Scenario Outline: A TPP user cannot navigate beyond their birth year or previous year on the choose test results page
    Given I am a TPP user setup to use medical record version 2
    And the GP Practice has <Test Results> test results
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Test results link on my record - Medical Record v2
    Then I see <Test Results> test results - Medical Record v2
    And I click the menu item 'View all test results'
    And I see the previous 5 years
    And I cannot see the previous pagination link
    And I retrieve the last choose test results page
    And I cannot see the next pagination link
    Examples:
      | Test Results |
      | 6            |

  Scenario: A TPP user cannot navigate beyond their birth year or previous year on the choose test results page while proxying
    Given I am a TPP user with linked profiles
    And I am logged in
    And I have switched to a linked profile
    And the GP Practice has enabled all medical records for the proxy patient
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Test results link on my record - Medical Record v2
    Then I see 6 test results - Medical Record v2
    And I click the menu item 'View all test results'
    And I see the previous 5 years
    And I cannot see the previous pagination link
    And I retrieve the last choose test results page for a linked account user
    And I cannot see the next pagination link

  Scenario Outline: A TPP user can navigate to the specific test results page
    Given I am a TPP user setup to use medical record version 2
    And the GP Practice has <Test Results> test results
    And the GP practice has <Test Results> historic test results for the previous 5 years
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Test results link on my record - Medical Record v2
    Then I see <Test Results> test results - Medical Record v2
    And I click the menu item 'View all test results'
    And I see the previous 5 years
    And I click the previous years test results
    And I see <Test Results> test results - Medical Record v2
    Examples:
      | Test Results |
      | 6            |

  Scenario Outline: A TPP user can navigate to next and previous on the specific test results page
    Given I am a TPP user setup to use medical record version 2
    And the GP Practice has <Test Results> test results
    And the GP practice has <Test Results> historic test results for the previous 5 years
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Test results link on my record - Medical Record v2
    Then I see <Test Results> test results - Medical Record v2
    And I click the menu item 'View all test results'
    And I see the previous 5 years
    And I click the previous years test results
    And I can see the next pagination link
    When I click the next pagination link
    Then I see <Test Results> test results - Medical Record v2
    And I can see the previous pagination link
    When I click the previous pagination link
    Then I see <Test Results> test results - Medical Record v2
    Examples:
      | Test Results |
      | 6            |

  Scenario Outline: A TPP user cannot navigate beyond their birth year or previous year on the specific test results page
    Given I am a TPP user setup to use medical record version 2
    And the GP Practice has <Test Results> test results
    And the GP practice has <Test Results> historic test results for the previous 5 years
    And the GP practice has <Test Results> historic test results for patients birth year
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Test results link on my record - Medical Record v2
    Then I see <Test Results> test results - Medical Record v2
    And I click the menu item 'View all test results'
    And I see the previous 5 years
    And I click the previous years test results
    When I click the previous pagination link
    Then I cannot see the previous pagination link
    When I retrieve the last specific test results page
    Then I cannot see the next pagination link
    Examples:
      | Test Results |
      | 6            |

  Scenario Outline: A TPP user cannot navigate beyond their birth year or previous year on the specific test results page while proxying
    Given I am a TPP user with linked profiles
    And I am logged in
    And I have switched to a linked profile
    And the GP Practice has <Test Results> test results
    And the GP practice has <Test Results> historic test results for the previous 5 years
    And the GP practice has <Test Results> historic test results for linked account birth year
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Test results link on my record - Medical Record v2
    Then I see <Test Results> test results - Medical Record v2
    And I click the menu item 'View all test results'
    And I see the previous 5 years
    And I click the previous years test results
    When I click the previous pagination link
    Then I cannot see the previous pagination link
    When I retrieve the last specific test results page for a linked account user
    Then I cannot see the next pagination link
    Examples:
      | Test Results |
      | 6            |

  Scenario Outline: A TPP user can navigate to the specific test results page and use breadcrumbs to go back in the journey
    Given I am a TPP user setup to use medical record version 2
    And the GP Practice has test result details
    And the GP Practice has <Test Results> test results
    And the GP practice has <Test Results> historic test results for the previous 5 years
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Test results link on my record - Medical Record v2
    And I click the menu item 'View all test results'
    Then I click the previous years test results
    And I see <Test Results> test results - Medical Record v2
    When I click the first test result
    And I click the 'Test results' breadcrumb
    Then I see the specific test results page for '2021'
    And I click the 'Test results' breadcrumb
    And I see the previous 5 years
    And I click the 'Test results' breadcrumb
    And I see <Test Results> test results - Medical Record v2
    And I click the 'Your GP health record' breadcrumb
    And I see the 'GP health record' page
    Examples:
      | Test Results |
      | 6            |

  Scenario Outline: A TPP user can navigate to the test results V2 page and use breadcrumbs to go back in the journey
    Given I am a TPP user setup to use medical record version 2
    And the GP Practice has test result details
    And the GP Practice has <Test Results> test results
    And I am logged in
    When I retrieve the 'gp medical record' page directly
    Then the Medical Record Warning Page is displayed
    When I click the 'Continue' button
    And I click the Test results link on my record - Medical Record v2
    And I click the first test result
    And I click the 'Test results' breadcrumb
    And I see <Test Results> test results - Medical Record v2
    And I click the 'Your GP health record' breadcrumb
    And I see the 'GP health record' page
    Examples:
      | Test Results |
      | 6            |

