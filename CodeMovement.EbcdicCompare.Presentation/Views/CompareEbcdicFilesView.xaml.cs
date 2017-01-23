using System.Windows.Controls;
using Wcb.Eco.EbcdicCompare.Ui.ViewModel;

namespace Wcb.Eco.EbcdicCompare.Ui.Views
{
    /// <summary>
    /// Interaction logic for CompareEbcdicFilesView.xaml
    /// </summary>
    public partial class CompareEbcdicFilesView : UserControl
    {
        public CompareEbcdicFilesView(CompareEbcdicFilesViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
        }

        public CompareEbcdicFilesViewModel ViewModel
        {
            get { return DataContext as CompareEbcdicFilesViewModel; }
            set { DataContext = value; }
        }
    }
}
