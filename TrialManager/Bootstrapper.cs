﻿using MaterialDesignThemes.Wpf;
using Serilog;
using StyletIoC;
using System;
using System.IO;
using TrialManager.Services;
using TrialManager.ViewModels;

namespace TrialManager
{
    public class Bootstrapper : Stylet.Bootstrapper<ShellViewModel>
    {
        public const string LOG_FILE_NAME = "log.log";

        protected override void OnStart()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Debug()
                .WriteTo.File(GetAppdataFilePath(LOG_FILE_NAME))
                .CreateLogger();

            base.OnStart();
        }

        protected override void ConfigureIoC(IStyletIoCBuilder builder)
        {
            builder.Bind<ICsvImportService>().To<CsvImportService>().InSingletonScope();
            builder.Bind<IDataExportService>().To<DataExportService>().InSingletonScope();
            builder.Bind<IDrawCreationService>().To<DrawCreationService>().InSingletonScope();
            builder.Bind<ILocationService>().To<LocationService>().InSingletonScope();
            builder.Bind<INavigationService>().To<NavigationService>().InSingletonScope();
            builder.Bind<ISnackbarMessageQueue>().ToFactory(_ => new SnackbarMessageQueue(new TimeSpan(0, 0, 5))).InSingletonScope();

            base.ConfigureIoC(builder);
        }

        #region Appdata Helpers

        /// <summary>
        /// Gets the path to the appdata store of respective platforms
        /// </summary>
        /// <returns>
        /// <see cref="Environment.SpecialFolder.Personal"/> when running on Android
        /// <see cref="Environment.SpecialFolder.MyDocuments"/> when running on iOS
        /// <see cref="Environment.SpecialFolder.ApplicationData"/> when running on any other platform
        /// </returns>
        public static string GetPlatformAppdataPath()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TrialManager");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        /// <summary>
        /// Gets the path to a file in the local appdata
        /// </summary>
        /// <param name="fileName">The name of the file to resolve the path to</param>
        /// <returns></returns>
        public static string GetAppdataFilePath(string fileName) => Path.Combine(GetPlatformAppdataPath(), fileName);

        #endregion
    }
}
