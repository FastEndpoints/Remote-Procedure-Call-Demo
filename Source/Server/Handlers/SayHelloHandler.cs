using Contracts;
using FastEndpoints;

namespace Warehouse;

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