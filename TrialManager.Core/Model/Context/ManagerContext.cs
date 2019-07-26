using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using TrialManager.Core.Model.ContextModel;

namespace TrialManager.Core.Model.Context
{
    public sealed class ManagerContext : DbContext, IManagerContext
    {
        private const string SQL_LITE_CONNECTION_PREFIX = "Data Source=";
        internal const string DB_PATH = "trialManager.db";

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
