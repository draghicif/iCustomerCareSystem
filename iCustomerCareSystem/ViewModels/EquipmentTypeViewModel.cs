using iCustomerCareSystem.Data;
using iCustomerCareSystem.Models;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Linq;

namespace iCustomerCareSystem.ViewModels
{
    public class EquipmentTypeViewModel : BasePopupViewModel, INotifyPropertyChanged
    {
        private readonly ClientsDbContext _clientsDbContext;
        private ObservableCollection<ProductType> _productTypes;
        private ProductType _selectedProductType;

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

        private bool _isAddElementEnabled;
        public bool IsAddElementEnabled
        {
            get { return _isAddElementEnabled; }
            set
            {
                _isAddElementEnabled = value;
                OnPropertyChanged(nameof(IsAddElementEnabled));
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
                UpdateCanAddEnabled();
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
                UpdateCanAddEnabled();
            }
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteItemCommand { get; }
        public event EventHandler ClientAddedSuccessfully;

        public EquipmentTypeViewModel(ClientsDbContext clientsDbContext)
        {
            _clientsDbContext = clientsDbContext;
            InitializeAsync();
            AddCommand = new DelegateCommand(async () => await AddAsync());
            DeleteItemCommand = new DelegateCommand(async () => await DeleteAsync());
        }

        private async void InitializeAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                ProductTypes = new ObservableCollection<ProductType>(
                    await _clientsDbContext.ProductType.ToListAsync()
                );

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading clients: {ex.Message}");
            }
        }

        private void UpdateCanAddEnabled()
        {
            IsAddElementEnabled = CanAdd();
        }

        private bool CanAdd()
        {
            return !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Description);
        }

        private async Task DeleteAsync()
        {
            if (SelectedProductType != null)
            {
                bool elementInUse = _clientsDbContext.ClientProducts.FirstOrDefault(x => x.ProductType == SelectedProductType) != null;
                if (elementInUse)
                {
                    MessageBox.Show("Acest Tip de echipament este folosit in Lista de produse in service!", "Erroare stergere element!",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                _clientsDbContext.ProductType.Remove(SelectedProductType);
                await SaveAsync(_clientsDbContext);
            }
        }

        private async Task AddAsync()
        {
            var newItem = new ProductType();
            newItem.Name = Name;
            newItem.Description = Description;

            _clientsDbContext.ProductType.Add(newItem);

            Name = Description = string.Empty;

            await SaveAsync(_clientsDbContext);
        }

        private async Task SaveAsync(ClientsDbContext clientsDbContext)
        {
            bool isSuccess = false;
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
                await LoadDataAsync();
                ClientAddedSuccessfully?.Invoke(this, EventArgs.Empty);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
