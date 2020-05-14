@terms
Feature: Use Terms and conditions page

  Scenario: A patient who has not accepted the terms and conditions can log in and see the terms and conditions page
    Given I am a EMIS patient who has not already accepted terms and conditions
    And I am logged in
    Then the Terms and Conditions page is displayed

  Scenario: A patient who has accepted the terms and conditions log ins and does not see the terms and conditions page
    Given I am a EMIS patient who has already accepted terms and conditions
    And I am logged in
    Then I see the home page

  Scenario: A patient cannot proceed to the app without accepting the terms and conditions
    Given I am a EMIS patient who has not already accepted terms and conditions
    And I am logged in
    Then the Terms and Conditions page is displayed
    When I click the continue button on Terms and Conditions
    Then I see error messages indicating I have not yet accepted the terms and conditions

  Scenario: A patient can proceed to the app after accepting the terms and conditions
    Given I am a EMIS patient who has not already accepted terms and conditions
    And I am logged in
    Then the Terms and Conditions page is displayed
    When I check the agree to terms and conditions checkbox
    And I click the continue button on Terms and Conditions
    Then I see the home page

  Scenario: A patient can proceed to the app after accepting updated terms and conditions
    Given I am a EMIS patient who has accepted terms and conditions but updated terms and conditions exist
    And I am logged in
    Then the updated Terms and Conditions page is displayed
    When I click the continue button on Updated Terms and Conditions
    Then I see error messages indicating I have not yet accepted the updated terms and conditions
    When I agree to the updated terms and conditions
    Then I see the home page

  Scenario: A patient can navigate to the Privacy policy page
    Given I am a EMIS patient who has not already accepted terms and conditions
    And I am logged in
    Then the Terms and Conditions page is displayed
    When I click the link called 'privacy policy' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/privacy/'
    Then a new tab has been opened by the link

  Scenario: A patient can navigate to the Cookies policy page
    Given I am a EMIS patient who has not already accepted terms and conditions
    And I am logged in
    Then the Terms and Conditions page is displayed
    When I click the link called 'cookies policy' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/cookies/'
    Then a new tab has been opened by the link

  Scenario: A patient can navigate to the Terms of use page
    Given I am a EMIS patient who has not already accepted terms and conditions
    And I am logged in
    Then the Terms and Conditions page is displayed
    When I click the link called 'terms of use' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/terms/'
    Then a new tab has been opened by the link

  Scenario: A patient can consent to analytics cookies on the terms and conditions page
    Given I am a EMIS patient who has not already accepted terms and conditions
    And I am logged in
    And the Terms and Conditions page is displayed
    When I check the agree to cookies checkbox
    And I check the agree to terms and conditions checkbox
    And I click the continue button on Terms and Conditions
    And I navigate to the Manage cookies page
    Then I can see the toggle button is set to 'on'

  Scenario: A user with proof level 5 is able to accept terms and conditions
    Given I am a patient with proof level 5 who has not already accepted terms and conditions
    And I am logged in
    Then the Terms and Conditions page is displayed
    When I check the agree to terms and conditions checkbox
    And I click the continue button on Terms and Conditions
    Then I see the home page

  Scenario: A user with proof level 5 that does not accept cookies is able to do so from the Manage cookies page
    Given I am a patient with proof level 5 who has not already accepted terms and conditions
    And I am logged in
    And the Terms and Conditions page is displayed
    When I check the agree to terms and conditions checkbox
    And I click the continue button on Terms and Conditions
    And I navigate to the Manage cookies page
    Then I can see the toggle button is set to 'off'
    When I change the cookie consent toggle to 'on'
    Then I can see the toggle button is set to 'on'

  Scenario: A user with proof level 5 that accepts cookies is able to disable them from the Manage cookies page
    Given I am a patient with proof level 5 who has not already accepted terms and conditions
    And I am logged in
    And the Terms and Conditions page is displayed
    When I check the agree to cookies checkbox
    And I click the continue button on Terms and Conditions
    Then I see error messages indicating I have not yet accepted the terms and conditions
    When I check the agree to terms and conditions checkbox
    And I click the continue button on Terms and Conditions
    And I navigate to the Manage cookies page
    Then I can see the toggle button is set to 'on'
    And I change the cookie consent toggle to 'off'
    And I can see the toggle button is set to 'off'

  Scenario: A user with proof level 5 that has already accepted terms and conditions is asked to do so again
    Given I am a patient with proof level 5 who has updated terms and conditions
    And I am logged in
    Then the updated Terms and Conditions page is displayed
    When I click the continue button on Updated Terms and Conditions
    Then I see error messages indicating I have not yet accepted the updated terms and conditions
    When I agree to the updated terms and conditions
    Then I see the home page

  Scenario: A user with proof level 5 that has accepted terms and conditions is able to view their previous cookie consent decision
    Given I am a patient with proof level 5 who has updated terms and conditions
    And I am logged in
    Then the updated Terms and Conditions page is displayed
    When I agree to the updated terms and conditions
    Then I see the home page
    And I navigate to the Manage cookies page
    And I can see the toggle button is set to 'on'