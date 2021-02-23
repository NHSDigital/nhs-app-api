Feature: View Cookies Page and Manage Cookies Consent

  Scenario: A Patient Navigate to Manage Cookies Page
    Given I am an EMIS patient
    And I am logged in
    And I click the more icon
    And I click the cookies link
    Then I can see the toggle button is set to 'on'

  Scenario: A patient can navigate to the Cookies policy page
    Given I am an EMIS patient
    And I am logged in
    And I click the more icon
    And I click the cookies link
    When I click the link called 'Cookies policy' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/cookies/'
    Then a new tab has been opened by the link

  Scenario: when the analytics cookies is switched off, the third-party cookies are also deleted. A dummy cookie is created and get deleted when the toggle is switched off
    Given I am an EMIS patient
    And I am logged in
    And I click the more icon
    And I click the cookies link
    And I add the dummy cookie
    When I change the cookie consent toggle to 'off'
    Then the dummy cookie is deleted

