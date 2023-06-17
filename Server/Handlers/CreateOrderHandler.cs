using Contracts;
using FastEndpoints;

namespace Warehouse;

public sealed class CreateOrderHandler : ICommandHandler<CreateOrderCommand, CreateOrderResult>
{
    private readonly ILogger<CreateOrderHandler> logger;

    public CreateOrderHandler(ILogger<CreateOrderHandler> logger)
    {
        this.logger = logger;
    }

    public Task<CreateOrderResult> ExecuteAsync(CreateOrderCommand command, CancellationToken ct = default)
    {
        logger.LogInformation("Create Order Command Received!");

        return Task.FromResult(new CreateOrderResult()
        {
            Message = $"Order {command.OrderId} created for {command.CustomerName}"
        });
    }
}

public sealed class SayHelloHandler : ICommandHandler<SayHelloCommand>
{
    private readonly ILogger<SayHelloHandler> logger;

    public SayHelloHandler(ILogger<SayHelloHandler> logger)
    {
        this.logger = logger;
    }

    public Task ExecuteAsync(SayHelloCommand command, CancellationToken ct)
    {
        logger.LogInformation("Hello from {from}", command.From);
        return Task.CompletedTask;
    }
}