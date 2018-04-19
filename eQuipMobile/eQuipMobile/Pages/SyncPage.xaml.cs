using ModernHttpClient;
using Newtonsoft.Json;
using Plugin.Connectivity;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace eQuipMobile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SyncPage : ContentPage
	{
        private SQLiteConnection _connection;
        private HttpClient _client = new HttpClient(new NativeMessageHandler());
        private string _userID;
        private string _LastSyncDate;
        private string _url;
        public SyncPage(SQLiteConnection _connection_, string UserID, string URL, bool loggedin = true)
        {
            InitializeComponent();
            _userID = UserID;
            _url = URL;
            _connection = _connection_;
            if (Application.Current.Properties.ContainsKey("LastSyncdate"))
            {
                _LastSyncDate = Application.Current.Properties["LastSyncdate"].ToString();
            }
            else
            {
                var loginData = Database.Login.GetTableData(_connection);
                _LastSyncDate = loginData.First().LastSyncDate ?? "mm/dd/yyyy hh:mm:ss";
            }
            if (!loggedin)
                LogIntoEquip(Application.Current.Properties["user"].ToString(), Application.Current.Properties["password"].ToString(),
                    Application.Current.Properties["url"].ToString());
            pg1.Progress = 0;
        }
        private async void LogIntoEquip(string username, string password, string url)
        {
            string returnedData = "";
            try
            {

                returnedData = await SyncClass.API_Login(username, password, url);


                var ParsedLoginData = JsonConvert.DeserializeObject<List<LoginDataClass>>(returnedData);

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
                        LastSyncDate = _LastSyncDate,
                        Permissions = Permissions
                    }
                };
                foreach (var loginRecord in LoginData)
                {
                    var query = Database.Login.GetTableData(_connection);
                    if (query.Count() > 0)
                    {
                        Database.Login.Update(_connection, loginRecord);
                    }
                    else
                    {
                        await DisplayAlert("Error", "Please log in", "OK");
                    }
                }
            }
            catch (Exception e)
            {
                DependencyService.Get<IError>().SendRaygunError(e, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                if (e is LoginException)
                {
                    await DisplayAlert("Error", e.Message, "OK");
                }
                else
                    await DisplayAlert("Error", e.Message + ". Please check the url or connection.", "OK");
            }
        }
        private async void SyncLaterButton(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
        private async void SyncNowButton(object sender, EventArgs e)
        {
            
            if (CrossConnectivity.Current.IsConnected)
            {
                try
                {
                    StartSyncProcess();
                }
                catch (Exception exc)
                {
                    DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                    await DisplayAlert("Error", exc.Message, "OK");
                }
                
            }
            else
            {
                await DisplayAlert("Error", "No internet connection.", "OK");
            }
        }

        public async void StartSyncProcess()
        {
            Application.Current.Properties["Error"] = null;
            //TODO - find better solution than having 2 instances of sites stacks to search from
            double progressCounter = 1.0000 / 15.0000;
            ActivityIndicator.IsVisible = true;
            ActivityIndicator.IsRunning = true;
            var StackSites = new Stack<SitesClass>();
            var StackSites_Assets = new Stack<SitesClass>();
            var sitesList = new List<SitesDbTable>();
            try
            {
                var query = Database.Login.GetTableData(_connection);
                //Check if login Table has mobile sites
                if (query.First().MobileSites != "null" && query.First().MobileSites != null)
                {
                    //if it does, set the sites records to the mobile sites
                    pg1Label.Text = "Mobile sites found, saving mobile sites to database.";
                    //convert into array
                    var mobilesiteArray = query.First().MobileSites;
                    //convert to sites db table type
                    pg1Label.Text = "Converting data";
                    sitesList = JsonConvert.DeserializeObject<List<SitesDbTable>>(mobilesiteArray);
                    StackSites = JsonConvert.DeserializeObject<Stack<SitesClass>>(mobilesiteArray);
                    StackSites_Assets = JsonConvert.DeserializeObject<Stack<SitesClass>>(mobilesiteArray);
                    //Insert All records into database
                    pg1Label.Text = "Storing sites to database.";
                    Database.Sites.DeleteAndInsert(_connection, sitesList);
                }
                else
                {
                    //NO mobile sites
                    pg1Label.Text = "No Mobile sites found, getting site data from eQuip.";
                    //API call
                    var Sitesresponse = await SyncClass.NewSyncClass.API_GetAllSites(_userID, _url);
                    pg1Label.Text = "Converting data";
                    sitesList = JsonConvert.DeserializeObject<List<SitesDbTable>>(Sitesresponse);
                    StackSites = JsonConvert.DeserializeObject<Stack<SitesClass>>(Sitesresponse);
                    StackSites_Assets = JsonConvert.DeserializeObject<Stack<SitesClass>>(Sitesresponse);
                    //Insert All sites into database
                    pg1Label.Text = "Storing sites to database.";
                    Database.Sites.DeleteAndInsert(_connection, sitesList);
                }

                if (_LastSyncDate == "mm/dd/yyyy hh:mm:ss")
                {
                    pg1Label.Text = "New Install, checking for mobile sites.";
                    pg1.Progress = pg1.Progress + progressCounter;
                    try
                    {
                        await SyncLocationToPropertyPass(progressCounter, StackSites);
                        InsertAssetData(progressCounter, StackSites_Assets, true);
                    }
                    catch(Exception exc)
                    {
                       // DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                        throw exc;
                    }
                }
                else
                {
                    try
                    {
                        progressCounter = 1.0000 / 25.0000;

                        UpdateData(progressCounter);

                        await SyncLocationToPropertyPass(progressCounter, StackSites);

                        InsertAssetData(progressCounter, StackSites_Assets);
                    }
                    catch(Exception exc)
                    {
                        throw exc;
                    }
                }
            }
            catch (Exception e)
            {
                DependencyService.Get<IError>().SendRaygunError(e, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                await DisplayAlert("Error", e.Message + " Please try again later.", "OK");
                await Navigation.PopToRootAsync();
            }
        }


        private void UpdateData(double progressCounter)
        {
            try
            {
                pg1.Progress = pg1.Progress + progressCounter;
                //Get Assets that need to be updated
                pg1Label.Text = "Getting Updated Asset Data.";
                var UpdateAssets = Database.Assets.GetUpdatedAssets(_connection);
                //convert the DB Data to stack of AssetClass 
                pg1Label.Text = "Converting data";
                var StackUpdateAssets = Database.Assets.ConvertDbToStack(UpdateAssets);
                //Submit the stack to be updated
                pg1Label.Text = "Updating data";
                SyncClass.UpdateSyncClass.API_GetAssetToUpdate(_userID, _url, ref StackUpdateAssets, _connection);
                pg1.Progress = pg1.Progress + progressCounter;

                //Get Assets that need to be cerated
                pg1Label.Text = "Getting Created Asset Data.";
                var CreatedAssets = Database.Assets.GetInsertedAssets(_connection);
                pg1Label.Text = "Converting data";
                var StackInsertAssets = Database.Assets.ConvertDbToStackInsert(CreatedAssets);
                pg1Label.Text = "Updating data";
                SyncClass.UpdateSyncClass.API_GetCreatedAssets(_userID, _url, ref StackInsertAssets, _connection);
                pg1.Progress = pg1.Progress + progressCounter;

                //Get Assets to be transferred
                pg1Label.Text = "Getting Transferred Asset Data.";
                var TransferData = Database.Transfers.GetTableData(_connection);
                pg1Label.Text = "Converting data";
                var StackTransfers = Database.Transfers.ConvertDbToStack(TransferData);
                pg1Label.Text = "Updating data";
                SyncClass.UpdateSyncClass.API_GetTransferAssets(_userID, _url, ref StackTransfers, _connection);
                //Delete transfer table data
                pg1.Progress = pg1.Progress + progressCounter;

                //Send Asset Records to be deleted
                pg1Label.Text = "Getting Deleted Asset Data.";
                var DeleteData = Database.Deletes.GetTableData(_connection);
                pg1Label.Text = "Converting data";
                var StackDeletes = Database.Deletes.ConvertDbToStack(DeleteData);
                pg1Label.Text = "Deleting data";
                SyncClass.UpdateSyncClass.API_GetDeleteAssetList(_userID, _url, ref StackDeletes, _connection);
                //Delete delete table data
                pg1.Progress = pg1.Progress + progressCounter;

                //Update PropertyPassTables
                pg1Label.Text = "Getting Updated Property Pass Data.";
                var test = Convert.ToDateTime(_LastSyncDate).ToString("s");
                var PropertyPassUpdatedata = Database.PropertyPassTable.GetUpdatedTableData(_connection, test);
                pg1Label.Text = "Converting data";
                var StackPropertyPassUpdates = Database.PropertyPassTable.ConvertDbToStack(PropertyPassUpdatedata);
                pg1Label.Text = "Updating data";
                SyncClass.UpdateSyncClass.API_GetPropertyPassTablesToUpdateOrCreate(_connection, _userID, _url, ref StackPropertyPassUpdates, false);
                pg1.Progress = pg1.Progress + progressCounter;

                //Create PropertyPass tables
                pg1Label.Text = "Getting Created Property Pass Data.";
                var PropertyPassCreatedata = Database.PropertyPassTable.GetCreatedTableData(_connection);
                pg1Label.Text = "Converting data";
                var StackPropertyPassesCreates = Database.PropertyPassTable.ConvertDbToStack(PropertyPassCreatedata);
                pg1Label.Text = "Updating data";
                SyncClass.UpdateSyncClass.API_GetPropertyPassTablesToUpdateOrCreate(_connection, _userID, _url, ref StackPropertyPassesCreates, true);
                pg1.Progress = pg1.Progress + progressCounter;

                //Update PropertyPassItems
                var Table_data = Database.PropertyPassItem.GetTableData(_connection);
                pg1Label.Text = "Getting Updated Property Pass Items Data.";
                var PropertyPassItemUpdatedata = Database.PropertyPassItem.GetUpdatedTableData(_connection, test);
                pg1Label.Text = "Converting data";
                var StackPropertyPassItemUpdates = Database.PropertyPassItem.ConvertDbToStack(PropertyPassItemUpdatedata);
                pg1Label.Text = "Updating data";
                SyncClass.UpdateSyncClass.API_GetPropertyPassItemsToUpdateOrCreate(_connection, _userID, _url, ref StackPropertyPassItemUpdates, false);
                pg1.Progress = pg1.Progress + progressCounter;

                //Create PropertyPassItems
                pg1Label.Text = "Getting Created Property Pass Items Data.";
                var PropertyPassItemsCreatedata = Database.PropertyPassItem.GetCreatedTableData(_connection);
                pg1Label.Text = "Converting data";
                var StackPropertyPassesItemCreates = Database.PropertyPassItem.ConvertDbToStack(PropertyPassItemsCreatedata);
                pg1Label.Text = "Updating data";
                SyncClass.UpdateSyncClass.API_GetPropertyPassItemsToUpdateOrCreate(_connection, _userID, _url, ref StackPropertyPassesItemCreates, true);
                pg1.Progress = pg1.Progress + progressCounter;

                ////Get Records to be deleted - Iff all data is reentered, this is not necessary
                //pg1Label.Text = "Getting Deleted Data.";
                //var RecordsToDelete = await SyncClass.UpdateSyncClass.API_GetDeletedTables(_userID, _url, _LastSyncDate);
                //pg1.Progress = pg1.Progress + progressCounter;

                ////Delete Deleted records
                //pg1Label.Text = "Deleting Data.";
                //Database.Deletes.DeleteRecords(_connection, RecordsToDelete);
                //pg1.Progress = pg1.Progress + progressCounter;
            }
            catch (Exception)
            {}
            
        }

        private async void InsertAssetData(double progressCounter, Stack<SitesClass> StackSites_Assets, bool new_ = false)
        {
            try
            {
                //Insert Asset Data
                pg1Label.Text = "Getting Asset data from eQuip.";
                var AssetResponse = await SyncClass.NewSyncClass.API_GetAssets(_userID, _url, StackSites_Assets);

                pg1Label.Text = "Converting data";
                //convert string to ist of JSON Object
                var AssetRecords = JsonConvert.DeserializeObject<List<AssetJsonObject>>(AssetResponse);
                //Create new list to parse records and make into AssetClass
                var AssetClassList = new List<AssetClass>();
                foreach (AssetJsonObject AssetJSON in AssetRecords)
                {
                    AssetClassList.Add(new AssetClass(AssetJSON, true));
                }
                //Convert asset class to string then to DbClass to be stored
                var AssetDbList = JsonConvert.DeserializeObject<IEnumerable<AssetDbClass>>(JsonConvert.SerializeObject(AssetClassList));
                pg1Label.Text = "Storing Assets to database.";
                Database.Assets.DeleteAndInsert(_connection, AssetDbList);
                pg1.Progress = pg1.Progress + progressCounter;

                pg1.Progress = pg1.Progress + progressCounter;

                //Get Time using Login method
                var LoginInfo = JsonConvert.DeserializeObject<List<LoginDataClass>>(await SyncClass.API_Login(Application.Current.Properties["user"].ToString(), Application.Current.Properties["password"].ToString(), _url));
                //Store the LastSync Date
                Application.Current.Properties["LastSyncdate"] = LoginInfo[0].ServerTime.ToString();
                Database.Login.UpdateSync(_connection, LoginInfo[0].ServerTime.ToString());
                await Application.Current.SavePropertiesAsync();
                pg1.Progress = pg1.Progress + progressCounter;
                ActivityIndicator.IsVisible = false;
                ActivityIndicator.IsRunning = false;
                if (new_)
                    await DisplayAlert("Sync Status", "New Synchronization Complete!", "OK");
                else
                    await DisplayAlert("Sync Status", "Update Synchronization Complete!", "OK");
                await Navigation.PopToRootAsync();
            }
            catch (Exception exc)
            {
                DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                await DisplayAlert("Error!", exc.Message+ ". Please wipe database and try again.", "OK");
            }
            
        }

        private IEnumerable<AssetDbClass> ConvertToDB(string AssetResponse)
        {
            pg1Label.Text = "Converting data";
            //convert string to ist of JSON Object
            var AssetRecords = JsonConvert.DeserializeObject<List<AssetJsonObject>>(AssetResponse);
            //Create new list to parse records and make into AssetClass
            var AssetClassList = new List<AssetClass>();
            foreach (AssetJsonObject AssetJSON in AssetRecords)
            {
                AssetClassList.Add(new AssetClass(AssetJSON, false));
            }
            //Convertassetclasstostringthen to DbClass to be stored
            var AssetDbList = JsonConvert.DeserializeObject<IEnumerable<AssetDbClass>>(JsonConvert.SerializeObject(AssetClassList));
            return AssetDbList;
        }

        public async Task<bool> SyncLocationToPropertyPass(double progressCounter, Stack<SitesClass> StackSites_)
        {
            try
            {
                //12 calls
                //Insert Location Data
                pg1Label.Text = "Getting location data from eQuip.";
                var LocationResponse = await SyncClass.NewSyncClass.API_GetLocation(_userID, _url, StackSites_);

                pg1Label.Text = "Converting data";
                var LocationList = JsonConvert.DeserializeObject<IEnumerable<LocationsDbTable>>(LocationResponse);
                pg1Label.Text = "Storing Locations to database.";
                Database.Locations.DeleteAndInsert(_connection, LocationList);

                pg1.Progress = pg1.Progress + progressCounter;


                //Insert SubLocation Data
                pg1Label.Text = "Getting sublocation data from eQuip.";
                var SubLocationResponse = await SyncClass.NewSyncClass.API_GetAllSublocations(_userID, _url);

                pg1Label.Text = "Converting data";
                var SubLocationList = JsonConvert.DeserializeObject<IEnumerable<SubLocationsDbTable>>(SubLocationResponse);
                pg1Label.Text = "Storing SubLocations to database.";
                Database.SubLocations.DeleteAndInsert(_connection, SubLocationList);

                pg1.Progress = pg1.Progress + progressCounter;


                //Insert Department Data
                pg1Label.Text = "Getting Department data from eQuip.";
                var DepartmeteResponse = await SyncClass.NewSyncClass.API_GetDepartments(_userID, _url);

                pg1Label.Text = "Converting data";
                var DepartmentList = JsonConvert.DeserializeObject<IEnumerable<DepartmentsDbTable>>(DepartmeteResponse);
                pg1Label.Text = "Storing Departments to database.";
                Database.Department.DeleteAndInsert(_connection, DepartmentList);


                pg1.Progress = pg1.Progress + progressCounter;


                //Insert People Data
                pg1Label.Text = "Getting People data from eQuip.";
                var PeopleResponse = await SyncClass.NewSyncClass.API_GetPeople(_userID, _url);

                pg1Label.Text = "Converting data";
                var PeopleList = JsonConvert.DeserializeObject<IEnumerable<PeopleDbTable>>(PeopleResponse);
                pg1Label.Text = "Storing People to database.";
                Database.People.DeleteAndInsert(_connection, PeopleList);

                pg1.Progress = pg1.Progress + progressCounter;


                //Insert Usage Data
                pg1Label.Text = "Getting Usage data from eQuip.";
                var UsageResponse = await SyncClass.NewSyncClass.API_GetUsages(_userID, _url);

                pg1Label.Text = "Converting data";
                var UsageList = JsonConvert.DeserializeObject<IEnumerable<UsageDbTable>>(UsageResponse);
                pg1Label.Text = "Storing Usage to database.";
                Database.Usage.DeleteAndInsert(_connection, UsageList);

                pg1.Progress = pg1.Progress + progressCounter;


                //Insert Condition Data
                pg1Label.Text = "Getting Condition data from eQuip.";
                var ConditionResponse = await SyncClass.NewSyncClass.API_GetConditions(_userID, _url);

                pg1Label.Text = "Converting data";
                var ConditionList = JsonConvert.DeserializeObject<IEnumerable<ConditionDbTable>>(ConditionResponse);
                pg1Label.Text = "Storing Condition to database.";
                Database.Condition.DeleteAndInsert(_connection, ConditionList);

                pg1.Progress = pg1.Progress + progressCounter;


                //Insert Vendor Data
                pg1Label.Text = "Getting Vendor data from eQuip.";
                var VendorResponse = await SyncClass.NewSyncClass.API_GetVendors(_userID, _url);

                pg1Label.Text = "Converting data";
                var VendorList = JsonConvert.DeserializeObject<IEnumerable<VendorDbTable>>(VendorResponse);
                pg1Label.Text = "Storing Vendors to database.";
                Database.Vendor.DeleteAndInsert(_connection, VendorList);

                pg1.Progress = pg1.Progress + progressCounter;


                //Insert Manufacturer Data
                pg1Label.Text = "Getting Manufacturer data from eQuip.";
                var MfgResponse = await SyncClass.NewSyncClass.API_GetMfg(_userID, _url);

                pg1Label.Text = "Converting data";
                var MfgList = JsonConvert.DeserializeObject<IEnumerable<ManufacturerDbTable>>(MfgResponse);
                pg1Label.Text = "Storing Manufacturer to database.";
                Database.Manufacturer.DeleteAndInsert(_connection, MfgList);

                pg1.Progress = pg1.Progress + progressCounter;


                //Insert AssetCategory Data
                pg1Label.Text = "Getting AssetCategory data from eQuip.";
                var AssetCategoryResponse = await SyncClass.NewSyncClass.API_GetAssetCategory(_userID, _url);

                pg1Label.Text = "Converting data";
                var AssetCategoryList = JsonConvert.DeserializeObject<IEnumerable<CategoryDbTable>>(AssetCategoryResponse);
                pg1Label.Text = "Storing AssetCategory to database.";
                Database.Category.DeleteAndInsert(_connection, AssetCategoryList);

                pg1.Progress = pg1.Progress + progressCounter;


                //Insert PropertyPass Data
                pg1Label.Text = "Getting PropertyPass data from eQuip.";
                var PropertyPassResponse = await SyncClass.NewSyncClass.API_GetPropertyPassTable(_userID, _url);

                pg1Label.Text = "Converting data";
                var PropertyPassList = JsonConvert.DeserializeObject<IEnumerable<PropertyPassDbTable>>(PropertyPassResponse);
                pg1Label.Text = "Storing PropertyPass to database.";
                Database.PropertyPassTable.DeleteAndInsert(_connection, PropertyPassList);

                pg1.Progress = pg1.Progress + progressCounter;

                //Insert PropertyPass Data
                pg1Label.Text = "Getting PropertyPass Item data from eQuip.";
                var PropertyPassItemResponse = await SyncClass.NewSyncClass.API_GetPropertyPassItem(_userID, _url);

                pg1Label.Text = "Converting data";
                var PropertyPassItemList = JsonConvert.DeserializeObject<IEnumerable<PropertyPassItemDbTable>>(PropertyPassItemResponse);
                pg1Label.Text = "Storing PropertyPass Item to database.";
                Database.PropertyPassItem.DeleteAndInsert(_connection, PropertyPassItemList);

                pg1.Progress = pg1.Progress + progressCounter;

                //Insert FormDesigner Data
                pg1Label.Text = "Getting Form Designer data from eQuip.";
                var FormDesignerResponse = await SyncClass.NewSyncClass.API_GeFormDesigner(_userID, _url);

                pg1Label.Text = "Converting data";
                var FormDesignerList = JsonConvert.DeserializeObject<FormDesignerClass>(FormDesignerResponse);
                var AssetDetailsNames = JsonConvert.SerializeObject(new AssetDetailNames(FormDesignerList));
                pg1Label.Text = "Storing Form Designer.";
                Application.Current.Properties["DesignInformation"] = AssetDetailsNames;
                await Application.Current.SavePropertiesAsync();
                pg1.Progress = pg1.Progress + progressCounter;
                return true;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
    }
}