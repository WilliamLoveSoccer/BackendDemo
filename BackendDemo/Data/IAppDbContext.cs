using BackendDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendDemo.Data;

public interface IAppDbContext
{
    DbSet<Alert> Alerts { get; }
    DbSet<Group> Groups { get; }
    DbSet<AlertGroup> AlertGroups { get; }
    DbSet<DeliveryLog> DeliveryLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    int SaveChanges();
}
