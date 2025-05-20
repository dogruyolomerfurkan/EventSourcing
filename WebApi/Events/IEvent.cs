namespace WebApi.Events;

public interface IEvent
{
    Guid AggregateId { get; }
    DateTime CreatedAt { get; }
}
