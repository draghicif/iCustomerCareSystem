using iCustomerCareSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace iCustomerCareSystem.Views
{
    /// <summary>
    /// Interaction logic for EquipmentTypeView.xaml
    /// </summary>
    public partial class EquipmentTypeView : Window
    {
        public EquipmentTypeView(EquipmentTypeViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
