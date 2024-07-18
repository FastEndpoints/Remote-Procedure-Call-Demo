using Contracts;
using FastEndpoints;

namespace Warehouse;

public sealed class CreateOrderHandler(ILogger<CreateOrderHandler> logger) : ICommandHandler<CreateOrderCommand, CreateOrderResult>
{
    public Task<CreateOrderResult> ExecuteAsync(CreateOrderCommand command, CancellationToken ct = default)
    {
        logger.LogInformation("Create Order Command Received!");

        return Task.FromResult(
            new CreateOrderResult
            {
                Message = $"Order {command.OrderId} created for {command.CustomerName}"
            });
    }
}