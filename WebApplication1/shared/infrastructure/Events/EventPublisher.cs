namespace WebApplication1.shared.infrastructure.Events;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;


/// <summary>
/// Publicador de eventos para comunicación entre módulos
/// </summary>
public class EventPublisher
{
    private readonly ConcurrentDictionary<string, List<Func<object, Task>>> _handlers = new();

    /// <summary>
    /// Suscribe un handler a un evento
    /// </summary>
    public void Subscribe<TEvent>(string eventName, Func<TEvent, Task> handler) where TEvent : class
    {
        if (!_handlers.ContainsKey(eventName))
        {
            _handlers[eventName] = new List<Func<object, Task>>();
        }

        // Convertir el handler tipado a un handler genérico
        Func<object, Task> genericHandler = async (eventObj) =>
        {
            if (eventObj is TEvent typedEvent)
            {
                await handler(typedEvent);
            }
        };

        _handlers[eventName].Add(genericHandler);
        Console.WriteLine($"[EventPublisher] Handler subscribed to event: {eventName}");
    }

    /// <summary>
    /// Publica un evento a todos los handlers suscritos
    /// </summary>
    public async Task PublishAsync<TEvent>(string eventName, TEvent @event)
    {
        if (!_handlers.TryGetValue(eventName, out var handlers) || handlers.Count == 0)
        {
            Console.WriteLine($"[EventPublisher] No handlers for event: {eventName}");
            return;
        }

        Console.WriteLine($"[EventPublisher] Publishing event: {eventName}");
        
        foreach (var handler in handlers)
        {
            try
            {
                await handler(@event!);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EventPublisher] Error in handler for {eventName}: {ex.Message}");
            }
        }
    }
}

/// <summary>
/// Singleton instance del EventPublisher
/// </summary>
public static class EventPublisherInstance
{
    private static readonly Lazy<EventPublisher> _instance = 
        new(() => new EventPublisher());

    public static EventPublisher Instance => _instance.Value;
}