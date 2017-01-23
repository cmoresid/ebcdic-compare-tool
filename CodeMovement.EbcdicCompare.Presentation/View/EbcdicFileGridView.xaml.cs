using System.Windows.Controls;
using CodeMovement.EbcdicCompare.Presentation.ViewModel;

namespace CodeMovement.EbcdicCompare.Presentation.View
{
    /// <summary>
    /// Interaction logic for EbcdicFileGridView.xaml
    /// </summary>
    public partial class EbcdicFileGridView : UserControl
    {
        public EbcdicFileGridView(EbcdicFileGridViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
        }

        public EbcdicFileGridViewModel ViewModel
        {
            get { return DataContext as EbcdicFileGridViewModel; }
            set { DataContext = value; }
        }
    }
}
