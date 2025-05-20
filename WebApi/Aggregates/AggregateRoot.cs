using WebApi.Events;

namespace WebApi.Aggregates;

public abstract class AggregateRoot
{
    private readonly List<IEvent> _uncommitted = [];

    public Guid Id { get; protected set; }
    public int Version { get; protected set; }

    protected void Raise(IEvent @event)
    {
        Apply(@event);
        _uncommitted.Add(@event);
    }
    protected abstract void Apply(IEvent @event);
    public IEnumerable<IEvent> GetUncommitted() => _uncommitted;
    public void ClearUncommitted() => _uncommitted.Clear();

    public void ApplyFromHistory(IEnumerable<IEvent> eventsHistories)
    {
        foreach (var e in eventsHistories)
            Apply(e);

        Version = eventsHistories.Count();
    }
}
