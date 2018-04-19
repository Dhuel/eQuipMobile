using Newtonsoft.Json;
using SQLite;
using System;
using System.Linq;


using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace eQuipMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuditPage : ContentPage
    {
        private SQLiteConnection _connection;
        string _siteID;
        SettingsDb settings;
        private static AssetDetailNames Names_ = JsonConvert.DeserializeObject<AssetDetailNames>(Application.Current.Properties["DesignInformation"].ToString());
        public AuditPage(SQLiteConnection connection_, string SiteID_ = null, string LocID_ = null, string SubLocID_ = null)
        {
            _connection = connection_;
            InitializeComponent();
            HeaderLabel.Text = "Select the "+ Names_.Site+", "+ Names_.Location+ " and/or "+ Names_.SubLocation+ " where you are auditing assets.";
            SiteName.Text = Names_.Site;
            LocationName.Text = Names_.Location;
            SubLocationName.Text = Names_.SubLocation;
            settings = Database.Settings.GetTableData(_connection);
            if (Application.Current.Properties.ContainsKey("AuditedSite") && Application.Current.Properties["AuditedSite"] != null)
                LoadSites(Application.Current.Properties["AuditedSite"].ToString());
            else
                LoadSites();
        }

        private void assetSite__SelectedIndexChanged(object sender, EventArgs e)
        {
            if (assetSite_.SelectedIndex != -1)
            {
                assetLocation_.Items.Clear();
                var siteName = assetSite_.Items[assetSite_.SelectedIndex];
                var SiteForLocation = SitesClass.DbToSite(Database.Sites.GetTableData(_connection), _connection).First(cm => cm.SiteName == siteName);
                if (Application.Current.Properties.ContainsKey("AuditedLocation") && Application.Current.Properties["AuditedLocation"]!=null)
                    LoadLocation(SiteForLocation.SiteIdInternal, Application.Current.Properties["AuditedLocation"].ToString());
                else
                    LoadLocation(SiteForLocation.SiteIdInternal);
            }
        }

        private void assetLocation__SelectedIndexChanged(object sender, EventArgs e)
        {
            if (assetLocation_.SelectedIndex != -1)
            {
                assetSubLocation_.Items.Clear();
                if (assetLocation_.SelectedIndex > -1)
                {
                    var locationName = assetLocation_.Items[assetLocation_.SelectedIndex];
                    var LocationInfo = LocationClass.DbToLocation(Database.Locations.GetTableDataFromSites(_connection, _siteID), _connection).First(cm => cm.LocationName == locationName);
                    if (Application.Current.Properties.ContainsKey("AuditedSublocation") && Application.Current.Properties["AuditedSublocation"]!= null)
                        LoadSublocations(LocationInfo.LocationIdInternal, Application.Current.Properties["AuditedSublocation"].ToString());
                    else
                        LoadSublocations(LocationInfo.LocationIdInternal);
                }
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

        private void LoadSublocations(string LocationID = null, string SublocationName = null)
        {
            var SublocationList = SublocationClass.DbToSubLocation(Database.SubLocations.GetTableDataFromLocations(_connection, LocationID));
            assetSubLocation_.Items.Add("No " + Names_.SubLocation);
            foreach (SublocationClass sublocation in SublocationList)
            {
                assetSubLocation_.Items.Add(sublocation.SubLocationName);
            }
            if (SublocationName != null)
                assetSubLocation_.SelectedItem = SublocationName;
            else
                assetSubLocation_.SelectedIndex = 0;
        }

        private async void SaveAndContinue(object sender, EventArgs e)
        {
            if (assetSite_.SelectedIndex < 0 || assetLocation_.SelectedIndex < 0)
            {
                await DisplayAlert("Error", "Please select  values from the dropdown", "OK");
            }
            else
            {
                var siteName = assetSite_.Items[assetSite_.SelectedIndex];
                var SiteDetails = SitesClass.DbToSite(Database.Sites.GetTableData(_connection), _connection).First(cm => cm.SiteName == siteName);

                var LocationName = assetLocation_.Items[assetLocation_.SelectedIndex];
                var LocationDetails = LocationClass.DbToLocation(Database.Locations.GetTableDataFromSites(_connection, SiteDetails.SiteIdInternal), _connection).First(cm => cm.LocationName == LocationName);

                if (assetSubLocation_.SelectedIndex <= 0)
                {
                    await Navigation.PushAsync(new SaveAndContinuePage(_connection, Names_, SiteDetails.SiteIdInternal, LocationDetails.LocationIdInternal));
                }
                else
                {
                    await Navigation.PushAsync(new SaveAndContinuePage(_connection, Names_, SiteDetails.SiteIdInternal, LocationDetails.LocationIdInternal, assetSubLocation_.Items[assetSubLocation_.SelectedIndex]));
                }

            }

        }

        private void GetData()
        {
            //instead of doing this, get all assets data from that location and send it to audit list
            string locationName;
            LocationClass LocationInfo = new LocationClass();

            //gets location id internal
            if (assetLocation_.SelectedIndex >= 0)
            {
                locationName = assetLocation_.Items[assetLocation_.SelectedIndex];
                LocationInfo = LocationClass.DbToLocation(Database.Locations.GetTableDataFromSites(_connection, _siteID), _connection).First(cm => cm.LocationName == locationName);
            }
            if (assetSubLocation_.SelectedIndex >= 1)
            {
                try
                {
                    var sublocation = SublocationClass.DbToSubLocation(Database.SubLocations.GetTableData(_connection)).First(cm => cm.SubLocationName == assetSubLocation_.Items[assetSubLocation_.SelectedIndex]);
                    Navigation.PushAsync(new AssetListPage(_connection, LocationInfo, Names_, sublocation));
                }
                catch (Exception exc)
                {
                    DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                    Navigation.PushAsync(new AssetListPage(_connection, LocationInfo, Names_));
                }
            }
            else
            {
                Navigation.PushAsync(new AssetListPage(_connection, LocationInfo, Names_));
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (assetSite_.SelectedIndex < 0 || assetLocation_.SelectedIndex < 0)
            {
                await DisplayAlert("Error", "Please select  values from the dropdown", "OK");
            }
            else
            {
                try
                {
                    GetData();
                }
                catch (Exception exc)
                {
                    DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                    await DisplayAlert("Error", exc.Message, "OK");
                }
            }
        }
    }
}