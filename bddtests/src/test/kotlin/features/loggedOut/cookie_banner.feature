@logged-out
Feature: Cookie Banner

  Cookie Banner displayed when logged-out, until the user has explicitly dismissed it

  Scenario Outline: Cookie banner appears on the "non logged in" login page, on the web with Javascript <js enabled?>, if not yet acknowledged
    Given I have <js enabled?> javascript
    When I am on the login logged-out page
    Then I see the cookie banner
    When I click the link called 'cookies policy' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/cookies-policy#manage'
    Then a new tab has been opened by the link
    Examples:
      | js enabled? |
      | enabled     |
      | disabled    |

  @native
  @nativesmoketest
  Scenario: Cookie banner doesn't appear on the "non logged in" login page, on the native app
    Given I am using the native app user agent
    And I have the instructions cookie
    When I am on the login logged-out page
    Then I do not see the cookie banner

  Scenario: When the Cookie Banner is acknowledged on the login web page with Javascript enabled, the banner no longer appears with a refresh but cookies will reappear when swapping page
    Given I have enabled javascript
    And I am on the login logged-out page
    When I close the cookie banner
    And I refresh the page
    Then I do not see the cookie banner
    And pages will display the cookie banner
      | /login               |

  Scenario Outline: Cookie Banner doesn't appear when logged in on web, when Javascript <js enabled?>
    Given I have <js enabled?> javascript
    And I am a EMIS patient
    And I am about to directly access every page
    And I am logged in
    And I see the home page
    Then pages will not display the cookie banner
      | /account                                    |
      | /appointments                               |
      | /appointments/booking-guidance              |
      | /appointments/cancelling                    |
      | /appointments/booking                       |
      | /appointments/confirmation                  |
      | /data-sharing                               |
      | /                                           |
      | /more                                       |
      | /my-record                                  |
      | /my-record/noaccess                         |
      | /prescriptions                              |
      | /prescriptions/repeat-courses               |
      | /prescriptions/confirm-prescription-details |
      | /symptoms                                   |
      | /terms-and-conditions                       |
    Examples:
      | js enabled? |
      | enabled     |
      | disabled    |

  Scenario Outline: Unacknowledged Cookie Banner reappears when logged out of the web, when Javascript <js enabled?>
    Given I have <js enabled?> javascript
    And I am a EMIS patient
    And I am logged in
    And I see the home page
    When I sign out
    Then I see the cookie banner
    Examples:
      | js enabled? |
      | disabled    |
      | enabled     |

  Scenario: Acknowledged Cookie Banner reappears when browser closed and repoened, when Javascript enabled
    Given I have enabled javascript
    And I am a EMIS patient
    And I am on the login logged-out page
    And session storage is cleared
    And I close the cookie banner
    And I reopen the app
    Then I see the cookie banner
