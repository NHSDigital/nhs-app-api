@logged-out
Feature: Cookie Banner

  Cookie Banner displayed when logged-out, until the user has explicitly dismissed it

  Scenario Outline: Cookie banner appears on the "non logged in" <target page> page, on the web with Javascript <js enabled?>, if not yet acknowledged
    Given I have <js enabled?> javascript
    When I am on the <target page> logged-out page
    Then I see the cookie banner
    And no cookie is created that would hide this banner
    When I click the link called 'Find out more about cookies' with a url of 'https://www.nhs.uk/using-the-nhs/nhs-services/the-nhs-app/cookies-policy#manage'
    Then a new tab has been opened by the link
    Examples:
      | target page         | js enabled? |
      | login               | enabled     |
      | check your symptoms | enabled     |
      | login               | disabled    |
      | check your symptoms | disabled    |
      | gp finder           | enabled     |
      | gp finder           | disabled    |

  @native
  @nativesmoketest
  Scenario Outline: Cookie banner doesn't appear on the "non logged in" <target page> page, on the native app
    When I am on the <target page> logged-out page
    Then I do not see the cookie banner
    Examples:
      | target page         |
      | login               |
      | check your symptoms |
      | gp finder           |
    
  Scenario Outline: When the Cookie Banner is acknowledged on the <target page> web page with Javascript <js enabled?>, the cookie is created and the banner no longer appears
    Given I have <js enabled?> javascript
    And I am on the <target page> logged-out page
    When I close the cookie banner
    Then a local cookie is created with expiry date
    And pages will not display the cookie banner
      | /login               |
      | /gp-finder           |
      | /check-your-symptoms |
    Examples:
      | target page         | js enabled? |
      | login               | enabled     |
      | check your symptoms | enabled     |
      | login               | disabled    |
      | check your symptoms | disabled    |
      | gp finder           | enabled     |
      | gp finder           | disabled    |

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
      | enabled     |
      | disabled    |

  Scenario Outline: Acknowledged Cookie Banner does not appear when logged out of the web, when Javascript <js enabled?>
    Given I have <js enabled?> javascript
    And I am a EMIS patient
    And I am on the login logged-out page
    And I close the cookie banner
    And I am logged in
    And I see the home page
    When I sign out
    Then I do not see the cookie banner
    Examples:
      | js enabled? |
      | enabled     |
      | disabled    |