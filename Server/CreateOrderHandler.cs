using Contracts;
using FastEndpoints;
using Microsoft.Extensions.Logging;

namespace Warehouse
{
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
}