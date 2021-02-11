@courses
@prescription
Feature: View Courses Frontend
  In order to view courses associated with a user
  As a logged in user
  I want to see a list of repeat courses that I can order

  Scenario Outline: The <GP System> User has repeatable prescriptions
    Given I am a <GP System> patient
    And I have historic prescriptions
    And I have 10 assigned prescriptions
    And 10 of my prescriptions are of type repeat
    And 10 of my prescriptions can be requested
    And I am logged in
    And I navigate to prescriptions
    When I click 'Order a new repeat prescription'
    Then I see the available repeatable prescriptions
    And I click 'Help with medical abbreviations'
    And I see the medical abbreviations help link
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: The <GP System> User has 0 repeatable prescriptions
    Given I am a <GP System> patient
    And I have historic prescriptions
    And I have 0 assigned prescriptions
    And 0 of my prescriptions are of type repeat
    And 0 of my prescriptions can be requested
    And I am logged in
    And I navigate to prescriptions
    When I click 'Order a new repeat prescription'
    Then I see the available repeatable prescriptions
    And a message is displayed indicating that you don't have any medication available to order
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: The <GP System> user has 1 repeatable prescription
    Given I am a <GP System> patient
    And I have historic prescriptions
    And I have 1 assigned prescriptions
    And 1 of my prescriptions are of type repeat
    And 1 of my prescriptions can be requested
    And I am logged in
    And I navigate to prescriptions
    When I click 'Order a new repeat prescription'
    Then I see the available repeatable prescriptions
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: The user <GP System> should only see max 100 repeatable prescriptions
    Given I am a <GP System> patient
    And I have historic prescriptions
    And I have 101 assigned prescriptions
    And 101 of my prescriptions are of type repeat
    And 101 of my prescriptions can be requested
    And I am logged in
    And I navigate to prescriptions
    When I click 'Order a new repeat prescription'
    Then I see the available repeatable prescriptions
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: The <GP System> user has the max number of repeatable prescriptions
    Given I am a <GP System> patient
    And I have historic prescriptions
    And I have 100 assigned prescriptions
    And 100 of my prescriptions are of type repeat
    And 100 of my prescriptions can be requested
    And I am logged in
    And I navigate to prescriptions
    When I click 'Order a new repeat prescription'
    Then I see the available repeatable prescriptions
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: The <GP System> user has over 5 repeat dispense prescriptions
    Given I am a <GP System> patient
    And I have historic prescriptions
    And I have 10 assigned prescriptions
    And 5 of my prescriptions are of type repeat
    And 3 of my prescriptions can be requested
    And I am logged in
    And I navigate to prescriptions
    When I click 'Order a new repeat prescription'
    Then I see the available repeatable prescriptions
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |

  Scenario Outline: The <GP System> User has selected repeat prescriptions to order
    Given I am a <GP System> patient
    And I have historic prescriptions
    And there are 5 repeatable prescriptions available
    And I am logged in
    And I navigate to prescriptions
    And I select 5 repeatable prescriptions out of 5 available
    When I click Continue on the Order a repeat prescription page
    Then I see the previously selected prescriptions on the Confirm repeat prescription page
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: The <GP System> User has selected one repeat prescription to order
    Given I am a <GP System> patient
    And I have historic prescriptions
    And there are 4 repeatable prescriptions available
    And I am logged in
    And I navigate to prescriptions
    And I select 2 repeatable prescriptions out of 4 available
    When I click Continue on the Order a repeat prescription page
    Then I see the previously selected prescriptions on the Confirm repeat prescription page
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: The <GP System> User has selected no repeat prescriptions to order
    Given I am a <GP System> patient
    And I have historic prescriptions
    And there are 5 repeatable prescriptions available
    And I am logged in
    And I navigate to prescriptions
    And I select 0 repeatable prescriptions out of 5 available
    When I click Continue on the Order a repeat prescription page
    Then A validation message is displayed indicating the user has not selected any repeat prescriptions
    When I navigate to prescriptions
    And I click 'Order a new repeat prescription'
    Then A validation message is not displayed indicating the user has not selected any repeat prescriptions

    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: The <GP System> User alters a repeat prescriptions selection and views previous selection
    Given I am a <GP System> patient
    And I have historic prescriptions
    And there are 5 repeatable prescriptions available
    And I am logged in
    And I navigate to prescriptions
    And I select 5 repeatable prescriptions out of 5 available
    And I click Continue on the Order a repeat prescription page
    When I click the Back link
    Then I see my previously selected repeat prescriptions selected
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: The <GP System> User alters a repeat prescriptions selection and the special request text and sees the updated confirmation
    Given I am a <GP System> patient
    And I have historic prescriptions
    And Azure organisation search is working
    And there are 5 repeatable prescriptions available
    And I am logged in
    And I navigate to prescriptions
    And I select 4 repeatable prescriptions out of 5 available
    And I enter text "Please call when prescription received." for special request
    And I click Continue on the Order a repeat prescription page
    Then I see the previously selected prescriptions on the Confirm repeat prescription page
    And I see the entered special request text
    When I click 'Change' for the prescriptions on the Prescription confirmation page
    And I select 1 additional repeat prescriptions
    And I click Continue on the Order a repeat prescription page
    And I click 'Change' for the special request on the Prescription confirmation page
    And I enter text "Note I'm allergic to paracetamol." for special request
    And I click Continue on the Order a repeat prescription page
    Then I see the previously selected prescriptions on the Confirm repeat prescription page
    And I see the entered special request text
    Examples:
      | GP System |
      | EMIS      |
      | VISION    |
      | MICROTEST |
    @smoketest
    Examples:
      | GP System |
      | TPP       |

  Scenario Outline: The <GP System> special request text is optional and 'None' is displayed if they don't enter a value
    Given I am a <GP System> patient
    And I have historic prescriptions
    And there are 1 repeatable prescriptions available
    And I am logged in
    And I navigate to prescriptions
    And I select 1 repeatable prescriptions out of 1 available
    And I click Continue on the Order a repeat prescription page
    Then I see the default special request text
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: The user can't see prescription special request when the gp system <GP System> has disabled it
    Given I am a <GP System> patient
    And special request text has been disabled
    And I have historic prescriptions
    And there are 10 repeatable prescriptions available
    And I am logged in
    And I navigate to prescriptions
    And I have 10 assigned prescriptions
    And 10 of my prescriptions are of type repeat
    And 10 of my prescriptions can be requested
    And I select 1 repeatable prescriptions out of 10 available
    Then I don't see the special request text area
    When I click Continue on the Order a repeat prescription page
    Then I don't see the special request text on prescription confirmation
    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  Scenario Outline: The user can see prescription special request when the gp system <GP System> has enabled it
    Given I am a <GP System> patient
    And special request text has been enabled and is 'Optional'
    And I have historic prescriptions
    And I am logged in
    And I navigate to prescriptions
    And I have 10 assigned prescriptions
    And 10 of my prescriptions are of type repeat
    And 10 of my prescriptions can be requested
    When I click 'Order a new repeat prescription'
    Then I see the special request text area
    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  Scenario: An Emis user sees a validation message when they select a repeat prescription but do not enter the mandatory special request text
    Given I am a EMIS patient
    And special request text has been enabled and is 'Mandatory'
    And I have historic prescriptions
    And there are 5 repeatable prescriptions available
    And I am logged in
    And I navigate to prescriptions
    And I select 1 repeatable prescriptions out of 5 available
    When I click Continue on the Order a repeat prescription page
    Then A validation message is displayed indicating the user has not entered special request text
    When I navigate to prescriptions
    And I click 'Order a new repeat prescription'
    Then A validation message is not displayed indicating the user has not entered special request text

  Scenario Outline: An <GP System> User manipulates the url to go to the repeat prescriptions page and the service is disabled at a GP Practice level
    Given I am patient using the <GP System> GP System
    And prescriptions is disabled at a GP Practice level
    And I have historic prescriptions
    And I am logged in
    And I navigate to prescriptions
    When I browse to the page at /prescriptions/repeat-courses
    Then I see a message informing me that I don't currently have access to this service
    Examples:
      | GP System |
      | EMIS      |
      | VISION    |

  Scenario: The User manipulates the url to go to the confirm repeat prescriptions page and the service is disabled at a GP Practice level
    Given I am a EMIS patient
    And I have historic prescriptions
    And prescriptions is disabled at a GP Practice level
    And I am logged in
    When I browse to the page at /prescriptions/confirm-prescription-details
    And the Prescriptions Hub page is displayed
    And I click the Order a repeat prescription button
    Then I see a message informing me that I don't currently have access to this service

  Scenario: The user has 1 repeatable prescription with missing quantity info
    Given I am a EMIS patient
    And I have historic prescriptions
    And I am logged in
    And I navigate to prescriptions
    And I have 1 assigned prescriptions which have only dosage info
    And 1 of my prescriptions are of type repeat
    And 1 of my prescriptions can be requested
    When I click 'Order a new repeat prescription'
    Then I see the available repeatable prescriptions

  Scenario: The user has 1 repeatable prescription with missing dosage info
    Given I am a EMIS patient
    And I have historic prescriptions
    And I am logged in
    And I navigate to prescriptions
    And I have 1 assigned prescriptions which have only quantity info
    And 1 of my prescriptions are of type repeat
    And 1 of my prescriptions can be requested
    When I click 'Order a new repeat prescription'
    Then I see the available repeatable prescriptions

  Scenario: The user has 1 repeatable prescription with missing dosage and quantity info
    Given I am a EMIS patient
    And I have historic prescriptions
    And I am logged in
    And I navigate to prescriptions
    And I have 1 assigned prescriptions which have no info
    And 1 of my prescriptions are of type repeat
    And 1 of my prescriptions can be requested
    When I click 'Order a new repeat prescription'
    Then I see the available repeatable prescriptions

  Scenario: The User has selected repeat prescriptions to order with missing quantity info
    Given I am a EMIS patient
    And I have historic prescriptions
    And I have 1 repeatable prescriptions available which have only dosage info
    And I am logged in
    And I navigate to prescriptions
    And I select 1 repeatable prescriptions out of 1 available
    When I click Continue on the Order a repeat prescription page
    Then I see the previously selected prescriptions on the Confirm repeat prescription page

  Scenario: The User has selected repeat prescriptions to order with missing dosage info
    Given I am a EMIS patient
    And I have historic prescriptions
    And there are 1 repeatable prescriptions available
    And I have 1 repeatable prescriptions available which have only quantity info
    And I am logged in
    And I navigate to prescriptions
    And I select 1 repeatable prescriptions out of 1 available
    When I click Continue on the Order a repeat prescription page
    Then I see the previously selected prescriptions on the Confirm repeat prescription page

  Scenario: The User has selected repeat prescriptions to order with missing dosage and quantity info
    Given I am a EMIS patient
    And I have historic prescriptions
    And there are 1 repeatable prescriptions available
    And I have 1 repeatable prescriptions available which have no info
    And I am logged in
    And I navigate to prescriptions
    And I select 1 repeatable prescriptions out of 1 available
    When I click Continue on the Order a repeat prescription page
    Then I see the previously selected prescriptions on the Confirm repeat prescription page

  Scenario Outline: An <GP System> user recovers the GP session before an error when ordering a prescription
    Given I am an <GP System> patient whose GP system is unavailable
    And I am logged in
    When I navigate to prescriptions
    And The <GP System> GP system becomes available
    And I have no booked appointments for <GP System>
    And I have historic prescriptions
    And I have 1 assigned prescriptions which have only quantity info
    And 1 of my prescriptions are of type repeat
    And 1 of my prescriptions can be requested
    And I click the Order a repeat prescription button
    Then I see the available repeatable prescriptions
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: An <GP System> user without a GP session is able to recover their session on try again
    Given I am an <GP System> patient whose GP system is unavailable
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And The <GP System> GP system is still unavailable
    And I click the Order a repeat prescription button
    Then I see appropriate try again error message for prescriptions when there is no GP session
    And The <GP System> GP system becomes available
    And I have historic prescriptions
    And I have 1 assigned prescriptions which have only quantity info
    And 1 of my prescriptions are of type repeat
    And 1 of my prescriptions can be requested
    When I click the 'Try again' button
    Then I see the available repeatable prescriptions
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

  Scenario Outline: A <GP System> user GP session eventually becomes available when ordering a prescription
    Given I am an <GP System> patient whose GP system is unavailable
    And I am logged in
    When I retrieve the 'Your Prescriptions' page directly
    And The <GP System> GP system is still unavailable
    And I click the Order a repeat prescription button
    Then I see appropriate try again error message for prescriptions when there is no GP session
    When I click the 'Try again' button
    Then I see what I can do next with a prescriptions error message and reference code '3p'
    And I click the session error back link
    And I click the Order a repeat prescription button
    And I see what I can do next with a prescriptions error message and reference code '3p'
    And I click the session error back link
    And the Prescriptions Hub page is displayed
    And The <GP System> GP system becomes available
    And I have historic prescriptions
    And I have 1 assigned prescriptions which have only quantity info
    And 1 of my prescriptions are of type repeat
    And 1 of my prescriptions can be requested
    When I click 'Order a new repeat prescription'
    Then I see the available repeatable prescriptions
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |
      | VISION    |
      | MICROTEST |

