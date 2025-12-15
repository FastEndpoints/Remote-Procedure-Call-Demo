using Contracts;
using FastEndpoints.Messaging.Remote.Testing;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace Test;

public class TestFixture : IDisposable
{
    public HttpClient StoreFrontClient { get; set; }

    readonly WebApplicationFactory<Warehouse.Program> _warehouse = new();   //this is the grpc server that hosts command handlers
    readonly WebApplicationFactory<StoreFront.Program> _storefront = new(); //this is the client app that initiates command executions

    public TestFixture()
    {
        //get a reference to the TestServer of the grpc handler server/app
        var warehouse = _warehouse.WithWebHostBuilder(
            c =>
            {
                c.ConfigureTestServices(
                    s =>
                    {
                        //fake command handlers can be registered for commands
                        s.RegisterTestCommandHandler<SayHelloCommand, TestCommandHandler>();
                    });
            }).Server;

        //create and store a httpclient for calling endpoints on the client app
        StoreFrontClient = _storefront.WithWebHostBuilder(
            c =>
            {
                c.ConfigureTestServices(
                    s =>
                    {
                        //connect the test remote/grpc server to the client app
                        s.RegisterTestRemote(warehouse);
                    });
            }).CreateClient();
    }

    public void Dispose()
    {
        StoreFrontClient.Dispose();
        _warehouse.Dispose();
        _storefront.Dispose();
        GC.SuppressFinalize(this);
    }
}