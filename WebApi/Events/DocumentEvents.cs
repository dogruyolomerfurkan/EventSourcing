using System.Text.Json.Serialization;

namespace WebApi.Events;

[method: JsonConstructor]
public record DocumentCreated(Guid AggregateId, DateTime CreatedAt, string Title, string Content) : IEvent;
[method: JsonConstructor]
public record DocumentUpdated(Guid AggregateId, DateTime CreatedAt, string Title, string Content) : IEvent;

[method: JsonConstructor]
public record DocumentDeleted(Guid AggregateId, DateTime CreatedAt) : IEvent;

[method: JsonConstructor]
public record DocumentApproved(Guid AggregateId, DateTime CreatedAt) : IEvent;

[method: JsonConstructor]
public record DocumentInvalidated(Guid AggregateId, DateTime CreatedAt) : IEvent;