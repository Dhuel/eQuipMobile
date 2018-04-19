using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace eQuipMobile
{
    //TODO - mobile sites need to go to database, pull all sites and select the sites that match then save the selected records to Settings. in update, check if new mobile sites were set if they were, set them before getting all sietes
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        SettingsDb _settings;
        private SQLiteConnection _connection;
        bool loading = true;
        public SettingsPage(SQLiteConnection connection_)
        {
            InitializeComponent();
            _connection = connection_;
            _settings = Database.Settings.GetTableData(_connection);
            BindingContext = _settings;
            loading = false;
        }

        private void Settings_OnChanged(object sender, ToggledEventArgs e)
        {
            if (!loading)
            {
                var current_settings = StoreDetails();
                Database.Settings.UpdateData(_connection, current_settings);
                DisplayAlert("Complete", "Settings updated!", "OK");
            }
                
        }
        public SettingsDb StoreDetails()
        {
            var settings_ = new SettingsDb
            {
                FastAudit = fastaudit.IsToggled,
                BlindAudit = blindaudit.IsToggled,
                PriceLock = pricelock.IsToggled,
                FrontCamera = frontcamera.IsToggled,
                FastAuditEntry = fastauditentry.IsToggled
            };
            return settings_;
        }

        private async void ShowMobileSites(object sender, EventArgs e)
        {
            try
            {
                var sites = await SyncClass.NewSyncClass.API_GetAllSites(Database.Login.GetTableData(_connection).First().UserID, Application.Current.Properties["url"].ToString());
                await Navigation.PushAsync(new SetMobileSitesPage(_connection, sites));
            }
            catch (Exception exc)
            {
                DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                await DisplayAlert("Error", "Unable to load mobile sites. Please log in again.", "OK");
                throw (exc);
            }
        }
    }
}