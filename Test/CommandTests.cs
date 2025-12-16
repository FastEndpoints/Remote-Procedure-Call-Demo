using System.Net;
using Contracts;

namespace Test;

public class CommandTests(TestFixture fixture) : IClassFixture<TestFixture>
{
    readonly HttpClient _storeFrontClient = fixture.StoreFrontClient;
    CancellationToken _ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Fake_Command_Handler_Is_Executed()
    {
        //this test verifies that the handler server executes a fake handler for a given command instead of the real command handler.
        //see fixture setup for registration of the fake handler

        var rsp = await _storeFrontClient.GetAsync("/", _ct); //this endpoint executes a SayHelloCommand

        Assert.Equal(HttpStatusCode.OK, rsp.StatusCode);
        Assert.True(await FakeCommandHandler.IsTestPassed());
    }

    [Fact]
    public async Task Real_Command_Handler_Is_Executed()
    {
        //this test verifies that hitting the /{id} route executes a CreateOrderCommand which executes it's real command handler
        //and asserts that everything went as planned by checking the response of the endpoint.

        var res = await _storeFrontClient.GetStringAsync("/123", _ct);

        Assert.Equal("\"Result from remote handler: Order 123 created for Holly Simms\"", res);
    }

    [Fact]
    public async Task Endpoint_Issues_A_CreateOrderCommand_With_Given_Id()
    {
        //this test verifies that hitting the /{id} route executes a CreateOrderCommand with Id 321.
        //in this test we only care that the endpoint executed the correct command and nothing else.

        await _storeFrontClient.GetStringAsync("/321", _ct);

        //obtain a test command receiver via the fixture
        var cmdReceiver = fixture.GetCommandReceiverForWarehouse<CreateOrderCommand>();

        //wait until the correct command is received by the handler server with a timeout period of 5 seconds.
        var received = await cmdReceiver.WaitForMatchAsync(c => c.OrderId == 321, timeoutSeconds: 5, _ct);

        //verify a command with order id 321 was actually received.
        Assert.Contains(received, c => c.OrderId == 321);
    }
}