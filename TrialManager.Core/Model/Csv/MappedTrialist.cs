using System;
using TrialManager.Core.Model.TrialistDb;

namespace TrialManager.Core.Model.Csv
{
    public class MappedTrialist
    {
        public string FullName;
        public EntityStatus Status;
        public string Address;
        public string PhoneNumber;
        public string Email;
        public string PreferredDay;
        public string TravellingPartner;

        public string DogOneName;
        public EntityStatus DogOneStatus;
        public string DogTwoName;
        public EntityStatus DogTwoStatus;
        public string DogThreeName;
        public EntityStatus DogThreeStatus;
        public string DogFourName;
        public EntityStatus DogFourStatus;
        public string DogFiveName;
        public EntityStatus DogFiveStatus;

        public Trialist ToTrialist()
        {
            Trialist trialist = new Trialist
            {
                FullName = FullName,
                Status = Status,
                PhoneNumber = PhoneNumber,
                Email = Email
            };

            // Parse preferred day
            DateTime.TryParse(PreferredDay, out DateTime preferredDay);
            trialist.PreferredDay = preferredDay;

            return trialist;
        }
    }
}
