using Contracts;
using FastEndpoints;

namespace StoreFront;

internal class WhenSomethingHappens : IEventHandler<SomethingHappened>
{
    private readonly ILogger<WhenSomethingHappens> _logger;

    public WhenSomethingHappens(ILogger<WhenSomethingHappens> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(SomethingHappened evnt, CancellationToken ct)
    {
        _logger.LogInformation("{number} - {description}", evnt.Id, evnt.Description);
        return Task.CompletedTask;
    }
}