Feature: My appointments for desktop experience
  Users can view their upcoming and past appointments in the My Appointments screen.

  @mobileWeb
  Scenario Outline: A <GP System> user sees Service currently unavailable message when GP system is unavailable - mobile
    Given the <GP System> GP appointment system is unavailable
    And I am using a <Device> device
    And I am logged in as a <GP System> user
    When I am on the My Appointments page
    Then I see page header indicating there is an appointment data error
    And I see the appropriate error messages for the appointment data error
    And I log out
    Examples:
      | GP System | Device |
      | EMIS      | mobile |
#      | EMIS      | desktop |
#      | EMIS      | native android |
#      | EMIS      | native ios |
