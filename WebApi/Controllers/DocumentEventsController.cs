using Microsoft.AspNetCore.Mvc;
using WebApi.Aggregates;
using WebApi.Repositories;

namespace WebApi.Controllers;

[ApiController]
[Route("api/documents")]
public class DocumentEventsController(IEventRepository eventRepository) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateDocumentEventDto createDocumentEventDto)
    {
        var aggregateId = Guid.CreateVersion7();
        var aggregate = new DocumentEventAggregate(aggregateId, createDocumentEventDto.Title, createDocumentEventDto.Content);
        await eventRepository.SaveAsync(aggregate.GetUncommitted(), 0);
        return CreatedAtAction(nameof(Get), new { id = aggregateId },null);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateDocumentEventDto updateDto)
    {
        var eventHistories = await eventRepository.GetByAggregateIdAsync(id);
        if (eventHistories.Count == 0) return NotFound();

        var aggregate = new DocumentEventAggregate();
        aggregate.ApplyFromHistory(eventHistories);
        aggregate.Update(updateDto.Title, updateDto.Content);
        await eventRepository.SaveAsync(aggregate.GetUncommitted(), aggregate.Version);
        return NoContent();
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id) => await HandleSimple(id, a => a.Approve());

    [HttpPost("{id:guid}/invalidate")]
    public async Task<IActionResult> Invalidate(Guid id) => await HandleSimple(id, a => a.Invalidate());

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id) => await HandleSimple(id, a => a.Delete());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var history = await eventRepository.GetByAggregateIdAsync(id);
        if (history.Count == 0) return NotFound();

        var agg = new DocumentEventAggregate();

        agg.ApplyFromHistory(history);
        return Ok(new { agg.Id, agg.Title, agg.Content, agg.IsApproved, agg.IsDeleted });
    }

    [HttpGet("{id:guid}/history")]
    public async Task<IActionResult> History(Guid id)
    {
        var history = await eventRepository.GetByAggregateIdAsync(id);
        return Ok(history.OrderBy(e => e.CreatedAt));
    }

    private async Task<IActionResult> HandleSimple(Guid id, Action<DocumentEventAggregate> action)
    {
        var history = await eventRepository.GetByAggregateIdAsync(id);
        if (history.Count == 0) return NotFound();
        var agg = new DocumentEventAggregate();
        agg.ApplyFromHistory(history);
        action(agg);
        await eventRepository.SaveAsync(agg.GetUncommitted(), agg.Version);
        return NoContent();
    }

}
public record CreateDocumentEventDto(string Title, string Content);
public record UpdateDocumentEventDto(string Title, string Content);