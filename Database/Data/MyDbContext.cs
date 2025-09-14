using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Database.Data;

public class MyDbContext : DbContext
{
    private static readonly Serilog.ILogger logger = Log.ForContext<MyDbContext>();
    
    public DbSet<Shared.Models.Transcription> Transcriptions { get; set; }
    
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        logger.Debug("Configuring database model");
        
        modelBuilder.Entity<Shared.Models.Transcription>().HasIndex(t => t.Url);
        
        logger.Debug("Database model configured successfully");
        base.OnModelCreating(modelBuilder);
    }
}