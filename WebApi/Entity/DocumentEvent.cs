namespace WebApi.Entity;

public class DocumentEvent
{
    public Guid Id { get; set; }
    public Guid AggregateId { get; set; }
    public int Version { get; set; }
    public required string EventType { get; set; }
    public required string Data { get; set; }
    public DateTime CreatedAt { get; set; }
}