using System.Windows.Controls;
using CodeMovement.EbcdicCompare.Presentation.ViewModel;

namespace CodeMovement.EbcdicCompare.Presentation.View
{
    /// <summary>
    /// Interaction logic for ManageCopybookView.xaml
    /// </summary>
    public partial class ManageCopybooksView : UserControl
    {
        public ManageCopybooksView(ManageCopybooksViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
        }

        public ManageCopybooksViewModel ViewModel
        {
            get { return DataContext as ManageCopybooksViewModel; }
            set { DataContext = value; }
        }
    }
}
