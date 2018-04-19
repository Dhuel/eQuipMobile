using Newtonsoft.Json;
using Plugin.Connectivity;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace eQuipMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private SQLiteConnection _connection;
        private string url_passed;

        public LoginPage(SQLiteConnection connection_)
        {
            InitializeComponent();
            if (Device.RuntimePlatform == Device.iOS)
                Padding = new Thickness(0, 20, 20, 0);
            _connection = connection_;

            if (Application.Current.MainPage.Width <= 320)
            {
                LoginUserName.WidthRequest = 150;
                LoginPassWord.WidthRequest = 150;
                LoginUrl.WidthRequest = 150;
                SyncdateText.FontSize = 15;
                _LastSyncDate.FontSize = 15;
            }

        }

        protected override void OnAppearing()
        {
            //private static HttpClient _client = new HttpClient(new Xamarin.Android.Net.AndroidClientHandler());
            try
            {
                var loginData = Database.Login.GetTableData(_connection);
                _LastSyncDate.Text = loginData.First().LastSyncDate ?? "mm/dd/yyyy hh:mm:ss";
                Application.Current.Properties["LastSyncdate"] = _LastSyncDate.Text;
            }
            catch (Exception)
            {

                _LastSyncDate.Text = "mm/dd/yyyy hh:mm:ss";
                Application.Current.Properties["LastSyncdate"] = _LastSyncDate.Text;
            }

            if (Application.Current.Properties.ContainsKey("user") && Application.Current.Properties["user"] != null)
            {
                LoginUserName.Text = Application.Current.Properties["user"].ToString();
                LoginPassWord.Text = Application.Current.Properties["password"].ToString();
                LoginUrl.Text = Application.Current.Properties["url"].ToString();
            }
            base.OnAppearing();
        }

        private async void Login_Clicked(object sender, System.EventArgs e)
        {
            if (LoginUserName.Text == null || LoginPassWord.Text == null || LoginUrl.Text == null)
            {
                await DisplayAlert("Error", "Please enter all your login information.", "OK");
                return;
            }
            if (_Reinitialize.IsToggled == true)
            {
                try
                {
                    var AssetsToUpdate = CheckforSyncData();
                    if (AssetsToUpdate)
                    {
                        var WipeOrNot = await DisplayAlert("Warning", "You have un-synced assets. If you do not sync, the data will be removed.", "Remove data", "Sync Data");
                        if (WipeOrNot)
                        {
                            Reinitialize(false);
                        }
                        else
                        {
                            await Navigation.PushAsync(new SyncPage(_connection, Application.Current.Properties["UserID"].ToString(), Application.Current.Properties["url"].ToString()));
                        }
                    }
                    else
                    {
                        Reinitialize(false);
                    }

                }
                catch (Exception r)
                {
                    DependencyService.Get<IError>().SendRaygunError(r, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                    await DisplayAlert("Reinitialize error", r.Message, "oK");
                }
            }
            else
            {
                LogIntoEquip(LoginUserName.Text, LoginPassWord.Text, LoginUrl.Text, false);
            }
        }

        private bool CheckforSyncData()
        {
            var NeedToSync = Database.Assets.GetUpdatedAssets(_connection);
            if (NeedToSync.Count() > 0)
            {
                return true;
            }
            else
                return false;
        }

        private void Reinitialize(bool second_attempt)
        {
            Application.Current.Properties["Error"] = null;
            Application.Current.Properties["AuditedSite"] = null;
            Application.Current.Properties["AuditedLocation"] = null;
            Application.Current.Properties["AuditedSublocation"] = null;
            Application.Current.Properties["LastAssetID"] = null;
            Application.Current.Properties["Department"] = null;
            Application.Current.Properties["url"] = null;
            Application.Current.Properties["user"] = null;
            Application.Current.Properties["password"] = null;
            Application.Current.Properties["UserID"] = null;
            Application.Current.Properties["LastSyncdate"] = _LastSyncDate.Text = "mm/dd/yyyy hh:mm:ss";
            Database.Reinitialize(_connection);
            if (CrossConnectivity.Current.IsConnected)
            {
                try
                {
                    LogIntoEquip(LoginUserName.Text, LoginPassWord.Text, LoginUrl.Text, second_attempt);
                }
                catch (Exception) { }
                
            }
            else
            {
                DisplayAlert("Error", "No internet connection.", "OK");
            }
        }
        private async void LogIntoEquip(string username, string password, string url, bool second_attempt)
        {
            if (url.ToLower().IndexOf("https://") != 0)
            {
                await DisplayAlert("Error", "Please check the url, there is no http present", "OK");
            }
            else
            {
                string returnedData = "";
                try
                {
                    url_passed = url;
                    ActivityIndicator.IsRunning = true;
                    ActivityIndicator.IsVisible = true;
                    returnedData = await SyncClass.API_Login(username.Trim(), password.Trim(), url.Trim());
                    ActivityIndicator.IsRunning = false;
                    ActivityIndicator.IsVisible = false;
                    //check if data is erroroneous

                    var ParsedLoginData = JsonConvert.DeserializeObject<List<LoginDataClass>>(returnedData);
                    //stores login, user and password to Application storage
                    Application.Current.Properties["url"] = url;
                    Application.Current.Properties["user"] = username;
                    Application.Current.Properties["password"] = password;
                    Application.Current.Properties["UserID"] = ParsedLoginData[0].UserID;

                    //Store login data to variables to be used to sore to database
                    var Servertime_ = ParsedLoginData[0].ServerTime.ToString();
                    var UserID_ = ParsedLoginData[0].UserID;
                    var RoleID_ = ParsedLoginData[0].RoleID;
                    var AllowDup_ = ParsedLoginData[0].AllowDuplicateBarcode.ToString();
                    var eQuipVer_ = ParsedLoginData[0].eQuipVersion;
                    var Framework_ = ParsedLoginData[0].Framework;
                    var RequiredFields_ = JsonConvert.SerializeObject(ParsedLoginData[0].RequiredFields);
                    var MobileSites_ = ParsedLoginData[0].GetMobileSites();
                    var Permissions = JsonConvert.SerializeObject(ParsedLoginData[0].Permissions);
                    var LoginData = new List<LoginDbTable> {
                        new LoginDbTable {
                            ServerTime = Servertime_,
                            UserID = UserID_,
                            RoleID = RoleID_.ToString(),
                            AllowDuplicateBarcode = AllowDup_,
                            eQuipVersion = eQuipVer_,
                            Framework = Framework_,
                            RequiredFields = RequiredFields_,
                            MobileSites = MobileSites_,
                            LastSyncDate = _LastSyncDate.Text,
                            Permissions = Permissions
                        }
                    };
                    //Adds data to storage
                    //check if value alredy present, then update
                    //https://components.xamarin.com/gettingstarted/sqlite-net
                    foreach (var loginRecord in LoginData)
                    {
                        //TODO
                        //Change it so that it checks for specific parameter
                        var query = Database.Login.GetTableData(_connection); //QueryValuations(_connection, LoginData[0]);
                        if (query.Count() > 0)
                        {
                            if (loginRecord.UserID == query.First().UserID && loginRecord.MobileSites == query.First().MobileSites)
                            {
                                //Same user is logging in
                                //loginRecord.Id = query.First().Id;
                                Database.Login.Update(_connection, loginRecord);// _connection.Update(loginRecord);
                                if (!second_attempt)
                                    await DisplayAlert("Ok", "Same user logging in : " + loginRecord.ServerTime, "Ok");
                                await Application.Current.SavePropertiesAsync();
                                await Navigation.PushAsync(new SyncPage(_connection, UserID_, url_passed));
                            }
                            else
                            {
                                //Different user logging in
                                var AssetsToUpdate = CheckforSyncData();
                                if (AssetsToUpdate)
                                {
                                    await DisplayAlert("Warning", "New user. There is data that has not been Synced. Sync before continuing", "Ok");
                                }
                                else
                                {
                                    //clear database and login with new user data
                                    if (second_attempt)
                                        await Navigation.PushAsync(new SyncPage(_connection, UserID_, url_passed));
                                    else
                                    {
                                        await DisplayAlert("Alert", "Different user/mobile sites logging in. Clearing data for login", "Ok");
                                        Reinitialize(true);
                                    }
                                }
                            }
                        }
                        else
                        {
                            //Completely new user
                            Database.Login.Insert(_connection, loginRecord);//_connection.Insert(loginRecord);
                            if (!second_attempt)
                                await DisplayAlert("Ok", "No data present, inserting Data: " + loginRecord.ServerTime, "Ok");
                            await Navigation.PushAsync(new SyncPage(_connection, UserID_, url_passed));
                        }
                    }
                }
                catch (Exception e)
                {
                    try
                    {
                        DependencyService.Get<IError>().SendRaygunError(e, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                        //TODO - send raygun Error
                        ActivityIndicator.IsRunning = false;
                        ActivityIndicator.IsVisible = false;
                        if (e is LoginException)
                        {
                            await DisplayAlert("Error", e.Message, "OK");
                            LoginPassWord.Text = "";
                        }
                        else if (e.Message == "The resource could not be loaded because the App Transport Security policy requires the use of a secure connection.")
                        {
                            LoginUrl.Text = LoginUrl.Text.Replace("http", "https");
                            LogIntoEquip(LoginUserName.Text, LoginPassWord.Text, LoginUrl.Text, second_attempt);
                        }
                        else
                            await DisplayAlert("Error", e.Message + " Please check the url or connection.", "OK");
                    }
                    catch (Exception) { }

                }
            }
        }

        private void CancelButton(object sender, EventArgs e) {
            Navigation.PopToRootAsync();
        }

        private async void Logout(object sender, EventArgs e)
        {
            Application.Current.Properties.Remove("user");
            Application.Current.Properties.Remove("password");
            Application.Current.Properties.Remove("url");
            await Application.Current.SavePropertiesAsync();
            await Navigation.PopToRootAsync();
        }
    }
}