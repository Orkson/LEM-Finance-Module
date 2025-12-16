using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Application.Abstractions;

namespace Infrastructure
{
    public class LemDbContext : IdentityDbContext<User>, IApplicationDbContext
    {
        public LemDbContext(DbContextOptions<LemDbContext> options) : base(options)
        {
        }

        public DbSet<Device> Devices { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<MeasuredValue> MeasuredValues { get; set; }
        public DbSet<MeasuredRange> MeasuredRanges { get; set; }
        public DbSet<PhysicalMagnitude> PhysicalMagnitudes { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<ModelCooperation> ModelCooperation { get; set; }
        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        public DbSet<ExpensePlanner> ExpensePlanner { get; set; }
        public DbSet<Service> Service { get; set; }
        public DbSet<DeviceRelations> DeviceRelations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Device>(d =>
            {

                d.HasIndex(x => x.IdentificationNumber)
                .IsUnique();

                d.HasMany(x => x.Documents)
                .WithOne(x => x.Device)
                .HasForeignKey(x => x.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Company>(x => x.HasIndex(x => x.Name));

            modelBuilder.Entity<PhysicalMagnitude>(pm =>
            {
                pm.HasMany(x => x.MeasuredValues)
                .WithOne(x => x.PhysicalMagnitude)
                .HasForeignKey(x => x.PhysicalMagnitudeId);
            });

            modelBuilder.Entity<MeasuredValue>(mv =>
            {
                mv.HasMany(x => x.MeasuredRanges)
                .WithOne(x => x.MeasuredValue)
                .HasForeignKey(x => x.MeasuredValueId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ModelCooperation>(mc =>
            {
                mc.HasOne(x => x.ModelFrom)
                .WithMany(x => x.CooperateFrom)
                .HasForeignKey(x => x.ModelFromId)
                .OnDelete(DeleteBehavior.Cascade);

                mc.HasOne(x => x.ModelTo)
                .WithMany(x => x.CooperateTo)
                .HasForeignKey(x => x.ModelToId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MeasuredRange>(mr =>
                mr.Property(x => x.AccuracyInPercet)
                .HasConversion<double>()
            );

            modelBuilder.Entity<ExpensePlanner>()
                .HasOne(e => e.Service)
                .WithMany()
                .HasForeignKey(e => e.ServiceId);

            modelBuilder.Entity<ExpensePlanner>()
                .HasOne(e => e.Device)
                .WithMany()
                .HasForeignKey(e => e.DeviceId);

            modelBuilder.Entity<DeviceRelations>(j =>
            {
                j.ToTable("DeviceRelations");

                j.HasKey(x => new { x.DeviceId, x.RelatedDeviceId });

                j.HasOne(x => x.Device)
                 .WithMany(d => d.RelatedDevices)
                 .HasForeignKey(x => x.DeviceId)
                 .OnDelete(DeleteBehavior.Cascade);

                j.HasOne(x => x.RelatedDevice)
                 .WithMany(d => d.RelatedByDevices)
                 .HasForeignKey(x => x.RelatedDeviceId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var timeoutInterceptor = new BusyTimeoutCommandInterceptor(5000);

                optionsBuilder.UseSqlite("Data Source=mydb.db;")
                              .AddInterceptors(timeoutInterceptor);
            }

        }
    }
}
