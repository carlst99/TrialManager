using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace TrialManager.Core.Model.TrialistDb
{
    public sealed class TrialistContext : DbContext, ITrialistContext
    {
        public DbSet<Trialist> Trialists { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=trialManager.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Trialist>()
                        .Property(t => t.Dogs)
                        .HasConversion(
                            f => MessagePack.MessagePackSerializer.Serialize(f),
                            b => MessagePack.MessagePackSerializer.Deserialize<ObservableCollection<Dog>>(b));
            base.OnModelCreating(modelBuilder);
        }
    }
}
