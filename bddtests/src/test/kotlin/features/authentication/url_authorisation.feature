@authentication
Feature: Authorisation occurs during each URL visit
  Access to the secure area is only granted to authenticated users.  This feature ensures this occurs when directly navigating using URLs too.

  Scenario: User has never logged in and attempts to navigate to a restricted url
    Given I am a EMIS patient
    And I am not logged in
    When I browse to the pages at the following urls I see the login page
      | /account                                    |
      | /appointments                               |
      | /appointments/booking-guidance              |
      | /appointments/cancelling                    |
      | /appointments/booking                       |
      | /appointments/confirmation                  |
      | /data-sharing                               |
      | /                                           |
      | /logout                                     |
      | /more                                       |
      | /my-record                                  |
      | /my-record/noaccess                         |
      | /my-record/testresultdetail                 |
      | /prescriptions                              |
      | /prescriptions/repeat-courses               |
      | /prescriptions/confirm-prescription-details |
      | /symptoms                                   |
      | /terms-and-conditions                       |

  Scenario: User has just logged out and attempts to navigate to a restricted url
    Given I am a EMIS patient
    And I have just logged out
    And I see the login page
    When I browse to the pages at the following urls I see the login page
      | /account                                    |
      | /appointments                               |
      | /appointments/booking-guidance              |
      | /appointments/cancelling                    |
      | /appointments/booking                       |
      | /appointments/confirmation                  |
      | /data-sharing                               |
      | /                                           |
      | /logout                                     |
      | /more                                       |
      | /my-record                                  |
      | /my-record/noaccess                         |
      | /my-record/testresultdetail                 |
      | /prescriptions                              |
      | /prescriptions/repeat-courses               |
      | /prescriptions/confirm-prescription-details |
      | /symptoms                                   |
      | /terms-and-conditions                       |

  @tech-debt  @NHSO-3580
  Scenario: User session has expired and attempts to navigate to a restricted url
    Given I am a EMIS patient
    And my session has expired
    When I browse to the pages at the following urls I see the login page
      | /account                                    |
      | /appointments                               |
      | /appointments/booking-guidance              |
      | /appointments/cancelling                    |
      | /appointments/booking                       |
      | /appointments/confirmation                  |
      | /data-sharing                               |
      | /                                           |
      | /logout                                     |
      | /more                                       |
      | /my-record                                  |
      | /my-record/noaccess                         |
      | /my-record/testresultdetail                 |
      | /prescriptions                              |
      | /prescriptions/repeat-courses               |
      | /prescriptions/confirm-prescription-details |
      | /symptoms                                   |
      | /terms-and-conditions                       |
    
Scenario: User browses to url when logged in
  Given I am a EMIS patient
  And I am about to directly access every page
  And I am logged in
  And I see the home page
  When I browse to the pages at the following urls I see the relevant page
    | /account                                    | /account                       |
    | /appointments                               | /appointments                  |
    | /appointments/booking-guidance              | /appointments/booking-guidance |
    | /appointments/booking                       | /appointments/booking          |
    | /data-sharing                               | /data-sharing                  |
    | /                                           | /                              |
    | /my-record                                  | /gp-medical-record             |
    | /prescriptions                              | /prescriptions                 |
    | /prescriptions/repeat-courses               | /prescriptions/repeat-courses  |
    | /prescriptions/confirm-prescription-details | /prescriptions                 |
    | /symptoms                                   | /symptoms                      |
    | /terms-and-conditions                       | /                              |
    | /logout                                     | /login                         |

@bug @NHSO-8672
  #To be merged into the above test once the bug is fixed
  Scenario: User browses to url when logged in - Appointments Bug NHSO-8672
    Given I am a EMIS patient
    And I am about to directly access every page
    And I am logged in
    And I see the home page
    When I browse to the pages at the following urls I see the relevant page
      | /appointments/cancelling                    | /appointments/gp-appointments  |
      | /appointments/confirmation                  | /appointments/gp-appointments  |

  Scenario Outline: User has never logged in and attempts to navigate to a restricted <Url> is taken to the <Page> after login
    Given I am a EMIS patient
    And I am not logged in
    When I browse to the <Url> and see the login page
    When I login
    Then I am on the relevant <Page> page
    Examples:
      | Url                                         | Page                           |
      | /appointments                               | /appointments                  |
      | /data-sharing                               | /data-sharing                  |
      | /                                           | /                              |
      | /my-record                                  | /gp-medical-record             |
      | /prescriptions                              | /prescriptions                 |
      | /prescriptions/confirm-prescription-details | /prescriptions                 |
      | /symptoms                                   | /symptoms                      |
      | /terms-and-conditions                       | /                              |
      | /logout                                     | /login                         |
      | /redirector                                 | /                              |
      | /redirector?redirect_to=appointments        | /appointments                  |
      | /nonexistent                                | /                              |

