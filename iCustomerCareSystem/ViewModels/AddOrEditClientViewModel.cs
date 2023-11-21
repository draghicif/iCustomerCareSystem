using iCustomerCareSystem.Data;
using iCustomerCareSystem.Models;
using Prism.Commands;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace iCustomerCareSystem.ViewModels
{
    public class AddOrEditClientViewModel: BasePopupViewModel, INotifyPropertyChanged
    {
        private Client _client;
        private readonly ClientsDbContext _clientsDbContext;

        public Client Client
        {
            get { return _client; }
            set
            {
                _client = value;
                OnPropertyChanged(nameof(Client));
            }
        }

        public ICommand SaveCommand { get; }
        public event EventHandler ClientAddedSuccessfully;

        public AddOrEditClientViewModel(ClientsDbContext clientsDbContext, Client? client = null)
        {
            _clientsDbContext = clientsDbContext;
            _client = client ?? new Client();
            SaveCommand = new DelegateCommand(async () => await SaveClientAsync());
        }

        private async Task SaveClientAsync()
        {
            if (Client != null)
            {
                if (_client.ClientId == 0)
                {
                    _clientsDbContext.Clients.Add(Client);
                }
                else
                {
                    _clientsDbContext.Clients.Update(Client);
                }

                await SaveClientAsync(_clientsDbContext);
            }
        }
        private async Task SaveClientAsync(ClientsDbContext clientsDbContext)
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
