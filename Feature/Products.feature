Feature: Products

  Scenario: Verify the price of iPhone 13 Pro
    When I search products for "iPhone 13 Pro"
    Then the response status code should be 200
    And the product "iPhone 13 Pro" should be found with price 1099.99

  Scenario: Retrieve the 15th product using pagination
    When I request page item number 15 with only id, title and price
    Then the response status code should be 200
    And the response should contain exactly 1 product
    And the product id should be 15
    And the product fields should match the product details endpoint
    And the product should only contain the fields "id,title,price"

  Scenario: Update the title of product 121
    When I update the title of product 121 to "Updated Test Title"
    Then the response status code should be 200
    And the response product title should be "Updated Test Title"