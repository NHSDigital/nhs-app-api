@authentication
@authentication-login
Feature: Login
  Logging into the service is handled via the CitizenID service.
  A user will be shown personalised welcome messages upon successful login.


  @smoketest
  Scenario Outline: A <GP System> user sees the home page after logging in
    Given I have no booked appointments for <GP System>
    And I have no repeat prescriptions for <GP System>
    And I am logged in
    Then I see a welcome message
    And I see the patient details of name, date of birth and NHS number
    And I see the home page header
    And I see the navigation menu
    And I see and can follow links within the home page body
    Examples:
      | GP System |
      | TPP       |
      | VISION    |

  @native-smoketest
  @android
    Examples:
      | GP System |
      | EMIS      |

  Scenario: A EMIS user clicks the home link in the footer from another page
    Given I have upcoming appointments before cutoff time for EMIS
    And I am logged in
    And I am on the Appointment Guidance page
    When I click the home link in the footer
    Then I see the home page

  Scenario: A EMIS user sees the home page after logging in and tries to access the NHS sites footer link
    Given I am logged in as a EMIS user
    When I see the home page
    Then I click the NHS sites link in the footer

  Scenario: A EMIS user sees the home page after logging in and tries to access the about us footer link
    Given I am logged in as a EMIS user
    When I see the home page
    Then I click the about us link in the footer

  Scenario: A EMIS user sees the home page after logging in and tries to access the contact us footer link
    Given I am logged in as a EMIS user
    When I see the home page
    Then I click the contact us link in the footer

  Scenario: A EMIS user sees the home page after logging in and tries to access the site map footer link
    Given I am logged in as a EMIS user
    When I see the home page
    Then I click the site map link in the footer

  Scenario: A EMIS user sees the home page after logging in and tries to access the accessibility footer link
    Given I am logged in as a EMIS user
    When I see the home page
    Then I click the accessibility link in the footer
    
  Scenario: A EMIS user sees the home page after logging in and tries to access the policies footer link
    Given I am logged in as a EMIS user
    When I see the home page
    Then I click the policies link in the footer

  Scenario Outline: A <GP System> user can still log in when the Im1 Connection Token doesn't contain a key
    Given I am logged in as a <GP System> user created before Im1 Cache Keys existed
    Then I see a welcome message
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |


  Scenario Outline: A <GP System> user sees a beta banner on the home page
    Given I am logged in as a <GP System> user
    Then I see the home page
    And I see the beta banner
    Examples:
      | GP System |
      | TPP       |

  @native-smoketest
    Examples:
      | GP System |
      | EMIS      |

  @pending
  @native-smoketest
  Scenario: Any user sees the current app version on the login page
    Given I am at the login page
    And I see the login page
    Then I see the current app version

  Scenario Outline: A <GP System> user can log in, log out, and log in again
    Given I am logged in as a <GP System> user
    Then I see the home page
    When I log out
    Then I see the login page
    When I log in again
    Then I see the home page
    When I log out
    Then I see the login page
    When I log in again
    Then I see the home page
    Examples:
      | GP System |
      | TPP       |

  @native-smoketest
    Examples:
      | GP System |
      | EMIS      |

  Scenario: A EMIS user can log out using the header link
    Given I am logged in as a EMIS user
    Then I see the home page
    When I use the header link to log out of the website
    Then I see the login page
    
  Scenario: A EMIS user can cycle the header links
    Given I am logged in as a EMIS user
    And I have no booked appointments for EMIS
    And I have no repeat prescriptions for EMIS
    When I see the home page
    Then I can cycle through the header links

  Scenario: A user can log in, and when they receive a 401 are redirected to the login page
    Given I am logged in as a EMIS user
    When I am idle long enough for the backend session to expire
    And I navigate to prescriptions
    Then I see the login page

  @manual
  # to enable survey link change HOTJAR_SURVEY_VISIBLE env variable value
  Scenario Outline: A <GP System> user sees a survey link on the home page if enabled
    Given I am logged in as a <GP System> user
    Then I see the home page
    And I see a collapsible link to a survey, which I can follow
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |


  @manual # covered in Manual Regression Test pack
  Scenario Outline: <GPSS> User logs in using CitizenID
    Given I am not logged in as a <GPSS> user
    When I log in
    Then I am redirected to 'http://citizenidaddresshere'
    Examples:
      | GPSS   |
      | EMIS   |
      | TPP    |
      | VISION |

  @native
  @pending
  Scenario: Any user can click the help icon on the login page
    Given I am at the login page
    Given I see the help icon on the login page
    When I click the help icon on the login page
    Then a new tab opens https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/help-and-support/

  Scenario Outline: A <GP System> user that is 13 years old can log in
    Given I attempt to log in as a <GP System> user that is 13
    Then I see the home page
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  @backend
  Scenario: Logging in with a cached Im1 Connection Token for an EMIS user will remove that token from the cache
    Given I have valid EMIS linkage details and it's the first time a linkage key has been created for my nhs number
    And no IM1 Connection Token is currently cached
    And I call the EMIS Linkage POST endpoint
    And I POST to IM1 Connection to register the user
    When I have logged in with the user associated with the IM1 Connection Token
    Then the IM1 Connection Token is no longer in the cache

  @backend
  Scenario: Logging in with a cached Im1 Connection Token for a TPP user will remove that token from the cache
    Given I have valid TPP linkage details for posting
    And no IM1 Connection Token is currently cached
    And I call the TPP Linkage POST endpoint
    And I POST to IM1 Connection to register the user
    When I have logged in with the user associated with the IM1 Connection Token
    Then the IM1 Connection Token is no longer in the cache

  @backend
  Scenario: Logging in as a different EMIS user after an Im1 Connection Token is cached won't remove that token
    Given another EMIS user has a new linkage key created for them
    And no IM1 Connection Token is currently cached
    And they call the EMIS Linkage POST endpoint
    And the IM1 Connection Token is in the cache
    When I have logged into EMIS and have a valid session cookie
    Then the IM1 Connection Token is in the cache

  @backend
  Scenario: Logging in as a different TPP user after an Im1 Connection Token is cached won't remove that token
    Given another user has valid TPP linkage details
    And no IM1 Connection Token is currently cached
    And they call the TPP Linkage POST endpoint
    And the IM1 Connection Token is in the cache
    When I have logged into TPP and have a valid session cookie
    Then the IM1 Connection Token is in the cache
