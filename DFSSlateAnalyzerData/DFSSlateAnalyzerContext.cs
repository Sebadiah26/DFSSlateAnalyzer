using DFSSlateAnalyzerData.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace DFSSlateAnalyzerData
{
    public class DFSSlateAnalyzerContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<Contest> Contests { get; set; }
        public DbSet<EntryMember> EntryMembers { get; set; }
        public DbSet<Owner> Owners { get; set; }

        public static readonly ILoggerFactory _loggerFactory
          = LoggerFactory.Create(builder => { builder.AddDebug(); });

        public DFSSlateAnalyzerContext(DbContextOptions<DFSSlateAnalyzerContext> options) : base(options)
        {
            

        }

      

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer("Server=DESKTOP-FT0FCJQ\\CKIELINSKI;Database=Projects;Integrated Security=true;TrustServerCertificate=True;MultipleActiveResultSets=true");
                optionsBuilder.UseLoggerFactory(_loggerFactory);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EntryMember>(entity =>
            {
               // entity.HasNoKey();
                entity.HasKey(e => new { e.EntryID, e.EntryMemberPlayerName });
               

            });

                modelBuilder.Entity<Entry>()
              .HasMany(s => s.EntryMembers)
              .WithOne(s => s.Entry)
              .HasForeignKey(s => s.EntryID)
              .HasPrincipalKey(s => s.EntryID) ;

                    modelBuilder.Entity<Contest>()
               
               .HasMany(s => s.Entries)
               .WithOne(s => s.Contest) 
               .HasForeignKey(s => s.ContestID)
               .HasPrincipalKey(s => s.ContestID) ;

                modelBuilder.Entity<Contest>()

              .HasMany(s => s.ContestPlayers)
              .WithOne(s => s.Contest)
              .HasForeignKey(s => s.ContestID)
              .HasPrincipalKey(s => s.ContestID);

            modelBuilder.Entity<Player>(entity =>
            {
                // entity.HasNoKey();
                entity.HasKey(e => new { e.PlayerName, e.ContestID });


            });
        }

    }
}