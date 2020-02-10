using TrialManager.Model.TrialistDb;

namespace TrialManager.Model.Csv
{
    public struct DuplicateTrialistEntry
    {
        public Trialist FirstTrialist { get; set; }
        public Trialist SecondTrialist { get; set; }
        public bool KeepFirstTrialist { get; set; }
        public bool KeepSecondTrialist { get; set; }

        public DuplicateTrialistEntry(Trialist firstTrialist, Trialist secondTrialist)
        {
            FirstTrialist = firstTrialist;
            SecondTrialist = secondTrialist;
            KeepFirstTrialist = true;
            KeepSecondTrialist = true;
        }

        public override bool Equals(object obj)
        {
            return obj is DuplicateTrialistEntry entry
                && entry.FirstTrialist.Equals(FirstTrialist)
                && entry.SecondTrialist.Equals(SecondTrialist)
                && entry.KeepFirstTrialist.Equals(KeepFirstTrialist)
                && entry.KeepSecondTrialist.Equals(KeepSecondTrialist);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = (hash * 7) + FirstTrialist.GetHashCode();
                hash = (hash * 7) + SecondTrialist.GetHashCode();
                hash = (hash * 7) + KeepFirstTrialist.GetHashCode();
                return (hash * 7) + KeepSecondTrialist.GetHashCode();
            }
        }

        public static bool operator ==(DuplicateTrialistEntry left, DuplicateTrialistEntry right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DuplicateTrialistEntry left, DuplicateTrialistEntry right)
        {
            return !(left == right);
        }
    }
}
