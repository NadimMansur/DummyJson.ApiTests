Feature: Users

  Scenario: Count users with red hair
    When I request all users with hair colour "Red"
    Then the response status code should be 200
    And the reported total should match the number of users returned
    And every returned user should have hair colour "Red"

  Scenario: Add a new user
    When I add a new user with the following details
      | FirstName | LastName | Age | Gender | HairColor | HairType | Address       | City   | State | StateCode | PostalCode | Country       |
      | Jane      | Tester   | 34  | female | Black     | Curly    | 1 High Street | London | Surrey | SR       | CR0 1AA    | United Kingdom |
    Then the response status code should be 201
    And the created user should echo the submitted details