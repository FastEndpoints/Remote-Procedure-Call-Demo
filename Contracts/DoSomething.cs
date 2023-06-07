using FastEndpoints;

namespace Contracts;

public class DoSomethingCommand : ICommand
{
    public string Message { get; set; }
}