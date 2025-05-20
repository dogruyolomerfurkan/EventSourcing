using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WebApi.Database;
using WebApi.Entity;
using WebApi.Events;

namespace WebApi.Repositories;

public class EventRepository(EventDbContext eventDbContext) : IEventRepository
{
    private readonly static JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<List<IEvent>> GetByAggregateIdAsync(Guid id)
    {
        var entries = await eventDbContext.DocumentEvents
            .Where(x => x.AggregateId == id)
            .OrderBy(x => x.Version)
            .ToListAsync();

        var events = entries
            .Select(x =>
            {
                var eventType = Type.GetType(x.EventType) ?? throw new InvalidOperationException($"Event type '{x.EventType}' could not be resolved.");
                var deserializedEvent = JsonSerializer.Deserialize(x.Data, eventType, _jsonOptions);
                if (deserializedEvent is not IEvent @event)
                {
                    throw new InvalidOperationException($"Deserialized event is not of type IEvent. EventType: {x.EventType}");
                }

                return @event;
            })
            .ToList();

        return events;
    }

    public async Task SaveAsync(IEnumerable<IEvent> events, int expectedVersion)
    {
        var existingEvents = await eventDbContext.DocumentEvents
            .Where(x => x.AggregateId == events.First().AggregateId)
            .OrderBy(x => x.Version)
            .ToListAsync();

        if (existingEvents.Count != expectedVersion)
            throw new Exception("Concurrency violation");

        int version = expectedVersion;

        foreach (var e in events)
        {
            version++;
            var documentEvent = new DocumentEvent
            {
                AggregateId = e.AggregateId,
                Version = version,
                EventType = e.GetType().AssemblyQualifiedName ?? throw new InvalidOperationException("Event type could not be resolved."),
                Data = JsonSerializer.Serialize(e, e.GetType(), _jsonOptions),
                CreatedAt = e.CreatedAt
            };
            eventDbContext.DocumentEvents.Add(documentEvent);
        }

        await eventDbContext.SaveChangesAsync();
    }
}
