using System.Text.Json.Nodes;
using DummyJson.ApiTests.Support;
using NUnit.Framework;
using Reqnroll;
using RestSharp;

namespace DummyJson.ApiTests.StepDefinitions;

[Binding]
public class ProductSteps(ApiContext ctx)
{
    private JsonNode Json => JsonNode.Parse(ctx.Response!.Content!)!;

    [When("I search products for {string}")]
    public async Task Search(string q) =>
        ctx.Response = await ctx.Client.ExecuteAsync(new RestRequest("products/search").AddQueryParameter("q", q));

    [Then("the product {string} should be found with price {double}")]
    public void PriceIs(string title, double price)
    {
        var match = Json["products"]!.AsArray()
            .FirstOrDefault(p => p!["title"]!.GetValue<string>() == title);
        Assert.That(match, Is.Not.Null, $"'{title}' not found");
        Assert.That(match!["price"]!.GetValue<double>(), Is.EqualTo(price).Within(0.001));
    }

    [When("I request page item number {int} with only id, title and price")]
    public async Task Paginate(int n)
    {
        var req = new RestRequest("products")
            .AddQueryParameter("limit", "1")
            .AddQueryParameter("skip", (n - 1).ToString())
            .AddQueryParameter("select", "id,title,price");
        ctx.Response = await ctx.Client.ExecuteAsync(req);
    }

    [Then("the response should contain exactly {int} product")]
    public void Count(int n) =>
        Assert.That(Json["products"]!.AsArray().Count, Is.EqualTo(n));

    [Then("the product id should be {int}")]
    public void ProductId(int id) =>
        Assert.That(Json["products"]![0]!["id"]!.GetValue<int>(), Is.EqualTo(id));

    [Then("the product fields should match the product details endpoint")]
    public async Task MatchesDetails()
    {
        var item = Json["products"]![0]!;
        var id = item["id"]!.GetValue<int>();
        var detail = JsonNode.Parse((await ctx.Client.ExecuteAsync(new RestRequest($"products/{id}"))).Content!)!;
        Assert.Multiple(() =>
        {
            Assert.That(item["title"]!.GetValue<string>(), Is.EqualTo(detail["title"]!.GetValue<string>()));
            Assert.That(item["price"]!.GetValue<double>(), Is.EqualTo(detail["price"]!.GetValue<double>()).Within(0.001));
        });
    }

    [Then("the product should only contain the fields {string}")]
    public void OnlyFields(string csv)
    {
        var expected = csv.Split(',').OrderBy(x => x);
        var actual = Json["products"]![0]!.AsObject().Select(kv => kv.Key).OrderBy(x => x);
        Assert.That(actual, Is.EqualTo(expected));
    }

    [When("I update the title of product {int} to {string}")]
    public async Task UpdateTitle(int id, string title) =>
        ctx.Response = await ctx.Client.ExecuteAsync(
            new RestRequest($"products/{id}", Method.Put).AddJsonBody(new { title }));

    [Then("the response product title should be {string}")]
    public void TitleIs(string title) =>
        Assert.That(Json["title"]!.GetValue<string>(), Is.EqualTo(title));
}