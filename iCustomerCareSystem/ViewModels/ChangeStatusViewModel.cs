using iCustomerCareSystem.Data;
using iCustomerCareSystem.Models;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace iCustomerCareSystem.ViewModels
{
    public class ChangeStatusViewModel : BasePopupViewModel, INotifyPropertyChanged
    {
        public ObservableCollection<int> MonthComboBoxItemsSource { get; } = new ObservableCollection<int> { 0, 1, 2, 3 };
        private readonly ClientsDbContext _clientsDbContext;
        private ObservableCollection<ProductStatus> _productStatus;

        public ObservableCollection<ProductStatus> ProductStatus
        {
            get { return ProductStatus; }
            set
            {
                _productStatus = value;
                OnPropertyChanged(nameof(ProductStatus));
            }
        }

        private bool _isFixed;
        private bool _isReturnedSelected;

        private StatusId _selectedStatus;
        public StatusId SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                if (_selectedStatus != value)
                {
                    _selectedStatus = value;
                    OnPropertyChanged(nameof(SelectedStatus));

                    _isReturnedSelected = (_selectedStatus == StatusId.Returned);

                    if (!_isReturnedSelected)
                    {
                        IsFixed = false;
                    }

                    // Enable/disable the CheckBox based on the selected status
                    IsReturnedEnabled = _isReturnedSelected; // Enable the CheckBox if "Returned" is selected
                    if (IsReturnedEnabled)
                    {
                        SelectedMonthValue = null;
                    }

                    OnPropertyChanged(nameof(IsReturnedSelected));
                }
            }
        }

        public bool IsReturnedSelected => _isReturnedSelected;

        public bool IsFixed
        {
            get => _isFixed;
            set
            {
                if (_isFixed != value)
                {
                    _isFixed = value;
                    OnPropertyChanged(nameof(IsFixed));

                    if (SelectedClientProduct != null)
                    {
                        SelectedClientProduct.Fixed = _isFixed;
                    }
                }
            }
        }

        private bool _isReturnedEnabled = false;
        public bool IsReturnedEnabled
        {
            get => _isReturnedEnabled;
            set
            {
                if (_isReturnedEnabled != value)
                {
                    _isReturnedEnabled = value;
                    OnPropertyChanged(nameof(IsReturnedEnabled));
                }
            }
        }

        private ClientProducts? _selectedClientProduct;
        public ClientProducts? SelectedClientProduct
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

        public event EventHandler ClientAddedSuccessfully;
        public ICommand SaveCommand { get; }

        private int? _selectedMonthValue;
        public int? SelectedMonthValue
        {
            get => _selectedMonthValue;
            set
            {
                if (_selectedMonthValue != value)
                {
                    _selectedMonthValue = value;
                    OnPropertyChanged(nameof(SelectedMonthValue));
                    CalculateNumberOfMonths(); // Calculate number of months whenever the value changes
                }
            }
        }

        private int _numberOfMonths;
        public int NumberOfMonths
        {
            get => _numberOfMonths;
            set
            {
                if (_numberOfMonths != value)
                {
                    _numberOfMonths = value;
                    OnPropertyChanged(nameof(NumberOfMonths));
                }
            }
        }

        public ChangeStatusViewModel(ClientsDbContext clientsDbContext, ClientProducts clientProduct)
        {
            _clientsDbContext = clientsDbContext;

            InitializeAsync();

            _selectedClientProduct = clientProduct;
            _selectedStatus = (StatusId)_selectedClientProduct.ProductStatusId;

            SaveCommand = new DelegateCommand(async () => await SaveAsync());
        }

        private async void InitializeAsync()
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                ProductStatus = new ObservableCollection<ProductStatus>(
                    await _clientsDbContext.ProductStatus.ToListAsync()
                );

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading clients: {ex.Message}");
            }
        }

        private async Task SaveAsync()
        {
            bool isSuccess = false;
            try
            {
                if (SelectedClientProduct != null && SelectedStatus != (StatusId)SelectedClientProduct.ProductStatusId)
                {
                    SelectedClientProduct.ProductStatusId = (int)SelectedStatus;
                    if (SelectedStatus == StatusId.Returned && !SelectedClientProduct.IsReturnInService)
                    {
                        SelectedClientProduct.DateOut = DateTime.Now;
                    }
                    SelectedClientProduct.IsReturnInService = false;
                    if (SelectedMonthValue != null && SelectedMonthValue != 0)
                    {
                        SelectedClientProduct.WarantyEndDate = DateTime.Now.AddMonths((int)SelectedMonthValue);
                    }
                }
                await _clientsDbContext.SaveChangesAsync();
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

        private void CalculateNumberOfMonths()
        {
            // Add your logic to calculate the number of months based on the selected value
            if (SelectedMonthValue.HasValue)
            {
                // For example, if 1, 2, or 3 is selected, set the NumberOfMonths accordingly
                NumberOfMonths = SelectedMonthValue.Value * 3; // Modify this logic as per your requirement
            }
            else
            {
                NumberOfMonths = 0; // Set a default value when no selection is made
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
