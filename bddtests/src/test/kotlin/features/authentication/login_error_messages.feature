@authentication
@authentication-login
Feature: Login error messages

  The user is shown appropriate error messages if something goes wrong during login

  #400
  Scenario: CitizenID provides invalid data after successful login
    Given I am logged into Citizen ID but am receiving invalid data
    And 'NHS UK' responds to requests for '/nhs-app-contact-us'
    Then I see a shutter 'Contact us if you keep seeing this message, quoting error code' link with a url of 'http://stubs.local.bitraft.io:8080/external/nhsuk/nhs-app-contact-us?errorcode=3a' and error prefix of '3a'
    When I click the shutter 'Back to login' link
    Then I see the login page

  #465
  Scenario Outline: Cannot log in as a <GP System> user with no Date of Birth
    Given I attempt to log in as a <GP System> user without a date of birth
    And 'NHS COVID Pass' responds to requests for type '/'
    And 'COVID Pass or proof' responds to requests for type '/get-your-covid-pass-letter'
    And '111' responds to requests for '/home'
    Then I see a message informing me I cannot log in as I am under the minimum age
    When I click the link called <Link text> with a url of <Link url>
    Then a new tab has been opened by the link
    Examples:
      | GP System | Link text                                            | Link url                                                                                                            |
      | EMIS      | 'Get your digital NHS COVID Pass'                    | 'http://stubs.local.bitraft.io:8080/external/covid-status-service-nhsx-nhs-uk'                                      |
      | TPP       | 'Get your digital NHS COVID Pass'                    | 'http://stubs.local.bitraft.io:8080/external/covid-status-service-nhsx-nhs-uk'                                      |
      | EMIS      | 'Get your NHS COVID Pass letter sent to you by post' | 'http://stubs.local.bitraft.io:8080/external/conditions/coronavirus-covid-19/covid-pass/get-your-covid-pass-letter' |
      | TPP       | 'Get your NHS COVID Pass letter sent to you by post' | 'http://stubs.local.bitraft.io:8080/external/conditions/coronavirus-covid-19/covid-pass/get-your-covid-pass-letter' |
      | EMIS      | 'Go to 111.nhs.uk'                                   | 'http://stubs.local.bitraft.io:8080/external/111/home'                                                              |
      | TPP       | 'Go to 111.nhs.uk'                                   | 'http://stubs.local.bitraft.io:8080/external/111/home'                                                              |

  #465
  Scenario Outline: Cannot log in as a TPP user with an age under 13
    Given I attempt to log in as a TPP user with an age under 13
    And 'NHS COVID Pass' responds to requests for type '/'
    And 'COVID Pass or proof' responds to requests for type '/get-your-covid-pass-letter'
    And '111' responds to requests for '/home'
    Then I see a message informing me I cannot log in as I am under the minimum age
    When I click the link called <Link text> with a url of <Link url>
    Then a new tab has been opened by the link
    Examples:
      | Link text                                            | Link url                                                                                                            |
      | 'Get your digital NHS COVID Pass'                    | 'http://stubs.local.bitraft.io:8080/external/covid-status-service-nhsx-nhs-uk'                                      |
      | 'Get your NHS COVID Pass letter sent to you by post' | 'http://stubs.local.bitraft.io:8080/external/conditions/coronavirus-covid-19/covid-pass/get-your-covid-pass-letter' |
      | 'Go to 111.nhs.uk'                                   | 'http://stubs.local.bitraft.io:8080/external/111/home'                                                              |

  #464
  Scenario Outline: Cannot log in as a EMIS user with invalid ODS Code
    Given I attempt to log in as a EMIS user with invalid ODS Code
    And 'NHS UK' responds to requests for '/nhs-app-contact-us'
    And 'My Health Online' responds to requests for type '/myhealthonline'
    And '111' responds to requests for '/wales'
    And 'COVID Pass or proof' responds to requests for type ''
    And 'Northern Ireland' responds to requests for type '/www-nidirect-gov-uk/articles/gp-out-hours-service'
    And I see the Contact us link with a url of 'http://stubs.local.bitraft.io:8080/external/nhsuk/nhs-app-contact-us?errorcode=3f'
    When I click the link called <Link text> with a url of <Link url>
    Then a new tab has been opened by the link
    Examples:
      | Link text             | Link url                                                                                        |
      | 'My Health Online'    | 'http://stubs.local.bitraft.io:8080/external/111/myhealthonline'                                |
      | '111.wales.uk'        | 'http://stubs.local.bitraft.io:8080/external/111/wales'                                         |
      | 'COVID Pass or proof' | 'http://stubs.local.bitraft.io:8080/external/conditions/coronavirus-covid-19/covid-pass'        |
      | 'Northern Ireland'    | 'http://stubs.local.bitraft.io:8080/external/www-nidirect-gov-uk/articles/gp-out-hours-service' |

  #468
  Scenario Outline: Cannot log in as a EMIS user without ODS Code
    Given I attempt to log in as a EMIS user without an ODS Code
    And 'NHS UK' responds to requests for '/nhs-app-contact-us'
    And '111' responds to requests for '/home'
    And 'COVID Pass or proof' responds to requests for type ''
    And I see the shutter 'Contact us' link with a url of 'http://stubs.local.bitraft.io:8080/external/nhsuk/nhs-app-contact-us?errorcode=3r'
    When I click the shutter <Link text> link with a url of <Link url>
    Then a new tab has been opened by the link
    Examples:
      | Link text         | Link url                                                                                 |
      | 'Contact us'      | 'http://stubs.local.bitraft.io:8080/external/nhsuk/nhs-app-contact-us?errorcode=3r'      |
      | '111.nhs.uk'      | 'http://stubs.local.bitraft.io:8080/external/111/home'                                   |
      | 'NHS COVID Pass'  | 'http://stubs.local.bitraft.io:8080/external/conditions/coronavirus-covid-19/covid-pass' |

  #469
  Scenario Outline: Cannot log in as a EMIS user with no NHS Number
    Given I attempt to log in as a EMIS user without an NHS Number
    And 'NHS UK' responds to requests for '/nhs-app-contact-us'
    And '111' responds to requests for '/home'
    And I see the shutter 'Contact us' link with a url of 'http://stubs.local.bitraft.io:8080/external/nhsuk/nhs-app-contact-us?errorcode=3u'
    When I click the shutter <Link text> link with a url of <Link url>
    Then a new tab has been opened by the link
    Examples:
      | Link text           | Link url                                                                            |
      | 'Contact us'        | 'http://stubs.local.bitraft.io:8080/external/nhsuk/nhs-app-contact-us?errorcode=3u' |
      | '111.nhs.uk'        | 'http://stubs.local.bitraft.io:8080/external/111/home'                              |

  #504 timeout
  Scenario: Cannot log in as a EMIS when the request timeout I see error code with "zn" prefix
    Given I attempt to log in as an EMIS and the CID request timeout
    And 'NHS UK' responds to requests for '/nhs-app-contact-us'
    Then I click a shutter 'Contact us if you keep seeing this message, quoting error code' link with a url of 'http://stubs.local.bitraft.io:8080/external/nhsuk/nhs-app-contact-us?errorcode=zn' and error prefix of 'zn'
    And a new tab has been opened by the link

#502
  Scenario: Cannot log in when NHS Login returns internal server error response and I see error code with "3n" prefix
    Given I attempt to log in as an EMIS and received internal server error from CID
    And 'NHS UK' responds to requests for '/nhs-app-contact-us'
    Then I click a shutter 'Contact us if you keep seeing this message, quoting error code' link with a url of 'http://stubs.local.bitraft.io:8080/external/nhsuk/nhs-app-contact-us?errorcode=3n' and error prefix of '3n'
    And a new tab has been opened by the link
