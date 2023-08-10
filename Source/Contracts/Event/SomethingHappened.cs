using FastEndpoints;

namespace Contracts;

public sealed class SomethingHappened : IEvent
{
    public int Id { get; set; }
    public string Description { get; set; }
}
