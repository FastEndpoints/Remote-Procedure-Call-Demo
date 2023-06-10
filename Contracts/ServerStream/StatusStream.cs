﻿using FastEndpoints;

namespace Contracts;

public class StatusStreamCommand : IServerStreamCommand<StatusUpdate>
{
    public int Id { get; set; }
}

public class StatusUpdate
{
    public string Message { get; set; }
}