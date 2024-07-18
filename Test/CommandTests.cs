using System.Net;

namespace Test;

public class CommandTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    public readonly HttpClient StoreFrontClient = fixture.StoreFrontClient;

    [Fact]
    public async Task Void_Command_Handler_Is_Executed()
    {
        var rsp = await StoreFrontClient.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        Assert.True(await TestCommandHandler.IsTestPassed());
    }

    [Fact]
    public async Task Unary_Command_Handler_Is_Executed()
    {
        var res = await StoreFrontClient.GetStringAsync("/123");

        Assert.Equal("\"Result from remote handler: Order 123 created for Holly Simms\"", res);
    }
}