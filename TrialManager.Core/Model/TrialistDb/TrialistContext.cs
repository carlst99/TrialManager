using MessagePack;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using TrialManager.Core.Model.LocationDb;

namespace TrialManager.Core.Model.TrialistDb
{
    public sealed class TrialistContext : DbContext, ITrialistContext
    {
        public DbSet<Trialist> Trialists { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=TrialManager.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Trialist>()
                        .Property(t => t.Dogs)
                        .HasConversion(
                            f => MessagePackSerializer.Serialize(f),
                            b => MessagePackSerializer.Deserialize<ObservableCollection<Dog>>(b));

            modelBuilder.Entity<Trialist>()
                        .Property(l => l.Location)
                        .HasConversion(
                            f => MessagePackSerializer.Serialize(f),
                            b => MessagePackSerializer.Deserialize<Location>(b));
            base.OnModelCreating(modelBuilder);
        }
    }
}
