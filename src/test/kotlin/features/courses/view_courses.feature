Feature: View courses
In order to view courses associated with a user
As a logged in user
I want to see a list of repeat courses that I can order

Background:
Given wiremock is initialised

@backend
@bug
Scenario: Requesting courses with correct data returns a list of repeat courses that can be requested
Given I have logged in and have a valid session cookie
And I have 10 previous courses but only 5 of type repeat of which 2 can be requested
When I get the users courses with a valid cookie
Then I receive a list of 2 repeating courses that can be requested
