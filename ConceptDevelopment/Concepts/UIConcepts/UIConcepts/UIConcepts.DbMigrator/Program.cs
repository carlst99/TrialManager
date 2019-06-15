using System;

namespace UIConcepts.DbMigrator
{
    public static class Program
    {
        public static void Main()
        {
            Console.WriteLine("Hello World!");

            // Set target project switch: -Project UIConcepts.Core
            // Set build project switch: -StartupProject UIConcepts.DbMigrator
            // Using VS Package Manager Console
            // Create new migration: Add-Migration -Name XXXXX -Project UIConcepts.Core -StartupProject UIConcepts.DbMigrator
            // Update database to specified migration: Update-Database -Migration XXXXX -Project UIConcepts.Core -StartupProject UIConcepts.DbMigrator
        }
    }
}
