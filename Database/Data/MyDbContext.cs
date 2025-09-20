using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Database.Data;

public class MyDbContext(DbContextOptions<MyDbContext> options) : DbContext(options)
{
    private static readonly Serilog.ILogger logger = Log.ForContext<MyDbContext>();
    
    public DbSet<Shared.Models.Transcription> Transcriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        logger.Debug("Configuring database models");

        modelBuilder.Entity<Shared.Models.Transcription>().HasKey(t => t.Url);
        
        logger.Debug("Database models configured successfully");
        base.OnModelCreating(modelBuilder);
    }
}