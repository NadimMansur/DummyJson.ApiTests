Feature: Auth

  Scenario: Authenticate the user and store the access token
    Given I log in with username "emilys" and password "emilyspass"
    Then the response status code should be 200
    And an access token should be stored

  Scenario: Validate the current authenticated user
    Given I am authenticated
    When I request the current user
    Then the response status code should be 200
    And the user id should be 1
    And the user first name should be "Emily"
    And the user last name should be "Johnson"