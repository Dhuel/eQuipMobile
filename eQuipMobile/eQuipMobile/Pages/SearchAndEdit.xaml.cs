using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace eQuipMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchAndEditPage : ContentPage
    {
        private SQLiteConnection _connection;
        public SearchAndEditPage(SQLiteConnection connection)
        {
            _connection = connection;
            InitializeComponent();
        }
        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SitesPage(_connection));
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ScanBarcodePage(_connection));
        }
        protected override void OnAppearing()
        {
            if (Application.Current.MainPage.Width >= 768 && Application.Current.MainPage.Height >= 1024)
            {
                title_.Margin = new Thickness(0, 50, 0, 100);
                ScanBarcode.WidthRequest = 500;
                ScanBarcode.Margin = new Thickness(0, 100, 0, 100);
                Advancedsearch.Margin = new Thickness(0, 50, 0, 100);
            }
            base.OnAppearing();
        }
    }
}