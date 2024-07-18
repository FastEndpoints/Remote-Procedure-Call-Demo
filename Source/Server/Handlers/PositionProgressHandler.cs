using Contracts;
using FastEndpoints;

namespace Warehouse;

public sealed class PositionProgressHandler(ILogger<PositionProgressHandler> logger) : IClientStreamCommandHandler<CurrentPosition, ProgressReport>
{
    public async Task<ProgressReport> ExecuteAsync(IAsyncEnumerable<CurrentPosition> stream, CancellationToken ct)
    {
        var currentNumber = 0;

        try
        {
            await foreach (var position in stream)
            {
                logger.LogInformation("Current number: {pos}", position.Number);
                currentNumber = position.Number;
            }
        }
        catch (IOException) { }
        catch (OperationCanceledException) { }

        return new() { LastNumber = currentNumber };
    }
}