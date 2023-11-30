using iCustomerCareSystem.Data;
using iCustomerCareSystem.Models;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace iCustomerCareSystem.ViewModels
{
    public class AddOrEditProductViewModel : BasePopupViewModel, INotifyPropertyChanged
    {
        private ClientProducts _selectedClientProduct;
        private Client? _selectedClient;
        private readonly ClientsDbContext _clientsDbContext;

        private ObservableCollection<ProductType> _productTypes;
        private ProductType _selectedProductType;

        public Client? SelectedClient
        {
            get { return _selectedClient; }
            set
            {
                _selectedClient = value;
                OnPropertyChanged(nameof(SelectedClient));
            }
        }

        public ProductType SelectedProductType
        {
            get { return _selectedProductType; }
            set
            {
                _selectedProductType = value;
                OnPropertyChanged(nameof(SelectedProductType));
            }
        }

        public ObservableCollection<ProductType> ProductTypes
        {
            get { return _productTypes; }
            set
            {
                _productTypes = value;
                OnPropertyChanged(nameof(ProductTypes));
            }
        }

        public ClientProducts SelectedClientProduct
        {
            get { return _selectedClientProduct; }
            set
            {
                if (value != null)
                {
                    _selectedClientProduct = value;
                    OnPropertyChanged(nameof(SelectedClientProduct));
                }
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand ProductTypeSelectionChangedCommand { get; }
        public ICommand OperationTypeSelectionChangedCommand { get; }

        public event EventHandler ClientAddedSuccessfully;

        public AddOrEditProductViewModel(ClientsDbContext clientsDbContext, ClientProducts? selectedClientProduct = null)
        {
            _clientsDbContext = clientsDbContext;
            _selectedClientProduct = selectedClientProduct;
            ProductTypes = new ObservableCollection<ProductType>(clientsDbContext.ProductType);
            SaveCommand = new DelegateCommand(async () => await SaveClientAsync());
        }
        public AddOrEditProductViewModel(ClientsDbContext clientsDbContext, Client selectedClient)
        {
            _clientsDbContext = clientsDbContext;
            _selectedClient = selectedClient;
            _selectedClientProduct = new ClientProducts();
            _selectedClientProduct.Client = _selectedClient;
            _selectedClientProduct.Fixed = false;
            ProductTypes = new ObservableCollection<ProductType>(clientsDbContext.ProductType);
            SaveCommand = new DelegateCommand(async () => await SaveClientAsync());
        }

        private async Task SaveClientAsync()
        {
            if (SelectedClientProduct != null)
            {
                if (_selectedClientProduct.ClientId == 0)
                {
                    _clientsDbContext.ClientProducts.Add(SelectedClientProduct);
                }
                else
                {
                    _clientsDbContext.ClientProducts.Update(SelectedClientProduct);
                }

                await SaveClientAsync(_clientsDbContext);
            }
        }

        private async Task SaveClientAsync(ClientsDbContext clientsDbContext)
        {
            bool isSuccess = false;
            SelectedClientProduct.DateIn = DateTime.Now;
            try
            {
                await clientsDbContext.SaveChangesAsync();
                isSuccess = true;
            }
            catch (Exception)
            {
                throw;
            }
            if (isSuccess)
            {
                ClientAddedSuccessfully?.Invoke(this, EventArgs.Empty);
                ChildWindow?.Close();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
