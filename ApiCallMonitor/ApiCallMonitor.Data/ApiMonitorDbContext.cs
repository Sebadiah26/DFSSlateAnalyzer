using ApiCallMonitor.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiCallMonitor.Data;

/// <summary>EF Core context for the whole app. Backed by SQLite (see Program.cs) so the tool is
/// self-contained - no external database server to stand up just to configure and run some API
/// calls. Schema is created via EnsureCreatedAsync at startup rather than migrations, since this is
/// a single-file store with no upgrade-in-place requirement.</summary>
public class ApiMonitorDbContext : DbContext
{
    public ApiMonitorDbContext(DbContextOptions<ApiMonitorDbContext> options)
        : base(options)
    {
    }

    public DbSet<ApiCallCollection> Collections => Set<ApiCallCollection>();

    public DbSet<ApiCallDefinition> Calls => Set<ApiCallDefinition>();

    public DbSet<CallRun> Runs => Set<CallRun>();

    public DbSet<CallRunResult> RunResults => Set<CallRunResult>();

    public DbSet<IncidentIqConnectionSettings> IncidentIqConnectionSettings => Set<IncidentIqConnectionSettings>();

    public DbSet<UserEditLogEntry> UserEditLog => Set<UserEditLogEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApiCallCollection>(entity =>
        {
            entity.HasMany(c => c.Calls)
                .WithOne(call => call.Collection!)
                .HasForeignKey(call => call.CollectionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany<CallRun>()
                .WithOne(run => run.Collection!)
                .HasForeignKey(run => run.CollectionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ApiCallDefinition>(entity =>
        {
            entity.Property(call => call.Method).HasConversion<string>().HasMaxLength(16);
        });

        modelBuilder.Entity<CallRun>(entity =>
        {
            entity.Property(run => run.Status).HasConversion<string>().HasMaxLength(32);

            entity.HasMany(run => run.Results)
                .WithOne(result => result.CallRun!)
                .HasForeignKey(result => result.CallRunId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CallRunResult.ApiCallDefinitionId is deliberately not a foreign key: results must survive
        // the source call definition being edited or deleted, since they're a historical record of
        // what actually ran (see the model's own doc comment).
        modelBuilder.Entity<CallRunResult>(entity =>
        {
            entity.Property(result => result.Method).HasMaxLength(16);
        });

        // Single fixed-Id row (Id = IncidentIqConnectionSettings.SingletonId) rather than an
        // autoincrementing key - ValueGeneratedNever so EF never tries to have SQLite assign it.
        modelBuilder.Entity<IncidentIqConnectionSettings>(entity =>
        {
            entity.Property(s => s.Id).ValueGeneratedNever();
        });
    }
}
