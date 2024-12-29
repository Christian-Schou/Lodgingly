using System.Collections.Concurrent;
using System.Reflection;
using Lodgingly.Framework.Application.Messaging.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Lodgingly.Framework.Infrastructure.Messaging.Outbox;

public static class DomainEventHandlersFactory
{ 
    private static readonly ConcurrentDictionary<string, Type[]> HandlersDictionary = new();

    public static IEnumerable<IDomainEventHandler> GetHandlers(
        Type type,
        IServiceProvider serviceProvider,
        Assembly assembly)
    {
        Type[] domainEventHandlerTypes = HandlersDictionary.GetOrAdd(
            $"{assembly.GetName().Name}-{type.Name}",
            _ =>
            {
                Type[] domainEventHandlerTypes = assembly.GetTypes()
                    .Where(x => x.IsAssignableTo(typeof(IDomainEventHandler<>).MakeGenericType(type)))
                    .ToArray();

                return domainEventHandlerTypes;
            });

        List<IDomainEventHandler> handlers = [];
        
        foreach (Type domainEventHandlerType in domainEventHandlerTypes)
        {
            object domainEventHandler = serviceProvider.GetRequiredService(domainEventHandlerType);
            
            handlers.Add((domainEventHandler as IDomainEventHandler)!);
        }

        return handlers;
    }
}