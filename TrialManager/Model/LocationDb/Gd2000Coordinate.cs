﻿using MessagePack;
using System;

namespace TrialManager.Model.LocationDb
{
    [MessagePackObject]
    public class Gd2000Coordinate
    {
        /// <summary>
        /// Gets the X coordinate of this location point
        /// </summary>
        [Key(0)]
        public double Gd2000X { get; set; }

        /// <summary>
        /// Gets the Y coordinate of this location point
        /// </summary>
        [Key(1)]
        public double Gd2000Y { get; set; }

        public Gd2000Coordinate() { }

        public Gd2000Coordinate(double gd2000X, double gd2000Y)
        {
            Gd2000X = gd2000X;
            Gd2000Y = gd2000Y;
        }

        #region Object overrides

        public override string ToString()
        {
            return Gd2000X + "," + Gd2000Y;
        }

        public override bool Equals(object obj)
        {
            return obj is Gd2000Coordinate loc
                && loc.Gd2000X.Equals(Gd2000X)
                && loc.Gd2000Y.Equals(Gd2000Y);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 23;
                hash = (hash * 13) + Gd2000X.GetHashCode();
                return (hash * 13) + Gd2000Y.GetHashCode();
            }
        }

        #endregion

        /// <summary>
        /// Gets the absolute distance, in coordinate points, from one location to another
        /// </summary>
        /// <param name="lFrom"></param>
        /// <param name="lTo"></param>
        /// <returns>The distance between the two location points, 'as the crow flies'</returns>
        public static double DistanceTo(Gd2000Coordinate lFrom, Gd2000Coordinate lTo)
        {
            if (lFrom is null || lTo is null)
                return 0;

            double xDistance = Math.Abs(lFrom.Gd2000X - lTo.Gd2000X);
            double yDistance = Math.Abs(lFrom.Gd2000Y - lTo.Gd2000Y);

            return Math.Sqrt(Math.Pow(xDistance, 2) + Math.Pow(yDistance, 2));
        }
    }
}
