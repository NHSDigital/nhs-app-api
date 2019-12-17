Feature: View Cookies Page and Manage Cookies Consent

  Background:
    Given I am an EMIS patient
    And I am logged in
    And I click the settings icon
    And I click the cookies link

  Scenario: A Patient Navigate to Manage Cookies Page
    Then I can see the toggle button to change my current consent

  Scenario: A patient can navigate to the Cookies policy page
    When I click the link called 'Cookies policy' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/cookies/'
    Then a new tab has been opened by the link

  Scenario: when the analytics cookies is switched off, the third-party cookies are also deleted. A dummy cookie is created and get deleted when the toggle is switched off
    And I add the dummy cookie
    When I click the change consent toggle
    Then the dummy cookie is deleted

