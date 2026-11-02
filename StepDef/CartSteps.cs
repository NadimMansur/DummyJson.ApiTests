using System.Text.Json.Nodes;
using DummyJson.ApiTests.Support;
using NUnit.Framework;
using Reqnroll;
using RestSharp;

namespace DummyJson.ApiTests.StepDefinitions;

[Binding]
public class CartSteps(ApiContext ctx)
{
    private JsonNode Json => JsonNode.Parse(ctx.Response!.Content!)!;

    [When("I add a cart for user {int} with the following products")]
    public async Task AddCart(int userId, DataTable table)
    {
        var products = table.Rows.Select(r => new
        {
            id = int.Parse(r["ProductId"]),
            quantity = int.Parse(r["Quantity"])
        }).ToList();

        ctx.Response = await ctx.Client.ExecuteAsync(
            new RestRequest("carts/add", Method.Post).AddJsonBody(new { userId, products }));
    }

    [Then("the cart should contain {int} products")]
    public void ProductCount(int n) =>
        Assert.That(Json["products"]!.AsArray().Count, Is.EqualTo(n));

    [Then("the total discount percentage should be consistent with the cart totals")]
    public void DiscountPercentage()
    {
        var j = Json;
        var total = j["total"]!.GetValue<double>();
        var discounted = j["discountedTotal"]!.GetValue<double>();
        var lines = j["products"]!.AsArray();

        var sumLineTotal = lines.Sum(p => p!["total"]!.GetValue<double>());
        var sumLineDiscounted = lines.Sum(p => p!["discountedPrice"]!.GetValue<double>());

        var overallPct = Math.Round((total - discounted) / total * 100, 2);
        TestContext.Out.WriteLine($"Total discount percentage: {overallPct}%");

        Assert.Multiple(() =>
        {
            Assert.That(total, Is.EqualTo(sumLineTotal).Within(0.01), "cart total vs sum of line totals");
            Assert.That(discounted, Is.EqualTo(sumLineDiscounted).Within(0.01), "cart discountedTotal vs sum of line discountedPrice");
            Assert.That(overallPct, Is.GreaterThan(0).And.LessThan(100));
        });
    }

    [When("I delete cart {int}")]
    public async Task DeleteCart(int id) =>
        ctx.Response = await ctx.Client.ExecuteAsync(new RestRequest($"carts/{id}", Method.Delete));

    [Then("the cart should be flagged as deleted")]
    public void Deleted()
    {
        var j = Json;
        Assert.Multiple(() =>
        {
            Assert.That(j["isDeleted"]!.GetValue<bool>(), Is.True);
            Assert.That(j["deletedOn"], Is.Not.Null);
        });
    }
}