using iCustomerCareSystem.ViewModels;
using System.Windows;

namespace iCustomerCareSystem.Views
{
    /// <summary>
    /// Interaction logic for AddOrEditClientVIew.xaml
    /// </summary>
    public partial class AddOrEditClientVIew : Window
    {
        public AddOrEditClientVIew(AddOrEditClientViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
