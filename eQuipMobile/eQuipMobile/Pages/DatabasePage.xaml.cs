using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace eQuipMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DatabasePage : ContentPage
    {
        private SQLiteConnection _connection;
        //private ObservableCollection<LoginDbTable> _LoginTable;
        //private ObservableCollection<SitesDbTable> _SitesTable;
        private ObservableCollection<SubLocationsDbTable> _SublocationTable;
        public DatabasePage()
        {
            InitializeComponent();
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            //var logindetails = Database.Login.GetLoginTableData(_connection);// QueryValuations(_connection, "Login");
            //if (logindetails.Count() > 0)
            //{
            //    _LoginTable = new ObservableCollection<LoginDbTable>(logindetails);
            //    LoginDatabaseList.ItemsSource = _LoginTable;
            //    base.OnAppearing();
            //}
            //else
            //    DisplayAlert("Alert", "No records found", "Ok");
        }

        private void Button_Clicked1(object sender, EventArgs e)
        {
            //TODO - find out why this isn't working
            //_connection.CreateTable<LoginDbTable>();
            //var Sitesdetails = Database.Sites.GetSiteTableData(_connection);// QueryValuations(_connection, "Login");
            //if (Sitesdetails.Count() > 0)
            //{
            //    _SitesTable = new ObservableCollection<SitesDbTable>(Sitesdetails);
            //    SitesDatabaseList.ItemsSource = _SitesTable;
            //    base.OnAppearing();
            //}
            //else
            //    DisplayAlert("Alert", "No records found", "Ok");

            var Sublocations = Database.SubLocations.GetTableData(_connection);// QueryValuations(_connection, "Login");
            if (Sublocations.Count() > 0)
            {
                _SublocationTable = new ObservableCollection<SubLocationsDbTable>(Sublocations);
                SitesDatabaseList.ItemsSource = _SublocationTable;
                base.OnAppearing();
            }
            else
                DisplayAlert("Alert", "No records found", "Ok");
        }
        private void TestButton(object sender, EventArgs e)
        {
            TestSyncProocess("ebeser2@e-isg.com", "Usevam@123", "564f0b8e-27eb-4fb3-82ea-6bba717e69f9", "https://equip.e-isg.com");
        }
        public async void TestSyncProocess(string username, string password, string UserID, string url)
        {
            try
            {
                //var login_info = await SyncClass.API_Login(username, password, url);
                //await DisplayAlert("Login Complete", login_info, "OK");

                //var Sites = await SyncClass.NewSyncClass.API_GetAllSites(UserID, url);
                //await DisplayAlert("Sites Complete", Sites, "OK");

                //var Locations = await SyncClass.NewSyncClass.API_GetLocation(UserID, url);
                //await DisplayAlert("Get All Locations Complete", Locations, "OK");

                //var SitesStack = JsonConvert.DeserializeObject<Stack<SitesClass>>(Sites);
                //var Locations2 = await SyncClass.NewSyncClass.API_GetLocation(UserID, url, SitesStack);
                //await DisplayAlert("Get All Locations by Site Complete", Locations2, "OK");

                //var assets = await SyncClass.NewSyncClass.API_GetAssets(UserID, url);
                //await DisplayAlert("Get All Assets Complete", assets, "OK");

                //var assetsbysite = await SyncClass.NewSyncClass.API_GetAssets(UserID, url, SitesStack);
                //await DisplayAlert("Get All Assets by site Complete", assetsbysite, "OK");

                //var assetsbyBarcode = await SyncClass.NewSyncClass.API_GetAssetByBarcode(UserID, url, "010101");
                //await DisplayAlert("Get All Assets by barcode Complete", assetsbyBarcode, "OK");

                //var testupdate = TestCode.testAssetupdate(UserID, url);
                //await DisplayAlert("Asset update status", testupdate, "OK");

                //var testcreate = TestCode.testCreateAsset(UserID, url);
                //await DisplayAlert("Asset create status", testcreate, "OK");
            }
            catch (Exception e)
            {
                DependencyService.Get<IError>().SendRaygunError(e, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                await DisplayAlert("Error", e.Message, "OK");
            }


        }
    }
}