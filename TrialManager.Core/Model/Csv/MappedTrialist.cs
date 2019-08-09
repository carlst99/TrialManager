using TrialManager.Core.Model.TrialistDb;

namespace TrialManager.Core.Model.Csv
{
    public class MappedTrialist
    {
        public string FullName;
        public EntityStatus Status;
        public string Address;
        public string PhoneNumber;

        public Trialist ToTrialist()
        {
            Trialist trialist = new Trialist();

            string[] nameComponents = FullName.Split(' ');

            return trialist;
        }
    }
}
