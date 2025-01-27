using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BncPayments.Models;

public partial class DbEpaymentsContext : DbContext
{
    public DbEpaymentsContext()
    {
    }

    public DbEpaymentsContext(DbContextOptions<DbEpaymentsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<RequestDb> Requests { get; set; }

    public virtual DbSet<ResponseDb> Responses { get; set; }

    public virtual DbSet<WorkingKey> WorkingKeys { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("ApplicationDBContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDBContextConnection' not found.");

            optionsBuilder.UseSqlServer(connectionString);
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RequestDb>(entity =>
        {
            entity.HasOne(d => d.IdWorkingKeyNavigation).WithMany(p => p.Requests)
                .HasForeignKey(d => d.IdWorkingKey)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Requests_WorkingKeys");
        });

        modelBuilder.Entity<ResponseDb>(entity =>
        {
            entity.Property(e => e.StatusCode).HasMaxLength(50);

            entity.HasOne(d => d.IdRequestNavigation).WithMany(p => p.Responses)
                .HasForeignKey(d => d.IdRequest)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Responses_Requests");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
