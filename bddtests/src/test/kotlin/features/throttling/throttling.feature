@throttling
Feature: Throttling

  Background:
    Given I am not logged in and I have not completed the beta throttling flow
    And I see the GP Finder Page

  @pending
  Scenario: A user searches for their practice to find out it is not participating in beta
    Given There are multiple GP Practices for my search criteria
    When I submit my search
    Then I see the GP Search Results Page with 2 search results
    When My GP Practice is not participating in beta
    And I select my GP Practice
    Then I see the Practice Not Participating page

  @pending
  Scenario: A user searches for their practice to find out it is participating in beta and logs in
    Given There are multiple GP Practices for my search criteria
    When I submit my search
    Then I see the GP Search Results Page with 2 search results
    When My GP Practice is participating in beta
    And I select my GP Practice
    Then I see the Practice Participating page
    When I click the Continue button on the Practice Participating page
    Then I see the CID login page

  @pending
  Scenario: A user does not see the Too many results warning if the max results are returned
    Given There are the maximum limit GP Practices for my search criteria
    When I submit my search
    Then I see the GP Search Results Page with 20 search results
    And The Too many results error message is not visible

  @pending
  Scenario: A user selects the wrong GP practice and returns to the GP finder page
    Given There are multiple GP Practices for my search criteria
    When I submit my search
    Then I see the GP Search Results Page with 2 search results
    When My GP Practice is participating in beta
    And I select my GP Practice
    And I click the This is not my GP surgery button
    Then I see the GP Finder Page

  @pending
  Scenario: A user sees an error message if the NHS Service Search is unavailable
    Given The NHS Service Search is unavailable
    When I submit my search
    Then I see the GP Search Results Page with 0 search results
    And The Technical problems error message is visible

  @pending
  Scenario: A user sees an error message if no results are found
    Given There are no GP Practices for my search criteria
    When I submit my search
    Then I see the GP Search Results Page with 0 search results
    And The No results found error message is visible

   @pending
   Scenario: A user sees a message if too many results are found
     Given There are more than the maximum GP Practices for my search criteria
     When I submit my search
     Then I see the GP Search Results Page with 20 search results
     And The Too many results error message is visible

   @pending
   Scenario: A user is given an error message if they search with invalid criteria
     When I submit no search criteria
     Then I see the GP Finder page with a search criteria error message
     When I submit blank search criteria
     Then I see the GP Finder page with a search criteria error message

   @pending
   Scenario: A user skips the throttling flow and sees the login page and cannot navigate directly back to the GP Finder
     When I click the link to skip the throttling flow
     Then I see the login page
     When I browse to the page at /gp-finder
     Then I see the login page

  @pending
  Scenario: A user registers their email with brothermailer
    Given The brothermailer service will return a successful response
    And There are multiple GP Practices for my search criteria
    When I submit my search
    Then I see the GP Search Results Page with 2 search results

    When My GP Practice is not participating in beta
    And I select my GP Practice
    Then I see the Practice Not Participating page

    When I click the Practice Not Participating continue button
    Then I see the Sending Email Page

    And I enter a valid email and submit
    Then I see the Sending Email Results Page

    When I click the back button on the Sending Email Results Page
    Then I can see the login page


  @pending
  Scenario: A user cant registers an invalid email with brothermailer
    Given There are multiple GP Practices for my search criteria
    When I submit my search
    Then I see the GP Search Results Page with 2 search results

    When My GP Practice is not participating in beta
    And I select my GP Practice
    Then I see the Practice Not Participating page

    When I click the Practice Not Participating continue button
    Then I see the Sending Email Page

    When I enter a invalid email and submit
    Then I see the invalid email error


  @pending
  Scenario: A user can get back to Participating page from Sending Email page
    Given The brothermailer service will return a successful response
    And There are multiple GP Practices for my search criteria
    When I submit my search
    Then I see the GP Search Results Page with 2 search results

    When My GP Practice is not participating in beta
    And I select my GP Practice
    Then I see the Practice Not Participating page

    When I click the Practice Not Participating continue button
    Then I see the Sending Email Page

    When I click the back button on Sending Email page
    Then I see the Practice Not Participating page

    When I click the Practice Not Participating continue button
    Then I see the Sending Email Page


  @pending
  Scenario: Brothermailer email registration service is down
    Given The brothermailer service is down
    And There are multiple GP Practices for my search criteria
    When I submit my search
    Then I see the GP Search Results Page with 2 search results

    When My GP Practice is not participating in beta
    And I select my GP Practice
    Then I see the Practice Not Participating page

    When I click the Practice Not Participating continue button
    Then I see the Sending Email Page

    And I enter a valid email and submit
    Then I see the brothermailer service is down error
