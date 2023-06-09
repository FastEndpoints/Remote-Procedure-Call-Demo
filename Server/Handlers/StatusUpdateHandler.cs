using Contracts;
using FastEndpoints;
using System.Runtime.CompilerServices;

namespace Warehouse;

public sealed class StatusUpdateHandler : IServerStreamCommandHandler<StatusStreamCommand, StatusUpdate>
{
    public async IAsyncEnumerable<StatusUpdate> ExecuteAsync(StatusStreamCommand command, [EnumeratorCancellation] CancellationToken ct)
    {
        for (int i = 1; !ct.IsCancellationRequested; i++)
        {
            try
            {
                await Task.Delay(1000, ct);
            }
            catch (TaskCanceledException)
            {
                //do nothing
            }
            yield return new() { Message = $"Id: {command.Id} - {i}" };
        }
    }
}
