@force-update
@native
@manual
  # manual until we can have native tests (involves checking for native pop up)
Feature: Force a user to update their native app if the version of the NHS they have installed is no longer supported

  Scenario: User with outdated app version is forced to update
    Given I am using an outdated version of the app
    When I load the app
    Then I am presented with a native pop up informing me I can't use the app
    And the app exits when I click on 'Close'

  Scenario: User with supported app version is not forced to update
    Given I am using a supported version of the app
    When I load the app
    Then I am not presented with a native pop up informing me I can't use the app