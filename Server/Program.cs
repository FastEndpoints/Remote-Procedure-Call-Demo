using Contracts;
using FastEndpoints;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Warehouse;

var bld = WebApplication.CreateBuilder();
bld.WebHost.ConfigureKestrel(o =>
{
    o.ListenLocalhost(6000, o => o.Protocols = HttpProtocols.Http2); // for GRPC
    o.ListenLocalhost(5001, o => o.Protocols = HttpProtocols.Http1AndHttp2); // for REST
});
bld.AddHandlerServer();

var app = bld.Build();
app.MapHandlers(h =>
{
    h.Register<SayHelloCommand, SayHelloHandler>();
    h.Register<CreateOrderCommand, CreateOrderHandler, CreateOrderResult>();
    h.RegisterServerStream<StatusStreamCommand, StatusUpdateHandler, StatusUpdate>();
    h.RegisterClientStream<CurrentPosition, PositionProgressHandler, ProgressReport>();
    h.RegisterEventHub<SomethingHappened>();
});

app.MapGet("/event/{name}", async (string name) =>
{
    for (int i = 1; i <= 100; i++)
    {
        new SomethingHappened
        {
            Id = i,
            Description = name
        }
        .Broadcast();

        await Task.Delay(1000);
    }
    return Results.Ok("events published!");
});

app.Run();
