using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using iCustomerCareSystem.Data;
using iCustomerCareSystem.Models;
using iCustomerCareSystem.Views;
using Microsoft.EntityFrameworkCore;
using Ookii.Dialogs.Wpf;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Unity;
using Application = System.Windows.Application;
using MessageBox = System.Windows.Forms.MessageBox;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;

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
        private string _searchText;
        private readonly ClientsDbContext _clientsDbContext;
        private BitmapImage _imageSource;
        private ObservableCollection<Client> _clients;
        private Client? _selectedClient;
        private ObservableCollection<ClientProducts> _activeInWarrantyProducts;
        private ClientProducts _selectedActiveInWarrantyProduct;
        private ClientProducts _selectedHistoricalClient;

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

        public ObservableCollection<ClientProducts> ActiveInWarrantyProducts
        {
            get { return _activeInWarrantyProducts; }
            set
            {
                _activeInWarrantyProducts = value;
                OnPropertyChanged(nameof(ActiveInWarrantyProducts));
            }
        }

        public ClientProducts SelectedActiveInWarrantyProduct
        {
            get { return _selectedActiveInWarrantyProduct; }
            set
            {
                _selectedActiveInWarrantyProduct = value;
                OnPropertyChanged(nameof(SelectedActiveInWarrantyProduct));
            }
        }
        public ClientProducts SelectedHistoricalClient
        {
            get { return _selectedHistoricalClient; }
            set
            {
                _selectedHistoricalClient = value;
                OnPropertyChanged(nameof(SelectedHistoricalClient));
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

        public ICollectionView FilteredClientProducts
        {
            get => _filteredClients;
            set
            {
                if (_filteredClients != value)
                {
                    _filteredClients = value;
                    OnPropertyChanged(nameof(FilteredClientProducts));
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

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(SearchText);
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
        public ICommand AddNewClientProductCommand { get; private set; }
        public ICommand EditClientProductCommand { get; private set; }
        public ICommand PrintServiceEntryCommand { get; private set; }
        public ICommand OpenProductTypesNomenclatorCommand { get; private set; }
        public ICommand ChangeProductStatusCommand { get; private set; }
        public ICommand ReEntryClientProductCommand { get; private set; }
        public ICommand SearchHistoricalProductCommand { get; private set; }
        public ICommand PrintNewServiceEntryCommand { get; private set; }        

        #endregion

        #region Constructor

        public MainWindowViewModel()
        {
            if (System.Windows.Application.Current.Resources["UnityContainer"] is IUnityContainer container)
            {
                _clientsDbContext = container.Resolve<ClientsDbContext>();
            }

            InitializeAsync();

            AddNewClientCommand = new DelegateCommand(AddNewClient);
            EditClientCommand = new DelegateCommand(EditClient);

            AddNewClientProductCommand = new DelegateCommand(AddNewClientProduct);
            EditClientProductCommand = new DelegateCommand(EditClientProduct);
            PrintServiceEntryCommand = new DelegateCommand(PrintServiceEntry);
            OpenProductTypesNomenclatorCommand = new DelegateCommand(OpenProductTypesNomenclator);
            ChangeProductStatusCommand = new DelegateCommand(ChangeProductStatus);
            ReEntryClientProductCommand = new DelegateCommand(ReEntryClientProduct);
            SearchHistoricalProductCommand = new DelegateCommand(SearchHistoricalProduct);
            PrintNewServiceEntryCommand = new DelegateCommand(PrintNewServiceEntry);

            LoadLogo();

            InitializeDefaults();
        }

        private void InitializeDefaults()
        {
            InitializeDefaultSaveFileLocation();
        }

        private void InitializeDefaultSaveFileLocation()
        {

            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                var appSettings = config.AppSettings.Settings;
                if (appSettings["DefaultSaveClientProductFileLocation"] != null && string.IsNullOrEmpty(appSettings["DefaultSaveClientProductFileLocation"].Value))
                {
                    string newValue = string.Empty;
                    VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
                    bool? result = dialog.ShowDialog();
                    if (result == true)
                    {
                        newValue = dialog.SelectedPath;
                        appSettings["DefaultSaveClientProductFileLocation"].Value = newValue;

                        // Save the changes back to the configuration file
                        config.Save(ConfigurationSaveMode.Modified);
                        ConfigurationManager.RefreshSection("appSettings");
                    }
                }
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Private methods

        private async void InitializeAsync()
        {
            await LoadDataAsync();
            FilteredClientProducts = CollectionViewSource.GetDefaultView(ClientProducts);
        }

        private async System.Threading.Tasks.Task LoadDataAsync()
        {
            try
            {
                ClientProducts = new ObservableCollection<ClientProducts>(
                     await _clientsDbContext.ClientProducts
                         .Where(x => x.DateOut == null || (x.DateOut != null && x.IsReturnInService))
                         .Include(c => c.Client)
                         .Include(c => c.ProductType)
                         .Include(c => c.ProductStatus)
                         .OrderBy(x => x.ClientProductId)
                         .ToListAsync()
                );

                Clients = new ObservableCollection<Client>(
                    await _clientsDbContext.Clients
                        .OrderBy(x => x.FirstName)
                        .ToListAsync()
                );

                ActiveInWarrantyProducts = new ObservableCollection<ClientProducts>(
                    await _clientsDbContext.ClientProducts
                        .Where(x => x.ProductStatusId == (int)StatusId.Returned && x.WarantyEndDate != null && x.WarantyEndDate >= DateTime.Today)
                        .Include(c => c.Client)
                        .Include(c => c.ProductType)
                        .Include(c => c.ProductStatus)
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
                FilteredClientProducts = CollectionViewSource.GetDefaultView(ClientProducts);
                return;
            }

            var filtered = new ObservableCollection<ClientProducts>(
                ClientProducts.Where(c => DoesClientMatchFilterText(c, FilterText))
            );

            FilteredClientProducts = CollectionViewSource.GetDefaultView(filtered);
        }

        private bool DoesClientMatchFilterText(ClientProducts clientProduct, string filterText)
        {
            var filterCriteria = new List<string> { "ClientProductId", "FirstName", "LastName", "Telephone" };
            var searchText = filterText.ToLower();

            // for ClientProduct
            foreach (var property in clientProduct.GetType().GetProperties())
            {
                if (filterCriteria.Contains(property.Name))
                {
                    var value = property.GetValue(clientProduct)?.ToString()?.ToLower();
                    if (value != null && value.Contains(searchText))
                        return true;
                }
            }

            // for Client reference
            foreach (var property in clientProduct.Client.GetType().GetProperties())
            {
                if (filterCriteria.Contains(property.Name))
                {
                    var value = property.GetValue(clientProduct.Client)?.ToString()?.ToLower();
                    if (value != null && value.Contains(searchText))
                        return true;
                }
            }
            return false;
        }

        #endregion

        private async void SearchHistoricalProduct()
        {
            if (!string.IsNullOrEmpty(SearchText))
            {
                var searchText = SearchText.ToLower();
                HistoricalClients = new ObservableCollection<ClientProducts>(
                    await _clientsDbContext.ClientProducts
                    .Where(x => x.DateOut != null && x.ProductStatusId == (int)StatusId.Returned && (x.Client.FirstName.ToLower().Contains(searchText) || x.Client.LastName.ToLower().Contains(searchText)))
                    .Include(c => c.Client)
                             .Include(c => c.ProductType)
                             .Include(c => c.ProductStatus)
                             .OrderBy(x => x.ClientProductId)
                             .ToListAsync());
            }
        }

        private void AddNewClientProduct()
        {
            AddNewClientProduct(_clientsDbContext, SelectedClient);
        }

        private void AddNewClientProduct(ClientsDbContext clientsDbContext, Client? client)
        {
            var addEditProductViewModel = new AddOrEditProductViewModel(clientsDbContext, client);
            AddOrEditProductView childWindow = new AddOrEditProductView(addEditProductViewModel);
            childWindow.Owner = Application.Current.MainWindow;
            childWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            addEditProductVie`wModel.ChildWindow = childWindow;
            addEditProductViewModel.ClientAddedSuccessfully += HandleClientAddedSuccessfully;
            childWindow.ShowDialog();
        }

        private void AddNewClient()
        {
            _selectedClient = null;
            EditClient();
        }

        private void EditClient()
        {
            var addEditViewModel = new AddOrEditClientViewModel(_clientsDbContext, SelectedClient);
            AddOrEditClientVIew childWindow = new AddOrEditClientVIew(addEditViewModel);
            childWindow.Owner = Application.Current.MainWindow;
            childWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            addEditViewModel.ChildWindow = childWindow;
            addEditViewModel.ClientAddedSuccessfully += HandleClientAddedSuccessfully;
            childWindow.ShowDialog();
        }

        private void EditClientProduct()
        {
            EditClientProduct(_clientsDbContext, SelectedClientProduct);
        }

        private void EditClientProduct(ClientsDbContext dbContext, ClientProducts clientProduct)
        {
            var addEditProductViewModel = new AddOrEditProductViewModel(dbContext, clientProduct);
            AddOrEditProductView childWindow = new AddOrEditProductView(addEditProductViewModel);
            childWindow.Owner = Application.Current.MainWindow;
            childWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            addEditProductViewModel.ChildWindow = childWindow;
            addEditProductViewModel.ClientAddedSuccessfully += HandleClientAddedSuccessfully;
            childWindow.ShowDialog();
        }

        private void ReEntryClientProduct()
        {
            ReEntryClientProduct(SelectedActiveInWarrantyProduct, false);
        }

        private async void ReEntryClientProduct(ClientProducts clientProduct, bool isWaranty)
        {
            if (clientProduct != null)
            {
                clientProduct.IsReturnInService = isWaranty ? true : false;
                clientProduct.ProductStatusId = (int)StatusId.Recepted;
                clientProduct.StatusReason = isWaranty ? "Returnare in service - garantie" : string.Empty;
                if (isWaranty)
                {
                    var newEntryProduct = CopyObjectWithoutId(clientProduct);
                    newEntryProduct.DateIn = DateTime.Now;
                    newEntryProduct.DateOut = null;
                    newEntryProduct.IsUrgent = false;
                    newEntryProduct.StatusReason = string.Empty;
                    newEntryProduct.Fixed = null;
                    newEntryProduct.WarantyEndDate = null;
                    newEntryProduct.Price = 0;
                    _clientsDbContext.ClientProducts.Add(newEntryProduct);
                    try
                    {
                        await _clientsDbContext.SaveChangesAsync();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    return;
                }
                var addEditProductViewModel = new AddOrEditProductViewModel(_clientsDbContext, clientProduct);
                AddOrEditProductView childWindow = new AddOrEditProductView(addEditProductViewModel);
                childWindow.Owner = Application.Current.MainWindow;
                childWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                addEditProductViewModel.ChildWindow = childWindow;
                addEditProductViewModel.ClientAddedSuccessfully += HandleClientAddedSuccessfully;
                childWindow.ShowDialog();
            }
        }

        static T CopyObjectWithoutId<T>(T original)
        {
            T copied = Activator.CreateInstance<T>();
            PropertyInfo[] properties = typeof(T).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (property.Name != "ClientProductId")
                {
                    property.SetValue(copied, property.GetValue(original));
                }
            }

            return copied;
        }

        private void OpenProductTypesNomenclator()
        {
            var openProductTypeNomenclatorViewModel = new EquipmentTypeViewModel(_clientsDbContext);
            EquipmentTypeView childWindow = new EquipmentTypeView(openProductTypeNomenclatorViewModel);
            childWindow.Owner = Application.Current.MainWindow;
            childWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            openProductTypeNomenclatorViewModel.ChildWindow = childWindow;
            openProductTypeNomenclatorViewModel.ClientAddedSuccessfully += HandleClientAddedSuccessfully;
            childWindow.ShowDialog();
        }

        private void ChangeProductStatus()
        {
            var changeProductStatusViewModel = new ChangeStatusViewModel(_clientsDbContext, SelectedClientProduct);
            ChangeStatusView childWindow = new ChangeStatusView(changeProductStatusViewModel);
            childWindow.Owner = Application.Current.MainWindow;
            childWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            changeProductStatusViewModel.ChildWindow = childWindow;
            changeProductStatusViewModel.ClientAddedSuccessfully += HandleClientAddedSuccessfully;
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
            if (SelectedClientProduct != null)
            {
                PrintServiceEntry(SelectedClientProduct);
            }
        }

        private void PrintNewServiceEntry()
        {
            if (SelectedHistoricalClient != null)
            {
                ReEntryClientProduct(SelectedHistoricalClient, true);
            }
        }

        private void PrintServiceEntry(ClientProducts selectedClientProduct)
        {
            string outputPath = ConfigurationManager.AppSettings["DefaultSaveClientProductFileLocation"].ToString();
            if (string.IsNullOrWhiteSpace(outputPath))
                return;

            Dictionary<string, string> bookmarksDictionary = PrepareBookmarks(selectedClientProduct ?? new ClientProducts());

            string? executingLocation = Path.GetDirectoryName(path: System.Reflection.Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrWhiteSpace(executingLocation))
                return;

            string resourcesFolder = Path.Combine(executingLocation, "Resources");
            string templatePath = Path.Combine(resourcesFolder, "fisa_service_template.docx");
            string docName = selectedClientProduct?.Client.FirstName + " " + selectedClientProduct?.Client.LastName + " " + selectedClientProduct?.DateIn.ToString("dd-mm-yyy") + ".docx";
            string newFilePath = Path.Combine(outputPath, docName);

            if (File.Exists(newFilePath))
            {
                File.Delete(newFilePath);
            }

            File.Copy(templatePath, newFilePath, true);

            bool isSuccess = false;
            try
            {
                isSuccess = CreateServiceEntryDocument(bookmarksDictionary, newFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (isSuccess)
                {
                    DialogResult result = MessageBox.Show("Do you want to open the document?", "Open Document", MessageBoxButtons.YesNo);

                    if (result == DialogResult.Yes)
                    {
                        OpenDocument(Path.GetFullPath(newFilePath));
                    }
                }
            }
        }

        private void OpenDocument(string filePath)
        {
            Type wordType = Type.GetTypeFromProgID("Word.Application");

            if (wordType != null)
            {
                dynamic wordApp = Activator.CreateInstance(wordType);

                try
                {
                    wordApp.Visible = true; 

                    // Open the document
                    dynamic doc = wordApp.Documents.Open(filePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error");
                }
                finally
                {
                }
            }
            else
            {
                MessageBox.Show("Microsoft Word is not installed.", "Error");
            }
        }

        private bool CreateServiceEntryDocument(Dictionary<string, string> bookmarksDictionary, string newFilePath)
        {
            bool isSuccess;
            using (WordprocessingDocument doc = WordprocessingDocument.Open(newFilePath, true))
            {
                MainDocumentPart? mainPart = doc.MainDocumentPart;

                foreach (var item in bookmarksDictionary)
                {
                    BookmarkStart? bookmarkStart = mainPart?.Document?.Body?.Descendants<BookmarkStart>()
                                                                        .FirstOrDefault(b => b.Name == item.Key);

                    if (bookmarkStart != null)
                    {
                        Run newRun = new Run(new Text(item.Value));
                        RunProperties runProperties = new RunProperties();
                        newRun.Append(runProperties);

                        Run run = new Run();
                        run.Append(newRun);

                        bookmarkStart?.Parent?.InsertAfter(run, bookmarkStart);
                    }
                }

                // Save changes
                mainPart?.Document.Save();
                isSuccess = true;
            }

            return isSuccess;
        }

        private Dictionary<string, string> PrepareBookmarks(ClientProducts selectedClientProduct)
        {
            Dictionary<string, string> bookmarks = new Dictionary<string, string>
            {
                { "numar_fisa", selectedClientProduct.ClientProductId.ToString() },
                { "data_fisa", selectedClientProduct.DateIn.ToString("dd-MM-yyyy") },
                { "client_name", selectedClientProduct.Client.FirstName + " " + selectedClientProduct.Client.LastName },
                { "telefon", selectedClientProduct.Client.Telephone },
                { "echipament", selectedClientProduct.ProductType.Name + " " +  selectedClientProduct.ProductName},
                { "serial_no", selectedClientProduct.ProductConfiguration },
                { "defect", selectedClientProduct.ServiceOperation },
                { "accesorii", selectedClientProduct.ProductConfiguration },
                { "observatii", selectedClientProduct.Reason }
            };


            return bookmarks;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
