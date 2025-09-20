using Microsoft.EntityFrameworkCore;
using Serilog;
using Shared.Entities;

namespace Database.Data;

public class MyDbContext(DbContextOptions<MyDbContext> options) : DbContext(options)
{
    private static readonly Serilog.ILogger logger = Log.ForContext<MyDbContext>();

    public DbSet<Transcription> Transcriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transcription>().HasKey(t => t.Url);
        base.OnModelCreating(modelBuilder);
    }
}
