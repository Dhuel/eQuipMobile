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
    public partial class AssetListPage : ContentPage
    {
        LocationClass _location;
        SublocationClass _sublocation;
        DepartmentClass _department;
        private SQLiteConnection _connection;
        AssetDetailNames _Names;
        public AssetListPage(SQLiteConnection connection_ , LocationClass location, AssetDetailNames Names_, SublocationClass sublocation = null, DepartmentClass department = null)
        {
            _Names = Names_;
            MessagingCenter.Subscribe<string>(this, "Datawedge", (sender) => {
                SearchBar_.Text = sender;
            });
            if (department != null)
                BindingContext = department ?? throw new ArgumentNullException();
            else if (sublocation != null)
                BindingContext = sublocation ?? throw new ArgumentNullException();
            else
                BindingContext = location ?? throw new ArgumentNullException();
            _location = location;
            _sublocation = sublocation;
            _department = department;
            InitializeComponent();
            BarcodeImage.Source = ImageSource.FromResource("eQuipMobile.Images.Camera_icon.png");
            _connection = connection_;
            AssetListView.ItemsSource = GetAssetList(_location, _sublocation, _department);
        }

        List<AssetClass> GetAssetList(LocationClass location, SublocationClass sublocation, DepartmentClass department, string searchText = null)
        {
            var AssetList = AssetClass.DbToAssetClass(Database.Assets.GetAssetListForSearch(_connection, location, sublocation, department), _Names);
            if (String.IsNullOrWhiteSpace(searchText))
                return AssetList;
            return AssetList.Where(c => (c.AssetName.Contains(searchText)|| c.Barcode.Contains(searchText))).ToList();
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            AssetListView.ItemsSource = GetAssetList(_location, _sublocation, _department, e.NewTextValue);
        }

        private void AssetListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var Asset_list = e.Item as AssetClass;
            Navigation.PushAsync(new AddAssetPage(_connection, Asset_list));
            //DisplayAlert("Asset record gotten", Asset_list.AssetName, "OK");
        }

        private void AssetListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            AssetListView.SelectedItem = null;
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

                var options = new MobileBarcodeScanningOptions
                {
                    AutoRotate = false,
                    UseFrontCameraIfAvailable = false,
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