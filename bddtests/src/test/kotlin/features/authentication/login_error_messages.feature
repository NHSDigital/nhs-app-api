@authentication
@authentication-login
Feature: Login error messages

  The user is shown appropriate error messages if something goes wrong during login
#400
  @nativesmoketest
  Scenario: CitizenID provides invalid data after successful login
    Given I am logged into Citizen ID but am receiving invalid data
    And 'NHS UK' responds to requests for '/nhs-app-contact-us'
    Then I see an error 'Contact us if you keep seeing this message, quoting error code' link with a url of 'http://stubs.local.bitraft.io:8080/external/nhsuk/nhs-app-contact-us?errorcode=3a' and error prefix of '3a'
    When I click the error 'Back to home' link
    Then I see the login page

  #465
  Scenario Outline: Cannot log in as a <GP System> user with no Date of Birth
    Given I attempt to log in as a <GP System> user without a date of birth
    And '111' responds to requests for '/home'
    Then I see an error message informing me I cannot log in as I am under the minimum age
    When I click the error 'Go to 111.nhs.uk' link with a url of 'http://stubs.local.bitraft.io:8080/external/111/home'
    Then a new tab has been opened by the link
    Examples:
      | GP System |
      | TPP       |

    @nativesmoketest
    Examples:
      | GP System  |
      | EMIS       |

    #465
  Scenario: Cannot log in as a TPP user with an age under 13
    Given I attempt to log in as a TPP user with an age under 13
    And '111' responds to requests for '/home'
    Then I see an error message informing me I cannot log in as I am under the minimum age
    When I click the error 'Go to 111.nhs.uk' link with a url of 'http://stubs.local.bitraft.io:8080/external/111/home'
    Then a new tab has been opened by the link

    #464
  Scenario Outline: Cannot log in as a <GP System> user with no NHS Number
    Given I attempt to log in as a <GP System> user without an NHS Number
    And 'NHS UK' responds to requests for '/nhs-app-contact-us'
    And '111' responds to requests for '/myhealthonline'
    And '111' responds to requests for '/wales'
    And '111' responds to requests for '/home'
    Then The reference error and label and prefix is shown as "Reference: 3f"
    When I click the error <Link text> link with a url of <Link url>
    Then a new tab has been opened by the link
    Examples:
      | GP System | Link text           | Link url                                                                            |
      | TPP       | 'contact us'        | 'http://stubs.local.bitraft.io:8080/external/nhsuk/nhs-app-contact-us?errorcode=3f' |
      | TPP       | 'My Health Online'  | 'http://stubs.local.bitraft.io:8080/external/111/myhealthonline'                    |
      | TPP       | '111.wales.nhs.uk'  | 'http://stubs.local.bitraft.io:8080/external/111/wales'                             |
      | TPP       | '111.nhs.uk'        | 'http://stubs.local.bitraft.io:8080/external/111/home'                              |

    @nativesmoketest
    Examples:
      | GP System  | Link text           | Link url                                                                           |
      | EMIS       | 'contact us'        | 'http://stubs.local.bitraft.io:8080/external/nhsuk/nhs-app-contact-us?errorcode=3f'|
      | EMIS       | 'My Health Online'  | 'http://stubs.local.bitraft.io:8080/external/111/myhealthonline'                   |
      | EMIS       | '111.wales.nhs.uk'  | 'http://stubs.local.bitraft.io:8080/external/111/wales'                            |
      | EMIS       | '111.nhs.uk'        | 'http://stubs.local.bitraft.io:8080/external/111/home'                                  |


  #464
  @nativesmoketest
  Scenario Outline: Cannot log in as a EMIS user with invalid ODS Code
    Given I attempt to log in as a EMIS user with invalid ODS Code
    And 'NHS UK' responds to requests for '/nhs-app-contact-us'
    And '111' responds to requests for '/myhealthonline'
    And '111' responds to requests for '/wales'
    And '111' responds to requests for '/home'
    Then The reference error and label and prefix is shown as "Reference: 3f"
    When I click the error <Link text> link with a url of <Link url>
    Then a new tab has been opened by the link
    Examples:
      | Link text           | Link url                                                                            |
      | 'contact us'        | 'http://stubs.local.bitraft.io:8080/external/nhsuk/nhs-app-contact-us?errorcode=3f' |
      | 'My Health Online'  | 'http://stubs.local.bitraft.io:8080/external/111/myhealthonline'                    |
      | '111.wales.nhs.uk'  | 'http://stubs.local.bitraft.io:8080/external/111/wales'                             |
      | '111.nhs.uk'        | 'http://stubs.local.bitraft.io:8080/external/111/home'                                  |

#504 timeout
  Scenario: Cannot log in as a EMIS when the request timeout I see error code with "zn" prefix
    Given I attempt to log in as an EMIS and the CID request timeout
    And 'NHS UK' responds to requests for '/nhs-app-contact-us'
    Then I click an error 'Contact us if you keep seeing this message, quoting error code' link with a url of 'http://stubs.local.bitraft.io:8080/external/nhsuk/nhs-app-contact-us?errorcode=zn' and error prefix of 'zn'
    And a new tab has been opened by the link
