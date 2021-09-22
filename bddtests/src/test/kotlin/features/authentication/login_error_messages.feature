@authentication
@authentication-login
Feature: Login error messages

  The user is shown appropriate error messages if something goes wrong during login
  #400
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
      | EMIS      |
      | TPP       |

    #465
  Scenario: Cannot log in as a TPP user with an age under 13
    Given I attempt to log in as a TPP user with an age under 13
    And '111' responds to requests for '/home'
    Then I see an error message informing me I cannot log in as I am under the minimum age
    When I click the error 'Go to 111.nhs.uk' link with a url of 'http://stubs.local.bitraft.io:8080/external/111/home'
    Then a new tab has been opened by the link

  #464
  Scenario Outline: Cannot log in as a EMIS user with invalid ODS Code
    Given I attempt to log in as a EMIS user with invalid ODS Code
    And 'NHS UK' responds to requests for '/nhs-app-contact-us'
    And 'My Health Online' responds to requests for type '/myhealthonline'
    And '111' responds to requests for '/wales'
    And 'COVID Pass or proof' responds to requests for type '/conditions/coronavirus-covid-19/covid-pass'
    And 'Northern Ireland' responds to requests for type '/www-nidirect-gov-uk/articles/gp-out-hours-service'
    And I see the error 'Contact us' link with a url of 'http://stubs.local.bitraft.io:8080/external/nhsuk/nhs-app-contact-us?errorcode=3f'
    When I click the error <Link text> link with a url of <Link url>
    Then a new tab has been opened by the link
    Examples:
      | Link text             | Link url                                                                                        |
      | 'Contact us'          | 'http://stubs.local.bitraft.io:8080/external/nhsuk/nhs-app-contact-us?errorcode=3f'             |
      | 'My Health Online'    | 'http://stubs.local.bitraft.io:8080/external/111/myhealthonline'                                |
      | '111.wales.uk'        | 'http://stubs.local.bitraft.io:8080/external/111/wales'                                         |
      | 'COVID Pass or proof' | 'http://stubs.local.bitraft.io:8080/external/conditions/coronavirus-covid-19/covid-pass'        |
      | 'Northern Ireland'    | 'http://stubs.local.bitraft.io:8080/external/www-nidirect-gov-uk/articles/gp-out-hours-service' |

  #468
  Scenario Outline: Cannot log in as a EMIS user without ODS Code
    Given I attempt to log in as a EMIS user without an ODS Code
    And 'NHS UK' responds to requests for '/nhs-app-contact-us'
    And '111' responds to requests for '/home'
    And 'NHS COVID Pass' responds to requests for type '/covid-status-service-nhsx-nhs-uk'
    And I see the error 'Contact us' link with a url of 'http://stubs.local.bitraft.io:8080/external/nhsuk/nhs-app-contact-us?errorcode=3r'
    When I click the error <Link text> link with a url of <Link url>
    Then a new tab has been opened by the link
    Examples:
      | Link text               | Link url                                                                            |
      | 'Contact us'            | 'http://stubs.local.bitraft.io:8080/external/nhsuk/nhs-app-contact-us?errorcode=3r' |
      | '111.nhs.uk'            | 'http://stubs.local.bitraft.io:8080/external/111/home'                              |
      | 'NHS COVID Pass'        | 'http://stubs.local.bitraft.io:8080/external/covid-status-service-nhsx-nhs-uk'      |

  #469
  Scenario Outline: Cannot log in as a EMIS user with no NHS Number
    Given I attempt to log in as a EMIS user without an NHS Number
    And 'NHS UK' responds to requests for '/nhs-app-contact-us'
    And '111' responds to requests for '/home'
    And I see the error 'Contact us' link with a url of 'http://stubs.local.bitraft.io:8080/external/nhsuk/nhs-app-contact-us?errorcode=3u'
    When I click the error <Link text> link with a url of <Link url>
    Then a new tab has been opened by the link
    Examples:
      | Link text           | Link url                                                                            |
      | 'Contact us'        | 'http://stubs.local.bitraft.io:8080/external/nhsuk/nhs-app-contact-us?errorcode=3u' |
      | '111.nhs.uk'        | 'http://stubs.local.bitraft.io:8080/external/111/home'                              |

  #504 timeout
  Scenario: Cannot log in as a EMIS when the request timeout I see error code with "zn" prefix
    Given I attempt to log in as an EMIS and the CID request timeout
    And 'NHS UK' responds to requests for '/nhs-app-contact-us'
    Then I click an error 'Contact us if you keep seeing this message, quoting error code' link with a url of 'http://stubs.local.bitraft.io:8080/external/nhsuk/nhs-app-contact-us?errorcode=zn' and error prefix of 'zn'
    And a new tab has been opened by the link
