using WebApi.Events;

namespace WebApi.Aggregates;

public class DocumentEventAggregate : AggregateRoot
{
    public string Title { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public bool IsApproved { get; private set; }
    public bool IsDeleted { get; private set; }

    public DocumentEventAggregate() { }

    public DocumentEventAggregate(Guid id, string title, string content)
    {
        Raise(new DocumentCreated(id, DateTime.UtcNow, title, content));
    }

    public void Update(string title, string content)
    {
        if (IsDeleted)
            throw new InvalidOperationException("Cannot update a deleted document.");
        Raise(new DocumentUpdated(Id, DateTime.UtcNow, title, content));
    }

    public void Approve() => Raise(new DocumentApproved(Id, DateTime.UtcNow));
    public void Invalidate() => Raise(new DocumentInvalidated(Id, DateTime.UtcNow));
    public void Delete() => Raise(new DocumentDeleted(Id, DateTime.UtcNow));

    protected override void Apply(IEvent @event)
    {
        switch (@event)
        {
            case DocumentCreated e:
                Id = e.AggregateId;
                Title = e.Title;
                Content = e.Content;
                break;
            case DocumentUpdated e:
                Title = e.Title;
                Content = e.Content;
                break;
            case DocumentApproved:
                IsApproved = true;
                break;
            case DocumentInvalidated:
                IsApproved = false;
                break;
            case DocumentDeleted:
                IsDeleted = true;
                break;
        }
    }
}
