@my-record
@vision-diagnosis
  Feature: View My Medical Record Information - Diagnosis

    Scenario Outline: A <Service> user can view diagnosis information
      Given the my record wiremocks are initialised for <Service>
      And the GP Practice has enabled demographics functionality for <Service>
      And the GP Practice has multiple diagnosis
      And I am on my record information page
      When I click the diagnosis section
      Then I see diagnosis information

      Examples:
        | Service |
        | VISION  |

    Scenario Outline: A <Service> user has no access to diagnosis section
      Given the my record wiremocks are initialised for <Service>
      And the GP Practice has enabled demographics functionality for <Service>
      And I do not have access to diagnosis
      And I am on my record information page
      When I click the diagnosis section
      Then I see a message indicating that I have no access to view Diagnosis on My Record

      Examples:
        | Service |
        | VISION  |

    Scenario Outline: An error occurs when trying to retrieve diagnosis data from <Service>
      Given the my record wiremocks are initialised for <Service>
      And the GP Practice has enabled demographics functionality for <Service>
      And an error occurred retrieving the diagnosis
      And I am on my record information page
      When I click the diagnosis section
      Then I see an error occurred message with Diagnosis on My Record

      Examples:
        | Service |
        | VISION  |