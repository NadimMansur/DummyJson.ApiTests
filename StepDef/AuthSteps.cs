using System.Text.Json.Nodes;
using DummyJson.ApiTests.Support;
using NUnit.Framework;
using Reqnroll;
using RestSharp;

namespace DummyJson.ApiTests.StepDefinitions;

[Binding]
public class AuthSteps(ApiContext ctx)
{
    [Given("I log in with username {string} and password {string}")]
    public async Task Login(string username, string password)
    {
        var req = new RestRequest("auth/login", Method.Post)
            .AddJsonBody(new { username, password, expiresInMins = 30 });
        ctx.Response = await ctx.Client.ExecuteAsync(req);
    }

    [Then("an access token should be stored")]
    public void TokenStored()
    {
        var token = JsonNode.Parse(ctx.Response!.Content!)!["accessToken"]?.GetValue<string>();
        Assert.That(token, Is.Not.Null.And.Not.Empty);
        ApiContext.AccessToken = token;
    }

    [Given("I am authenticated")]
    public async Task Authenticated()
    {
        if (!string.IsNullOrEmpty(ApiContext.AccessToken)) return;
        await Login("emilys", "emilyspass");
        TokenStored();
    }

    [When("I request the current user")]
    public async Task GetMe()
    {
        var req = new RestRequest("auth/me")
            .AddHeader("Authorization", $"Bearer {ApiContext.AccessToken}");
        ctx.Response = await ctx.Client.ExecuteAsync(req);
    }

    [Then("the user id should be {int}")]
    public void UserId(int id) =>
        Assert.That(JsonNode.Parse(ctx.Response!.Content!)!["id"]!.GetValue<int>(), Is.EqualTo(id));

    [Then("the user first name should be {string}")]
    public void FirstName(string name) =>
        Assert.That(JsonNode.Parse(ctx.Response!.Content!)!["firstName"]!.GetValue<string>(), Is.EqualTo(name));

    [Then("the user last name should be {string}")]
    public void LastName(string name) =>
        Assert.That(JsonNode.Parse(ctx.Response!.Content!)!["lastName"]!.GetValue<string>(), Is.EqualTo(name));
}