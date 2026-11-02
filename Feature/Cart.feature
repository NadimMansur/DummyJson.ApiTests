Feature: Cart

  Scenario: Add 5 products to a cart and validate the discount percentage
    When I add a cart for user 1 with the following products
      | ProductId | Quantity |
      | 1         | 1        |
      | 2         | 2        |
      | 3         | 1        |
      | 4         | 3        |
      | 5         | 1        |
    Then the response status code should be 201
    And the cart should contain 5 products
    And the total discount percentage should be consistent with the cart totals

  Scenario: Delete a cart
    When I delete cart 1
    Then the response status code should be 200
    And the cart should be flagged as deleted