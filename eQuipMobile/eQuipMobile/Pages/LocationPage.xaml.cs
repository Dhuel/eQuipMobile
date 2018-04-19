using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace eQuipMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocationPage : ContentPage
    {
        private SQLiteConnection _connection;
        string _locationSiteID;
        private AssetDetailNames _Names;
        public LocationPage(SQLiteConnection connection_, SitesClass site, AssetDetailNames Names_)
        {
            _Names = Names_;
            MessagingCenter.Subscribe<string>(this, "Datawedge", (sender) => {
                SearchBar_.Text = sender;
            });
            BindingContext = site ?? throw new ArgumentNullException();
            InitializeComponent();
            BarcodeImage.Source = ImageSource.FromResource("eQuipMobile.Images.Camera_icon.png");
            _connection = connection_;
            _locationSiteID = site.SiteIdInternal;
            locationListView.ItemsSource = GetLocations(_locationSiteID);

        }
        List<LocationClass> GetLocations(string site_id, string searchText = null)
        {
            var locations = LocationClass.DbToLocation(Database.Locations.GetTableDataFromSites(_connection, site_id), _connection, _Names);
            if (String.IsNullOrWhiteSpace(searchText))
                return locations;
            return locations.Where(c => (c.LocationName.Contains(searchText)|| c.LocationBarcode.Contains(searchText))).ToList();
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            locationListView.ItemsSource = GetLocations(_locationSiteID, e.NewTextValue);
        }

        private void LocationListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            locationListView.SelectedItem = null;
        }

        private async void LocationListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var location_ = e.Item as LocationClass;
            var SublocationsPresent = Database.SubLocations.GetTableDataFromLocations(_connection, location_.LocationIdInternal);
            if (SublocationsPresent.Count() > 0)
                await Navigation.PushAsync(new SublocationPage(_connection, location_, _Names));
            else
                await Navigation.PushAsync(new DepartmentPage(_connection, location_, _Names));

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
                        SearchBar_.Text = _result;
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