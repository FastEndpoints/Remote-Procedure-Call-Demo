using Contracts;
using FastEndpoints;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Warehouse;

var bld = WebApplication.CreateBuilder();
bld.WebHost.ConfigureKestrel(o => o.ListenLocalhost(6000, o => o.Protocols = HttpProtocols.Http2));
bld.AddHandlerServer();

var app = bld.Build();
app.MapHandlers(h =>
{
    h.Register<CreateOrderCommand, CreateOrderHandler, CreateOrderResult>();
    h.RegisterServerStream<StatusStreamCommand, StatusUpdateHandler, StatusUpdate>();
    h.RegisterClientStream<CurrentPosition, PositionProgressHandler, ProgressReport>();
});
app.Run();
