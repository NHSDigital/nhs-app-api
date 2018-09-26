Feature: Use Terms and conditions page

  Background:
    Given a patient from EMIS is defined

  @NHSO-2633 @tech-debt
  Scenario: A patient who has not accepted the terms and conditions can log in and see the terms and conditions page
    Given I am logged in and have not accepted the terms and conditions
    Then I am on the Terms and conditions page

  @NHSO-2633 @tech-debt
  Scenario: A patient who has accepted the terms and conditions log ins and does not see the terms and conditions page
    When I am logged in
    Then I see the home page

  @NHSO-2633 @tech-debt
  Scenario: A patient cannot proceed to the app without accepting the terms and conditions
    Given I am logged in and have not accepted the terms and conditions
    And I am on the Terms and conditions page
    When I click the continue button
    Then I see error messages indicating I have not yet accepted the terms and conditions

  @NHSO-2633 @tech-debt
  Scenario: A patient can proceed to the app after accepting the terms and conditions
    Given I am logged in and have not accepted the terms and conditions
    And I am on the Terms and conditions page
    When I check the agree to terms and conditions checkbox
    And I click the continue button
    Then I see the home page

  @NHSO-2633 @tech-debt
  Scenario: A patient can navigate back to the the login page when clicking the back arrow
    Given I am logged in and have not accepted the terms and conditions
    And I am on the Terms and conditions page
    When I click the back arrow
    Then I see the login page

  @NHSO-2633 @tech-debt
  Scenario: A patient can navigate to the Privacy policy page
    Given I am logged in and have not accepted the terms and conditions
    And I am on the Terms and conditions page
    When I click on Privacy policy
    Then a new tab opens https://beta.nhs.uk/

  @NHSO-2633 @tech-debt
  Scenario: A patient can navigate to the Cookies policy page
    Given I am logged in and have not accepted the terms and conditions
    And I am on the Terms and conditions page
    When I click on Cookies policy
    Then a new tab opens https://beta.nhs.uk/

  @NHSO-2633 @tech-debt
  Scenario: A patient can navigate to the Terms of use page
    Given I am logged in and have not accepted the terms and conditions
    And I am on the Terms and conditions page
    When I click on Terms of use
    Then a new tab opens https://beta.nhs.uk/
