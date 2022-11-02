using Microsoft.EntityFrameworkCore;
using Xarajat.Data.Enttities;

namespace Xarajat.Data.Context;

public class XarajatDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Outlay> Outlays { get; set; }

    
    public XarajatDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        

        //With configuration class
        OutlayConfiguration.Configure(modelBuilder.Entity<Outlay>());

        //With configuration classes from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(XarajatDbContext).Assembly);
    }

}
