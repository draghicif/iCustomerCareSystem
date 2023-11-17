using iCustomerCareSystem.Data;
using iCustomerCareSystem.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using Microsoft.EntityFrameworkCore;
using System.Windows.Data;
using System.Windows;
using iCustomerCareSystem.Core;
using Unity;

namespace iCustomerCareSystem.ViewModels
{
    public class ClientViewModel : INotifyPropertyChanged
    {
        #region Private fields

        private ObservableCollection<Client> _clients;
        private ObservableCollection<Client> _historicalClients;
        private Client _selectedClient;
        private ICollectionView _filteredClients;
        private string _filterText;
        private readonly ClientsDbContext _clientsDbContext;

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

        public ObservableCollection<Client> HistoricalClients
        {
            get { return _historicalClients; }
            set
            {
                _historicalClients = value;
                OnPropertyChanged(nameof(HistoricalClients));
            }
        }

        public Client SelectedClient
        {
            get { return _selectedClient; }
            set
            {
                _selectedClient = value;
                OnPropertyChanged(nameof(SelectedClient));
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

        #endregion

        #region Behaviors  

        public DelegateCommand LoadClientsCommand { get; private set; }
        public ICommand FilterTextChangedCommand { get; }
        public ICommand AddClientCommand { get; private set; }
        public ICommand EditClientCommand { get; private set; }

        #endregion

        #region Constructor

        public ClientViewModel()
        {
            if (Application.Current.Resources["UnityContainer"] is IUnityContainer container)
            {
                _clientsDbContext = container.Resolve<ClientsDbContext>();
            }

            InitializeAsync();
        }

        #endregion

        #region Private methods

        private async void InitializeAsync()
        {
            await LoadDataAsync();
            FilteredClients = CollectionViewSource.GetDefaultView(Clients);
        }

        private async Task LoadDataAsync()
        {
            try
            {
                Clients = new ObservableCollection<Client>(
                    await _clientsDbContext.Clients
                        .Include(c => c.ProductType)
                        .Include(c => c.OperationType)
                        .ToListAsync()
                );

                HistoricalClients = new ObservableCollection<Client>(
                    await _clientsDbContext.Clients
                        .Where(x => x.DateOut != null)
                        .Include(c => c.ProductType)
                        .Include(c => c.OperationType)
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
            if (Clients == null)
                return;

            if (string.IsNullOrWhiteSpace(FilterText))
            {
                FilteredClients = CollectionViewSource.GetDefaultView(Clients);
                return;
            }

            var filtered = new ObservableCollection<Client>(
                Clients.Where(c => DoesClientMatchFilterText(c, FilterText))
            );

            FilteredClients = CollectionViewSource.GetDefaultView(filtered);
        }

        private bool DoesClientMatchFilterText(Client client, string filterText)
        {
            var filterCriteria = new List<string> { "FirstName", "LastName", "Telephone" };
            var searchText = filterText.ToLower();
            foreach (var property in client.GetType().GetProperties())
            {
                if (filterCriteria.Contains(property.Name))
                {
                    var value = property.GetValue(client)?.ToString()?.ToLower();
                    if (value != null && value.Contains(searchText))
                        return true;
                }
            }
            return false;
        }

        #endregion

        //private void AddClient()
        //{
        //    // Logic to open a separate window to add a new client
        //    // This window can bind to a separate AddClientViewModel
        //}

        //private void EditClient()
        //{
        //    if (SelectedClient != null)
        //    {
        //        // Logic to open a separate window to edit the selected client
        //        // This window can bind to a separate EditClientViewModel
        //    }
        //}

        // Implement INotifyPropertyChanged for property change notification
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
