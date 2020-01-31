using Realms;
using System;
using System.Collections.Generic;
using TrialManager.Model.LocationDb;
using TrialManager.Model.TrialistDb;
using TrialManager.Services;

namespace TrialManager.Model.Csv
{
    internal class MappedTrialist
    {
        private readonly ILocationService _locationService;

        #region Mapped Properties

        public string FullName;
        public EntityStatus Status;
        public string Address;
        public string PreferredDayString;
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

        #endregion

        public MappedTrialist(ILocationService locationService)
        {
            _locationService = locationService;
        }

        /// <summary>
        /// Converts this <see cref="MappedTrialist"/> to a <see cref="Trialist"/>. Does not fill <see cref="Trialist.TravellingPartner"/>
        /// </summary>
        /// <returns></returns>
        public Trialist ToTrialist(Realm realm, Dictionary<string, DateTimeOffset> preferredDayMappings)
        {
            Trialist trialist = new Trialist
            {
                Id = RealmHelpers.GetNextId<Trialist>(realm),
                Name = FullName,
                Status = Status,
                Address = Address,
                TravellingPartner = null
            };

            // Parse preferred day
            if (preferredDayMappings.ContainsKey(PreferredDayString))
                trialist.PreferredDay = preferredDayMappings[PreferredDayString];
            else
                trialist.PreferredDay = DateTimeOffset.MinValue;

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
            if (_locationService.TryResolve(Address, out ILocation location))
                trialist.CoordinatePoint = location.Location;

            return trialist;
        }
    }
}
