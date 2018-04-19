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
    public partial class PropertyPassListPage : ContentPage
    {
        private SQLiteConnection _connection;
        public PropertyPassListPage(SQLiteConnection _connection_)
        {
            _connection = _connection_;
            InitializeComponent();
            var PropertyPassData_ = Database.PropertyPassTable.GetTableData(_connection);
            PropertyPassList.ItemsSource = PropertyPassData_;
        }

        private void PropertyPassList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            PropertyPassList.SelectedItem = null;
        }

        private async void PropertyPassList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            //select list of property pass items within prop
            var PropertyPassTable = e.Item as PropertyPassDbTable;
            //go to page that will have check in/ check out
            await Navigation.PushAsync(new CheckinCheckoutPage(_connection, PropertyPassTable.PropertyPassID_Internal));
        }

        private async void AddPropertyPass(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddPropertyPassPage(_connection));
        }
    }
}