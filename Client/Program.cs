using Contracts;
using FastEndpoints;

var bld = WebApplication.CreateBuilder();
var app = bld.Build();
app.MapRemoteHandlers("http://localhost:6000", c =>
{
    c.Register<CreateOrderCommand, CreateOrderResult>();
});

app.MapGet("/{id}", async (int id) =>
{
    var result = await new CreateOrderCommand
    {
        OrderId = id,
        CustomerName = "Holly Simms"
    }
    .RemoteExecuteAsync();

    return Results.Ok("Result from remote handler: " + result.Message);
});

app.Run();
