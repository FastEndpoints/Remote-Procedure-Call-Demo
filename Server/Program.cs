using Contracts;
using FastEndpoints;
using Microsoft.Extensions.Hosting;
using Warehouse;

var bld = new HostApplicationBuilder();
bld.Services.AddHandlerServer(s =>
{
    s.Host = "localhost";
    s.Port = 6000;
    s.MapHandler<CreateOrderCommand, CreateOrderHandler, CreateOrderResult>();
});

var app = bld.Build();
app.StartHandlerServer();
app.Run();
