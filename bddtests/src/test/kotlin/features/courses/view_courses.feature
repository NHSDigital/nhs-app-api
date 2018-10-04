@courses
@prescription
Feature: View courses
  In order to view courses associated with a user
  As a logged in user
  I want to see a list of repeat courses that I can order

  Scenario Outline: The <GP System> User has repeatable prescriptions
    Given a patient from <GP System> is defined
    And I have historic prescriptions
    And I have 10 <GP System> assigned prescriptions
    And 10 of my prescriptions are of type repeat
    And 10 of my prescriptions can be requested
    And I am logged in
    And I navigate to prescriptions
    When I click 'Order a new repeat prescription'
    Then I see the available repeatable prescriptions
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  
  Scenario Outline: The <GP System> User has 0 repeatable prescriptions
    Given a patient from <GP System> is defined
    And I have historic prescriptions
    And I have 0 <GP System> assigned prescriptions
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

  
  Scenario Outline: The <GP System> user has 1 repeatable prescription
    Given a patient from <GP System> is defined
    And I have historic prescriptions
    And I have 1 <GP System> assigned prescriptions
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


  
  Scenario Outline: The user <GP System> should only see max 100 repeatable prescriptions
    Given a patient from <GP System> is defined
    And I have historic prescriptions
    And I have 101 <GP System> assigned prescriptions
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

  
  Scenario Outline: The <GP System> user has the max number of repeatable prescriptions
    Given a patient from <GP System> is defined
    And I have historic prescriptions
    And I have 100 <GP System> assigned prescriptions
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

  
  Scenario Outline: The <GP System> user has over 5 repeat dispense prescriptions
    Given a patient from <GP System> is defined
    And I have historic prescriptions
    And I have 10 <GP System> assigned prescriptions
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

  Scenario Outline: The <GP System> User has selected repeat prescriptions to order
    Given a patient from <GP System> is defined
    And I have historic prescriptions
    And I am logged in
    And I navigate to prescriptions
    And I select 5 <GP System> repeatable prescriptions out of 5 available
    When I click Continue on the Order a repeat prescription page
    Then I see the previously selected prescriptions on the Confirm repeat prescription page
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: The <GP System> User has selected one repeat prescription to order
    Given a patient from <GP System> is defined
    And I have historic prescriptions
    And I am logged in
    And I navigate to prescriptions
    And I select 1 <GP System> repeatable prescriptions out of 1 available
    When I click Continue on the Order a repeat prescription page
    Then I see the previously selected prescriptions on the Confirm repeat prescription page
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: The <GP System> User has selected no repeat prescriptions to order
    Given a patient from <GP System> is defined
    And I have historic prescriptions
    And I am logged in
    And I navigate to prescriptions
    And I select 0 <GP System> repeatable prescriptions out of 5 available
    When I click Continue on the Order a repeat prescription page
    Then A validation message is displayed indicating the user has not selected any repeat prescriptions
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: The <GP System> User alters a repeat prescriptions selection and views previous selection
    Given a patient from <GP System> is defined
    And I have historic prescriptions
    And I am logged in
    And I navigate to prescriptions
    And I select 5 <GP System> repeatable prescriptions out of 5 available
    And I click Continue on the Order a repeat prescription page
    When I click 'Change this repeat prescription' on the Prescription confirmation page
    Then I see my previously selected repeat prescriptions selected
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  @smoketest
  Scenario Outline: The <GP System> User alters a repeat prescriptions selection and the special request text and sees the updated confirmation
    Given a patient from <GP System> is defined
    And I have historic prescriptions
    And I am logged in
    And I navigate to prescriptions
    And I select 4 <GP System> repeatable prescriptions out of 5 available
    And I enter text "Please call when prescription received." for special request
    And I click Continue on the Order a repeat prescription page
    Then I see the previously selected prescriptions on the Confirm repeat prescription page
    And I see the special request text "Please call when prescription received."
    When I click 'Change this repeat prescription' on the Prescription confirmation page
    And I select 1 additional repeat prescriptions
    And I enter text "Note I'm allergic to paracetamol." for special request
    When I click Continue on the Order a repeat prescription page
    Then I see the previously selected prescriptions on the Confirm repeat prescription page
    And I see the special request text "Note I'm allergic to paracetamol."
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario Outline: The <GP System> special request text is optional and 'None' is displayed if they don't enter a value
    Given a patient from <GP System> is defined
    And I have historic prescriptions
    And I am logged in
    And I navigate to prescriptions
    And I select 1 <GP System> repeatable prescriptions out of 1 available
    And I click Continue on the Order a repeat prescription page
    Then I see the special request text "None"
    Examples:
      | GP System |
      | EMIS      |
      | TPP       |

  Scenario: The User manipulates the url to go to the repeat prescriptions page and the service is disabled at a GP Practice level
    Given a patient from EMIS is defined
    And I have historic prescriptions
    And I am logged in
    And I navigate to prescriptions
    And I am using EMIS GP System
    And prescriptions is disabled at a GP Practice level
    When I browse to the page at /prescriptions/repeat-courses
    Then I see a message informing me that I don't currently have access to this service

  Scenario: The User manipulates the url to go to the confirm repeat prescriptions page and the service is disabled at a GP Practice level
    Given a patient from EMIS is defined
    And I have historic prescriptions
    And I am logged in
    And I navigate to prescriptions
    And I am using EMIS GP System
    And prescriptions is disabled at a GP Practice level
    When I browse to the page at /prescriptions/confirm-prescription-details
    Then I see a message informing me that I don't currently have access to this service

  Scenario: The user has 1 repeatable prescription with missing quantity info
    Given a patient from EMIS is defined
    And I have historic prescriptions
    And I am logged in
    And I navigate to prescriptions
    And I have 1 assigned prescriptions which have only dosage info
    And 1 of my prescriptions are of type repeat
    And 1 of my prescriptions can be requested
    When I click 'Order a new repeat prescription'
    Then I see the available repeatable prescriptions

  Scenario: The user has 1 repeatable prescription with missing dosage info
    Given a patient from EMIS is defined
    And I have historic prescriptions
    And I am logged in
    And I navigate to prescriptions
    And I have 1 assigned prescriptions which have only quantity info
    And 1 of my prescriptions are of type repeat
    And 1 of my prescriptions can be requested
    When I click 'Order a new repeat prescription'
    Then I see the available repeatable prescriptions

  Scenario: The user has 1 repeatable prescription with missing dosage and quantity info
    Given a patient from EMIS is defined
    And I have historic prescriptions
    And I am logged in
    And I navigate to prescriptions
    And I have 1 assigned prescriptions which have no info
    And 1 of my prescriptions are of type repeat
    And 1 of my prescriptions can be requested
    When I click 'Order a new repeat prescription'
    Then I see the available repeatable prescriptions

  Scenario: The User has selected repeat prescriptions to order with missing quantity info
    Given a patient from EMIS is defined
    And I have historic prescriptions
    And I am logged in
    And I navigate to prescriptions
    And I select 1 repeatable prescriptions out of 1 available which have only dosage info
    When I click Continue on the Order a repeat prescription page
    Then I see the previously selected prescriptions on the Confirm repeat prescription page

  Scenario: The User has selected repeat prescriptions to order with missing dosage info
    Given a patient from EMIS is defined
    And I have historic prescriptions
    And I am logged in
    And I navigate to prescriptions
    And I select 1 repeatable prescriptions out of 1 available which have only quantity info
    When I click Continue on the Order a repeat prescription page
    Then I see the previously selected prescriptions on the Confirm repeat prescription page

  Scenario: The User has selected repeat prescriptions to order with missing dosage and quantity info
    Given a patient from EMIS is defined
    And I have historic prescriptions
    And I am logged in
    And I navigate to prescriptions
    And I select 1 repeatable prescriptions out of 1 available which have no info
    When I click Continue on the Order a repeat prescription page
    Then I see the previously selected prescriptions on the Confirm repeat prescription page