# DummyJSON API Test Automation Framework

BDD-style API tests for https://dummyjson.com using C#, Reqnroll (Gherkin), NUnit and RestSharp.

## How to run
1. Open `DummyJson.ApiTests.sln` in Visual Studio
2. Build the solution (restores NuGet packages)
3. Open Test Explorer → Run All (9 scenarios)

## Structure
- `Features/` – Gherkin scenarios, one file per resource (Auth, Users, Products, Cart)
- `StepDef/` – step bindings, grouped by resource
- `Support/ApiContext.cs` – shared RestClient, last response and stored token

## Design decisions
- **Reqnroll over SpecFlow**: SpecFlow is end-of-life; Reqnroll is its maintained successor.
- **RestSharp**: concise request building and built-in JSON handling.
- **JsonNode instead of full DTOs**: keeps assertions readable and tolerant of unrelated fields.
- **Token reuse**: stored in a static field after the login scenario; the "I am authenticated" step logs in if absent so scenarios can run independently.
- **Stateless API**: write operations (POST/PUT/DELETE) are simulated, so tests assert on the response
  payload and never expect changes to persist.
- **Red hair count**: `users/filter` with `limit=0` returns all matches; the test checks that `total`
  equals the number of returned users and that every user is red-haired, rather than hard-coding a count.
- **15th product**: `limit=1&skip=14&select=id,title,price`, cross-checked against `/products/15`.
- **Discount percentage**: derived from `total` and `discountedTotal`, and compared with the percentage
  rebuilt from each line's `discountPercentage` (tolerance 0.5).

## Use of AI
In line with the challenge's responsible-AI guidance: I used an AI assistant (Claude by Anthropic) to
help scaffold the project structure and draft initial feature files and step definitions.
I reviewed, ran and modified all generated code, verified the expected values against the live API,
and I can explain every part of the solution.
