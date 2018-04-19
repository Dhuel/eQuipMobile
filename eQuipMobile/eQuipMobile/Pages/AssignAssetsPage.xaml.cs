using SQLite;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace eQuipMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AssignAssetsPage : ContentPage
    {
        private SQLiteConnection _connection;
        public AssignAssetsPage(SQLiteConnection connection_)
        {

            _connection = connection_;
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PropertyPassListPage(_connection));
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TransferWithoutWorkOrderPage(_connection, true));
        }
        protected override void OnAppearing()
        {
            if (Application.Current.MainPage.Width >= 768 && Application.Current.MainPage.Height >= 1024)
            {
                PropertyPass.WidthRequest = 500;
                PropertyPass.Margin = new Thickness(0, 100, 0, 100);
                AssignPerson.Margin = new Thickness(0, 50, 0, 100);
            }
            base.OnAppearing();
        }
    }
}