using Newtonsoft.Json;
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
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PeopleListPage : ContentPage
    {
        private SQLiteConnection _connection;
        private static AssetDetailNames _Names = JsonConvert.DeserializeObject<AssetDetailNames>(Application.Current.Properties["DesignInformation"].ToString());
        public PeopleListPage(IEnumerable<PeopleDbTable> PeopleData, SQLiteConnection connection)
        {
            _connection = connection;
            InitializeComponent();
            PeopleListView.ItemsSource = PeopleData;
        }

        private void PeopleListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            PeopleListView.SelectedItem = null;
        }

        private async void PeopleListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var Person = e.Item as PeopleDbTable;
            var PersonsAssets = Database.Assets.GetAssetDataByPeople(_connection, Person.PeopleIDInternal);
            if (PersonsAssets.Count() == 0)
                await DisplayAlert("Error", "This person does not have any assets assigned", "OK");
            else
                await Navigation.PushAsync(new AuditList(_Names, PersonsAssets, _connection, true, null, null, null, 0, Person.PeopleIDInternal, Person.PeopleName));
        }
    }
}