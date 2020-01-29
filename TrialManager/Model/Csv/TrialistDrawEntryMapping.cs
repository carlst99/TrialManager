using CsvHelper.Configuration;

namespace TrialManager.Model.Csv
{
    internal class TrialistDrawEntryMapping : ClassMap<TrialistDrawEntry>
    {
        public TrialistDrawEntryMapping()
        {
            Map(m => m.RunNumber).Index(0).Name("RunNumber");
            Map(m => m.TrialistName).Index(2).Name("TrialistName");
            Map(m => m.CompetingDogName).Index(3).Name("DogName");
        }
    }
}
