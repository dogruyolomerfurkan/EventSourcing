using Microsoft.EntityFrameworkCore;
using WebApi.Entity;

namespace WebApi.Database;

public class EventDbContext(DbContextOptions<EventDbContext> options) : DbContext(options)
{
    public DbSet<DocumentEvent> DocumentEvents { get; set; }
}
