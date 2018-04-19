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
    public partial class TransferListPage : ContentPage
    {
        string _Site, _Location, _Sublocation, _People;
        private SQLiteConnection _connection;
        AssetDetailNames _Names;
        public TransferListPage(AssetDetailNames Names_, SQLiteConnection _connection_, IEnumerable<AssetDisplayClass> transferList_, string Site, string Location, string Sublocation = null, string People = null)
        {
            _Names = Names_;
            _connection = _connection_;
            InitializeComponent();
            _Site = Site;
            _People = People;
            _Location = Location;
            _Sublocation = Sublocation;
            TransferListView.ItemsSource = AssetDisplayClass.FullDetails(transferList_, _Names); 
        }
        private void Button_Clicked_1(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private void TransferListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            TransferListView.SelectedItem = null;
        }

        private async void TransferListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var transfer = e.Item as AssetDisplayClass;
            if (transfer.Quantity == 0)
                await DisplayAlert("Error", "This asset has a quantity of 0, it cannot be moved", "OK");
            else
                await Navigation.PushModalAsync(new TransferModalpage(_Names, _connection, 2, transfer, _Site, _Location, _Sublocation, _People));
        }
    }
}