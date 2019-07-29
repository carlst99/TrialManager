using AddressConverterHelper.Model;
using System;

namespace AddressConverterHelper
{
    public static class Program
    {
        public static void Main(string[] args) 
		{
            LocationContext locations = new LocationContext();
            Console.WriteLine("Deleting previous database...");
            locations.Database.EnsureDeleted();
            Console.WriteLine("Previous database delected");
            Console.WriteLine("Creating new database...");
            locations.Database.EnsureCreated();
            Console.WriteLine("New database created");
		}
    }
}
