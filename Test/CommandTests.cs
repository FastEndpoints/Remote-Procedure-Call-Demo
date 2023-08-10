using System.Net;

namespace Test;

public class CommandTests : IClassFixture<TestFixture>
{
    public readonly HttpClient _storeFrontClient;

    public CommandTests(TestFixture fixture)
    {
        _storeFrontClient = fixture.StoreFrontClient;
    }

    [Fact]
    public async Task Void_Command_Handler_Is_Executed()
    {
        var rsp = await _storeFrontClient.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        Assert.True(await TestCommandHandler.IsTestPassed());
    }

    [Fact]
    public async Task Unary_Command_Handler_Is_Executed()
    {
        var res = await _storeFrontClient.GetStringAsync("/123");

        Assert.Equal("\"Result from remote handler: Order 123 created for Holly Simms\"", res);
    }
}