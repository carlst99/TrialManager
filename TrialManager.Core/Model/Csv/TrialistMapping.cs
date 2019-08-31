using CsvHelper.Configuration;

namespace TrialManager.Core.Model.Csv
{
    public class TrialistMapping : ClassMap<MappedTrialist>
    {
        public TrialistMapping()
        {
            Map(m => m.FullName).Index(3);
            Map(m => m.Status).Index(4);
            Map(m => m.Address).Index(5);
            Map(m => m.PhoneNumber).Index(6);
            Map(m => m.Email).Index(7);
            Map(m => m.PreferredDay).Index(8);
            Map(m => m.TravellingPartner).Index(9);

            Map(m => m.DogOneName).Index(10);
            Map(m => m.DogOneStatus).Index(11);
            Map(m => m.DogTwoName).Index(12);
            Map(m => m.DogTwoStatus).Index(13);
            Map(m => m.DogThreeName).Index(14);
            Map(m => m.DogThreeStatus).Index(15);
            Map(m => m.DogFourName).Index(16);
            Map(m => m.DogFourStatus).Index(17);
            Map(m => m.DogFiveName).Index(18);
            Map(m => m.DogFiveStatus).Index(19);

            //Map(m => m.FullName).Index(1);
            //Map(m => m.Status).Index(2);
            //Map(m => m.Address).Index(3);
            //Map(m => m.PhoneNumber).Index(4);
            //Map(m => m.Email).Index(5);
            //Map(m => m.PreferredDay).Index(6);
            //Map(m => m.TravellingPartner).Index(7);

            //Map(m => m.DogOneName).Index(8);
            //Map(m => m.DogOneStatus).Index(9);
            //Map(m => m.DogTwoName).Index(10);
            //Map(m => m.DogTwoStatus).Index(11);
            //Map(m => m.DogThreeName).Index(12);
            //Map(m => m.DogThreeStatus).Index(13);
            //Map(m => m.DogFourName).Index(14);
            //Map(m => m.DogFourStatus).Index(15);
            //Map(m => m.DogFiveName).Index(16);
            //Map(m => m.DogFiveStatus).Index(17);
        }
    }
}
