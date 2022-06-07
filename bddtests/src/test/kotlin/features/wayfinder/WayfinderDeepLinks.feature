@appointments
@wayfinder
Feature: Wayfinder DeepLinks

  Scenario: A user with an eRS referral can follow the deep link without seeing the third party warning screen
    Given I am using the native app user agent
    And I am a user whose surgery has enabled Wayfinder
    And I have referrals and upcoming appointments
    And I am logged in
    When I navigate to the Appointment Hub page
    Then the Appointments Hub page is displayed
    When I click the 'Referrals, hospital and other appointments' link on the Appointments Hub
    Then the Referrals, hospital and other appointments screen with data is displayed
    And I see an in-review referral
    When I click the 'View or manage this referral' link
    Then I am navigated to a third party site

  Scenario Outline: A user with a <Provider> appointment can follow the deep link and see the third party warning screen
    Given I am using the native app user agent
    And I am a user whose surgery has enabled Wayfinder
    And I have referrals and upcoming <Provider> appointments
    And <Provider> responds to requests for viewing an appointment
    And I am logged in
    When I navigate to the Appointment Hub page
    Then the Appointments Hub page is displayed
    When I click the 'Referrals, hospital and other appointments' link on the Appointments Hub
    Then the Referrals, hospital and other appointments screen with data is displayed
    And I see an appointment ready to confirm
    When I click the 'Contact the clinic to confirm' button
    Then I am redirected to the redirector page with the header 'View appointments'
    When I click the 'Continue' button on the redirector page with a url starting with '<DeepLink>'
    Then I am navigated to a third party site for <Provider>
  Examples:
    | Provider | DeepLink                                                                                                               |
    | PKB      | http://pkb.stubs.local.bitraft.io:8080/nhs-login/login?phrPath=%2Fdiary%2FviewAppointment.action%3FuniqueId%3D         |
    | Netcall  | http://netcall.stubs.local.bitraft.io:8080/i/nhsappintegration/p/27EFBAC1/40EFBAC1/66EFBAC1/417EFBAC1?remote_record_id |
    | Zesty    | http://zesty.stubs.local.bitraft.io:8080/nhs/origin_appointment?resource_id=XXXX                                       |
