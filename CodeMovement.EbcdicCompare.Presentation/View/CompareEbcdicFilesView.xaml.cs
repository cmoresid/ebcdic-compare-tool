using System.Windows.Controls;
using CodeMovement.EbcdicCompare.Presentation.ViewModel;

namespace CodeMovement.EbcdicCompare.Presentation.View
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
