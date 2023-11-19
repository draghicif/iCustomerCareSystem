using Prism.Commands;
using System.Windows;
using System.Windows.Input;

namespace iCustomerCareSystem.ViewModels
{
    public class BasePopupViewModel
    {
        public Window ChildWindow { get; set; }
        public ICommand CloseWindowCommand { get; }
        public BasePopupViewModel()
        {
            CloseWindowCommand = new DelegateCommand<object>(CloseWindow);
        }

        private void CloseWindow(object obj)
        {
            ChildWindow?.Close();
        }
    }
}
