using DFSSlateAnalyzerData.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;



namespace DFSSlateAnalyzerData
{
    public class DFSSlateAnalyzerContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<DFSPlayer> DFSPlayers { get; set; }
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
             entity.ToTable("EntryMember", "dfs"));
            modelBuilder.Entity<Entry>(entity =>
           entity.ToTable("Entry", "dfs"));
            modelBuilder.Entity<Contest>(entity =>
           entity.ToTable("Contest", "dfs"));
            modelBuilder.Entity<Owner>(entity =>
           entity.ToTable("Owner", "dfs"));
            modelBuilder.Entity<Player>(entity =>
           entity.ToTable("Player", "dfs"));
            modelBuilder.Entity<DFSPlayer>(entity =>
           entity.ToTable("DFSPlayer", "dfs"));



            modelBuilder.Entity<EntryMember>(entity =>
            {
               // entity.HasNoKey();
                entity.HasKey(e => new { e.ContestID, e.EntryID, e.EntryMemberPlayerName });
               


            });

            modelBuilder.Entity<Entry>()
          .HasMany(s => s.EntryMembers)
          .WithOne(s => s.Entry)
          .HasForeignKey(s => s.EntryID).HasPrincipalKey(s => s.EntryID).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Contest>()
               
               .HasMany(s => s.Entries)
               .WithOne(s => s.Contest) 
               .HasForeignKey(s => s.ContestID).OnDelete(DeleteBehavior.NoAction)
               .HasPrincipalKey(s => s.ContestID).OnDelete(DeleteBehavior.NoAction);

                modelBuilder.Entity<Contest>()

              .HasMany(s => s.ContestPlayers)
              .WithOne(s => s.Contest)
              .HasForeignKey(s => s.ContestID).OnDelete(DeleteBehavior.NoAction)
              .HasPrincipalKey(s => s.ContestID).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Player>(entity =>
            {
                // entity.HasNoKey();
                entity.HasKey(e => new { e.PlayerName, e.ContestID });


            });

            //  modelBuilder.Entity<EntryMember>()
            //.HasOne(s => s.Player)
            //   .WithOne(s => s.entryMember)
            //  .HasPrincipalKey<EntryMember>(s => new { s.ContestID, s.EntryMemberPlayerName }).OnDelete(DeleteBehavior.NoAction)
            //  .HasForeignKey<Player>(s => new { s.ContestID, s.PlayerName }).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Player>()

                .HasOne(s => s.DFSPlayer)
                .WithMany(s => s.Players)
                .HasForeignKey(e => e.PlayerID)
                .HasPrincipalKey(e => e.PlayerID);
               
               
               

        }

    }
}