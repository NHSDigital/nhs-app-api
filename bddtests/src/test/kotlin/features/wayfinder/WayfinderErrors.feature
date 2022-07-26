@appointments
@wayfinder
Feature: Wayfinder Errors

  Scenario Outline: A user sees a helpful error screen when Wayfinder is <An Error> and can try again
    Given I am a user whose surgery has enabled Wayfinder
    And I have referrals and upcoming appointments
    And the Wayfinder Aggregator API is <An Error>
    And I am logged in
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    When I click the 'Referrals, hospital and other appointments' link on the Appointments Hub
    Then I see a helpful message indicating unavailable secondary care services with a <Prefix> service desk reference
    When the Wayfinder Aggregator API issues are resolved and is returning referrals and upcoming appointments
    And I click the try again button on the unavailable secondary care services error screen
    Then I see an in-review referral
    Examples:
      | An Error              | Prefix |
      | timing out            | zu     |
      | encountering an issue | 4u     |

  Scenario Outline: A user can easily contact the service desk when they encounter an error with Wayfinder
    Given I am a user whose surgery has enabled Wayfinder
    And I have no referrals or appointments
    And the Wayfinder Aggregator API is <An Error>
    And I am logged in
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    When I click the 'Referrals, hospital and other appointments' link on the Appointments Hub
    Then I see a helpful message indicating unavailable secondary care services with a <Prefix> service desk reference
    When I click the contact us link with the <Prefix> error code
    Then a new tab has been opened by the link
    Examples:
      | An Error              | Prefix |
      | timing out            | zu     |
      | encountering an issue | 4u     |

  Scenario: A user can still see appointments and referrals when they encounter a partial error with Wayfinder
    Given I am a user whose surgery has enabled Wayfinder
    And I get a partial error returned for a supplier
    And I am logged in
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    When I click the 'Referrals, hospital and other appointments' link on the Appointments Hub
    Then I see a booked appointment

  Scenario: A user is not at the minimum age to get a valid response from Wayfinder
    Given I am a user whose surgery has enabled Wayfinder
    And I am too young to use Wayfinder to retrieve referrals and appointments
    And I am logged in
    When I navigate to Appointments
    Then the Appointments Hub page is displayed
    When I click the 'Referrals, hospital and other appointments' link on the Appointments Hub
    Then I see a message indicating secondary care services are unavailable because the user is under minimum age
