using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace eQuipMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TransferWithoutWorkOrderPage : ContentPage
    {
        string _siteID;
        string _Site, _Location, _Sublocation = null, _People = null;
        private SQLiteConnection _connection;
        private List<TransferClassData> transferList;
        private bool _Assign;
        AssetDetailNames _Names = JsonConvert.DeserializeObject<AssetDetailNames>(Application.Current.Properties["DesignInformation"].ToString());
        public TransferWithoutWorkOrderPage(SQLiteConnection connection_, bool Assign, bool IsTransferWithWorkorder = false)
        {
            _Assign = Assign;
            MessagingCenter.Subscribe<string>(this, "Datawedge", (sender) => {
                TransferSearchBar.Text = sender;
            });
            MessagingCenter.Subscribe<TransferModalpage, string>(this, "TransferAsset", (sender, arg) => {
                AddTransferAsset(arg);
            });
            _connection = connection_;
            InitializeComponent();
            transferTypevisibily.IsVisible = IsTransferWithWorkorder;
            LoadtransferTypes();
            BarcodeImage.Source = ImageSource.FromResource("eQuipMobile.Images.Camera_icon.png");
            if (Application.Current.MainPage.Width < 768)
                BarcodeImage.Margin = new Thickness(10, 0, 10, 0);
            if (Assign)
            {
                LoadPerson();
                Title = "Assign To Person";
                Peopledisplay.IsVisible = true;
                HeaderText.Text = "Select the " + _Names.Site + ", " + _Names.Location + ", " + _Names.SubLocation + " and " + _Names.People + " you would like to assign an asset to then scan the asset that you would like to assign";
                TransferringAssetsText.Text = "Assigning assets count";
                TransferAssetButton.Text = "Assign Asset(s)";
            }
            else
                HeaderText.Text = "Select the " + _Names.Site + ", " + _Names.Location + ", " + _Names.SubLocation + " and " + _Names.People + " you would like to transfer an asset to then scan the asset that you would like to transfer";
            LoadSites();
            SiteName_.Text = _Names.Site;
            LocationName_.Text = _Names.Location;
            SubLocationName_.Text = _Names.SubLocation;
            PersonName_.Text = _Names.People;
        }

        private void LoadtransferTypes()
        {
            if (transferTypevisibily.IsVisible)
            {
                transfertype_.Items.Add("Temporary");
                transfertype_.Items.Add("Permanent");
                transfertype_.Items.Add("Retire");
                transfertype_.Items.Add("Disposed");
                transfertype_.Items.Add("Auction");
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (TransferSearchBar.Text == null)
                await DisplayAlert("Warning", "Please enter a barcode.", "OK");
            else
            {
                if (assetLocation_.SelectedIndex < 0)
                {
                    await DisplayAlert("Warning", "Please select from the dropdown where the asset will be moved to.", "OK");
                }
                else
                {
                    GetDropDownData();
                    var dataList = AssetDisplayClass.FullDetails(Database.Assets.GetDisplayeDataByBarcodeLike(_connection, TransferSearchBar.Text), _Names);
                    if (dataList.Count() > 0)
                    {
                        if (dataList.Count() == 1)
                        {
                            //single asset
                            await Navigation.PushModalAsync(new TransferModalpage(_Names, _connection, 0, dataList.First(), _Site, _Location, _Sublocation, _People));
                        }
                        else
                        {
                            //multiple assets
                            await Navigation.PushModalAsync(new TransferListPage(_Names, _connection, dataList, _Site, _Location, _Sublocation, _People));
                        }
                    }
                    else
                    {
                        await DisplayAlert("Warning", "That asset was not found in the database.", "OK");
                    }
                }
            }
        }

        private void AddTransferAsset( string data)
        {
            try
            {
                var data_ = JsonConvert.DeserializeObject<TransferClassData>(data);
                if (transferList == null)
                {
                    var toSite = SitesClass.DbToSite(Database.Sites.GetTableData(_connection), _connection).First(cm => cm.SiteIdInternal == data_.ToSite);
                    data_.SiteNameTo = toSite.SiteName;
                    var fromsite = SitesClass.DbToSite(Database.Sites.GetTableData(_connection), _connection).First(cm => cm.SiteIdInternal == data_.FromSite);
                    data_.SiteNameFrom = fromsite.SiteName;
                    data_.LocationNameTo = LocationClass.DbToLocation(Database.Locations.GetTableDataFromSites(_connection, toSite.SiteIdInternal), _connection).First(cm => cm.LocationIdInternal == data_.ToLocation).LocationName;
                    data_.LocationNameFrom = LocationClass.DbToLocation(Database.Locations.GetTableDataFromSites(_connection, fromsite.SiteIdInternal), _connection).First(cm => cm.LocationIdInternal == data_.FromLocation).LocationName;
                    transferList = new List<TransferClassData> { data_ };
                } 
                else
                {
                    var toSite = SitesClass.DbToSite(Database.Sites.GetTableData(_connection), _connection).First(cm => cm.SiteIdInternal == data_.ToSite);
                    data_.SiteNameTo = toSite.SiteName;
                    var fromsite = SitesClass.DbToSite(Database.Sites.GetTableData(_connection), _connection).First(cm => cm.SiteIdInternal == data_.FromSite);
                    data_.SiteNameFrom = fromsite.SiteName;
                    data_.LocationNameTo = LocationClass.DbToLocation(Database.Locations.GetTableDataFromSites(_connection, toSite.SiteIdInternal), _connection).First(cm => cm.LocationIdInternal == data_.ToLocation).LocationName;
                    data_.LocationNameFrom = LocationClass.DbToLocation(Database.Locations.GetTableDataFromSites(_connection, fromsite.SiteIdInternal), _connection).First(cm => cm.LocationIdInternal == data_.FromLocation).LocationName;
                    transferList.Add(data_);
                }

                scanned_.Text = transferList.Count.ToString();
            }catch (Exception exc)
            {
                DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                DisplayAlert("Error", exc.Message, "OK");
            }
           
        }

        private void GetDropDownData()
        {
            var siteName = assetSite_.Items[assetSite_.SelectedIndex];
            _Site = SitesClass.DbToSite(Database.Sites.GetTableData(_connection), _connection).First(cm => cm.SiteName == siteName).SiteIdInternal;
            
            var LocationName = assetLocation_.Items[assetLocation_.SelectedIndex];
            var __ = LocationClass.DbToLocation(Database.Locations.GetTableDataFromSites(_connection, _Site), _connection);
            _Location = __.First(cm => (cm.LocationName == LocationName && cm.SiteIdInternal == _Site)).LocationIdInternal;

            if (assetSubLocation_.SelectedIndex > 0)
                _Sublocation = assetSubLocation_.Items[assetSubLocation_.SelectedIndex];

            if (_Assign)
            {
                if (person_.SelectedIndex >= 0)
                {
                    var personName = person_.Items[person_.SelectedIndex];
                     _People =  PeopleClass.DbToPeople(Database.People.GetTableData(_connection), _connection).First(cm => cm.PeopleName == personName).PeopleIDInternal;
                }
            }
        }

        private void AssetSite__SelectedIndexChanged(object sender, EventArgs e)
        {
            if (assetSite_.SelectedIndex != -1)
            {
                assetLocation_.Items.Clear();
                var siteName = assetSite_.Items[assetSite_.SelectedIndex];
                var SiteForLocation = SitesClass.DbToSite(Database.Sites.GetTableData(_connection), _connection).First(cm => cm.SiteName == siteName);
                LoadLocation(SiteForLocation.SiteIdInternal);
            }
        }

        private void AssetLocation__SelectedIndexChanged(object sender, EventArgs e)
        {
            if (assetLocation_.SelectedIndex != -1)
            {
                assetSubLocation_.Items.Clear();
                if (assetLocation_.SelectedIndex > -1)
                {
                    var locationName = assetLocation_.Items[assetLocation_.SelectedIndex];
                    var LocationInfo = LocationClass.DbToLocation(Database.Locations.GetTableDataFromSites(_connection, _siteID), _connection).First(cm => cm.LocationName == locationName);
                    LoadSublocations(LocationInfo.LocationIdInternal);
                }
            }
        }

        private async void Transfer(object sender, EventArgs e)
        {
            if (Convert.ToInt16(scanned_.Text) == 0)
            {
                if (_Assign)
                    await DisplayAlert("Error", "No Asset has been added to the assign list.", "OK");
                else
                    await DisplayAlert("Error", "No Asset has been added to the transfer list.", "OK");
            }
            else
            {
                await Navigation.PushAsync(new TransferredAssets(_connection,transferList));
            }
        }

        private void LoadSites(string siteID = null)
        {
            //assetSite_.Items.Clear();
            var selectedSiteName = "";
            var sitesList = SitesClass.DbToSite(Database.Sites.GetTableData(_connection), _connection);
            foreach (SitesClass siteClass_ in sitesList)
            {
                assetSite_.Items.Add(siteClass_.SiteName);
                if (siteID != null)
                {
                    if (siteClass_.SiteIdInternal == siteID)
                    {
                        selectedSiteName = siteClass_.SiteName;
                    }
                }
            }
            if (selectedSiteName != "")
                assetSite_.SelectedItem = selectedSiteName;
        }

        private void LoadLocation(string siteID = null, string LocationID = null)
        {
            var selectedLocationName = "";
            //site ID set so that it can be used to get locations later
            _siteID = siteID;
            var locationList = LocationClass.DbToLocation(Database.Locations.GetTableDataFromSites(_connection, siteID), _connection);
            foreach (LocationClass locations_ in locationList)
            {
                assetLocation_.Items.Add(locations_.LocationName);
                if (LocationID != null)
                {
                    if (locations_.LocationIdInternal == LocationID)
                    {
                        selectedLocationName = locations_.LocationName;
                    }
                }
            }
            if (selectedLocationName != "")
                assetLocation_.SelectedItem = selectedLocationName;
            else
                assetLocation_.SelectedIndex = 0;
        }

        private void LoadSublocations(string LocationID = null)
        {
            var selectedSublocation = "";
            var SublocationList = SublocationClass.DbToSubLocation(Database.SubLocations.GetTableDataFromLocations(_connection, LocationID));
            assetSubLocation_.Items.Add("No " + _Names.SubLocation);
            foreach (SublocationClass sublocation in SublocationList)
            {
                assetSubLocation_.Items.Add(sublocation.SubLocationName);
                if (LocationID != null)
                {
                    if (sublocation.Location_ID_Internal == LocationID)
                    {
                        selectedSublocation = sublocation.SubLocationName;
                    }
                }
                if (selectedSublocation != "")
                    assetSubLocation_.SelectedItem = selectedSublocation;
                else
                    assetSubLocation_.SelectedIndex = 0;
            }
        }
        private void LoadPerson()
        {
            var PersonList = PeopleClass.DbToPeople(Database.People.GetTableData(_connection), _connection);
            foreach (PeopleClass person in PersonList)
            {
                person_.Items.Add(person.PeopleName);
            }
            try
            {
                person_.SelectedIndex = 0;
            }
            catch (Exception exc)
            {
                DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                DisplayAlert("Error", "You need people records to use this function", "OK");
                Navigation.PopAsync();
            }
        }
        private void BarcodeIconTapped(object sender, EventArgs e)
        {
            BarcodeScannerPage();
        }
        private async void BarcodeScannerPage()
        {
            try
            {
                //setup options
                var frontCamera = Database.Settings.GetTableData(_connection).FrontCamera;
                var options = new MobileBarcodeScanningOptions
                {
                    AutoRotate = false,
                    UseFrontCameraIfAvailable = frontCamera,
                    TryHarder = true,
                };

                //add options and customize page
                var scanPage = new ZXingScannerPage(options)
                {
                    DefaultOverlayTopText = "Align the barcode within the frame",
                    DefaultOverlayBottomText = string.Empty,
                    DefaultOverlayShowFlashButton = true
                };
                scanPage.Title = "Scanning";
                string _result = "";
                scanPage.OnScanResult += (result) =>
                {
                    // Stop scanning
                    scanPage.IsScanning = false;

                    // Pop the page and show the result
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PopAsync();
                        _result = result.Text;
                        TransferSearchBar.Text = _result;
                    });
                };

                // Navigate to our scanner page
                await Navigation.PushAsync(scanPage);
            }
            catch (Exception exc)
            {
                DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                await DisplayAlert("Error", exc.Message, "OK");
            }
        }
    }
}