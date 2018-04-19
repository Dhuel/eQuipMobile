using Newtonsoft.Json;
using Plugin.Connectivity;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace eQuipMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SaveAndContinuePage : ContentPage
    {
        private SQLiteConnection _connection;
        string _SiteID, _LocationID, _Sublocation;
        SettingsDb SettingsData;
        AssetDetailNames _Names;
        bool Displaymsg;
        public SaveAndContinuePage(SQLiteConnection connection_, AssetDetailNames Names_, string SiteID, string LocationID, string Sublocation = null)
        {
            _Names = Names_;
            _connection = connection_;
            InitializeComponent();
            BarcodeImage.Source = ImageSource.FromResource("eQuipMobile.Images.Camera_icon.png");
            if (Application.Current.MainPage.Width < 768)
                BarcodeImage.Margin = new Thickness(10, 0, 10, 0);
            BarcodeBartext.Text = "Scan the " + Names_.Barcode + " or type it into the search bar to audit it.";
            _SiteID = SiteID;
            _LocationID = LocationID;
            _Sublocation = Sublocation;
            if (Application.Current.Properties.ContainsKey("Department") && Application.Current.Properties["Department"] != null)
                LoadDepartment(Convert.ToInt16(Application.Current.Properties["Department"].ToString()));
            else
                LoadDepartment();
        }

        protected override void OnAppearing()
        {
            LoadAssetData();
            if (!Displaymsg)
                DisplayAlert("Attention!", "When an asset is audited, there will be a notification sound and the last asset audited on the screen will be updated. If you will not be looking on the screen, please turn up your volume.", "OK");
            Displaymsg = true;
            base.OnAppearing();
        }


        private void LoadAssetData()
        {
            if (Application.Current.Properties.ContainsKey("LastAssetID") && Application.Current.Properties["LastAssetID"] != null)
            {
                //get asset based on ID then assign it to list
                var IEnumAsset = Database.Assets.GetAssetDataByAssetIDInternal(_connection, Application.Current.Properties["LastAssetID"].ToString());
                if (Database.Settings.GetTableData(_connection).BlindAudit)
                {
                    AssetListView2.IsVisible = true;
                    AssetListView.IsVisible = false;
                    AssetListView2.ItemsSource = AssetClass.DbToAssetClass(IEnumAsset, _Names);
                }
                else
                {
                    AssetListView.IsVisible = true;
                    AssetListView2.IsVisible = false;
                    AssetListView.ItemsSource = AssetClass.DbToAssetClass(IEnumAsset, _Names);
                }
            }
            else
            {
                var _defaultAsset = new AssetClass
                {
                    AssetName = "Default Asset Name",
                    AuditStatusDisplay_ = _Names.AssetStatus,
                    Barcode = "Default Barcode",
                    QuantityDisplay_ = _Names.Quantity,
                    OriginalPartDisplay_ = _Names.OriginalPartNo,
                    UIDDisplay_ = _Names.Asset_UID,
                    AssetSerialNumberDisplay_ = _Names.SerialNo,
                    BarcodeDisplay_ = _Names.Barcode,
                    AssetJSON = new AssetJsonObject
                    {
                        AssetStatus = "Default Asset Status",
                        Quantity = 0,
                        OriginalPartNo = "00000000",
                        Asset_UID = "Example UID",
                        AssetSerialNo = "01010101",
                        Barcode = "Default Barcode",
                        AuditStatus = "RECON",

                    }
                };
                AssetListView.ItemsSource = new List<AssetClass> { _defaultAsset };
            }


            SettingsData = Database.Settings.GetTableData(_connection);
        }

        private void AssetListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            AssetListView.SelectedItem = null;
            AssetListView2.SelectedItem = null;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                SearchButton.IsEnabled = false;
                string departmentName;
                DepartmentClass departmentInfo = new DepartmentClass();
                if (assetDepartment_.SelectedIndex >= 1)
                {
                    departmentName = assetDepartment_.Items[assetDepartment_.SelectedIndex];
                    departmentInfo = DepartmentClass.DbToDepartment(Database.Department.GetTableData(_connection)).First(cm => cm.DepartmentName == departmentName);
                }

                if (AuditSearchBar.Text == null)
                    await DisplayAlert("Error", "Please enter a barcode", "OK");
                else
                {
                    var assetData = Database.Assets.GetTableDataByBarcodeLike(_connection, AuditSearchBar.Text);
                    if (assetData.Count() > 0)
                    {
                        //Not empty 1 or greater
                        if (assetData.Count() == 1)
                        {
                            if (SettingsData.FastAudit)
                            {
                                //Fast audit turned on on single asset
                                try
                                {
                                    var _assetclass = AssetClass.DbToAssetClass(assetData, _Names).First();
                                    AuditClass.Audit(_SiteID, _LocationID, _Sublocation, departmentInfo.ID, _assetclass, _connection);
                                    Application.Current.Properties["AuditedSite"] = _SiteID;
                                    Application.Current.Properties["AuditedLocation"] = _LocationID;
                                    Application.Current.Properties["AuditedSublocation"] = _Sublocation;
                                    Application.Current.Properties["LastAssetID"] = _assetclass.AssetIdInternal;
                                    Application.Current.Properties["Department"] = departmentInfo.ID;
                                    await Application.Current.SavePropertiesAsync();
                                    LoadSound(1);
                                    //await DisplayAlert("Complete","Asset Audited", "OK");
                                    LoadAssetData();
                                }
                                catch (Exception exc)
                                {
                                    SearchButton.IsEnabled = true;
                                    DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                                    await DisplayAlert("Error", exc.Message, "OK");
                                }
                            }
                            else
                            {
                                //regular audit
                                await Navigation.PushModalAsync(new AuditRecordPage(_Names, assetData.First(), _SiteID, _LocationID, _Sublocation, departmentInfo.ID, _connection));
                                //bring up screen
                            }
                        }
                        else
                        {
                            //Display multiple
                            LoadSound(2);
                            await Navigation.PushAsync(new AuditList(_Names, assetData, _connection, true, _SiteID, _LocationID, _Sublocation, departmentInfo.ID));
                        }
                    }
                    else
                    {
                        if (CrossConnectivity.Current.IsConnected)
                        {
                            //check database for asset
                            var logindata_ = Database.Login.GetTableData(_connection);
                            var test = await SyncClass.NewSyncClass.API_GetAssetByBarcode(logindata_.First().UserID, Application.Current.Properties["url"].ToString(), AuditSearchBar.Text);
                            var record = JsonConvert.DeserializeObject<List<AssetJsonObject>>(test);
                            if (record.Count() > 0)
                            {
                                var AssetClassRecord = new AssetClass(record.First(), true);
                                Database.Assets.Insert(_connection, AssetClass.AssetClassToDb(AssetClassRecord, true));
                                if (SettingsData.FastAudit)
                                {
                                    //may need to insert then audit
                                    AuditClass.Audit(_SiteID, _LocationID, _Sublocation, departmentInfo.ID, AssetClassRecord, _connection);
                                    Application.Current.Properties["AuditedSite"] = _SiteID;
                                    Application.Current.Properties["AuditedLocation"] = _LocationID;
                                    Application.Current.Properties["AuditedSublocation"] = _Sublocation;
                                    Application.Current.Properties["LastAssetID"] = AssetClassRecord.AssetIdInternal;
                                    LoadSound(1);
                                    //await DisplayAlert("Complete", "Asset Audited", "OK");
                                    LoadAssetData();
                                }
                                else
                                {
                                    //display the asset record pulled
                                    //convert asset class to asset db class
                                    await Navigation.PushModalAsync(new AuditRecordPage(_Names, AssetClass.AssetClassToDb(AssetClassRecord, false), _SiteID, _LocationID, _Sublocation, departmentInfo.ID, _connection));
                                }
                            }
                            else
                            {
                                if (SettingsData.FastAudit && !SettingsData.FastAuditEntry)
                                {
                                    AddAssetData(departmentInfo.ID);
                                }
                                else
                                {
                                    LoadSound(0);
                                    //display screen to add asset 
                                    await Navigation.PushModalAsync(new AddAuditPage(_Names, _SiteID, _LocationID, _Sublocation, departmentInfo.ID, AuditSearchBar.Text, _connection));
                                }
                            }
                        }
                        else
                        {
                            LoadSound(0);
                            //Add Asset
                            if (SettingsData.FastAudit && !SettingsData.FastAuditEntry)
                            {
                                AddAssetData(departmentInfo.ID);
                            }
                            else
                            {
                                //display screen to add asset
                                await Navigation.PushModalAsync(new AddAuditPage(_Names, _SiteID, _LocationID, _Sublocation, departmentInfo.ID, AuditSearchBar.Text, _connection));
                            }
                        }
                    }
                }
                SearchButton.IsEnabled = true;
            }
            catch (Exception exc)
            {
                SearchButton.IsEnabled = true;
                DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                await DisplayAlert("OK", exc.Message, "OK");
            }
        }

        private void LoadSound(int soundByte)
        {
            Stream audioStream = null;
            try
            {
                var assembly = typeof(App).GetTypeInfo().Assembly;
                if (soundByte == 0)
                    audioStream = assembly.GetManifestResourceStream("eQuipMobile.Sounds." + "beep.mp3");
                else if (soundByte == 1)
                    audioStream = assembly.GetManifestResourceStream("eQuipMobile.Sounds." + "confirmBeep.mp3");
                else
                    audioStream = assembly.GetManifestResourceStream("eQuipMobile.Sounds." + "Alert.mp3");
                var player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
                player.Load(audioStream);
                player.Play();
            }
            catch (Exception exc)
            {
                DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
            }
        }

        private void CancelButton(object sender, EventArgs e)
        {
            Navigation.PopToRootAsync();
        }

        private void ShowSummary(object sender, EventArgs e)
        {
            if (assetDepartment_.SelectedIndex >= 1)
            {
                var departmentName = assetDepartment_.Items[assetDepartment_.SelectedIndex];
                var departmentInfo = DepartmentClass.DbToDepartment(Database.Department.GetTableData(_connection)).First(cm => cm.DepartmentName == departmentName);
                Navigation.PushModalAsync(new ShowSummaryPage(_connection, _Names, _SiteID, _LocationID, _Sublocation, departmentInfo.ID));
            }
            else
            {
                Navigation.PushModalAsync(new ShowSummaryPage(_connection, _Names, _SiteID, _LocationID, _Sublocation));
            }
        }

        private void LoadDepartment(int DepartmentId = 0)
        {
            var selectedDepartment = "";
            var DepartmentsList = DepartmentClass.DbToDepartment(Database.Department.GetTableData(_connection));
            assetDepartment_.Items.Add("Select " + _Names.Department);
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

        private void AddAssetData(int department_)
        {
            try
            {
                var AssetCategory = Database.Category.GetTableData(_connection).First().AssetCategoryIDInternal;
                var AssetUsage = Database.Usage.GetTableData(_connection).First().UsageID;
                var AssetCondition = Database.Condition.GetTableData(_connection).First().ConditionID;
                var time = DateTime.Now;
                var newAsset = new AssetJsonObject
                {
                    DateModified = time,
                    SubLocation = _Sublocation,
                    AssetName = "RECON-Added Asset",
                    AssetDescription = "Asset created during audit.",
                    Barcode = AuditSearchBar.Text,
                    AssetIDInternal = GUID.Generate(),
                    AssetCategoryIDInternal = AssetCategory,
                    LocationIDInternal = _LocationID,
                    SiteIDInternal = _SiteID,
                    PeopleIDInternal = "00000000-0000-0000-0000-000000000000",
                    AssetUsageID = AssetUsage,
                    AssetConditionID = AssetCondition,
                    AssetStatus = "",
                    AssetSerialNo = "",
                    Price = 0,
                    PurchaseDate = Convert.ToDateTime("1/1/1900"),
                    PurchaseOrderNo = "",
                    Quantity = 1,
                    ShortageOverage = 1,
                    Vendor = "",
                    IsActive = true,
                    AuditDate = time,
                    AuditStatus = "RECON-Added",
                    Asset_UID = "",
                    DataGatherID = department_,
                    Mfg = "",
                    Model = "",
                    POLine = 0,
                    POStatus = "N",
                    OriginalPartNo = "",
                    ThumbnailImage = "",
                };
                Application.Current.Properties["LastAssetID"] = newAsset.AssetIDInternal;

                AssetClass asset = new AssetClass(newAsset);
                ///make into databse format
                var AssetForDb = AssetClass.AssetClassToDb(asset, false);
                //save to database
                Database.Assets.Insert(_connection, AssetForDb);
                LoadSound(1);
                //DisplayAlert("Complete", "Asset Audited", "OK");
                LoadAssetData();
            }
            catch (Exception exc)
            {
                DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                DisplayAlert("Error", exc.Message, "OK");
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
                var options = new MobileBarcodeScanningOptions
                {
                    AutoRotate = false,
                    UseFrontCameraIfAvailable = false,
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
                        AuditSearchBar.Text = _result;
                        Button_Clicked(null, null);
                    });
                };

                // Navigate to our scanner page
                await Navigation.PushAsync(scanPage);
            }
            catch (Exception exc)
            {
                DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                await DisplayAlert("Error", exc.Message, "OK");
            }
        }
    }
}