using Microsoft.EntityFrameworkCore;
using Plugin.Settings.Abstractions;
using System;
using UIConcepts.Core.Model.ContextModel;

namespace UIConcepts.Core.Model.Context
{
    public sealed class ManagerContext : DbContext, IManagerContext
    {
        private const string SQL_LITE_CONNECTION_PREFIX = "Data Source=";
        internal const string DB_PATH = "trialManager.db";

        private readonly ISettings _settings;
        private readonly string _connectPath;

        public DbSet<Trialist> Trialists { get; set; }

        /// <summary>
        /// To be used only when creating migrations
        /// </summary>
        /// <param name="connectPath"></param>
        internal ManagerContext(string connectPath)
        {
            _connectPath = connectPath;
        }

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
            string path;
            if (_settings != null)
                path = _settings.GetValueOrDefault(nameof(SettingsKeys.DbConnectionPath), DB_PATH);
            else if (!string.IsNullOrEmpty(_connectPath))
                path = _connectPath;
            else
                throw new InvalidOperationException("No database path has been provided");

            return SQL_LITE_CONNECTION_PREFIX + path;
        }
    }
}
