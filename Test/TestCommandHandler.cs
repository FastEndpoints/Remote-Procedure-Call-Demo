using Contracts;
using FastEndpoints;

namespace Test;

sealed class TestCommandHandler : ICommandHandler<SayHelloCommand>
{
    internal static SayHelloCommand? _received;

    public Task ExecuteAsync(SayHelloCommand c, CancellationToken ct)
    {
        _received = c;
        return Task.CompletedTask;
    }

    public static async Task<bool> IsTestPassed()
    {
        while (_received is null)
        {
            await Task.Delay(100);
        }
        return _received.From == "mars";
    }
}
