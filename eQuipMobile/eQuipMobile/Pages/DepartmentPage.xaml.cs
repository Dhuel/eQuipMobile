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
    public partial class DepartmentPage : ContentPage
    {
        LocationClass _location;
        SublocationClass _sublocation;
        private SQLiteConnection _connection;
        private AssetDetailNames _Names;
        public DepartmentPage(SQLiteConnection connection_, LocationClass location, AssetDetailNames Names_, SublocationClass sublocation = null)
        {
            _Names = Names_;
            MessagingCenter.Subscribe<string>(this, "Datawedge", (sender) => {
                SearchBar_.Text = sender;
            });
            if (sublocation != null)
                BindingContext = sublocation ?? throw new ArgumentNullException();
            else
                BindingContext = location ?? throw new ArgumentNullException();
            _location = location;
            _sublocation = sublocation;
            InitializeComponent();
            BarcodeImage.Source = ImageSource.FromResource("eQuipMobile.Images.Camera_icon.png");
            _connection = connection_;
            DepartmentListView.ItemsSource = GetDepartments();
        }

        List<DepartmentClass> GetDepartments(string searchText = null)
        {
            var Departments = DepartmentClass.DbToDepartment(Database.Department.GetTableData(_connection));
            if (String.IsNullOrWhiteSpace(searchText))
                return Departments;
            return Departments.Where(c => c.DepartmentName.Contains(searchText)).ToList();
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            DepartmentListView.ItemsSource = GetDepartments(e.NewTextValue);
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new MainPage());
            await Navigation.PushAsync(new AssetListPage(_connection, _location, _Names, _sublocation));
        }

        private void DepartmentListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            DepartmentListView.SelectedItem = null;
        }

        private async void DepartmentListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var department_ = e.Item as DepartmentClass;
            await Navigation.PushAsync(new AssetListPage(_connection, _location,_Names, _sublocation, department_));
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