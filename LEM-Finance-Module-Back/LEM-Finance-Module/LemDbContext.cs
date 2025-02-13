using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.Xml;
using TestLEM.Entities;

namespace TestLEM
{
    public class LemDbContext : DbContext
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Device>(d =>
            {
                d.HasOne(x => x.Model)
                .WithMany(m => m.Devices)
                .HasForeignKey(x => x.ModelId);

                d.HasIndex(x => x.IdentifiactionNumber)
                .IsUnique();
            });

            modelBuilder.Entity<Model>(m =>
            {
                m.HasOne(m => m.Company)
                .WithMany(c => c.Models)
                .HasForeignKey(m => m.CompanyId);

                m.HasIndex(x => x.SerialNumber)
                .IsUnique();

                m.HasIndex(x => x.Name)
                .IsUnique();

                m.HasMany(x => x.MeasuredValues)
                .WithOne(x => x.Model)
                .HasForeignKey(x => x.ModelId);
            });

            modelBuilder.Entity<Company>(x => x.HasIndex(x => x.Name).IsUnique());

            modelBuilder.Entity<PhysicalMagnitude>(pm =>
            {
                pm.HasIndex(x => x.Unit)
                .IsUnique();

                pm.HasIndex(x => x.Name)
                .IsUnique();

                pm.HasMany(x => x.MeasuredValues)
                .WithOne(x => x.PhysicalMagnitude)
                .HasForeignKey(x => x.PhysicalMagnitudeId);
            });

            modelBuilder.Entity<MeasuredValue>(mv =>
            {
                mv.HasMany(x => x.MeasuredRanges)
                .WithOne(x => x.MeasuredValue)
                .HasForeignKey(x => x.MeasuredValueId);
            });
        }
    }
}
