using System.Windows.Controls;
using Wcb.Eco.EbcdicCompare.Ui.ViewModel;

namespace Wcb.Eco.EbcdicCompare.Ui.Views
{
    /// <summary>
    /// Interaction logic for OpenEbcdicFileView.xaml
    /// </summary>
    public partial class OpenEbcdicFileView : UserControl
    {
        public OpenEbcdicFileView(OpenEbcdicFileViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }

        public OpenEbcdicFileViewModel ViewModel
        {
            get { return DataContext as OpenEbcdicFileViewModel; }
            set { DataContext = value; }
        }
    }
}
