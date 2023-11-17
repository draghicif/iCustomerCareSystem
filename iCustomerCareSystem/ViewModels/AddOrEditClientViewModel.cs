using iCustomerCareSystem.Data;
using iCustomerCareSystem.Models;
using Prism.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Input;

namespace iCustomerCareSystem.ViewModels
{
    public class AddOrEditClientViewModel : INotifyPropertyChanged
    {

        private Client _selectedClient;
        private readonly ClientsDbContext _clientsDbContext;

        private ObservableCollection<ProductType> _productTypes;
        private ObservableCollection<OperationType> _operationTypes;

        public ObservableCollection<ProductType> ProductTypes
        {
            get { return _productTypes; }
            set
            {
                _productTypes = value;
                OnPropertyChanged(nameof(ProductTypes));
            }
        }

        public ObservableCollection<OperationType> OperationTypes
        {
            get { return _operationTypes; }
            set
            {
                _operationTypes = value;
                OnPropertyChanged(nameof(OperationTypes));
            }
        }
        public Client? SelectedClient
        {
            get { return _selectedClient; }
            set
            {
                if (value != null)
                {
                    _selectedClient = value;
                    OnPropertyChanged(nameof(SelectedClient));
                }
            }
        }

        public ICommand SaveCommand { get; }

        public AddOrEditClientViewModel(ClientsDbContext clientsDbContext, Client? selectedClient = null)
        {
            _clientsDbContext = clientsDbContext;
            _selectedClient = selectedClient ?? new Client();
            ProductTypes = new ObservableCollection<ProductType>(clientsDbContext.ProductType);
            OperationTypes = new ObservableCollection<OperationType>(clientsDbContext.OperationType);
            SaveCommand = new DelegateCommand(async () => await SaveClientAsync());
        }

        private async Task SaveClientAsync()
        {
            if (SelectedClient != null)
            {
                if (_selectedClient.ClientId == 0)
                {
                    _clientsDbContext.Clients.Add(SelectedClient);
                }
                else
                {
                    _clientsDbContext.Clients.Update(SelectedClient);
                }

                await _clientsDbContext.SaveChangesAsync();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
