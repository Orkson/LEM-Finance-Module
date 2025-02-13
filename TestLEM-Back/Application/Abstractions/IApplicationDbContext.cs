using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Application.Abstractions
{
    public interface IApplicationDbContext
    {
        DbSet<Device> Devices { get; set; }
        DbSet<Model> Models { get; set; }
        DbSet<Company> Companies { get; set; }
        DbSet<MeasuredValue> MeasuredValues { get; set; }
        DbSet<MeasuredRange> MeasuredRanges { get; set; }
        DbSet<PhysicalMagnitude> PhysicalMagnitudes { get; set; }
        DbSet<Document> Documents { get; set; }
        DbSet<ModelCooperation> ModelCooperation { get; set; }

        DatabaseFacade Database { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
