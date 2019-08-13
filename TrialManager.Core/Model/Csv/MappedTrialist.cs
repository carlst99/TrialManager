using MvvmCross;
using System;
using TrialManager.Core.Model.LocationDb;
using TrialManager.Core.Model.TrialistDb;
using TrialManager.Core.Services;

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

        /// <summary>
        /// Converts this <see cref="MappedTrialist"/> to a <see cref="Trialist"/>. Does not fill <see cref="Trialist.TravellingPartner"/>
        /// </summary>
        /// <returns></returns>
        public Trialist ToTrialist()
        {
            Trialist trialist = new Trialist
            {
                FullName = FullName,
                Status = Status,
                Address = Address,
                PhoneNumber = PhoneNumber,
                Email = Email
            };

            // Parse preferred day
            DateTime.TryParse(PreferredDay, out DateTime preferredDay);
            trialist.PreferredDay = preferredDay;

            // Add dogs
            if (!string.IsNullOrEmpty(DogOneName))
                trialist.Dogs.Add(new Dog(DogOneName, DogOneStatus));
            if (!string.IsNullOrEmpty(DogTwoName))
                trialist.Dogs.Add(new Dog(DogTwoName, DogTwoStatus));
            if (!string.IsNullOrEmpty(DogThreeName))
                trialist.Dogs.Add(new Dog(DogThreeName, DogThreeStatus));
            if (!string.IsNullOrEmpty(DogFourName))
                trialist.Dogs.Add(new Dog(DogFourName, DogFourStatus));
            if (!string.IsNullOrEmpty(DogFiveName))
                trialist.Dogs.Add(new Dog(DogFiveName, DogFiveStatus));

            // Setup location
            ILocationService locService = Mvx.IoCProvider.Resolve<ILocationService>();
            if (locService.TryResolve(Address, out LocationBase location))
                trialist.Location = location.Location;

            return trialist;
        }
    }
}
