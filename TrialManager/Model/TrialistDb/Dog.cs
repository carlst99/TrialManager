namespace TrialManager.Model.TrialistDb
{
    public class Dog : ContextItem
    {
        public static Dog Default => new Dog
        {
            Name = "Dog",
            Status = EntityStatus.Maiden
        };

        #region Constructors

        public Dog() { }

        public Dog(string name, EntityStatus status)
        {
            Name = name;
            Status = status;
        }

        #endregion

        #region Object Overrides

        public override bool Equals(object obj)
        {
            return obj is Dog dog
                && Equals(dog);
        }

        public bool Equals(Dog dog)
        {
            return dog.Name.Equals(Name)
                && dog.Status.Equals(Status);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 13;
                hash = (hash * 7) + Name.GetHashCode();
                return (hash * 7) + Status.GetHashCode();
            }
        }

        #endregion
    }
}