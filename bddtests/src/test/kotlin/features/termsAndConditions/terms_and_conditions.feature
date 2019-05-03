Feature: Use Terms and conditions page

  @cosmos
  Scenario: A patient who has not accepted the terms and conditions can log in and see the terms and conditions page
    Given I am a EMIS patient
    And I have not already accepted terms and conditions
    And I am logged in
    Then I am on the Terms and conditions page

  @cosmos
  Scenario: A patient who has accepted the terms and conditions log ins and does not see the terms and conditions page
    Given I am a EMIS patient
    And I have already accepted terms and conditions
    And I am logged in
    Then I see the home page

  @cosmos
  Scenario: A patient cannot proceed to the app without accepting the terms and conditions
    Given I am a EMIS patient
    And I have not already accepted terms and conditions
    And I am logged in
    And I am on the Terms and conditions page
    When I click the continue button on Terms and Conditions
    Then I see error messages indicating I have not yet accepted the terms and conditions

  @cosmos
  Scenario: A patient can proceed to the app after accepting the terms and conditions
    Given I am a EMIS patient
    And I have not already accepted terms and conditions
    And I am logged in
    And I am on the Terms and conditions page
    When I check the agree to terms and conditions checkbox
    And I click the continue button on Terms and Conditions
    Then I see the home page

  @cosmos
  Scenario: A patient can proceed to the app after accepting updated terms and conditions
    Given I am a EMIS patient
    And I have previously accepted terms and conditions and updated terms and conditions exist
    And I am logged in
    And I see the updated Terms and conditions page

    When I click the continue button on Updated Terms and Conditions
    Then I see error messages indicating I have not yet accepted the updated terms and conditions

    When I agree to the updated terms and conditions
    Then I see the home page

  @cosmos
  Scenario: A patient can navigate back to the the login page when clicking the back arrow
    Given I am a EMIS patient
    And I have not already accepted terms and conditions
    And I am logged in
    When I am on the Terms and conditions page
    And I click the back arrow
    Then I see the login page

  @cosmos
  Scenario: A patient can navigate to the Privacy policy page
    Given I am a EMIS patient
    And I have not already accepted terms and conditions
    And I am logged in
    And I am on the Terms and conditions page
    When I click the link called 'privacy policy' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy-policy/'
    Then a new tab has been opened by the link

  @cosmos
  Scenario: A patient can navigate to the Cookies policy page
    Given I am a EMIS patient
    And I have not already accepted terms and conditions
    And I am logged in
    And I am on the Terms and conditions page
    When I click the link called 'cookies policy' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/cookies-policy/'
    Then a new tab has been opened by the link

  @cosmos
  Scenario: A patient can navigate to the Terms of use page
    Given I am a EMIS patient
    And I have not already accepted terms and conditions
    And I am logged in
    And I am on the Terms and conditions page
    When I click the link called 'terms of use' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/terms-of-use/'
    Then a new tab has been opened by the link
