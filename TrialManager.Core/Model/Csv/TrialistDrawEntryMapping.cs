using CsvHelper.Configuration;

namespace TrialManager.Core.Model.Csv
{
    internal class TrialistDrawEntryMapping : ClassMap<TrialistDrawEntry>
    {
        public TrialistDrawEntryMapping()
        {
            Map(m => m.RunNumber).Index(0).Name("RunNumber");
            Map(m => m.Day).Index(1).Name("Day");
            Map(m => m.TrialistName).Index(2).Name("TrialistName");
            Map(m => m.CompetingDogName).Index(3).Name("DogName");
        }
    }
}
