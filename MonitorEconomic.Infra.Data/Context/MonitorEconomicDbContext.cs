using Microsoft.EntityFrameworkCore;
using MonitorEconomic.Domain.Entities;


public class MonitorEconomicDbContext : DbContext
{
    public MonitorEconomicDbContext(DbContextOptions<MonitorEconomicDbContext> options)
        : base(options)
    {
    }

    public DbSet<IPCEntity> IPC { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IPCBaseDomain>().ToTable("ipc");
        modelBuilder.Entity<IPCEntity>().HasKey("Id");

        modelBuilder.Entity<IPCEntity>()
            .Property(e => e.Data)
            .HasColumnType("timestamp without time zone");

        base.OnModelCreating(modelBuilder);
    }
}

// vou mudar essa classe, vou conectar ela com banco de dados postgres, essa classe vai ser o contexto de integração com o banco de dados, ela deve ser inchuta para criar apenas esse contexto