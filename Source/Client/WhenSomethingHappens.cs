using Contracts;
using FastEndpoints;

namespace StoreFront;

class WhenSomethingHappens(ILogger<WhenSomethingHappens> logger) : IEventHandler<SomethingHappened>
{
    public Task HandleAsync(SomethingHappened evnt, CancellationToken ct)
    {
        logger.LogInformation("{number} - {description}", evnt.Id, evnt.Description);

        return Task.CompletedTask;
    }
}