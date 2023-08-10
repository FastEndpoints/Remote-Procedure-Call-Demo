﻿using FastEndpoints;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Test;

/// <summary>
/// WAF extension methods of integration testing gRPC event queues
/// </summary>
public static class WAFExtensions
{
    /// <summary>
    /// enables communicating with a remote gRPC server in the WAF testing environment
    /// </summary>
    /// <param name="remote">the <see cref="TestServer"/> of the target <see cref="WebApplicationFactory{TEntryPoint}"/></param>
    public static void RegisterTestRemote(this IServiceCollection s, TestServer remote)
        => s.AddSingleton(remote.CreateHandler());

    /// <summary>
    /// register test/fake/mock event handlers for integration testing gRPC event queues
    /// </summary>
    /// <typeparam name="TEvent">the type of the event model to register a test handler for</typeparam>
    /// <typeparam name="THandler">the type of the test event handler</typeparam>
    public static void RegisterTestEventHandler<TEvent, THandler>(this IServiceCollection s)
        where TEvent : IEvent
        where THandler : class, IEventHandler<TEvent>
    {
        s.AddTransient<IEventHandler<TEvent>, THandler>();
    }

    /// <summary>
    /// register test/fake/mock command handlers for integration testing gRPC commands
    /// </summary>
    /// <typeparam name="TCommand">the type of the command model to register a test handler for</typeparam>
    /// <typeparam name="THandler">the type of the test command handler</typeparam>
    public static void RegisterTestCommandHandler<TCommand, THandler>(this IServiceCollection s)
        where TCommand : ICommand
        where THandler : class, ICommandHandler<TCommand>
    {
        s.AddTransient<ICommandHandler<TCommand>, THandler>();
    }
}