using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace eQuipMobile
{
    public partial class MainPage : MasterDetailPage
    {
        private SQLiteConnection _connection;
        public MainPage()
        {
            _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
            InitializeComponent();
            if (Device.RuntimePlatform == Device.iOS)
                Padding = new Thickness(0, 20, 20, 0);
            logoImage.Source = ImageSource.FromResource("eQuipMobile.Images.equipLogo.png", Assembly.GetExecutingAssembly());
            NavigationListView.ItemsSource = LoadNavigationMenu();

            if (Application.Current.Properties.ContainsKey("user") && Application.Current.Properties["user"].ToString() != "")
            {
                HomeLabel.Text = "Welcome " + Application.Current.Properties["user"].ToString();
            }
        }

        private async void AddAssetsClicked(object sender, EventArgs e)
        {
            addAssetsButton.IsEnabled = false;
            if (Application.Current.Properties.ContainsKey("user") && Application.Current.Properties["LastSyncdate"].ToString() != "mm/dd/yyyy hh:mm:ss")
                await Navigation.PushAsync(new AddAssetPage(_connection));
            else
            {
                await DisplayAlert("Error", "You have to log in before using the features.", "Ok");
                await Navigation.PushAsync(new LoginPage(_connection));
            }
            addAssetsButton.IsEnabled = true;
        }

        private void NavigationListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            NavigationListView.SelectedItem = null;
        }
        List<NavigationMenuClass> LoadNavigationMenu(string searchText = null)
        {
            var _navigationMenu = new List<NavigationMenuClass>
            {
                new NavigationMenuClass{NavigationName = "Add Assets"},
                new NavigationMenuClass{NavigationName = "Search and Edit Assets"},
                new NavigationMenuClass{NavigationName = "Utilize Assets"},
                new NavigationMenuClass{NavigationName = "Assign Assets"},
                new NavigationMenuClass{NavigationName = "Transfer Assets"},
                new NavigationMenuClass{NavigationName = "Login"},
                new NavigationMenuClass{NavigationName = "Settings"},
                new NavigationMenuClass{NavigationName = "Sync"}
            };
            return _navigationMenu;
        }

        private async void NavigationListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var Nav_site = e.Item as NavigationMenuClass;
            switch (Nav_site.NavigationName)
            {

                case "Add Assets":
                    IsPresented = false;
                    if (Application.Current.Properties.ContainsKey("user") && Application.Current.Properties["LastSyncdate"].ToString() != "mm/dd/yyyy hh:mm:ss")
                        await Navigation.PushAsync(new AddAssetPage(_connection));
                    else
                        Notloggedin();
                    break;
                case "Search and Edit Assets":
                    IsPresented = false;
                    if (Application.Current.Properties.ContainsKey("user") && Application.Current.Properties["LastSyncdate"].ToString() != "mm/dd/yyyy hh:mm:ss")
                        await Navigation.PushAsync(new SearchAndEditPage(_connection));
                    else
                        Notloggedin();
                    break;
                case "Utilize Assets":
                    IsPresented = false;
                    if (Application.Current.Properties.ContainsKey("user") && Application.Current.Properties["LastSyncdate"].ToString() != "mm/dd/yyyy hh:mm:ss")
                        await Navigation.PushAsync(new UtilizeAssetsPage(_connection));
                    else
                        Notloggedin();

                    break;
                case "Assign Assets":
                    IsPresented = false;
                    if (Application.Current.Properties.ContainsKey("user") && Application.Current.Properties["LastSyncdate"].ToString() != "mm/dd/yyyy hh:mm:ss")
                        await Navigation.PushAsync(new AssignAssetsPage(_connection));
                    else
                        Notloggedin();
                    break;
                case "Transfer Assets":
                    IsPresented = false;
                    if (Application.Current.Properties.ContainsKey("user") && Application.Current.Properties["LastSyncdate"].ToString() != "mm/dd/yyyy hh:mm:ss")
                        await Navigation.PushAsync(new TransferAssetsPage(_connection));
                    else
                        Notloggedin();
                    break;
                case "Login":
                    IsPresented = false;
                    await Navigation.PushAsync(new LoginPage(_connection));
                    break;
                case "Settings":
                    IsPresented = false;
                    if (Application.Current.Properties.ContainsKey("user") && Application.Current.Properties["LastSyncdate"].ToString() != "mm/dd/yyyy hh:mm:ss")
                        await Navigation.PushAsync(new SettingsPage(_connection));
                    else
                        Notloggedin();
                    break;
                case "Sync":
                    IsPresented = false;
                    if (Application.Current.Properties.ContainsKey("UserID") && Application.Current.Properties.ContainsKey("user"))
                        await Navigation.PushAsync(new SyncPage(_connection, Application.Current.Properties["UserID"].ToString(), Application.Current.Properties["url"].ToString(), false));
                    else
                        Notloggedin();
                    break;
                default:
                    break;

            };
        }

        private async void Notloggedin()
        {
            await DisplayAlert("Error", "You have to log in before using the features.", "Ok");
            await Navigation.PushAsync(new LoginPage(_connection));
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (Application.Current.Properties.ContainsKey("user") && Application.Current.Properties["LastSyncdate"].ToString() != "mm/dd/yyyy hh:mm:ss")
                await Navigation.PushAsync(new SearchAndEditPage(_connection));
            else
            {
                await DisplayAlert("Error", "You have to log in before using the features.", "Ok");
                await Navigation.PushAsync(new LoginPage(_connection));
            }
        }
        protected override void OnAppearing()
        {
            if (Application.Current.MainPage.Width >= 768 && Application.Current.MainPage.Height >= 1024)
            {
                logoImage.Margin = new Thickness(0,50,0,50);
                logoImage.WidthRequest = 500;
                addAssetsButton.Margin = new Thickness(0, 50, 0, 50);
                SearchAndEdit.Margin = new Thickness(0, 50, 0, 50);
                Audit.Margin = new Thickness(0, 50, 0, 50);
                HomeLabel.Margin = new Thickness(0, 50, 0, 50);
            }
            var UpdatedAssetdata = Database.Assets.GetUpdatedAssets(_connection);
            if (UpdatedAssetdata.Count() > 0)
                _assetsync.Text = "There is local data that has not been synced.";
            else
            {
                var CreatedAssetData = Database.Assets.GetInsertedAssets(_connection);
                if (CreatedAssetData.Count() > 0)
                    _assetsync.Text = "There is local data that has not been synced.";
                else
                {
                    var TransferedData = Database.Transfers.GetTableData(_connection);
                    if (TransferedData.Count() > 0)
                        _assetsync.Text = "There is local data that has not been synced.";
                    else
                    {
                        var Deletedata = Database.Deletes.GetTableData(_connection);
                        if (Deletedata.Count() > 0)
                            _assetsync.Text = "There is local data that has not been synced.";
                        else
                        {
                            _assetsync.Text = "";
                        }
                    }
                }
            }
            if (Application.Current.Properties.ContainsKey("Error") && Application.Current.Properties["Error"] != null)
            {
                if (Application.Current.Properties["Error"].ToString() == "recon")
                    _Syncerror.Text = "There is data that has been sent to reconciliation, please  update reconciliation records on the web application";
                else
                    _Syncerror.Text = "There is data that has been sent to reconciliation, please contact support to have the records addressed.";

            }
            else
                _Syncerror.Text = "";
            if (Application.Current.Properties.ContainsKey("user") && Application.Current.Properties["LastSyncdate"].ToString() != "mm/dd/yyyy hh:mm:ss")
            {
                HomeLabel.Text = "Welcome " + Application.Current.Properties["user"].ToString() + "!";
                Login_.Text = "Logout";
            }
            else
            {
                Login_.Text = "Login";
                HomeLabel.Text = "Welcome!";
            }
                

            base.OnAppearing();
        }
        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage(_connection));
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            if (Application.Current.Properties.ContainsKey("user") && Application.Current.Properties["LastSyncdate"].ToString() != "mm/dd/yyyy hh:mm:ss")
            {
                try
                {
                    await Navigation.PushAsync(new AuditPage(_connection));
                }
                catch (Exception exc)
                {
                    DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString()??"", Application.Current.Properties["url"].ToString()??"", null);
                    await DisplayAlert("Error", exc.Message, "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "You have to log in before using the features.", "Ok");
                await Navigation.PushAsync(new LoginPage(_connection));
            }
        }
    }

}
