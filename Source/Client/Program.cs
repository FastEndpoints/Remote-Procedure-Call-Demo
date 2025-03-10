﻿using System.Runtime.CompilerServices;
using Contracts;
using FastEndpoints;
using Grpc.Core;
using StoreFront;

var bld = WebApplication.CreateBuilder();
var app = bld.Build();

app.MapRemote(
    "WAREHOUSE", // <- unix socket. for tcp use: http://localhost:6000
    c =>
    {
        c.Register<SayHelloCommand>();
        c.Register<CreateOrderCommand, CreateOrderResult>();
        c.RegisterServerStream<StatusStreamCommand, StatusUpdate>();
        c.RegisterClientStream<CurrentPosition, ProgressReport>();
        c.Subscribe<SomethingHappened, WhenSomethingHappens>();
    });

//VOID TEST
app.MapGet(
    "/",
    async () =>
    {
        await new SayHelloCommand
            {
                From = "mars"
            }
            .RemoteExecuteAsync();

        return Results.Ok();
    });

//UNARY TEST
app.MapGet(
    "/{id}",
    async (int id) =>
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
app.MapGet(
    "/server-stream/{id}",
    async (int id, HttpContext ctx) =>
    {
        try
        {
            var results = new StatusStreamCommand
                {
                    Id = id
                }
                .RemoteExecuteAsync(new CancellationTokenSource(5000).Token);

            ctx.Response.StatusCode = 200;
            ctx.Response.ContentType = "application/json"; //just so the web browser will render the chunks
            await ctx.Response.StartAsync();

            await foreach (var res in results)
                await ctx.Response.WriteAsync(res.Message + Environment.NewLine + Environment.NewLine);
        }
        catch (OperationCanceledException) { }
        catch (RpcException) { }
    });

//CLIENT STREAM TEST
app.MapGet(
    "/client-stream",
    async (IHostApplicationLifetime appLife, CancellationToken ct) =>
    {
        ProgressReport report;
        var cts = CancellationTokenSource.CreateLinkedTokenSource(appLife.ApplicationStopping, ct);

        try
        {
            report = await GetDataStream(cts.Token)
                         .RemoteExecuteAsync<CurrentPosition, ProgressReport>(cts.Token);
        }
        catch (RpcException)
        {
            return Results.Ok("server cancelled!");
        }
        catch (OperationCanceledException)
        {
            return Results.Ok("client cancelled!");
        }

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

namespace StoreFront
{
    public sealed class Program;
}