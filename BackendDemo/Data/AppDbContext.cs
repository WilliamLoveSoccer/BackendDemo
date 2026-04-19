using BackendDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendDemo.Data;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<AlertGroup> AlertGroups => Set<AlertGroup>();
    public DbSet<DeliveryLog> DeliveryLogs => Set<DeliveryLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Alert>()
            .HasKey(a => a.Id);

        modelBuilder.Entity<Group>()
            .HasKey(g => g.Id);

        modelBuilder.Entity<DeliveryLog>()
            .HasKey(dl => dl.Id);

        modelBuilder.Entity<AlertGroup>()
            .HasKey(ag => new { ag.AlertId, ag.GroupId });

        modelBuilder.Entity<AlertGroup>()
            .HasOne(ag => ag.Alert)
            .WithMany(a => a.AlertGroups)
            .HasForeignKey(ag => ag.AlertId);

        modelBuilder.Entity<AlertGroup>()
            .HasOne(ag => ag.Group)
            .WithMany(g => g.AlertGroups)
            .HasForeignKey(ag => ag.GroupId);
    }
}
