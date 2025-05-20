## Event Sourcing Code Flow

This project uses Event Sourcing to manage document state. Below is an overview of the code flow:

### 1. Command Initiation
- An action (e.g., create, update, approve, delete) is performed on a document through the aggregate (`DocumentEventAggregate`).

### 2. Event Creation
- The aggregate raises a domain event (such as `DocumentCreated`, `DocumentUpdated`, etc.) using the `Raise` method.
- The event is added to the aggregate’s list of uncommitted events.

### 3. Event Persistence
- The application calls `EventRepository.SaveAsync`, passing the uncommitted events and the expected version.
- Each event is serialized to JSON and stored as a `DocumentEvent` entity in the database via `EventDbContext.DocumentEvents`.
- The event’s type, data, aggregate ID, version, and timestamp are saved.

### 4. Event Retrieval
- To reconstruct a document’s state, `EventRepository.GetByAggregateIdAsync` is called.
- All events for the given `AggregateId` are loaded from the database, ordered by version.
- Each event is deserialized back to its original type.

### 5. Aggregate Rehydration
- The aggregate applies each event in order using its `Apply` method.
- The document’s state is rebuilt from the event history.

---

**Key Files:**
- `WebApi/Aggregates/DocumentEventAggregate.cs` – Aggregate logic and event application
- `WebApi/Repositories/EventRepository.cs` – Event persistence and retrieval
- `WebApi/Entity/DocumentEvent.cs` – Event storage model
- `WebApi/Database/EventDbContext.cs` – Entity Framework context for event storage
- `WebApi/Events/DocumentEvents.cs` – Event type definitions

This flow ensures every change is recorded as an immutable event, enabling full auditability and state reconstruction at any point in time.
