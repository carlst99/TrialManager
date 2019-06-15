using Microsoft.EntityFrameworkCore;
using Plugin.Settings.Abstractions;
using UIConcepts.Core.Model.ContextModel;

namespace UIConcepts.Core.Model.Context
{
    public sealed class ManagerContext : DbContext, IManagerContext
    {
        private const string SQL_LITE_CONNECTION_PREFIX = "Data Source=";
        internal const string DB_PATH = "trialManager.db";

        private readonly ISettings _settings;

        public DbSet<Trialist> Trialists { get; set; }

        public ManagerContext(ISettings settings)
        {
            _settings = settings;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(GetConnectionString());
        }

        private string GetConnectionString()
        {
            string path = _settings.GetValueOrDefault(nameof(SettingsKeys.DbConnectionPath), DB_PATH);
            return SQL_LITE_CONNECTION_PREFIX + path;
        }
    }
}
