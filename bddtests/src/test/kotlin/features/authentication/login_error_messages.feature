@authentication
@authentication-login
Feature: Login error messages

  The user is shown appropriate error messages if something goes wrong during login
#400
  @nativesmoketest
  Scenario: CitizenID provides invalid data after successful login
    Given I am logged into Citizen ID but am receiving invalid data
    Then In the error message I see the service reference number prefix with "3a"
    And I see the error 'Contact us' link with a url of 'https://www.nhs.uk/contact-us/nhs-app-contact-us?errorcode=3a'
    When I click the error 'Back to home' link
    Then I see the login page

#403
  @nativesmoketest
  Scenario: CitizenID login is successful but TPP GP System authentication fails
    Given I am logged into Citizen ID but GP System authentication fails
    Then In the error message I see the service reference number prefix with "3c"
    And I see the error 'Contact us' link with a url of 'https://www.nhs.uk/contact-us/nhs-app-contact-us?errorcode=3c'
    When I click the error 'Back to home' link
    Then I see the login page

#403
  Scenario: Cannot log in as an EMIS user with no userPatientLinkToken
    Given I attempt to log in as an EMIS user with no userPatientLinkToken
    Then In the error message I see the service reference number prefix with "3c"
    When I click the error 'Contact us' link with a url of 'https://www.nhs.uk/contact-us/nhs-app-contact-us?errorcode=3c'
    Then a new tab has been opened by the link

#502
  @nativesmoketest
  Scenario: CitizenID login is successful but EMIS session cannot be established
    Given I am logged into Citizen ID but EMIS session cannot be established
    Then In the error message I see the service reference number prefix with "3e"
    And I see the error 'Contact us' link with a url of 'https://www.nhs.uk/contact-us/nhs-app-contact-us?errorcode=3e'
    When I click the error 'Back to home' link
    Then I see the login page

  #465
  Scenario Outline: Cannot log in as a <GP System> user with no Date of Birth
    Given I attempt to log in as a <GP System> user without a date of birth
    Then I see an error message informing me I cannot log in as I am under the minimum age
    Examples:
      | GP System |
      | TPP       |

  @nativesmoketest
    Examples:
      | GP System |
      | EMIS      |

    #465
  Scenario: Cannot log in as a TPP user with an age under 13
    Given I attempt to log in as a TPP user with an age under 13
    Then I see an error message informing me I cannot log in as I am under the minimum age

    #464
  Scenario Outline: Cannot log in as a <GP System> user with no NHS Number
    Given I attempt to log in as a <GP System> user without an NHS Number
    Then The reference error and label and prefix is shown as "Reference: 3f"
    When I click the error <Link text> link with a url of <Link url>
    Then a new tab has been opened by the link
    Examples:
      | GP System | Link text           | Link url                                                        |
      | TPP       | 'contact us'        | 'https://www.nhs.uk/contact-us/nhs-app-contact-us?errorcode=3f' |
      | TPP       | 'My Health Online'  | 'https://111.wales.nhs.uk/contactus/myhealthonline/'            |
      | TPP       | '111.wales.nhs.uk'  | 'https://111.wales.nhs.uk'                                      |
      | TPP       | '111.nhs.uk'        | 'https://111.nhs.uk'                                            |

  @nativesmoketest
    Examples:
      | GP System  | Link text           | Link url                                                        |
      | EMIS       | 'contact us'        | 'https://www.nhs.uk/contact-us/nhs-app-contact-us?errorcode=3f' |
      | EMIS       | 'My Health Online'  | 'https://111.wales.nhs.uk/contactus/myhealthonline/'            |
      | EMIS       | '111.wales.nhs.uk'  | 'https://111.wales.nhs.uk'                                      |
      | EMIS       | '111.nhs.uk'        | 'https://111.nhs.uk'                                            |

  #464
  @nativesmoketest
  Scenario Outline: Cannot log in as a EMIS user with invalid ODS Code
    Given I attempt to log in as a EMIS user with invalid ODS Code
    Then The reference error and label and prefix is shown as "Reference: 3f"
    When I click the error <Link text> link with a url of <Link url>
    Then a new tab has been opened by the link
    Examples:
      | Link text           | Link url                                                        |
      | 'contact us'        | 'https://www.nhs.uk/contact-us/nhs-app-contact-us?errorcode=3f' |
      | 'My Health Online'  | 'https://111.wales.nhs.uk/contactus/myhealthonline/'            |
      | '111.wales.nhs.uk'  | 'https://111.wales.nhs.uk'                                      |
      | '111.nhs.uk'        | 'https://111.nhs.uk'                                            |

#504 timeout
  Scenario: Cannot log in as a EMIS when the request timeout I see error code with "zn" prefix
    Given I attempt to log in as an EMIS and the CID request timeout
    Then In the error message I see the service reference number prefix with "zn"
    When I click the error 'Contact us' link with a url of 'https://www.nhs.uk/contact-us/nhs-app-contact-us?errorcode=zn'
    Then a new tab has been opened by the link
