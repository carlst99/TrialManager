﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrialManager.Model.LocationDb;
using TrialManager.Model.TrialistDb;
using TrialManager.Services;

namespace TrialManager.Model.Csv
{
    public class MappedTrialist
    {
        /// <summary>
        /// Used when importing data to help with identifying duplicates
        /// </summary>
        public bool HasDuplicateClash;

        #region Mapped Properties

        public string FullName { get; set; }
        public string Status { get; set; }
        public string Address { get; set; }
        public string PreferredDayString { get; set; }

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

        #endregion

        /// <summary>
        /// Stores CSV file data
        /// </summary>
        /// <remarks>This parameterless constructor is required for CSVHelper to instantiate the object</remarks>
        public MappedTrialist()
        {
        }

        /// <summary>
        /// Converts this <see cref="MappedTrialist"/> to a <see cref="Trialist"/>. Does not fill <see cref="Trialist.TravellingPartner"/>
        /// </summary>
        /// <returns></returns>
        public Trialist ToTrialist(ILocationService locationService, IList<PreferredDayDateTimePair> preferredDayMappings)
        {
            Trialist trialist = new Trialist
            {
                Name = FullName,
                Status = Status,
                Address = Address,
                PreferredDay = preferredDayMappings.First(t => t.PreferredDay == PreferredDayString).Day
            };

            // Add dogs
            if (!string.IsNullOrWhiteSpace(DogOneName))
                trialist.Dogs.Add(new Dog(DogOneName, DogOneStatus));
            if (!string.IsNullOrWhiteSpace(DogTwoName))
                trialist.Dogs.Add(new Dog(DogTwoName, DogTwoStatus));
            if (!string.IsNullOrWhiteSpace(DogThreeName))
                trialist.Dogs.Add(new Dog(DogThreeName, DogThreeStatus));
            if (!string.IsNullOrWhiteSpace(DogFourName))
                trialist.Dogs.Add(new Dog(DogFourName, DogFourStatus));
            if (!string.IsNullOrWhiteSpace(DogFiveName))
                trialist.Dogs.Add(new Dog(DogFiveName, DogFiveStatus));

            // Setup location
            if (locationService.TryResolve(Address, out ILocation location))
                trialist.CoordinatePoint = location.Location;

            return trialist;
        }

        #region Object Overrides

        public override string ToString() => nameof(MappedTrialist) + ": " + FullName;

        public override bool Equals(object obj)
        {
            return obj is MappedTrialist mt
                && mt.FullName.Equals(FullName, StringComparison.OrdinalIgnoreCase)
                && mt.Status.Equals(Status, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = (hash * 7) + FullName.ToLower().GetHashCode();
                return (hash * 7) + Status.GetHashCode();
            }
        }

        #endregion
    }
}
