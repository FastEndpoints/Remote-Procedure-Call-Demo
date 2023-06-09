using Contracts;
using FastEndpoints;
using Grpc.Core;

var bld = WebApplication.CreateBuilder();
var app = bld.Build();
app.MapRemoteHandlers("http://localhost:6000", c =>
{
    c.Register<CreateOrderCommand, CreateOrderResult>();
    c.RegisterServerStream<StatusStreamCommand, StatusUpdate>();
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

app.MapGet("/server-stream/{id}", async (int id, HttpContext ctx) =>
{
    try
    {
        var iterator = new StatusStreamCommand
        {
            Id = id,
        }
        .RemoteExecuteAsync(new(cancellationToken: new CancellationTokenSource(5000).Token));

        ctx.Response.StatusCode = 200;
        ctx.Response.ContentType = "application/json";
        await ctx.Response.StartAsync();

        await foreach (var res in iterator)
            await ctx.Response.WriteAsync(res.Message + Environment.NewLine + Environment.NewLine);
    }
    catch (OperationCanceledException) { }
    catch (RpcException) { }
});

app.Run();
