using Newtonsoft.Json;
using Plugin.Connectivity;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace eQuipMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddAssetPage : TabbedPage
    {
        //TODO - change missing field to binding of details
        AssetClass _Asset;
        private SQLiteConnection _connection;
        string _siteID;
        bool PurchaceDateForceChanged = false;
        string MissingField;
        string intfield;
        List<AssetCategoryClass> _Category;
        string _AssetCategoryID;
        Stream _thumbStream;
        string _thumbnailImg;
        AssetDetailNames _Names;
        public AddAssetPage(SQLiteConnection connection_, AssetClass Asset = null, string AssetCategoryID = null)
        {
            try
            {
                MessagingCenter.Subscribe<string>(this, "Datawedge", (sender) => {
                    assetBarcode_.Text = sender;
                });
                MessagingCenter.Subscribe<AssetCategoryPage, string>(this, "AssetCategoryID", (sender, arg) => {
                    LoadCategory(arg);
                });
                _Asset = Asset;
                _AssetCategoryID = AssetCategoryID;
                _connection = connection_;
                InitializeComponent();
                LoadData();
                _Names = JsonConvert.DeserializeObject<AssetDetailNames>(Application.Current.Properties["DesignInformation"].ToString());
                BindingContext = _Names;

                BarcodeImage.Source = ImageSource.FromResource("eQuipMobile.Images.Camera_icon.png");
                //if (Device.RuntimePlatform == Device.iOS)
                //    Toolbar_Icon.Order = ToolbarItemOrder.Secondary;
                if (_Asset != null && _Asset.AssetJSON.ThumbnailImage != null)
                    ConvertBinaryToImage(_Asset.AssetJSON.ThumbnailImage);
                else
                    Thumbnail_Image.Source = ImageSource.FromResource("eQuipMobile.Images.no_image.png");
                if (Application.Current.MainPage.Width <= 320)
                {
                    assetName_.WidthRequest = 150;
                    assetBarcode_.WidthRequest = 125;
                    assetSerialNo_.WidthRequest = 150;
                    assetCategory_.WidthRequest = 150;
                    assetSite_.WidthRequest = 150;
                    assetLocation_.WidthRequest = 150;
                    MissingLoc.WidthRequest = 150;
                    assetSubLocation_.WidthRequest = 150;
                    assetDepartment_.WidthRequest = 150;
                    person_.WidthRequest = 150;
                    Quantity_.WidthRequest = 150;
                    Cost_.WidthRequest = 150;
                    assetDescription_.WidthRequest = 150;
                    assetCondition_.WidthRequest = 150;
                    assetUsage_.WidthRequest = 150;
                    dateModified_.WidthRequest = 150;
                    assetStatus_.WidthRequest = 150;
                    Model_.WidthRequest = 150;
                    vendor_.WidthRequest = 150;
                    manufacturer_.WidthRequest = 150;
                    OriginalPartNo_.WidthRequest = 150;
                    auditDate_.WidthRequest = 150;
                    auditStatus_.WidthRequest = 150;
                    POLineNo_.WidthRequest = 150;
                    POStatus_.WidthRequest = 150;
                    AssetUID_.WidthRequest = 150;
                    PurchaseDate_.WidthRequest = 150;
                    PurchaseOrderNo_.WidthRequest = 150;
                    //SyncdateText.FontSize = 15;
                    //_LastSyncDate.FontSize = 15;
                }
            }
            catch (Exception e)
            {
                DisplayAlert("OK", e.Message, "OK");
            }

        }
        private void LoadData()
        {
            if (_Asset != null)
            {
                _AssetCategoryID = _Asset.AssetJSON.AssetCategoryIDInternal;
                LoadValues(_Asset.AssetJSON);
                LoadPO(_Asset.AssetJSON.POStatus);
                LoadSites(_Asset.Asset_SiteIdInternal);
                LoadDepartment(_Asset.AssetJSON.DataGatherID);
                LoadPerson(_Asset.AssetJSON.PeopleIDInternal);
                LoadVendor(_Asset.AssetJSON.Vendor);
                LoadManufacturers(_Asset.AssetJSON.Mfg);
                LoadCondition(_Asset.AssetJSON.AssetConditionID);
                LoadUsage(_Asset.AssetJSON.AssetUsageID);
                LoadCategory(_AssetCategoryID);
            }
            else
            {
                DateModifiedvisibility.IsVisible = false;
                AuditStatusvisibilty.IsVisible = false;
                AuditDatevisibilty.IsVisible = false;
                LoadSites();
                LoadDepartment();
                LoadPerson();
                LoadVendor();
                LoadManufacturers();
                LoadCondition();
                LoadUsage();
                if (_AssetCategoryID != null)
                    LoadCategory(_AssetCategoryID);
            }
        }
        private void LoadPO(string PO)
        {
            switch (PO)
            {
                case "N":
                    POStatus_.SelectedItem = "N/A";
                    break;
                case "O":
                    POStatus_.SelectedItem = "Open";
                    break;
                case "C":
                    POStatus_.SelectedItem = "Closed";
                    break;
            }
        }

        protected override void OnAppearing()
        {
            
            if (PurchaseDate_.Date == Convert.ToDateTime("1/1/1900"))
            {
                PurchaceDateForceChanged = true;
                //purchace date was changed, make sure to change it back when saving
                PurchaseDate_.Date = DateTime.Now.Date.AddDays(-1);
            }
            var pricelock = Database.Settings.GetTableData(_connection);
            if (pricelock.PriceLock && _Asset != null && _Asset.Synced == true)
                Cost_.IsEnabled = false;
            base.OnAppearing();
        }
        private void LoadValues(AssetJsonObject data_)
        {
            assetName_.Text = data_.AssetName;
            assetBarcode_.Text = data_.Barcode;
            assetSerialNo_.Text = data_.AssetSerialNo;
            Quantity_.Text = data_.Quantity.ToString();
            Cost_.Text = data_.Price.ToString();
            assetDescription_.Text = data_.AssetDescription;
            dateModified_.Text = data_.DateModified.ToString();
            assetStatus_.Text = data_.AssetStatus;
            Model_.Text = data_.Model;
            OriginalPartNo_.Text = data_.OriginalPartNo;
            if (data_.AuditDate.ToString() == "1/1/1900 12:00:00 AM")
                auditDate_.Text = "";
            else
                auditDate_.Text = data_.AuditDate.ToString();
            auditStatus_.Text = data_.AuditStatus;
            POLineNo_.Text = data_.POLine.ToString();
            AssetUID_.Text = data_.Asset_UID;
            PurchaseDate_.Date = data_.PurchaseDate;
            PurchaseOrderNo_.Text = data_.PurchaseOrderNo;
            shortage_.Text = data_.ShortageOverage.ToString();

        }
        private void LoadSites(string siteID = null)
        {
            //assetSite_.Items.Clear();
            var selectedSiteName = "";
            var sitesList = SitesClass.DbToSite(Database.Sites.GetTableData(_connection), _connection);
            foreach (SitesClass siteClass_ in sitesList)
            {
                assetSite_.Items.Add(siteClass_.SiteName);
                if (siteID != null)
                {
                    if (siteClass_.SiteIdInternal == siteID)
                    {
                        selectedSiteName = siteClass_.SiteName;
                    }
                }
            }
            if (selectedSiteName != "")
                assetSite_.SelectedItem = selectedSiteName;
        }
        private void LoadDepartment(int DepartmentId = 0)
        {
            var selectedDepartment = "";
            var DepartmentsList = DepartmentClass.DbToDepartment(Database.Department.GetTableData(_connection));
            foreach (DepartmentClass DepartmentClass_ in DepartmentsList)
            {
                assetDepartment_.Items.Add(DepartmentClass_.DepartmentName);
                if (DepartmentId != 0)
                {
                    if (DepartmentClass_.ID == DepartmentId)
                    {
                        selectedDepartment = DepartmentClass_.DepartmentName;
                    }
                }
            }
            if (selectedDepartment != "")
                assetDepartment_.SelectedItem = selectedDepartment;
        }
        private void SiteChanged(object sender, EventArgs e)
        {
            if (assetSite_.SelectedIndex != -1)
            {
                assetLocation_.Items.Clear();
                var siteName = assetSite_.Items[assetSite_.SelectedIndex];
                var SiteForLocation = SitesClass.DbToSite(Database.Sites.GetTableData(_connection), _connection).First(cm => cm.SiteName == siteName);
                if (_Asset != null)
                {
                    LoadLocation(SiteForLocation.SiteIdInternal, _Asset.Asset_locationIdInternal);
                }
                else
                    LoadLocation(SiteForLocation.SiteIdInternal);
            }
        }
        private void LoadLocation(string siteID = null, string LocationID = null)
        {
            var selectedLocationName = "";
            //site ID set so that it can be used to get locations later
            _siteID = siteID;
            var locationList = LocationClass.DbToLocation(Database.Locations.GetTableDataFromSites(_connection, siteID), _connection);
            assetLocation_.Items.Add("No set value");
            foreach (LocationClass locations_ in locationList)
            {
                assetLocation_.Items.Add(locations_.LocationName);
                if (LocationID != null)
                {
                    if (locations_.LocationIdInternal == LocationID)
                    {
                        selectedLocationName = locations_.LocationName;
                    }
                }
            }
            if (selectedLocationName != "")
                assetLocation_.SelectedItem = selectedLocationName;
            else
                assetLocation_.SelectedIndex = 0;
        }
        private void LocationChanged(object sender, EventArgs e)
        {
            try
            {
                if (assetLocation_.SelectedIndex != -1)
                {
                    if (assetLocation_.SelectedIndex == 0)
                    {
                        MissingLocationStack.IsVisible = true;
                    }
                    else
                    {
                        MissingLocationStack.IsVisible = false;
                        assetSubLocation_.Items.Clear();
                        if (assetLocation_.SelectedIndex > -1)
                        {
                            var locationName = assetLocation_.Items[assetLocation_.SelectedIndex];
                            var LocationInfo = LocationClass.DbToLocation(Database.Locations.GetTableDataFromSites(_connection, _siteID), _connection).First(cm => cm.LocationName == locationName);
                            if (_Asset != null)
                            {
                                LoadSublocations(LocationInfo.LocationIdInternal);
                            }
                            else
                                //check this
                                LoadSublocations();
                        }
                    }
                }
            }
            catch (Exception)
            {
                //TODO - find out why I have this
            }

        }
        private void LoadSublocations(string LocationID = null)
        {
            var selectedSublocation = "";
            var SublocationList = SublocationClass.DbToSubLocation(Database.SubLocations.GetTableDataFromLocations(_connection, LocationID));
            foreach (SublocationClass sublocation in SublocationList)
            {
                assetSubLocation_.Items.Add(sublocation.SubLocationName);
                if (LocationID != null)
                {
                    if (sublocation.Location_ID_Internal == LocationID)
                    {
                        selectedSublocation = sublocation.SubLocationName;
                    }
                }
                if (selectedSublocation != "")
                    assetSubLocation_.SelectedItem = selectedSublocation;
                else
                    assetSubLocation_.SelectedIndex = 0;
            }
        }
        private void LoadPerson(string PeopleIDInternal = null)
        {
            var selectedPerson = "";
            var PersonList = PeopleClass.DbToPeople(Database.People.GetTableData(_connection), _connection);
            foreach (PeopleClass person in PersonList)
            {
                person_.Items.Add(person.PeopleName);
                if (PeopleIDInternal != null)
                {
                    if (person.PeopleIDInternal == PeopleIDInternal)
                    {
                        selectedPerson = person.PeopleName;
                    }
                }
                if (selectedPerson != "")
                    person_.SelectedItem = selectedPerson;
            }
        }
        private void LoadVendor(string VendorIDInternal = null)
        {
            var selectedVendor = "";
            var VendorList = VendorClass.DbToVendor(Database.Vendor.GetTableData(_connection), _connection);
            foreach (VendorClass vendor in VendorList)
            {
                vendor_.Items.Add(vendor.Company);
                if (VendorIDInternal != null)
                {
                    if (vendor.VendorIDInternal == VendorIDInternal)
                    {
                        selectedVendor = vendor.VendorIDInternal;
                    }
                }
                if (selectedVendor != "")
                    vendor_.SelectedItem = selectedVendor;
            }
        }
        private void LoadManufacturers(string mfgIDInternal = null)
        {
            var selectedMfg = "";
            var MfgList = ManufacturerClass.DbToManufacturer(Database.Manufacturer.GetTableData(_connection), _connection);
            foreach (ManufacturerClass mfg in MfgList)
            {
                manufacturer_.Items.Add(mfg.Company);
                if (mfgIDInternal != null)
                {
                    if (mfg.VendorIDInternal == mfgIDInternal)
                    {
                        selectedMfg = mfg.VendorIDInternal;
                    }
                }
                if (selectedMfg != "")
                    manufacturer_.SelectedItem = selectedMfg;
            }
        }
        private void LoadCondition(int ConditionID = 0)
        {
            var selectedCondition = "";
            var ConditionList = ConditionClass.DbToCondition(Database.Condition.GetTableData(_connection), _connection);
            foreach (ConditionClass condition in ConditionList)
            {
                assetCondition_.Items.Add(condition.ConditionLabel);
                if (ConditionID != 0)
                {
                    if (condition.ConditionID == ConditionID)
                    {
                        selectedCondition = condition.ConditionLabel;
                    }
                }
                if (selectedCondition != "")
                    assetCondition_.SelectedItem = selectedCondition;
            }
        }
        private void LoadUsage(int UsageID = 0)
        {
            var selectedUsage = "";
            var UsageList = UsageClass.DbToUsage(Database.Usage.GetTableData(_connection), _connection);
            foreach (UsageClass usage in UsageList)
            {
                assetUsage_.Items.Add(usage.UsageLabel);
                if (UsageID != 0)
                {
                    if (usage.UsageID == UsageID)
                    {
                        selectedUsage = usage.UsageLabel;
                    }
                }
                if (selectedUsage != "")
                    assetUsage_.SelectedItem = selectedUsage;
            }
        }
        private void LoadCategory(string AssetCategoryID)
        {
            var Category = Database.Category.GetMyvalueByCategoryIDInternal(_connection, AssetCategoryID);
            try
            {
                assetCategory_.Text = Category.First().AssetCategoryName;
                _Category = AssetCategoryClass.DbToCategory(Category, _connection);
            }
            catch (Exception e)
            {
                DependencyService.Get<IError>().SendRaygunError(e, Application.Current.Properties["user"].ToString()??"Unsynced", Application.Current.Properties["url"].ToString()??"Unsynced", assetCategory_.Text);
                DisplayAlert("OK", e.Message, "OK");
            }
        }
        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (_Asset != null)
            {
                await Navigation.PushModalAsync(new AssetCategoryPage(_connection, _Asset));
            }
            else
            {
                await Navigation.PushModalAsync(new AssetCategoryPage(_connection));
            }
        }
        private async void Delete(object sender, EventArgs e)
        {
            if (_Asset != null)
            {
                //get asset and add it to the delete table
                var DeleteData = Database.Assets.GetAssetDataByAssetIDInternal(_connection, _Asset.AssetIdInternal);
                var del = new DeleteDbTable
                {
                    IdInternal = DeleteData.First().AssetIdInternal,
                    TableName = "Assets"
                };
                Database.Deletes.Insert(_connection, del);
                //remove assset from database and add it to delete table
                Database.Assets.DeleteUsingAssetID(_connection, _Asset.AssetIdInternal);
                await DisplayAlert("Complete", "Asset Deleted", "OK");
                await Navigation.PopToRootAsync();
            }
            else
            {
                await DisplayAlert("Error", "The asset needs to be saved before deleting", "OK");
            }

        }
        private void SaveAsset(object sender, EventArgs e)
        {
            //check if all fields are filled out
            if (RequiredData())
            {
                //check if it is a duplicate barcode
                var records = Database.Assets.GetTableDataByBarcode(_connection, assetBarcode_.Text);
                if (records.Count() > 1)
                {
                    if (_Asset != null)
                    {
                        CheckAndSaveAsset();
                    }
                    else
                    {
                        var loginRecords = Database.Login.GetTableData(_connection);
                        if (loginRecords.First().AllowDuplicateBarcode == "True")
                        {
                            //duplicate barcodes are allowed
                            CheckAndSaveAsset();
                        }
                        else
                        {
                            DisplayAlert("Warning", "Duplicate barcodes are not allowed.", "OK");
                        }
                    }
                }
                else
                {
                    //not duplicate, continue process
                    CheckAndSaveAsset();
                }
            }
            else
            {
                if (intfield!= null)
                {
                    DisplayAlert("Error", "The  \"" + intfield + "\" value is not an integer", "OK");
                }
                else
                    DisplayAlert("Error", "There is a missing required data field. \"" + MissingField + "\" ", "OK");
            }
        }
        public async void CheckAndSaveAsset(bool Audit = false, bool SaveAndReuse = false)
        {
            try
            {
                if (_Asset != null)
                {
                    //UpdateAsset
                    // get asset records from ID Internal
                    var assetDetails = Database.Assets.GetAssetDataByAssetIDInternal(_connection, _Asset.AssetIdInternal);
                    var currentData = GetData();
                    //compare values
                    var time = DateTime.Now;
                    if (Audit)
                    {
                        currentData.AuditDate = time;
                        currentData.AuditStatus = "AUDITED";
                        currentData.DateModified = time;
                    }
                    var assetRecord = Database.Assets.Compare(assetDetails.First(), currentData);
                    //if nothing was changed, send message that nothing was changed
                    if (assetRecord.ChangesMade.Count < 1)
                    {
                        await DisplayAlert("Error", "Nothing was changed in this asset.", "OK");
                    }
                    else
                    {
                        //Convert to be stored
                        var AssetForDb = AssetClass.AssetClassToDb(assetRecord, assetDetails.First().Synced);
                        //store to the database
                        Database.Assets.Update(_connection, AssetForDb);
                        if (Audit)
                            await DisplayAlert("Complete", "Asset Updated and Audited!", "OK");
                        else
                            await DisplayAlert("Complete", "Asset Updated!", "OK");
                        if (SaveAndReuse)
                            ClearAsset();
                        else
                            await Navigation.PopToRootAsync();
                    }
                }
                else
                {
                    try
                    {
                        if (CrossConnectivity.Current.IsConnected)
                        {
                            var onlineAsset = await SyncClass.NewSyncClass.API_GetAssetByBarcode(Application.Current.Properties["UserID"].ToString(), Application.Current.Properties["url"].ToString()??"Unsynced", assetBarcode_.Text);
                            var record = JsonConvert.DeserializeObject<List<AssetJsonObject>>(onlineAsset);
                            if (record.Count() > 0)
                            {
                                var SitesNames = await SyncClass.NewSyncClass.API_GetAllSites(Application.Current.Properties["UserID"].ToString(), Application.Current.Properties["url"].ToString());
                                var sitesList = JsonConvert.DeserializeObject<List<SitesDbTable>>(SitesNames).Where(c => (c.SiteIdInternal == record.First().SiteIDInternal)).First();
                                await DisplayAlert("Error", "Add the mobile site " + sitesList.SiteName + " to access this asset record.", "OK");
                            }
                            else
                                createnewAsset(Audit, SaveAndReuse);
                        }
                        else
                        {
                            createnewAsset(Audit, SaveAndReuse);
                        }
                    }
                    catch
                    {
                        createnewAsset(Audit, SaveAndReuse);
                    }
                }
            }
            catch (Exception e)
            {
                DependencyService.Get<IError>().SendRaygunError(e, Application.Current.Properties["user"].ToString()??"Unsynced", Application.Current.Properties["url"].ToString()??"Unsynced", assetBarcode_.Text);
                await DisplayAlert("Error", e.Message, "OK");
            }
        }

        private async void createnewAsset(bool Audit, bool SaveAndReuse)
        {
            //new asset
            AssetJsonObject newAsset = GetData();
            //assigns guid
            newAsset.AssetIDInternal = GUID.Generate();
            newAsset.IsActive = true;
            var time = DateTime.Now;
            newAsset.DateModified = time;
            if (Audit)
            {
                newAsset.AuditDate = time;
                newAsset.AuditStatus = "AUDITED";
            }
            //make into class
            AssetClass asset = new AssetClass(newAsset);
            ///make into databse format
            var AssetForDb = AssetClass.AssetClassToDb(asset, false);
            //save to database
            Database.Assets.Insert(_connection, AssetForDb);
            if (Audit)
                await DisplayAlert("Complete", "Asset Created and Audited!", "OK");
            else
                await DisplayAlert("Complete", "Asset Created!", "OK");

            if (SaveAndReuse)
                ClearAsset();
            else
                await Navigation.PopToRootAsync();
        }
        private void ClearAsset()
        {
            _Asset = null;
            assetBarcode_.Text = "";
            assetSerialNo_.Text = "";
            auditStatus_.Text = "";
            auditDate_.Text = "";
            dateModified_.Text = "";
        }
        private async void DropDown(object sender, EventArgs e)
        {
            var ActionResult = await DisplayActionSheet("Asset Details Options", "Continue Editing", "Delete Asset", "Audit Asset", "Save Asset", "Save and reuse", "Cancel and Exit");
            switch (ActionResult)
            {
                case "Delete Asset":
                    DeleteQuestion();
                    break;
                case "Audit Asset":
                    AuditAsset(null, null);
                    break;
                case "Save Asset":
                    SaveAsset(null, null);
                    break;
                case "Save and Reuse":
                    SaveAndReuse(null, null);
                    break;
                case "Cancel and Exit":
                    CancelAndExit(null, null);
                    break;
                default:
                    break;
            }
        }

        private async void DeleteQuestion()
        {
            
            var deletepermission = JsonConvert.DeserializeObject<Permissions>(Database.Login.GetTableData(_connection).First().Permissions);
            if (deletepermission.DeleteAsset)
            {
                var result = await DisplayAlert("Warning!", "Are you sure that you want to delete this asset?", "Yes", "No");
                if (result)
                    Delete(null, null);
            }
            else
                await DisplayAlert("Error", "You do not have permission to delete assets", "Ok");



        }
        private void AuditAsset(object sender, EventArgs e)
        {
            //check if all fields are filled out
            if (RequiredData())
            {
                CheckAndSaveAsset(true);
            }
            else
            {
                DisplayAlert("Error", "There is a missing required data field. \"" + MissingField + "\" ", "OK");
            }
        }
        private async void CancelAndExit(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
        private void SaveAndReuse(object sender, EventArgs e)
        {
            //check if all fields are filled out
            if (RequiredData())
            {
                //check if it is a duplicate barcode
                var records = Database.Assets.GetTableDataByBarcode(_connection, assetBarcode_.Text);
                if (records.Count() > 1)
                {
                    if (_Asset != null)
                    {
                        CheckAndSaveAsset();
                    }
                    else
                    {
                        var loginRecords = Database.Login.GetTableData(_connection);
                        if (loginRecords.First().AllowDuplicateBarcode == "True")
                        {
                            //duplicate barcodes are allowed
                            CheckAndSaveAsset(false, true);
                        }
                        else
                        {
                            DisplayAlert("Warning", "Duplicate barcodes are not allowed.", "OK");
                        }
                    }
                }
                else
                {
                    //not duplicate, continue process
                    CheckAndSaveAsset(false, true);
                }
            }
            else
            {
                DisplayAlert("Error", "There is a missing required data field. \"" + MissingField + "\" ", "OK");
            }
        }
        private bool RequiredData()
        {
            try
            {
                var RequiredFields = JsonConvert.DeserializeObject<List<string>>(Database.Login.GetTableData(_connection).First().RequiredFields);

                foreach (string req_string in RequiredFields)
                {
                    switch (req_string)
                    {
                        case "AssetName":
                            MissingField = _Names.Name;
                            if (assetName_.Text == null)
                                return false;
                            break;
                        case "PeopleID":
                            MissingField = _Names.People;
                            if (person_.SelectedIndex < 0)
                                return false;
                            break;
                        case "SubLocation":
                            MissingField = _Names.SubLocation;
                            if (assetSubLocation_.SelectedIndex < 0)
                                return false;
                            break;
                        case "DataGatherID":
                            MissingField = _Names.Department;
                            if (assetDepartment_.SelectedIndex < 0)
                                return false;
                            break;
                        case "Barcode":
                            MissingField = _Names.Barcode;
                            if (assetBarcode_.Text == "")
                                return false;
                            break;
                        case "AssetStatus":
                            MissingField = _Names.AssetStatus;
                            if (assetStatus_.Text == "")
                                return false;
                            break;
                        case "AssetDescription":
                            MissingField = _Names.Description;
                            if (assetDescription_.Text == "")
                                return false;
                            break;
                        case "Asset_UID":
                            MissingField = _Names.Asset_UID;
                            if (AssetUID_.Text == "")
                                return false;
                            break;
                        case "AssetSerialNo":
                            MissingField = _Names.SerialNo;
                            if (assetSerialNo_.Text == "")
                                return false;
                            break;
                        case "PurchaseOrderNo":
                            MissingField = _Names.PurchaseOrderNo;
                            if (PurchaseOrderNo_.Text == "")
                                return false;
                            break;
                        case "Model":
                            MissingField = _Names.Model;
                            if (Model_.Text == "")
                                return false;
                            break;
                        case "SiteID":
                            MissingField = _Names.Site;
                            if (assetSite_.SelectedIndex < 0)
                                return false;
                            break;
                        case "LocationID":
                            MissingField = _Names.Location;
                            if (assetSite_.SelectedIndex < 0)
                                return false;
                            break;
                        case "Mfg":
                            MissingField = _Names.Mfg;
                            if (manufacturer_.SelectedIndex < 0)
                                return false;
                            break;
                        case "POLine":
                            MissingField = _Names.POLine;
                            if (POLineNo_.Text == "")
                                return false;
                            break;
                        case "AssetUsageID":
                            MissingField = _Names.Usage;
                            if (assetUsage_.SelectedIndex < 0)
                                return false;
                            break;
                        case "Vendor":
                            MissingField = _Names.Vendor;
                            if (vendor_.SelectedIndex < 0)
                                return false;
                            break;
                        case "AssetConditionID":
                            MissingField = _Names.Condition;
                            if (assetCondition_.SelectedIndex < 0)
                                return false;
                            break;
                        case "AssetCategoryID":
                            MissingField = _Names.Category;
                            if (_Category == null)
                                return false;
                            break;
                        case "Price":
                            MissingField = _Names.Price;
                            if (Cost_.Text == null)
                                return false;
                            try
                            {
                                Convert.ToDouble(Cost_.Text);
                            }
                            catch (Exception)
                            {
                                intfield = _Names.Price;
                                return false;
                            }
                            break;
                        case "Quantity":
                            MissingField = _Names.Quantity;
                            if (Cost_.Text == null)
                                return false;
                            try
                            {
                                Convert.ToInt16(Quantity_.Text);
                            }
                            catch (Exception)
                            {
                                intfield = _Names.Quantity;
                                return false;
                            }
                            break;
                        case "OriginalPartNo":
                            MissingField = _Names.OriginalPartNo;
                            if (OriginalPartNo_.Text == null)
                                return false;
                            try
                            {
                                Convert.ToInt16(OriginalPartNo_.Text);
                            }
                            catch (Exception)
                            {
                                intfield = _Names.OriginalPartNo;
                                return false;
                            }
                            break;
                    }
                }
                if (MissingLocationStack.IsVisible && MissingLoc.Text == null)
                {
                    MissingField = "New " + _Names.Location + " Name";
                    return false;
                }
                return true;
            }
            catch (Exception exe)
            {
                DependencyService.Get<IError>().SendRaygunError(exe, Application.Current.Properties["user"].ToString()??"Unsynced", Application.Current.Properties["url"].ToString()??"Unsynced", null);
                DisplayAlert("Error", exe.Message, "OK");
                return false;
            }
        }
        private AssetJsonObject GetData()
        {
            string locationName, siteName, personName, usageName, conditionName, vendorName, departmentName, mfgName,sublocation_, postatus = "";
            LocationClass LocationInfo = new LocationClass();
            SitesClass siteInfo = new SitesClass();
            PeopleClass personInfo = new PeopleClass();
            UsageClass UsageInfo = new UsageClass();
            ConditionClass conditionInfo = new ConditionClass();
            VendorClass vendorInfo = new VendorClass();
            DepartmentClass departmentInfo = new DepartmentClass();
            ManufacturerClass mfgInfo = new ManufacturerClass();
            AssetCategoryClass CategoryInfo = new AssetCategoryClass();


            //gets category id internal
            if (assetCategory_.Text != "Select Asset Category")
            {
                CategoryInfo = _Category.First();
            }

            //gets site
            if (assetSite_.SelectedIndex >= 0)
            {
                siteName = assetSite_.Items[assetSite_.SelectedIndex];
                siteInfo = SitesClass.DbToSite(Database.Sites.GetTableData(_connection), _connection).First(cm => cm.SiteName == siteName);
            }

            //gets location id internal
            if (assetLocation_.SelectedIndex >= 0)
            {
                if (assetLocation_.SelectedIndex == 0)
                {
                    string _Loc = GUID.Generate().Substring(0, 7);
                    LocationInfo = new LocationClass
                    {
                        LocationIdInternal = _Loc,
                        LocationName = MissingLoc.Text
                    };
                    Database.Locations.Insert(_connection, new LocationsDbTable
                    {
                        LocationBarcode = "Added "+ _Names.Location,
                        LocationDescription = "Added " + _Names.Location,
                        LocationIdInternal = _Loc,
                        LocationName = MissingLoc.Text,
                        SiteIdInternal = siteInfo.SiteIdInternal,
                    });
                    assetDescription_.Text = assetDescription_.Text + MissingLoc.Text;
                }
                else
                {
                    locationName = assetLocation_.Items[assetLocation_.SelectedIndex];
                    LocationInfo = LocationClass.DbToLocation(Database.Locations.GetTableDataFromSites(_connection, _siteID), _connection).First(cm => cm.LocationName == locationName);
                }
            }

            //gets people
            if (person_.SelectedIndex >= 0)
            {
                personName = person_.Items[person_.SelectedIndex];
                personInfo = PeopleClass.DbToPeople(Database.People.GetTableData(_connection), _connection).First(cm => cm.PeopleName == personName);
            }

            //gets usages
            if (assetUsage_.SelectedIndex >= 0)
            {
                usageName = assetUsage_.Items[assetUsage_.SelectedIndex];
                UsageInfo = UsageClass.DbToUsage(Database.Usage.GetTableData(_connection), _connection).First(cm => cm.UsageLabel == usageName);
            }

            //gets condition
            if (assetCondition_.SelectedIndex >= 0)
            {
                conditionName = assetCondition_.Items[assetCondition_.SelectedIndex];
                conditionInfo = ConditionClass.DbToCondition(Database.Condition.GetTableData(_connection), _connection).First(cm => cm.ConditionLabel == conditionName);
            }

            //gets vendor 
            if (vendor_.SelectedIndex >= 0)
            {
                vendorName = vendor_.Items[vendor_.SelectedIndex];
                vendorInfo = VendorClass.DbToVendor(Database.Vendor.GetTableData(_connection), _connection).First(cm => cm.Company == vendorName);
            }

            //gets department
            if (assetDepartment_.SelectedIndex >= 0)
            {
                departmentName = assetDepartment_.Items[assetDepartment_.SelectedIndex];
                departmentInfo = DepartmentClass.DbToDepartment(Database.Department.GetTableData(_connection)).First(cm => cm.DepartmentName == departmentName);
            }

            // gets Mfg

            if (manufacturer_.SelectedIndex >= 0)
            {
                mfgName = manufacturer_.Items[manufacturer_.SelectedIndex];
                mfgInfo = ManufacturerClass.DbToManufacturer(Database.Manufacturer.GetTableData(_connection), _connection).First(cm => cm.Company == mfgName);
            }

            //gets PO Status
            if (POStatus_.SelectedIndex >= 0)
            {
                switch (POStatus_.Items[POStatus_.SelectedIndex])
                {
                    case "N/A":
                        postatus = "N";
                        break;
                    case "Open":
                        postatus = "O";
                        break;
                    case "Closed":
                        postatus = "C";
                        break;
                }
            }

            if (assetSubLocation_.SelectedIndex == -1)
            {
                sublocation_ = null;
            }
            else
                sublocation_ = assetSubLocation_.Items[assetSubLocation_.SelectedIndex];
            try
            {
                string thumb = "";
                if (_thumbStream == null)
                    thumb = null;
                else
                {
                    thumb = _thumbnailImg;
                }
                string assetID_ = "";
                if (_Asset != null)
                    assetID_ = _Asset.AssetIdInternal;
                else
                    assetID_ = GUID.Generate();

                var retrievedData = new AssetJsonObject
                {
                    DateModified = DateTime.Now,
                    SubLocation = sublocation_,
                    AssetName = assetName_.Text,
                    AssetDescription = assetDescription_.Text,
                    Barcode = assetBarcode_.Text,
                    AssetIDInternal = assetID_,
                    AssetCategoryIDInternal = CategoryInfo.AssetCategoryIDInternal,
                    LocationIDInternal = LocationInfo.LocationIdInternal,
                    SiteIDInternal = siteInfo.SiteIdInternal,
                    PeopleIDInternal = personInfo.PeopleIDInternal ?? "00000000-0000-0000-0000-000000000000",
                    AssetUsageID = UsageInfo.UsageID,
                    AssetConditionID = conditionInfo.ConditionID,
                    AssetStatus = assetStatus_.Text,
                    AssetSerialNo = assetSerialNo_.Text,
                    Price = Convert.ToDouble(Cost_.Text),
                    PurchaseDate = (PurchaceDateForceChanged) ? Convert.ToDateTime("1/1/1900") : PurchaseDate_.Date,
                    PurchaseOrderNo = PurchaseOrderNo_.Text,
                    Quantity = Convert.ToInt32(Quantity_.Text),
                    ShortageOverage = Convert.ToInt32(shortage_.Text),
                    Vendor = vendorInfo.VendorIDInternal,
                    IsActive = true,
                    AuditStatus = auditStatus_.Text,
                    Asset_UID = AssetUID_.Text,
                    DataGatherID = departmentInfo.ID,
                    Mfg = mfgInfo.VendorIDInternal,
                    Model = Model_.Text,
                    POLine = Convert.ToInt32(POLineNo_.Text),
                    POStatus = postatus,
                    //if saving, date modified has to be set to now 
                    OriginalPartNo = OriginalPartNo_.Text,
                    ThumbnailImage = thumb,
                };
                if (auditDate_.Text == "")
                    retrievedData.AuditDate = Convert.ToDateTime("1/1/1900 12:00:00 AM");
                else
                    retrievedData.AuditDate = Convert.ToDateTime(auditDate_.Text);
                return retrievedData;
            }
            catch (Exception e)
            {
                DependencyService.Get<IError>().SendRaygunError(e, Application.Current.Properties["user"].ToString()??"Unsynced", Application.Current.Properties["url"].ToString()??"Unsynced", null);
                DisplayAlert("Error", e.Message, "OK");
                throw new Exception("Unable to get assset information");
            }
        }
        private void ConvertBinaryToImage(string thumbnailBinary)
        {
            try
            {
                Thumbnail_Image.Source = ImageSource.FromStream(() => AppImages.Base64toStream(thumbnailBinary));
                _thumbStream = AppImages.Base64toStream(thumbnailBinary);
                _thumbnailImg = thumbnailBinary;
            }
            catch (Exception Exc)
            {
                DependencyService.Get<IError>().SendRaygunError(Exc, Application.Current.Properties["user"].ToString()??"Unsynced", Application.Current.Properties["url"].ToString()??"Unsynced", null);
                DisplayAlert("Error", Exc.Message, "OK");
            }
            
        }

        private void ClearThumbnail(object sender, EventArgs e)
        {
            Thumbnail_Image.Source = ImageSource.FromResource("eQuipMobile.Images.no_image.png");
        }
        private async void CaptureImage(object sender, EventArgs e)
        {
            try
            {
                var result_ = await DisplayAlert("Image options", "How would you like to capture the thumbnail Image?", "Gallery", "Camera");
                if (result_)
                    GetByGallery(sender, e);
                else
                {
                    var photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions() { });

                    if (photo != null)
                    {
                        Thumbnail_Image.Source = ImageSource.FromStream(() =>{return photo.GetStream();});
                        _thumbStream = photo.GetStream();
                        _thumbnailImg = AppImages.StreamToBase64String(photo.GetStream());
                    }
                }
                //await DisplayAlert("OK", "Get by camera", "OK");
            }
            catch (Exception exc)
            {
                DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString()??"Unsynced", Application.Current.Properties["url"].ToString()??"Unsynced", null);
                await DisplayAlert("Error", exc.Message, "OK");
            }
        }
        private async void GetByGallery(object sender, EventArgs e)
        {
            try
            {
                //await DisplayActionSheet("Would you Like to use ")
                await CaptureImageAsync(sender, e);
            }
            catch (Exception exc)
            {
                DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString()??"Unsynced", Application.Current.Properties["url"].ToString()??"Unsynced", null);
                await DisplayAlert("Error", exc.Message, "OK");
            }
        }
        private async System.Threading.Tasks.Task CaptureImageAsync(object sender, EventArgs e)
        {
            try
            {
                pickPictureButton.IsEnabled = false;
                Stream stream = await DependencyService.Get<IPicturePicker>().GetImageStreamAsync();

                if (stream != null)
                {
                    _thumbStream = stream;
                    _thumbnailImg = AppImages.StreamToBase64String(_thumbStream);
                    Thumbnail_Image.Source = ImageSource.FromStream(() => AppImages.Base64toStream(_thumbnailImg));

                    //_thumbnailImg = AppImages.StreamToBase64String(_thumbStream);

                }
                else
                {
                    await DisplayAlert("Error", "Image was not selected. Please try again.", "OK");
                }
                pickPictureButton.IsEnabled = true;
            }
            catch (Exception exc)
            {
                await DisplayAlert("Error", exc.Message, "OK");
                pickPictureButton.IsEnabled = true;
            }
           
        }
        private void BarcodeIconTapped(object sender, EventArgs e)
        {
            BarcodeScannerPage();
        }

        private async void BarcodeScannerPage()
        {

            try
            {
                //setup options
                var frontCamera = Database.Settings.GetTableData(_connection).FrontCamera;
                var options = new MobileBarcodeScanningOptions
                {
                    AutoRotate = false,
                    UseFrontCameraIfAvailable = frontCamera,
                    TryHarder = true,
                };

                //add options and customize page
                var scanPage = new ZXingScannerPage(options)
                {
                    DefaultOverlayTopText = "Align the barcode within the frame",
                    DefaultOverlayBottomText = string.Empty,
                    DefaultOverlayShowFlashButton = true
                };
                scanPage.Title = "Scanning";
                string _result = "";
                scanPage.OnScanResult += (result) =>
                {
                    // Stop scanning
                    scanPage.IsScanning = false;

                    // Pop the page and show the result
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Navigation.PopAsync();
                        _result = result.Text;
                        assetBarcode_.Text = _result;
                    });
                };

                // Navigate to our scanner page
                await Navigation.PushAsync(scanPage);
            }
            catch (Exception exc)
            {
                DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString()??"unsynced", Application.Current.Properties["url"].ToString()??"unsynced", null);
                await DisplayAlert("Error", exc.Message, "OK");
            }
        }
    }
}