using System.Windows.Controls;
using CodeMovement.EbcdicCompare.Presentation.ViewModel;

namespace CodeMovement.EbcdicCompare.Presentation.View
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
