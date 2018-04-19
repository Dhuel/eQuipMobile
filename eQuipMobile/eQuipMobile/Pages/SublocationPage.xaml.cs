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
    public partial class SublocationPage : ContentPage
    {
        LocationClass _location;
        private SQLiteConnection _connection;
        private AssetDetailNames _Names;
        public SublocationPage(SQLiteConnection _connection_, LocationClass location, AssetDetailNames Names_)
        {
            _Names = Names_;
            MessagingCenter.Subscribe<string>(this, "Datawedge", (sender) => {
                SearchBar_.Text = sender;
            });
            BindingContext = location ?? throw new ArgumentNullException();
            _location = location;
            _connection = _connection_;
            var SublocationsPresent = Database.SubLocations.GetTableDataFromLocations(_connection, _location.LocationIdInternal);
            InitializeComponent();
            BarcodeImage.Source = ImageSource.FromResource("eQuipMobile.Images.Camera_icon.png");
            SublocationListView.ItemsSource = GetSublocations(_location);

        }
        List<SublocationClass> GetSublocations(LocationClass location_Class, string searchText = null)
        {
            var Sublocations = SublocationClass.DbToSubLocation(Database.SubLocations.GetTableDataFromLocations(_connection, location_Class.LocationIdInternal), _Names);
            if (String.IsNullOrWhiteSpace(searchText))
                return Sublocations;
            return Sublocations.Where(c => c.SubLocationName.Contains(searchText)).ToList();
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            SublocationListView.ItemsSource = GetSublocations(_location, e.NewTextValue);
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            //go to department Page
            await Navigation.PushAsync(new DepartmentPage(_connection, _location, _Names));
        }

        private void SublocationListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            SublocationListView.SelectedItem = null;
        }

        private async void SublocationListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var sublocation_ = e.Item as SublocationClass;
            sublocation_.LocationName = sublocation_.SubLocationName;
            await Navigation.PushAsync(new DepartmentPage(_connection, _location, _Names, sublocation_));
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