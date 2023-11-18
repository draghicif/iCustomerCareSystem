using iCustomerCareSystem.Data;
using iCustomerCareSystem.Models;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace iCustomerCareSystem.ViewModels
{
    public class AddOrEditClientViewModel : INotifyPropertyChanged
    {
        public Window ChildWindow { get; set; }

        private ClientProducts _selectedClient;
        private readonly ClientsDbContext _clientsDbContext;

        private ObservableCollection<ProductType> _productTypes;
        private ObservableCollection<OperationType> _operationTypes;
        private ProductType _selectedProductType;
        private OperationType _selectedOperationType;

        public ProductType SelectedProductType
        {
            get { return _selectedProductType; }
            set
            {
                _selectedProductType = value;
                OnPropertyChanged(nameof(SelectedProductType));
            }
        }

        public OperationType SelectedOperationType
        {
            get { return _selectedOperationType; }
            set
            {
                _selectedOperationType = value;
                OnPropertyChanged(nameof(SelectedOperationType));
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

        public ObservableCollection<OperationType> OperationTypes
        {
            get { return _operationTypes; }
            set
            {
                _operationTypes = value;
                OnPropertyChanged(nameof(OperationTypes));
            }
        }
        public ClientProducts? SelectedClient
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
        public ICommand ProductTypeSelectionChangedCommand { get; }
        public ICommand OperationTypeSelectionChangedCommand { get; }
        public ICommand CloseWindowCommand { get; }

        public event EventHandler ClientAddedSuccessfully;

        public AddOrEditClientViewModel(ClientsDbContext clientsDbContext, ClientProducts? selectedClient = null)
        {
            _clientsDbContext = clientsDbContext;
            _selectedClient = selectedClient ?? new ClientProducts();
            ProductTypes = new ObservableCollection<ProductType>(clientsDbContext.ProductType);
            OperationTypes = new ObservableCollection<OperationType>(clientsDbContext.OperationType);
            SaveCommand = new DelegateCommand(async () => await SaveClientAsync());
            //ProductTypeSelectionChangedCommand = new DelegateCommand<object>(ProductTypeSelectionChanged);
            //OperationTypeSelectionChangedCommand = new DelegateCommand<object>(OperationTypeSelectionChanged);
            CloseWindowCommand = new DelegateCommand<object>(CloseWindow);
        }

        private void CloseWindow(object obj)
        {
            ChildWindow?.Close();
        }

        private async Task SaveClientAsync()
        {
            if (SelectedClient != null)
            {
                if (_selectedClient.ClientId == 0)
                {
                    _clientsDbContext.ClientProducts.Add(SelectedClient);
                }
                else
                {
                    _clientsDbContext.ClientProducts.Update(SelectedClient);
                }

                await SaveClientAsync(_clientsDbContext);
            }
        }

        private async Task SaveClientAsync(ClientsDbContext clientsDbContext)
        {
            bool isSuccess = false;
            SelectedClient.DateIn = DateTime.Now;
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
