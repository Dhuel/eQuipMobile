using Newtonsoft.Json;
using Plugin.Connectivity;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace eQuipMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SetMobileSitesPage : ContentPage
    {
        private SQLiteConnection _connection;
        private List<MobileSitesClass> _AllSites;
        private ObservableCollection<MobileSitesClass> _allSitesObs;
        public SetMobileSitesPage(SQLiteConnection _connection_, string sites)
        {
            _connection = _connection_;
            InitializeComponent();
            LoadSitesList(sites);


        }
        private void LoadSitesList(string _sites)
        {
            _AllSites = JsonConvert.DeserializeObject<List<MobileSitesClass>>(_sites);
            //Convert the table data to mobilesitesdata
            var MobileSites = JsonConvert.DeserializeObject<List<MobileSitesClass>>(JsonConvert.SerializeObject(Database.Sites.GetTableData(_connection)));
            //find the values that match both
            foreach (var item1 in _AllSites)
            {
                foreach (var item2 in MobileSites)
                {
                    if (item2.SiteIdInternal == item1.SiteIdInternal)
                    {
                        _AllSites.Where(c => c.SiteIdInternal.Contains(item2.SiteIdInternal)).First().Toggled_ = true;
                        break;
                    }
                }
            }
            _allSitesObs = new ObservableCollection<MobileSitesClass>(_AllSites);
            sitesListView.ItemsSource = GetMobileSites(searchBar.Text);
        }

        private void SitesListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            sitesListView.SelectedItem = null;

        }

        private void SitesListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var site = e.Item as MobileSitesClass;
            var selected = _AllSites.Where(c => c.SiteIdInternal.Contains(site.SiteIdInternal)).First();
            if (selected.Toggled_ == true)
                selected.Toggled_ = false;
            else
                selected.Toggled_ = true;
            _allSitesObs = new ObservableCollection<MobileSitesClass>(_AllSites);
            sitesListView.ItemsSource = GetMobileSites(searchBar.Text);
        }

        private async void SaveMobileSites(object sender, EventArgs e)
        {
            try
            {
                var SelectedSites = _allSitesObs.Where(c => c.Toggled_ == true).ToList();
                //check connectivity
                if (CrossConnectivity.Current.IsConnected)
                {
                    //convert to mobile sites send Class then try to send that data
                    var sentSites = new MobileSitesSender(SelectedSites, Database.Login.GetTableData(_connection).First().UserID);
                    if (await SyncClass.SetMobileSites(sentSites, Application.Current.Properties["url"].ToString()))
                    {
                        await DisplayAlert("Complete", "Mobile sites saved. Sync now to update data.", "OK");
                        await Navigation.PopToRootAsync();
                    }  
                    else
                        await DisplayAlert("Error", "Unable to set mobile sites.", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "No internet connection. Unable to set mobile sites", "OK");
                }
            }
            catch(Exception exc)
            {
                DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                await DisplayAlert("Error", exc.ToString(), "OK");
            }
            
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            sitesListView.ItemsSource = GetMobileSites(e.NewTextValue);
        }
        ObservableCollection<MobileSitesClass> GetMobileSites(string searchText = null)
        {
            //Return List od SitesClass
            if (String.IsNullOrWhiteSpace(searchText))
                return _allSitesObs;
            var temp = new ObservableCollection<MobileSitesClass>(_allSitesObs.Where(c => c.SiteName.Contains(searchText)).ToList());
            return temp;
        }
        private void SearchBar_SearchButtonPressed(object sender, EventArgs e)
        {

        }
    }
}