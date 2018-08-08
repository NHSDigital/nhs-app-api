@appointment
Feature: Book an available appointment slot
  In order to complete a booking appointment
  As a logged in user
  I want to be able to select, confirm and book selected appointment

  @NHSO-72
  @NHSO-872
  Scenario Outline: A <GP System> user tries to book an appointment without describing symptoms
    Given there are <GP System> appointments available to book
    And I am logged in
    And I am on the available appointments page
    And I have selected an appointment slot to book
    When I click the 'Confirm and book appointment' button
    Then an error is displayed that "Describe your symptoms" is mandatory
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  @NHSO-72
  @smoketest
  @NHSO-872
  Scenario Outline: A <GP System> user tries to book an appointment describing symptoms at least 1 character
    Given there are <GP System> appointments available to book
    And I am logged in
    And I am on the available appointments page
    And I have selected an appointment slot to book
    When I enter symptoms of 1 characters
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking confirmation screen is displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  @NHSO-72
  @NHSO-872
  Scenario Outline: A <GP System> user tries to book an appointment describing symptoms no more 150 characters
    Given there are <GP System> appointments available to book
    And I am logged in
    And I am on the available appointments page
    And I have selected an appointment slot to book
    When I enter symptoms of 150 characters
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking confirmation screen is displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  @NHSO-72
  @NHSO-872
  Scenario Outline: A <GP System> user tries to enter symptoms with over 150 characters
    Given there are <GP System> appointments available to book
    And I am logged in
    And I am on the available appointments page
    And I have selected an appointment slot to book
    When I enter symptoms of 151 characters
    Then only the first 150 characters will be displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  @NHSO-72
  @smoketest
  @NHSO-872
  Scenario Outline: A <GP System> user tries to paste symptoms with over 150 characters
    Given there are <GP System> appointments available to book
    And I am logged in
    And I am on the available appointments page
    And I have selected an appointment slot to book
    When I paste symptoms of 151 characters
    Then only the first 150 characters will be displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  @NHSO-72
  @NHSO-872
  Scenario Outline: A <GP System> user who books successfully, but only the first 150 characters of the symptoms are sent
    Given there are <GP System> appointments available to book
    And I am logged in
    And I am on the available appointments page
    And I have selected an appointment slot to book
    When I enter symptoms of 151 characters
    And I click the 'Confirm and book appointment' button
    Then the Appointment Booking confirmation screen is displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  @NHSO-517
  @NHSO-872
  Scenario Outline: A <GP System> user sees appropriate information message when there is a timeout
    Given there are <GP System> appointments available to book, but GP system doesn't respond a timely fashion when booking
    And I am logged in
    And I am on the available appointments page
    And I have selected an appointment slot to book
    When I enter symptoms
    And  I click the 'Confirm and book appointment' button
    Then I see appropriate information message after 10 seconds when it times-out on appointment confirmation page
    And there should be a button to go back to my appointments
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  @NHSO-517
  @NHSO-872
  Scenario Outline: A <GP System> user sees appropriate information message when GP system is unavailable
    Given there are <GP System> appointments available to book, but the GP system is unavailable
    And I am logged in
    And I am on the available appointments page
    And I have selected an appointment slot to book
    When I enter symptoms
    And  I click the 'Confirm and book appointment' button
    Then I see appropriate information message when there is an error sending data on appointment confirmation page
    And there should be a button to go back to my appointments
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  @NHSO-517
  @NHSO-872
  Scenario Outline: A <GP System> user sees appropriate information message when appointment has already been booked
    Given there are <GP System> appointments available to book, but the appointment slot has already been booked by somebody else
    And I am logged in
    And I am on the available appointments page
    And I have selected an appointment slot to book
    When I enter symptoms
    And  I click the 'Confirm and book appointment' button
    Then I am able to filter on available slots
    And a message is displayed indicating that the slot has already been taken
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  @NHSO-517
  @NHSO-872
  Scenario Outline: A <GP System> user can return directly back to their appointments after trying to book one already booked
    Given there are <GP System> appointments available to book, but the appointment slot has already been booked by somebody else
    And I am logged in
    And I am on the available appointments page
    And I have selected an appointment slot to book
    And I enter symptoms
    And  I click the 'Confirm and book appointment' button
    When I click the button to go back to my appointments
    Then I will be on the My appointments screen
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  @NHSO-528
  @NHSO-872
  @smoketest
  Scenario Outline: A <GP System> user is navigated back to the 'Book this appointment' screen when 'Change this appointment' button selected.
    Given there are <GP System> appointments available to book
    And I am logged in
    And I am on the available appointments page
    And I have selected an appointment slot to book
    When I choose to change the appointment slot
    Then there is a filter for the appointment types
    And there is a filter for the appointment locations
    And there is a filter for the appointment doctors/nurses
    And there is a filter for the appointment time period
    And no available slots are displayed
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
