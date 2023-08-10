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
    public async Task Published_Events_Are_Received_By_Subscriber()
    {
        var rsp = await _storeFrontClient.GetAsync("/");

        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        Assert.True(await TestCommandHandler.IsTestPassed());
    }
}