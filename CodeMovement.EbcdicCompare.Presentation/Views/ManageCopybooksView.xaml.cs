using System.Windows.Controls;
using Wcb.Eco.EbcdicCompare.Ui.ViewModel;

namespace Wcb.Eco.EbcdicCompare.Ui.Views
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
