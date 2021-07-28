@legalAndCookiesHub
Feature: Legal and Cookies Hub

  Scenario: A patient can navigate to the Legal and cookies page
    Given I am a EMIS patient
    And I am logged in
    When I navigate to the Legal and cookies page
    Then the Legal and cookies Hub page is displayed

  Scenario: A patient can navigate to the Manage cookies page
    Given I am an EMIS patient
    And I am logged in
    When I navigate to the Legal and cookies page
    Then the Legal and cookies Hub page is displayed
    When I click Manage Cookies
    Then the Legal and cookies Manage cookies page is displayed

  Scenario: A patient can navigate to the Cookies policy page
    Given I am an EMIS patient
    And I am logged in
    When I navigate to the Legal and cookies page
    Then the Legal and cookies Hub page is displayed
    When I click Manage Cookies
    Then the Legal and cookies Manage cookies page is displayed
    When I click the link called 'Cookies policy' with a url of 'https://www.nhs.uk/nhs-app/nhs-app-legal-and-cookies/nhs-app-cookies-policy/'
    Then a new tab has been opened by the link

  Scenario: A patient can navigate to the Terms of use page
    Given I am an EMIS patient
    And I am logged in
    When I navigate to the Legal and cookies page
    Then the Legal and cookies Hub page is displayed
    When I click the link called 'Terms of use' with a url of 'https://www.nhs.uk/nhs-app/nhs-app-legal-and-cookies/nhs-app-terms-of-use/'
    Then a new tab has been opened by the link

  Scenario: A patient can navigate to the Privacy policy page
    Given I am an EMIS patient
    And I am logged in
    When I navigate to the Legal and cookies page
    Then the Legal and cookies Hub page is displayed
    When I click the link called 'Privacy policy' with a url of 'https://www.nhs.uk/nhs-app/nhs-app-legal-and-cookies/nhs-app-privacy-policy/privacy-policy/'
    Then a new tab has been opened by the link

  Scenario: A patient can navigate to the Accessibility statement page
    Given I am an EMIS patient
    And I am logged in
    When I navigate to the Legal and cookies page
    Then the Legal and cookies Hub page is displayed
    When I click the link called 'Accessibility statement' with a url of 'https://www.nhs.uk/nhs-app/nhs-app-legal-and-cookies/nhs-app-accessibility-statement/'
    Then a new tab has been opened by the link

  Scenario: A patient can navigate to the Open source licences page
    Given I am an EMIS patient
    And I am logged in
    When I navigate to the Legal and cookies page
    Then the Legal and cookies Hub page is displayed
    When I click the link called 'Open source licences' with a url of 'https://www.nhs.uk/nhs-app/nhs-app-legal-and-cookies/nhs-app-open-source-licences/'
    Then a new tab has been opened by the link

  Scenario: Originating from the legal and cookies hub, a patient can navigate to the Manage cookies page
    Given I am an EMIS patient
    And I am logged in
    And I navigate to the Legal and cookies page
    When I click Manage Cookies
    Then the Legal and cookies Manage cookies page is displayed
    And I can see the toggle button is set to 'on'

  Scenario: Originating from the legal and cookies hub, a patient can navigate to the Manage cookies page and then onto the Cookies policy page
    Given I am an EMIS patient
    And I am logged in
    And I navigate to the Legal and cookies page
    When I click Manage Cookies
    Then the Legal and cookies Manage cookies page is displayed
    When I click the link called 'Cookies policy' with a url of 'https://www.nhs.uk/nhs-app/nhs-app-legal-and-cookies/nhs-app-cookies-policy/'
    Then a new tab has been opened by the link

  Scenario: Originating from the legal and cookies hub, a patient can navigate to the Manage cookies page, and when the analytics cookies is switched off, the third-party cookies are also deleted. A dummy cookie is created and get deleted when the toggle is switched off
    Given I am an EMIS patient
    And I am logged in
    And I navigate to the Legal and cookies page
    When I click Manage Cookies
    Then the Legal and cookies Manage cookies page is displayed
    When I add the dummy cookie
    And I change the cookie consent toggle to 'off'
    Then the dummy cookie is deleted
