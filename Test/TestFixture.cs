using Contracts;
using FastEndpoints.Messaging.Remote.Testing;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Warehouse;

namespace Test;

public class TestFixture : IDisposable
{
    public HttpClient StoreFrontClient { get; set; }

    readonly WebApplicationFactory<Program> _warehouse = new();
    readonly WebApplicationFactory<StoreFront.Program> _storefront = new();

    public TestFixture()
    {
        var warehouse = _warehouse.WithWebHostBuilder(
            c =>
            {
                c.ConfigureTestServices(
                    s =>
                    {
                        s.RegisterTestCommandHandler<SayHelloCommand, TestCommandHandler>();
                    });
            }).Server;

        StoreFrontClient = _storefront.WithWebHostBuilder(
            c =>
            {
                c.ConfigureTestServices(
                    s =>
                    {
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