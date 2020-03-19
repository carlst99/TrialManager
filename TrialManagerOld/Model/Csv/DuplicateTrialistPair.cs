using System;

namespace TrialManager.Model.Csv
{
    public class DuplicateTrialistPair
    {
        public bool NeedsResolving { get; }
        public MappedTrialist FirstTrialist { get; set; }
        public MappedTrialist SecondTrialist { get; set; }
        public bool KeepFirstTrialist { get; set; }
        public bool KeepSecondTrialist { get; set; }

        public DuplicateTrialistPair(MappedTrialist trialist)
        {
            NeedsResolving = false;
            FirstTrialist = trialist ?? throw new ArgumentNullException(nameof(trialist));
            KeepFirstTrialist = true;
        }

        public DuplicateTrialistPair(MappedTrialist firstTrialist, MappedTrialist secondTrialist)
        {
            NeedsResolving = true;
            FirstTrialist = firstTrialist ?? throw new ArgumentNullException(nameof(firstTrialist));
            SecondTrialist = secondTrialist ?? throw new ArgumentNullException(nameof(secondTrialist));
            KeepFirstTrialist = true;
            KeepSecondTrialist = false;
        }

        public override bool Equals(object obj)
        {
            return obj is DuplicateTrialistPair entry
                && entry.FirstTrialist.Equals(FirstTrialist)
                && entry.SecondTrialist?.Equals(SecondTrialist) == true
                && entry.KeepFirstTrialist.Equals(KeepFirstTrialist)
                && entry.KeepSecondTrialist.Equals(KeepSecondTrialist);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = (hash * 7) + FirstTrialist.GetHashCode();
                if (SecondTrialist != null)
                    hash = (hash * 7) + SecondTrialist.GetHashCode();
                hash = (hash * 7) + KeepFirstTrialist.GetHashCode();
                return (hash * 7) + KeepSecondTrialist.GetHashCode();
            }
        }

        public static bool operator ==(DuplicateTrialistPair left, DuplicateTrialistPair right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DuplicateTrialistPair left, DuplicateTrialistPair right)
        {
            return !(left == right);
        }
    }
}
