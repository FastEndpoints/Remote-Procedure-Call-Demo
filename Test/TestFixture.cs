using Contracts;
using FastEndpoints.Messaging.Remote.Testing;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace Test;

public class TestFixture : IDisposable
{
    public HttpClient StoreFrontClient { get; set; }

    private readonly WebApplicationFactory<Warehouse.Program> _warehouse = new();
    private readonly WebApplicationFactory<StoreFront.Program> _rpcClient = new();

    public TestFixture()
    {
        var warehouse = _warehouse.WithWebHostBuilder(c =>
        {
            c.ConfigureTestServices(s =>
            {
                s.RegisterTestCommandHandler<SayHelloCommand, TestCommandHandler>();
            });
        }).Server;

        StoreFrontClient = _rpcClient.WithWebHostBuilder(c =>
        {
            c.ConfigureTestServices(s =>
            {
                s.RegisterTestRemote(warehouse);
            });
        }).CreateClient();
    }

    #region disposable
    private bool disposedValue;
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                StoreFrontClient.Dispose();
                _warehouse.Dispose();
                _rpcClient.Dispose();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}