using System.Windows.Controls;
using CodeMovement.EbcdicCompare.Presentation.ViewModel;

namespace CodeMovement.EbcdicCompare.Presentation.View
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeView(HomeViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
        }

        public HomeViewModel ViewModel
        {
            get { return DataContext as HomeViewModel; }
            set { DataContext = value; }
        }
    }
}
