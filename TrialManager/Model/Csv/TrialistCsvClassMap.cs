using CsvHelper.Configuration;

namespace TrialManager.Model.Csv
{
    public class TrialistCsvClassMap : ClassMap<MappedTrialist>
    {
        public TrialistCsvClassMap()
        {
            Map(m => m.FullName).Index(2);
            Map(m => m.Status).Index(3);
            Map(m => m.Address).Index(4);
            Map(m => m.PreferredDayString).Index(7);

            Map(m => m.DogOneName).Index(9);
            Map(m => m.DogOneStatus).Index(10);
            Map(m => m.DogTwoName).Index(11);
            Map(m => m.DogTwoStatus).Index(12);
            Map(m => m.DogThreeName).Index(13);
            Map(m => m.DogThreeStatus).Index(14);
            Map(m => m.DogFourName).Index(15);
            Map(m => m.DogFourStatus).Index(16);
            Map(m => m.DogFiveName).Index(17);
            Map(m => m.DogFiveStatus).Index(18);
        }
    }
}
