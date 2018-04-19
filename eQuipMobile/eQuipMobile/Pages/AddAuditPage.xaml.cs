using SQLite;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace eQuipMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddAuditPage : ContentPage
    {
        private SQLiteConnection _connection;
        string _siteID;
        AssetDetailNames _Names;
        string MissingField;
        public AddAuditPage(AssetDetailNames Names_, string site, string location, string sublocation, int department, string barcode, SQLiteConnection connection)
        {
            _Names = Names_;
            _connection = connection;
            InitializeComponent();
            BindingContext = _Names;
            assetBarcode_.Text = barcode;
            LoadSites(site);
            LoadLocation(site, location);
            LoadSublocations(location, sublocation);
            LoadDepartment(department);
            LoadCondition();
            LoadUsage();
        }

        private void assetSite__SelectedIndexChanged(object sender, EventArgs e)
        {
            if (assetSite_.SelectedIndex != -1)
            {
                assetLocation_.Items.Clear();
                var siteName = assetSite_.Items[assetSite_.SelectedIndex];
                var SiteForLocation = SitesClass.DbToSite(Database.Sites.GetTableData(_connection), _connection).First(cm => cm.SiteName == siteName);
                LoadLocation(SiteForLocation.SiteIdInternal);
            }
        }

        private void assetLocation__SelectedIndexChanged(object sender, EventArgs e)
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
                        LoadSublocations();
                    }
                }
            }
        }
        private void LoadSites(string siteID = null)
        {
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
        private void LoadLocation(string siteID = null, string LocationID = null)
        {
            _siteID = siteID;
            var selectedLocationName = "";
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
        private void LoadSublocations(string LocationID = null, string sublocation_ = null)
        {
            var selectedSublocation = "";
            var SublocationList = SublocationClass.DbToSubLocation(Database.SubLocations.GetTableDataFromLocations(_connection, LocationID));
            foreach (SublocationClass sublocation in SublocationList)
            {
                assetSubLocation_.Items.Add(sublocation.SubLocationName);
                if (LocationID != null)
                {
                    if (sublocation.SubLocationName == sublocation_)
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

        private void Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            try
            {
                if (CheckValues())
                {
                    LocationClass LocationInfo = new LocationClass();
                    string _LocationID = "00000000-0000-0000-0000-00000000";
                    string _Sublocation = null;
                    int _Department = 0;
                    string desc = "Asset created during audit. ";

                    string siteName = assetSite_.Items[assetSite_.SelectedIndex];
                    string _SiteID = SitesClass.DbToSite(Database.Sites.GetTableData(_connection), _connection).First(cm => cm.SiteName == siteName).SiteIdInternal;

                    //gets location id internal
                    if (assetLocation_.SelectedIndex >= 0)
                    {
                        _LocationID = GUID.Generate().Substring(0, 7);
                        if (assetLocation_.SelectedIndex == 0)
                        {
                            if (MissingLoc.Text == null)
                                MissingLoc.Text = "Location added without name";
                            Database.Locations.Insert(_connection, new LocationsDbTable
                            {
                                LocationBarcode = "Added " + _Names.Location,
                                LocationDescription = "Added " + _Names.Location,
                                LocationIdInternal = _LocationID,
                                LocationName = MissingLoc.Text,
                                SiteIdInternal = _SiteID,
                            });
                            desc = desc + MissingLoc.Text;
                        }
                        else
                        {
                            string locationName = assetLocation_.Items[assetLocation_.SelectedIndex];
                            _LocationID = LocationClass.DbToLocation(Database.Locations.GetTableDataFromSites(_connection, _siteID), _connection).First(cm => cm.LocationName == locationName).LocationIdInternal;
                        }
                    }

                    if (assetSubLocation_.SelectedIndex == -1)
                    {
                        _Sublocation = null;
                    }
                    else
                        _Sublocation = assetSubLocation_.Items[assetSubLocation_.SelectedIndex];

                    if (assetDepartment_.SelectedIndex >= 0)
                    {
                        string departmentName = assetDepartment_.Items[assetDepartment_.SelectedIndex];
                        _Department = DepartmentClass.DbToDepartment(Database.Department.GetTableData(_connection)).First(cm => cm.DepartmentName == departmentName).ID;
                    }

                    //gets condition
                    ConditionClass conditionInfo = new ConditionClass();
                    if (assetCondition_.SelectedIndex >= 0)
                    {
                        string conditionName = assetCondition_.Items[assetCondition_.SelectedIndex];
                        conditionInfo = ConditionClass.DbToCondition(Database.Condition.GetTableData(_connection), _connection).First(cm => cm.ConditionLabel == conditionName);
                    }

                    //gets usages
                    UsageClass UsageInfo = new UsageClass();
                    if (assetUsage_.SelectedIndex >= 0)
                    {
                        string usageName = assetUsage_.Items[assetUsage_.SelectedIndex];
                        UsageInfo = UsageClass.DbToUsage(Database.Usage.GetTableData(_connection), _connection).First(cm => cm.UsageLabel == usageName);
                    }
                    string defcategory = Database.Category.GetTableData(_connection).First().AssetCategoryIDInternal;

                    AssetJsonObject newAsset = new AssetJsonObject
                    {
                        SiteIDInternal = _SiteID,
                        LocationIDInternal = _LocationID,
                        SubLocation = _Sublocation,
                        DataGatherID = _Department,
                        AssetIDInternal = GUID.Generate(),
                        IsActive = true,
                        DateModified = DateTime.Now,
                        AuditDate = DateTime.Now,
                        AuditStatus = "RECON-Added",
                        AssetName = "RECON-Added Asset",
                        AssetDescription = desc,
                        Barcode = assetBarcode_.Text,
                        PeopleIDInternal = "00000000-0000-0000-0000-000000000000",
                        AssetStatus = "",
                        AssetSerialNo = "",
                        Price = 0,
                        PurchaseDate = Convert.ToDateTime("1/1/1900"),
                        PurchaseOrderNo = "",
                        Quantity = 1,
                        ShortageOverage = 1,
                        Vendor = "",
                        Asset_UID = "",
                        ThumbnailImage = "",
                        OriginalPartNo = "",
                        POStatus = "N",
                        POLine = 0,
                        Model = "",
                        Mfg = "",
                        AssetConditionID = conditionInfo.ConditionID,
                        AssetUsageID = UsageInfo.UsageID,
                        AssetCategoryIDInternal = defcategory
                    };

                    //make into class
                    AssetClass asset = new AssetClass(newAsset);
                    ///make into databse format
                    var AssetForDb = AssetClass.AssetClassToDb(asset, false);
                    //save to database
                    Database.Assets.Insert(_connection, AssetForDb);
                    Application.Current.Properties["AuditedSite"] = newAsset.SiteIDInternal;
                    Application.Current.Properties["AuditedLocation"] = newAsset.LocationIDInternal;
                    Application.Current.Properties["AuditedSublocation"] = newAsset.SubLocation;
                    Application.Current.Properties["LastAssetID"] = newAsset.AssetIDInternal;
                    Application.Current.Properties["Department"] = newAsset.DataGatherID;
                    await DisplayAlert("Complete", "Asset Audited", "OK");
                    await Navigation.PopModalAsync();
                }
                else
                {
                    await DisplayAlert("Error", "There is a missing required data field. \"" + MissingField + "\" ", "OK");
                }
            }
            catch (Exception exe)
            {
                DependencyService.Get<IError>().SendRaygunError(exe, Application.Current.Properties["user"].ToString() ?? "Unsynced", Application.Current.Properties["url"].ToString() ?? "Unsynced", null);
                await DisplayAlert("Error", exe.Message, "OK");
            }
        }
        
        private bool CheckValues()
        {
            if (assetName_.Text == null)
            {
                MissingField = _Names.Name;
                return false;
            }
            else if (assetBarcode_.Text == null)
            {
                MissingField = _Names.Barcode;
                return false;
            }
            else if (MissingLocationStack.IsVisible && MissingLoc.Text == null)
            {
                MissingField = "New "+ _Names.Location;
                return false;
            }
            else if (assetCondition_.SelectedIndex < 0)
            {
                MissingField = _Names.Condition;
                return false;
            }
            else if (assetSite_.SelectedIndex < 0)
            {
                MissingField = _Names.Site;
                return false;
            }
            else if (assetSerialNo_.Text == null)
            {
                MissingField = _Names.SerialNo;
                return false;
            }
            else if (assetUsage_.SelectedIndex < 0)
            {
                MissingField = _Names.Usage;
                return false;
            }
            else
                return true;
        }
    }
}