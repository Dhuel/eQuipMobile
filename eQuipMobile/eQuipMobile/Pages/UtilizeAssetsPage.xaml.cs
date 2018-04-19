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
    public partial class UtilizeAssetsPage : ContentPage
    {
        private SQLiteConnection _connection;
        public UtilizeAssetsPage(SQLiteConnection connection_)
        {
            _connection = connection_;
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new AuditPage(_connection));
            }
            catch(Exception exc)
            {
                DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                await DisplayAlert("Error", exc.Message, "OK");
            }
            
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AuditByPersonPage(_connection));
        }

        protected override void OnAppearing()
        {
            if (Application.Current.MainPage.Width >= 768 && Application.Current.MainPage.Height >= 1024)
            {
                AuditByLocation.WidthRequest = 500;
                AuditByLocation.Margin = new Thickness(0, 100, 0, 100);
                AuditByPeople.Margin = new Thickness(0, 50, 0, 100);
            }
            base.OnAppearing();
        }
    }
}