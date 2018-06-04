Feature: Get allergy data

  A user can get their allergy information

  Background:
    Given wiremock is initialised

  @GetAllergiesObject
  @backend
  Scenario: Requesting allergies returns allergies data
    Given I have logged in and have a valid session cookie
    When I get the users allergy data with a valid cookie
    Then I receive the allergies object