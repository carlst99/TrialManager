using CsvHelper.Configuration;

namespace TrialManager.Model.Csv
{
    public class TrialistCsvClassMap : ClassMap<MappedTrialist>
    {
        public string FullName { get; set; }
        public string Status { get; set; }
        public string Address { get; set; }
        public string PreferredDay { get; set; }
        public string DogOneName { get; set; }
        public string DogTwoName { get; set; }
        public string DogThreeName { get; set; }
        public string DogFourName { get; set; }
        public string DogFiveName { get; set; }
        public string DogOneStatus { get; set; }
        public string DogTwoStatus { get; set; }
        public string DogThreeStatus { get; set; }
        public string DogFourStatus { get; set; }
        public string DogFiveStatus { get; set; }

        public void SetupMappings()
        {
            Map(m => m.FullName).Name(FullName);
            Map(m => m.Status).Name(Status);
            Map(m => m.Address).Name(Address);
            Map(m => m.PreferredDayString).Name(PreferredDay);

            Map(m => m.DogOneName).Name(DogOneName);
            Map(m => m.DogTwoName).Name(DogTwoName);
            Map(m => m.DogThreeName).Name(DogThreeName);
            Map(m => m.DogFourName).Name(DogFourName);
            Map(m => m.DogFiveName).Name(DogFiveName);
            Map(m => m.DogOneStatus).Name(DogOneStatus);
            Map(m => m.DogTwoStatus).Name(DogTwoStatus);
            Map(m => m.DogThreeStatus).Name(DogThreeStatus);
            Map(m => m.DogFourStatus).Name(DogFourStatus);
            Map(m => m.DogFiveStatus).Name(DogFiveStatus);
        }
    }
}
