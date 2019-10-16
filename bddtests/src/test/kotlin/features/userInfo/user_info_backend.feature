@userinfo
@backend
Feature: User Info Backend

  Scenario: An api user can submit their details to the user info endpoint
    Given I am an api user wishing to submit their details to the user info endpoint
    And I have logged in and have a valid session cookie
    When I post to the user info endpoint
    Then I receive a "Created" success code
    And my details are available in the user info repository

  Scenario: An api user posting to the user info endpoint without an access token will receive a 401
    Given I am an api user wishing to submit their details to the user info endpoint
    And I have logged in and have a valid session cookie
    When I post to the user info endpoint without an access token
    Then I receive an "Unauthorized" error

  Scenario: An api user posting to the user info endpoint with an invalid access token will receive a 401
    Given I am an api user wishing to submit their details to the user info endpoint
    And I have logged in and have a valid session cookie
    Then posting to the user info endpoint with an invalid access token will return an Unauthorised error

  Scenario: An api user can get their information from the user info endpoint
    Given I am an api user with stored details wishing to get my details
    And I have logged in and have a valid session cookie
    When I get user info details from the user info endpoint
    Then I receive an "OK" success code
    And I receive my details from the user info endpoint

  Scenario: An api user getting their information from the user info endpoint without an access token will receive a 401
    Given I am an api user with stored details wishing to get my details
    And I have logged in and have a valid session cookie
    When I get user info details from the user info endpoint without an access token
    Then I receive an "Unauthorized" error

  Scenario: An api user getting their information from the user info endpoint with an invalid access token will receive a 401
    Given I am an api user with stored details wishing to get my details
    And I have logged in and have a valid session cookie
    Then getting details from the user info endpoint with an invalid access token will return an Unauthorised error

  Scenario: An api user that has no information stored getting their details from the user info endpoint will receive a 404
    Given I am an api user without stored details wishing to get my details
    And I have logged in and have a valid session cookie
    When I get user info details from the user info endpoint
    Then I receive a "Not Found" error

  Scenario: An api user can retrieve a list of NhsLoginIds that are linked to a given ods code
    Given I am an api user wishing to get a list of NhsLoginIds that are linked to a given ods code
    When I get user info details based on an ods code
    Then I receive an "OK" success code
    And I receive a list of NhsLoginIds from user info endpoint

  Scenario: An api user getting NhsLoginIds on the user info endpoint using an ods code with no linked records will receive an empty list
    Given I am an api user wishing to get NhsLoginIds, but the ods code I am using is not linked to any records
    When I get user info details based on an ods code
    Then I receive an "OK" success code
    And I receive an empty list of NhsLoginIds from user info endpoint

  Scenario: An api user can retrieve a list of NhsLoginIds that are linked to a given nhs number
    Given I am an api user wishing to get a list of NhsLoginIds that are linked to a given nhs number
    When I get user info details based on an nhs number
    Then I receive an "OK" success code
    And I receive a list of NhsLoginIds from user info endpoint

  Scenario: An api user getting NhsLoginIds on the user info endpoint using an nhs number with no linked records will receive an empty list
    Given I am an api user wishing to get NhsLoginIds, but the nhs number I am using is not linked to any records
    When I get user info details based on an nhs number
    Then I receive an "OK" success code
    And I receive an empty list of NhsLoginIds from user info endpoint

  Scenario: An api user getting NhsLoginIds on the user info endpoint without ods code or nhs number will receive a 400
    Given I am an api user wishing to get a list of NhsLoginIds
    When I get user info details without ods code or nhs number
    Then I receive a "Bad Request" error

  Scenario: An api user getting NhsLoginIds on the user info endpoint with both ods code and nhs number will receive a 400
    Given I am an api user wishing to get a list of NhsLoginIds
    When I get user info details with ods code and nhs number
    Then I receive a "Bad Request" error