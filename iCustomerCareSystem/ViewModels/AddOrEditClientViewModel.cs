using iCustomerCareSystem.Models;
using System.ComponentModel;
using System.Windows;

namespace iCustomerCareSystem.ViewModels
{
    public class AddOrEditClientViewModel: BasePopupViewModel, INotifyPropertyChanged
    {
        private Client _client;

        public Client Client
        {
            get { return _client; }
            set
            {
                _client = value;
                OnPropertyChanged(nameof(Client));
            }
        }

        public AddOrEditClientViewModel()
        {
                
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
