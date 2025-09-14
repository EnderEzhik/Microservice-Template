using Microsoft.EntityFrameworkCore;

namespace Database.Data;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) {}
}