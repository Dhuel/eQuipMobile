using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace eQuipMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CheckinCheckoutPage : ContentPage
    {
        private SQLiteConnection _connection;
        private string _PropertyPass;
        private PropertyPassDbTable _PPTable;
        public CheckinCheckoutPage(SQLiteConnection connection, string PropertyPassID)
        {
            var PPItemRecords = Database.PropertyPassItem.GetTableDataByPropertyPassTable(connection, PropertyPassID);
            _connection = connection;
            _PropertyPass = PropertyPassID;
            _PPTable = Database.PropertyPassTable.GetTableDataByID(_connection, _PropertyPass);
            InitializeComponent();
            if (PPItemRecords.Count() > 0)
                CheckIn.IsEnabled = true;
            else
                CheckOut.IsEnabled = true;
        }

        private async void CheckOut_Clicked(object sender, EventArgs e)
        {
            //go to scan page where they can scan assets to check out
            await Navigation.PushAsync(new ScanBarcodePage(_connection, null, true, _PPTable));
        }

        private async void CheckIn_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PropertyPassItemsList(_connection, _PPTable));
        }
    }
}