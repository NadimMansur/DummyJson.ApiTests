using System.Text.Json.Nodes;
using DummyJson.ApiTests.Support;
using NUnit.Framework;
using Reqnroll;
using RestSharp;

namespace DummyJson.ApiTests.StepDefinitions;

[Binding]
public class UserSteps(ApiContext ctx)
{
    [When("I request all users with hair colour {string}")]
    public async Task FilterByHair(string colour)
    {
        var req = new RestRequest("users/filter")
            .AddQueryParameter("key", "hair.color")
            .AddQueryParameter("value", colour)
            .AddQueryParameter("limit", "0");
        ctx.Response = await ctx.Client.ExecuteAsync(req);
    }

    [Then("the reported total should match the number of users returned")]
    public void TotalMatches()
    {
        var json = JsonNode.Parse(ctx.Response!.Content!)!;
        var total = json["total"]!.GetValue<int>();
        Assert.That(json["users"]!.AsArray().Count, Is.EqualTo(total));
        TestContext.Out.WriteLine($"Users with red hair: {total}");
    }

    [Then("every returned user should have hair colour {string}")]
    public void AllMatch(string colour)
    {
        var users = JsonNode.Parse(ctx.Response!.Content!)!["users"]!.AsArray();
        Assert.That(users, Is.Not.Empty);
        foreach (var u in users)
            Assert.That(u!["hair"]!["color"]!.GetValue<string>(), Is.EqualTo(colour));
    }

    [When("I add a new user with the following details")]
    public async Task AddUser(DataTable table)
    {
        var r = table.Rows[0];
        var body = new
        {
            firstName = r["FirstName"],
            lastName = r["LastName"],
            age = int.Parse(r["Age"]),
            gender = r["Gender"],
            hair = new { color = r["HairColor"], type = r["HairType"] },
            address = new
            {
                address = r["Address"],
                city = r["City"],
                state = r["State"],
                stateCode = r["StateCode"],
                postalCode = r["PostalCode"],
                country = r["Country"]
            }
        };
        ctx.Response = await ctx.Client.ExecuteAsync(new RestRequest("users/add", Method.Post).AddJsonBody(body));
        ScenarioStore.Table = r;
    }

    [Then("the created user should echo the submitted details")]
    public void Echo()
    {
        var r = ScenarioStore.Table!;
        var j = JsonNode.Parse(ctx.Response!.Content!)!;
        Assert.Multiple(() =>
        {
            Assert.That(j["id"], Is.Not.Null);
            Assert.That(j["firstName"]!.GetValue<string>(), Is.EqualTo(r["FirstName"]));
            Assert.That(j["lastName"]!.GetValue<string>(), Is.EqualTo(r["LastName"]));
            Assert.That(j["age"]!.GetValue<int>(), Is.EqualTo(int.Parse(r["Age"])));
            Assert.That(j["gender"]!.GetValue<string>(), Is.EqualTo(r["Gender"]));
            Assert.That(j["hair"]!["color"]!.GetValue<string>(), Is.EqualTo(r["HairColor"]));
            Assert.That(j["hair"]!["type"]!.GetValue<string>(), Is.EqualTo(r["HairType"]));
            Assert.That(j["address"]!["city"]!.GetValue<string>(), Is.EqualTo(r["City"]));
            Assert.That(j["address"]!["postalCode"]!.GetValue<string>(), Is.EqualTo(r["PostalCode"]));
            Assert.That(j["address"]!["country"]!.GetValue<string>(), Is.EqualTo(r["Country"]));
        });
    }
}

internal static class ScenarioStore
{
    [ThreadStatic] public static DataTableRow? Table;
}