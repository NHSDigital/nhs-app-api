Feature: EMIS Patient Registration Validation
    In order to allow users login to NHS online app
    As a Citizen ID User
    I want to be able to register an EMIS user's IM1 credentials

    Scenario: Registering an EMIS user's IM1 credentials with an ODS Code not in the expected format returns a "Bad Request" error message
        Given I have an EMIS user's IM1 credentials with an ODS Code not in the expected format
        When I register an EMIS user's IM1 credentials
        Then I receive a "Bad Request" error

    @bug
    @wip
    Scenario:Registering an EMIS user's IM1 credentials with a Surname not in the expected format returns a "Bad Request" error message
        Given I have an EMIS user's IM1 credentials with a Surname not in the expected format
        When I register an EMIS user's IM1 credentials
        Then I receive a "Bad Request" error

    @bug
    @wip
    Scenario: Registering an EMIS user's IM1 credentials with an Account ID not in the expected format returns a "Bad Request" error message
        Given I have an EMIS user's IM1 credentials with an Account ID not in the expected format
        When I register an EMIS user's IM1 credentials
        Then I receive a "Bad Request" error

    @bug
    @wip
    Scenario:Registering an EMIS user's IM1 credentials with a Linkage Key not in the expected format returns a "Bad Request" error message
        Given I have an EMIS user's IM1 credentials with a Linkage Key not in the expected format
        When I register an EMIS user's IM1 credentials
        Then I receive a "Bad Request" error

    Scenario:Registering an EMIS user's IM1 credentials with a Date Of Birth not in the expected format returns a "Bad Request" error message
        Given I have an EMIS user's IM1 credentials with a Date Of Birth not in the expected format
        When I register an EMIS user's IM1 credentials
        Then I receive a "Bad Request" error

    Scenario: Registering an EMIS user's IM1 credentials with missing ODS Code returns a "Bad Request" error message
        Given I have an EMIS user's IM1 credentials with missing ODS Code
        When I register an EMIS user's IM1 credentials
        Then I receive a "Bad Request" error

    Scenario:Registering an EMIS user's IM1 credentials with missing Surname returns a "Bad Request" error message
        Given I have an EMIS user's IM1 credentials with missing Surname
        When I register an EMIS user's IM1 credentials
        Then I receive a "Bad Request" error

    Scenario: Registering an EMIS user's IM1 credentials with missing Account ID returns a "Bad Request" error message
        Given I have an EMIS user's IM1 credentials with missing Account ID
        When I register an EMIS user's IM1 credentials
        Then I receive a "Bad Request" error

    Scenario:Registering an EMIS user's IM1 credentials with missing Linkage Key returns a "Bad Request" error message
        Given I have an EMIS user's IM1 credentials with missing Linkage Key
        When I register an EMIS user's IM1 credentials
        Then I receive a "Bad Request" error
