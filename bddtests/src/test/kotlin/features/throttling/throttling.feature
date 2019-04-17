@throttling
Feature: Throttling

  Background:
    Given I am not logged in and I have not completed the beta throttling flow
    And I see the GP Finder Page

  Scenario: A user searches for their practice to find out it is not participating in beta
    Given there are 2 GP Practices for my search criteria
    When I enter criteria and submit my search in the GP Practice finder
    Then I see the GP Search Results Page with 2 search results
    When I select a practice which is not participating in beta
    Then I see the Practice Not Participating page

  Scenario: A user is redirected to an external login page after continuing through the participating practice journey
    Given there are 2 GP Practices for my search criteria
    When I enter criteria and submit my search in the GP Practice finder
    Then I see the GP Search Results Page with 2 search results
    When I select a practice which is participating in beta
    Then I see the Practice Participating page
    When I click the 'Continue' button
    Then I see the CID login page

  Scenario Outline: A user searches can find their GP when searching using a valid postcode or outward code
    Given there is a GP Practice with a postcode like <Postcode>
    When I enter criteria and submit my search in the GP Practice finder
    Then I see the GP Search Results Page with 1 search results
    And the GP Practice found matches the searched postcode
    Examples:
    | Postcode |
    | SW9      |
    | SW91NG   |
    | SW9 1NG  |
    | Sw91ng   |

  Scenario: A user does not see the too many results warning if the max results are returned
    Given there are 20 GP Practices for my search criteria
    When I enter criteria and submit my search in the GP Practice finder
    Then I see the GP Search Results Page with 20 search results
    And the Too Many Results message for GP Search is not visible

  Scenario: A user sees a message if too many results are found
    Given there are 21 GP Practices for my search criteria
    When I enter criteria and submit my search in the GP Practice finder
    Then I see the GP Search Results Page with 20 search results
    And the Too Many Results message for GP Search is visible

  Scenario: A user sees a message if no results are found
    Given there are 0 GP Practices for my search criteria
    When I enter criteria and submit my search in the GP Practice finder
    Then the No Results Found page for GP Search is visible

  Scenario: A user is redirected to the GP finder page after selecting 'This is not my surgery' link
    Given there are 2 GP Practices for my search criteria
    When I enter criteria and submit my search in the GP Practice finder
    Then I see the GP Search Results Page with 2 search results
    When I select a practice which is participating in beta
    And I click the 'This is not my GP surgery' link
    Then I see the GP Finder Page

  Scenario: A user sees an error message if the NHS Service Search is unavailable
    Given the NHS Service Search is unavailable
    When I enter criteria and submit my search in the GP Practice finder
    Then I see the GP Search Results Page with no search results
    And the Technical Problems error message for GP Search is visible

  @native @3844
  Scenario: A user sees an error message if they search with invalid criteria
     When I submit no search criteria
     Then I see the GP Finder page with a search criteria error message
     When I submit blank search criteria
     Then I see the GP Finder page with a search criteria error message

  Scenario: A user can skip the throttling flow and be directed to the login page
     When I click the link to skip the throttling flow
     Then I see the login page

  @native @3844
  Scenario: A user who chooses to register their email with brothermailer sees the Waiting list joined page
    Given the brothermailer service will return a successful response
    And there are 2 GP Practices for my search criteria
    When I enter criteria and submit my search in the GP Practice finder
    Then I see the GP Search Results Page with 2 search results
    When I select a practice which is not participating in beta
    Then I see the Practice Not Participating page
    When I click the 'Continue' button
    Then I see the Sending Email Page

    When I choose to sign up to brothermailer
    And I enter a valid email and submit
    Then I see the GP Search Waiting List Joined page
    And I click the 'Go to home screen' button
    And I see the login page for practice not participating

  Scenario: A user who chooses not to register their email with brothermailer sees the Waiting list not joined page
    Given the brothermailer service will return a successful response
    And there are 2 GP Practices for my search criteria
    When I enter criteria and submit my search in the GP Practice finder
    Then I see the GP Search Results Page with 2 search results
    When I select a practice which is not participating in beta
    Then I see the Practice Not Participating page
    When I click the 'Continue' button
    Then I see the Sending Email Page

    When I choose not to sign up to brothermailer
    And I click the 'Continue' button
    Then I see the GP Search Waiting List Not Joined page

  Scenario: A user sees a message if they do not choose any option on the brothermailer signup page
    Given the brothermailer service will return a successful response
    And there are 2 GP Practices for my search criteria
    When I enter criteria and submit my search in the GP Practice finder
    Then I see the GP Search Results Page with 2 search results
    When I select a practice which is not participating in beta
    Then I see the Practice Not Participating page
    When I click the 'Continue' button
    Then I see the Sending Email Page

    When I click the 'Continue' button
    Then I see the make a choice error

  Scenario: A user sees a message if they enter an invalid email with brothermailer
    Given there are 2 GP Practices for my search criteria
    When I enter criteria and submit my search in the GP Practice finder
    Then I see the GP Search Results Page with 2 search results

    When I select a practice which is not participating in beta
    Then I see the Practice Not Participating page

    When I click the 'Continue' button
    Then I see the Sending Email Page

    When I enter an invalid email and submit
    Then I see the invalid email error

  Scenario: A user can get back to Participating page from Sending Email page
    Given the brothermailer service will return a successful response
    And there are 2 GP Practices for my search criteria
    When I enter criteria and submit my search in the GP Practice finder
    Then I see the GP Search Results Page with 2 search results

    When I select a practice which is not participating in beta
    Then I see the Practice Not Participating page

    When I click the 'Continue' button
    Then I see the Sending Email Page

    When I click the back button on Sending Email page
    Then I see the Practice Not Participating page

    When I click the 'Continue' button
    Then I see the Sending Email Page

  Scenario: A users sees an error if Brothermailer email registration service is down
    Given the brothermailer service is down
    And there are 2 GP Practices for my search criteria
    When I enter criteria and submit my search in the GP Practice finder
    Then I see the GP Search Results Page with 2 search results

    When I select a practice which is not participating in beta
    Then I see the Practice Not Participating page

    When I click the 'Continue' button
    Then I see the Sending Email Page

    When I choose to sign up to brothermailer
    And I enter a valid email and submit
    Then I see the brothermailer service is down error
