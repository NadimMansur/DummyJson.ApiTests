using DummyJson.ApiTests.Support;
using NUnit.Framework;
using Reqnroll;

namespace DummyJson.ApiTests.StepDefinitions;

[Binding]
public class CommonSteps(ApiContext ctx)
{
    [Then("the response status code should be {int}")]
    public void StatusCode(int expected) =>
        Assert.That((int)ctx.Response!.StatusCode, Is.EqualTo(expected), ctx.Response.Content);
}