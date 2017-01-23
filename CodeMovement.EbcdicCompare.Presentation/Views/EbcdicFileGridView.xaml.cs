using System.Windows.Controls;
using Wcb.Eco.EbcdicCompare.Ui.ViewModel;

namespace Wcb.Eco.EbcdicCompare.Ui.Views
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
