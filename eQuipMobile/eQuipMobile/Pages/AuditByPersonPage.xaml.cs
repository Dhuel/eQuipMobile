using Newtonsoft.Json;
using SQLite;
using System;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace eQuipMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuditByPersonPage : ContentPage
    {
        private SQLiteConnection _connection;
        private static AssetDetailNames _Names = JsonConvert.DeserializeObject<AssetDetailNames>(Application.Current.Properties["DesignInformation"].ToString());
        public AuditByPersonPage(SQLiteConnection connection_)
        {
            _connection = connection_;
            MessagingCenter.Subscribe<string>(this, "Datawedge", (sender) => {
                AuditbyPersonBar.Text = sender;
            });
            InitializeComponent();
            ScanLabel.Text = "Scan or type the " + _Names.People + " ID or Name into the searchbar to view their assets";
            BarcodeImage.Source = ImageSource.FromResource("eQuipMobile.Images.Camera_icon.png");
            if (Application.Current.MainPage.Width < 768)
                BarcodeImage.Margin = new Thickness(10, 0, 10, 0);
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var test = Database.People.GetTableData(_connection);
            var PeopleData = Database.People.GetTableDataByIdOrAccess(_connection, AuditbyPersonBar.Text);
            if (PeopleData.Count() > 0)
            {
                if (PeopleData.Count() > 1)
                {
                    await Navigation.PushAsync(new PeopleListPage(PeopleData, _connection));
                }
                else
                {
                    var PersonsAssets = Database.Assets.GetAssetDataByPeople(_connection, PeopleData.First().PeopleIDInternal);
                    if (PersonsAssets.Count() == 0)
                        await DisplayAlert("Error", "This person does not have any assets assigned", "OK");
                    else
                        await Navigation.PushAsync(new AuditList(_Names, PersonsAssets, _connection, true, null, null, null, 0, PeopleData.First().PeopleIDInternal, PeopleData.First().PeopleName));
                }
            }
            else
            {
                await DisplayAlert("Error", "Please check "+ _Names.People+ " ID / name.", "OK");
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
                        Navigation.PopModalAsync();
                        _result = result.Text;
                        AuditbyPersonBar.Text = _result;
                    });
                };

                // Navigate to our scanner page
                await Navigation.PushModalAsync(scanPage);
            }
            catch (Exception exc)
            {
                DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                await DisplayAlert("Error", exc.Message, "OK");
            }
        }
    }
}