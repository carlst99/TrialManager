using CsvHelper.Configuration;

namespace TrialManager.Model.Csv
{
    internal class TrialistDrawEntryMapping : ClassMap<TrialistDrawEntry>
    {
        public TrialistDrawEntryMapping()
        {
            Map(m => m.RunNumber).Index(0).Name("Run Number");
            Map(m => m.TrialistName).Index(1).Name("Trialist Name");
            Map(m => m.TrialistStatus).Index(2).Name("Trialist Status");
            Map(m => m.CompetingDogName).Index(3).Name("Dog Name");
            Map(m => m.CompetingDogStatus).Index(4).Name("Dog Status");
        }
    }
}
