Event Sourcing Code Flow
1.	Command Initiation
•	An action (such as create, update, approve, delete) is performed on a document via the aggregate (DocumentEventAggregate).
2.	Event Creation
•	The aggregate raises a domain event (e.g., DocumentCreated, DocumentUpdated) by calling Raise(new EventType(...)).
•	The event is added to the aggregate’s list of uncommitted events.
3.	Event Persistence
•	The application calls EventRepository.SaveAsync, passing the uncommitted events and the expected version.
•	Each event is serialized to JSON and stored as a DocumentEvent entity in the database using EventDbContext.DocumentEvents.
•	The event’s type and data are saved, along with metadata (aggregate ID, version, timestamp).
4.	Event Retrieval
•	To reconstruct a document’s state, EventRepository.GetByAggregateIdAsync is called.
•	All events for the given AggregateId are loaded from the database, ordered by version.
•	Each event is deserialized back to its original type.
5.	Aggregate Rehydration
•	The aggregate applies each event in order using its Apply method.
•	The document’s state is rebuilt from the event history.
---
Example Flow: Document Update
•	User requests to update a document.
•	Controller calls the aggregate’s Update method.
•	DocumentUpdated event is raised and added to uncommitted events.
•	Repository saves the event to the database.
•	When loading the document, all events are fetched and replayed to restore the current state.
---
Key Files Involved:
•	DocumentEventAggregate.cs: Business logic, event raising, and state application.
•	EventRepository.cs: Event persistence and retrieval.
•	DocumentEvent.cs: Event storage model.
•	EventDbContext.cs: Entity Framework context for event storage.
•	DocumentEvents.cs: Event type definitions.
---
This flow ensures every change is recorded as an immutable event, enabling full auditability and state reconstruction at any point in time.