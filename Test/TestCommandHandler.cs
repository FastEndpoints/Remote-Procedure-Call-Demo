using Contracts;
using FastEndpoints;

namespace Test;

sealed class TestCommandHandler : ICommandHandler<SayHelloCommand>
{
    internal static SayHelloCommand? Received;

    public Task ExecuteAsync(SayHelloCommand c, CancellationToken ct)
    {
        Received = c;

        return Task.CompletedTask;
    }

    public static async Task<bool> IsTestPassed()
    {
        while (Received is null)
            await Task.Delay(100);

        return Received.From == "mars";
    }
}