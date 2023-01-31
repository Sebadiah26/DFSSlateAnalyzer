using DFSSlateAnalyzerData.Data;
using Microsoft.EntityFrameworkCore;
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

        //public DFSSlateAnalyzerContext (DbContextOptions<DFSSlateAnalyzerContext> options) : base(options)
        //{


        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           optionsBuilder.UseSqlServer("Server=DESKTOP-FT0FCJQ\\CKIELINSKI;Database=Projects;Integrated Security=true;TrustServerCertificate=True;MultipleActiveResultSets=true");


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
          .HasPrincipalKey(s => s.EntryID)
         ;
        }

    }
}