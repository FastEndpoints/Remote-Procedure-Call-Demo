﻿using Contracts;
using FastEndpoints;
using Grpc.Core;
using System.Runtime.CompilerServices;

var bld = WebApplication.CreateBuilder();
var app = bld.Build();
app.MapRemoteHandlers("http://localhost:6000", c =>
{
    c.Register<CreateOrderCommand, CreateOrderResult>();
    c.RegisterServerStream<StatusStreamCommand, StatusUpdate>();
    c.RegisterClientStream<CurrentPosition, ProgressReport>();
});

//UNARY TEST
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

//SERVER STREAM TEST
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
        ctx.Response.ContentType = "application/json"; //just so the web browser will render the chunks
        await ctx.Response.StartAsync();

        await foreach (var res in iterator)
            await ctx.Response.WriteAsync(res.Message + Environment.NewLine + Environment.NewLine);
    }
    catch (OperationCanceledException) { }
    catch (RpcException) { }
});

//CLIENT STREAM TEST
app.MapGet("/client-stream", async (CancellationToken ct) =>
{
    var report = await GetDataStream(ct)
             .RemoteExecuteAsync<CurrentPosition, ProgressReport>(new(cancellationToken: ct));

    return Results.Ok(report);

    static async IAsyncEnumerable<CurrentPosition> GetDataStream([EnumeratorCancellation] CancellationToken ct)
    {
        var i = 0;
        while (!ct.IsCancellationRequested && i < 5)
        {
            i++;
            yield return new() { Number = i };
            await Task.Delay(1000, ct);
        }
    }
});

app.Run();
