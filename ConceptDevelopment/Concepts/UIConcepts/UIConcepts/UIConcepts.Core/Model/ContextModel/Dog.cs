namespace UIConcepts.Core.Model.ContextModel
{
    public class Dog : ContextItem
    {
        #region Fields

        private int _dogId;
        private string _name;
        private EntityStatus _status;

        #endregion Fields

        #region Properties

        public int DogId
        {
            get => _dogId;
            set => SetProperty(ref _dogId, value, nameof(DogId));
        }

        /// <summary>
        /// Gets or sets the name of the dog
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value, nameof(Name));
        }

        /// <summary>
        /// Gets or sets the status of the dog
        /// </summary>
        public EntityStatus Status
        {
            get => _status;
            set => SetProperty(ref _status, value, nameof(Status));
        }

        #endregion
    }
}
