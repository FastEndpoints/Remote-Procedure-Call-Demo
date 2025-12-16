using Contracts;
using FastEndpoints;
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
        _warehouse = _warehouse.WithWebHostBuilder(
            c =>
            {
                c.ConfigureTestServices(
                    s =>
                    {
                        s.RegisterTestCommandHandler<SayHelloCommand, FakeCommandHandler>(); //fake command handlers can be registered for commands
                        s.RegisterTestCommandReceivers();                                    //enables to verify a certain command was received by the handler server
                    });
            });

        _storefront = _storefront.WithWebHostBuilder(
            c =>
            {
                c.ConfigureTestServices(
                    s =>
                    {
                        s.RegisterTestRemote(_warehouse.Server); //connect the test remote/grpc server to the client app
                    });
            });

        //create and store a httpclient for calling endpoints on the client app
        StoreFrontClient = _storefront.CreateClient();
    }

    public ICommandReceiver<TCommand> GetCommandReceiverForWarehouse<TCommand>() where TCommand : ICommandBase
        => _warehouse.Services.GetTestCommandReceiver<TCommand>(); //a command receiver for a given command type can be obtained like this.

    public void Dispose()
    {
        _warehouse.Dispose();
        _storefront.Dispose();
        GC.SuppressFinalize(this);
    }
}