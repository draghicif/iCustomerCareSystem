using iCustomerCareSystem.Data;
using iCustomerCareSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Unity;
using Prism.Commands;
using iCustomerCareSystem.Views;
using System.Windows.Media.Imaging;

namespace iCustomerCareSystem.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Private fields

        private ObservableCollection<ClientProducts> _serviceClients;
        private ObservableCollection<ClientProducts> _historicalClients;
        private ClientProducts? _selectedClientProduct;
        private ICollectionView _filteredClients;
        private string _filterText;
        private readonly ClientsDbContext _clientsDbContext;
        private BitmapImage _imageSource;
        private ObservableCollection<Client> _clients;
        private Client? _selectedClient;

        #endregion

        #region Public properties

        public ObservableCollection<Client> Clients
        {
            get { return _clients; }
            set
            {
                _clients = value;
                OnPropertyChanged(nameof(Clients));
            }

        }

        public ObservableCollection<ClientProducts> ClientProducts
        {
            get { return _serviceClients; }
            set
            {
                _serviceClients = value;
                OnPropertyChanged(nameof(ClientProducts));
            }
        }

        public ObservableCollection<ClientProducts> HistoricalClients
        {
            get { return _historicalClients; }
            set
            {
                _historicalClients = value;
                OnPropertyChanged(nameof(HistoricalClients));
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

        public ICollectionView FilteredClients
        {
            get => _filteredClients;
            set
            {
                if (_filteredClients != value)
                {
                    _filteredClients = value;
                    OnPropertyChanged(nameof(FilteredClients));
                }
            }
        }

        public string FilterText
        {
            get => _filterText;
            set
            {
                if (_filterText != value)
                {
                    _filterText = value;
                    OnPropertyChanged(FilterText);
                    RefreshCollectionView();
                }
            }
        }

        public BitmapImage ImageSource
        {
            get { return _imageSource; }
            set
            {
                _imageSource = value;
                OnPropertyChanged(nameof(ImageSource));
            }
        }

        #endregion

        #region Behaviors  

        public ICommand AddNewClientCommand { get; private set; }
        public ICommand EditClientCommand { get; private set; }
        public ICommand PrintServiceEntryCommand { get; private set; }

        #endregion

        #region Constructor

        public MainWindowViewModel()
        {
            if (Application.Current.Resources["UnityContainer"] is IUnityContainer container)
            {
                _clientsDbContext = container.Resolve<ClientsDbContext>();
            }

            InitializeAsync();
            AddNewClientCommand = new DelegateCommand(AddNewClient);
            EditClientCommand = new DelegateCommand(EditClient);
            PrintServiceEntryCommand = new DelegateCommand(PrintServiceEntry);
            LoadLogo();
        }

        #endregion

        #region Private methods

        private async void InitializeAsync()
        {
            await LoadDataAsync();
            FilteredClients = CollectionViewSource.GetDefaultView(ClientProducts);
        }

        private async Task LoadDataAsync()
        {
            try
            {
                ClientProducts = new ObservableCollection<ClientProducts>(
                    await _clientsDbContext.ClientProducts
                        .Where(x => x.DateOut == null)
                        .Include(c => c.Client)
                        .Include(c => c.OperationType)
                        .Include(c => c.ProductType)
                        .ToListAsync()
                );

                Clients = new ObservableCollection<Client>(
                    await _clientsDbContext.Clients
                        .OrderBy(x => x.FirstName)
                        .ToListAsync()
                );

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading clients: {ex.Message}");
            }
        }

        private void RefreshCollectionView()
        {
            if (ClientProducts == null)
                return;

            if (string.IsNullOrWhiteSpace(FilterText))
            {
                FilteredClients = CollectionViewSource.GetDefaultView(ClientProducts);
                return;
            }

            var filtered = new ObservableCollection<ClientProducts>(
                ClientProducts.Where(c => DoesClientMatchFilterText(c, FilterText))
            );

            FilteredClients = CollectionViewSource.GetDefaultView(filtered);
        }

        private bool DoesClientMatchFilterText(ClientProducts clientProduct, string filterText)
        {
            var filterCriteria = new List<string> { "FirstName", "LastName", "Telephone" };
            var searchText = filterText.ToLower();
            foreach (var property in clientProduct.GetType().GetProperties())
            {
                if (property.Name == "Client")
                {
                    var refClient = property.GetType().GetProperties();
                    foreach (var clientProperty in refClient)
                    {
                        if (filterCriteria.Contains(clientProperty.Name))
                        {
                            var value = property.GetValue(clientProduct)?.ToString()?.ToLower();
                            if (value != null && value.Contains(searchText))
                                return true;
                        }
                    }
                }
            }
            return false;
        }

        #endregion

        private void AddNewClient()
        {
            _selectedClient = null;
            EditClient();
        }

        private void EditClient()
        {
            var addEditViewModel = new AddOrEditProductViewModel(_clientsDbContext, SelectedClientProduct);
            AddOrEditProductView childWindow = new AddOrEditProductView(addEditViewModel);
            childWindow.Owner = Application.Current.MainWindow;
            childWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            addEditViewModel.ChildWindow = childWindow;
            addEditViewModel.ClientAddedSuccessfully += HandleClientAddedSuccessfully;
            childWindow.ShowDialog();
        }

        private void HandleClientAddedSuccessfully(object? sender, EventArgs e)
        {
            RefreshMainWindowData();
        }

        private void RefreshMainWindowData()
        {
            InitializeAsync();
        }
        public void LoadLogo()
        {
            var path = System.Configuration.ConfigurationManager.ConnectionStrings["Logo"].ConnectionString;
            // Load the image from the file path
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(path);
            image.EndInit();

            // Set the ImageSource property
            ImageSource = image;
        }

        private void PrintServiceEntry()
        {
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
