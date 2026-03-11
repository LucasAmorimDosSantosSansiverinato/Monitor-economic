using Microsoft.EntityFrameworkCore;
//using MonitorEconomic.Infra.Data.Models;
using MonitorEconomic.Infra.Data.Entities;

public class MonitorEconomicDbContext : DbContext
{
    public MonitorEconomicDbContext(DbContextOptions<MonitorEconomicDbContext> options)
        : base(options)
    {
    }

    public DbSet<IPCEntity> IPC { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IPCEntity>().ToTable("ipc");
        modelBuilder.Entity<IPCEntity>().HasKey("Id");

        modelBuilder.Entity<IPCEntity>()
            .Property(e => e.Data)
            .HasColumnType("timestamp without time zone");

        base.OnModelCreating(modelBuilder);
    }
}