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
    public partial class TransferAssetsPage : ContentPage
    {
        private SQLiteConnection _connection;
        public TransferAssetsPage(SQLiteConnection connection_)
        {
            _connection = connection_;
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TransferWithoutWorkOrderPage(_connection, false));
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TransferWithoutWorkOrderPage(_connection, false, true));
        }

        protected override void OnAppearing()
        {
            if (Application.Current.MainPage.Width >= 768 && Application.Current.MainPage.Height >= 1024)
            {
                TWWO.WidthRequest = 500;
                TWWO.Margin = new Thickness(0, 100, 0, 100);
            }
            base.OnAppearing();
        }
    }
}