using WebApi.Events;

namespace WebApi.Repositories;

public interface IEventRepository
{
    Task SaveAsync(IEnumerable<IEvent> events, int expectedVersion);
    Task<List<IEvent>> GetByAggregateIdAsync(Guid id);
}
