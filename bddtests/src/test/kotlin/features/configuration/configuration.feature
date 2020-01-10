@configuration
@backend
Feature: Configuration Backend

  Scenario: An api user can get a response from the configuration v1 endpoint
    When I get the v1 configuration
    Then I receive a "OK" success code
    And the configuration response will have a isDeviceSupported property
    And the configuration response will have a isThrottlingEnabled property
    And the configuration response will have a fidoServerUrl property

  Scenario: An api user can get a response from the configuration v2 endpoint
    When I get the v2 configuration
    Then I receive a "OK" success code
    And the v2 configuration response will have a minimumSupportedAndroidVersion property
    And the v2 configuration response will have a minimumSupportediOSVersion property
    And the v2 configuration response will have a fidoServerUrl property
    And the v2 configuration response will have a knownServices property


