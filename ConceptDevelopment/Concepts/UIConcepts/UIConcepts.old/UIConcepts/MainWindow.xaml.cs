using System.ComponentModel;
using System.Windows;

namespace UIConcepts
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Fields

        private bool _drawerStatus;
        private FrameworkElement _pageContent;

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        #region Properties

        public bool DrawerStatus
        {
            get => _drawerStatus;
            set => SetProperty(ref _drawerStatus, value, nameof(DrawerStatus));
        }

        public FrameworkElement PageContent
        {
            get => _pageContent;
            set => SetProperty(ref _pageContent, value, nameof(PageContent));
        }

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            PageContent = new DataInputPage();
            //FrmContent.Content = new DataInputPage();
        }

        private void SetProperty<T>(ref T container, T value, string propertyName)
        {
            if (container != null)
            {
                if (!container.Equals(value))
                {
                    container = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
            } else
            {
                container = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
