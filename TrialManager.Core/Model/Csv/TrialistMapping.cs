using CsvHelper.Configuration;

namespace TrialManager.Core.Model.Csv
{
    public class TrialistMapping : ClassMap<MappedTrialist>
    {
        public TrialistMapping()
        {
            Map(m => m.FullName).Index(1).Name("Full Name");
            Map(m => m.Status).Index(2).Name("Status");
            Map(m => m.Address).Index(3).Name("Address");
            Map(m => m.PhoneNumber).Index(4).Name("Phone Number");
            Map(m => m.Email).Index(5).Name("Email");
            Map(m => m.PreferredDay).Index(6).Name("Preferred Day");
            Map(m => m.TravellingPartner).Index(7).Name("Travelling Partner");

            Map(m => m.DogOneName).Index(8);
            Map(m => m.DogOneStatus).Index(9);
            Map(m => m.DogTwoName).Index(10);
            Map(m => m.DogTwoStatus).Index(11);
            Map(m => m.DogThreeName).Index(12);
            Map(m => m.DogThreeStatus).Index(13);
            Map(m => m.DogFourName).Index(14);
            Map(m => m.DogFourStatus).Index(15);
            Map(m => m.DogFiveName).Index(16);
            Map(m => m.DogFiveStatus).Index(17);
        }
    }
}
