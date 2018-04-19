using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace eQuipMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScanBarcodePage : ContentPage
    {
        private SQLiteConnection _connection;
        string _Person;
        bool _PropertyPass;
        string DataIden, IAC_val, EIDCODE, IAD;
        int IAC_len;
        List<AssetDisplayClass> _PropertyPassDisplayList;
        PropertyPassDbTable _PropertyPassTableID;
        List<AssetDisplayClass> defaultList;
        AssetDetailNames _Names;

        public ScanBarcodePage(SQLiteConnection connection, string Person = null, bool PropertyPass = false, PropertyPassDbTable PropertyPassTableID = null)
        {
            MessagingCenter.Subscribe<string>(this, "Datawedge", (sender) => {
                searchbar.Text = sender;
            });
            loadvalues();
            if (Person != null)
                _Person = Person;
            _PropertyPass = PropertyPass;
            InitializeComponent();
            BarcodeImage.Source = ImageSource.FromResource("eQuipMobile.Images.Camera_icon.png");
            if (Application.Current.MainPage.Width < 768)
                BarcodeImage.Margin = new Thickness(10, 0, 10, 0);
            if (_PropertyPass)
            {
                _PropertyPassTableID = PropertyPassTableID;
                ScanLabel.Text = "Scan barcode of asset to add to property pass";
            }
            _connection = connection;
            assetbarcodeListview.ItemsSource = defaultList;
        }

        private async void SearchBar_SearchButtonPressed(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(searchbar.Text))
            {
                await DisplayAlert("Error", "Nothing was entered.", "OK");
                assetbarcodeListview.ItemsSource = defaultList;
            }
            else
            {
                if (_PropertyPass)
                {
                    //check if the asset is already in the property pass item table, if not, continue
                    var AssetForPropertyPass = Database.Assets.GetDisplayeDataByBarcode(_connection, searchbar.Text);
                    if (AssetForPropertyPass.Count() > 0)
                    {
                        if (Database.PropertyPassItem.GetTableDataByAssetID(_connection, AssetForPropertyPass.First().AssetIdInternal).Count() > 0)
                        {
                            await DisplayAlert("Error", "That asset has already been checked out", "OK");
                        }
                        else
                        {
                            if (_PropertyPassDisplayList == null)
                            {
                                _PropertyPassDisplayList = new List<AssetDisplayClass>();
                                CheckOutButton.IsVisible = true;
                            }
                            // check if the list already contains that value
                            if (_PropertyPassDisplayList.Exists(x => x.AssetIdInternal == AssetForPropertyPass.First().AssetIdInternal))
                                await DisplayAlert("Error", "That asset has already been scanned for this property pass.", "OK");
                            else
                            {
                                _PropertyPassDisplayList.Add(AssetDisplayClass.FullDetails(AssetForPropertyPass.First(), _Names));
                                assetbarcodeListview.ItemsSource = null;
                                assetbarcodeListview.ItemsSource = _PropertyPassDisplayList;
                            }
                        }
                    }
                    else
                    {
                        await DisplayAlert("Error", "No asset was found matching that value. The full barcode is required.", "OK");
                    }
                }
                else
                {
                    try
                    {
                        if (IsIUID())
                        {
                            SearchIUID();
                        }
                        else
                        {
                            var results = Database.Assets.GetDisplayeDataByBarcodeLike(_connection, searchbar.Text);
                            if (results.Count() > 0)
                            {
                                var test = new List<AssetDisplayClass>();
                                foreach (AssetDisplayClass Asset in results)
                                {
                                    test.Add(AssetDisplayClass.FullDetails(Asset, _Names));
                                }
                                assetbarcodeListview.ItemsSource = test;
                            }
                            else
                            {
                                assetbarcodeListview.ItemsSource = defaultList;
                                await DisplayAlert("Error", "No assets with that " + _Names.Barcode + " were found", "OK");
                            }
                        }
                    }
                    catch (Exception exp)
                    {
                        DependencyService.Get<IError>().SendRaygunError(exp, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                        await DisplayAlert("Error", exp.Message, "OK");
                    }
                }
            } 
        }

        private void SearchIUID()
        {
            //bool UIUDtype;
            string PartNum;
            string SerialNum_;
            var IUIDval = searchbar.Text.Trim();
            var start_ = IUIDval.IndexOf("~d0291P");
            var end = IUIDval.Substring(start_ + 7).IndexOf("~d029");
            if (start_ == -1)
            {
                //UIUDtype = true;
                start_ = IUIDval.IndexOf("~d03006~d029");
                end = IUIDval.Substring(start_ + 12).IndexOf("~d030~d004");
                SerialNum_ = IUIDval.Substring(start_, end);
                SerialNum_ = cutDown(cutDown(SerialNum_));
                if (SerialNum_.Length > IAC_len)
                {
                    IAD = SerialNum_.Substring(0, IAC_len);
                    SerialNum_ = SerialNum_.Substring(IAC_len);
                    if (SerialNum_.Length > 0)
                    {
                        //search using serial number
                        var results = Database.Assets.GetDisplayeDataBySerailNumberLike(_connection, SerialNum_);
                        if (results.Count() > 0)
                        {
                            var test = new List<AssetDisplayClass>();
                            foreach (AssetDisplayClass Asset in results)
                            {
                                test.Add(AssetDisplayClass.FullDetails(Asset, _Names));
                            }
                            assetbarcodeListview.ItemsSource = test;
                        }
                        else
                        {
                            assetbarcodeListview.ItemsSource = defaultList;
                            DisplayAlert("Error", "No assets with that "+ _Names.SerialNo +" were found", "OK");
                        }
                    }
                    else
                    {
                        DisplayAlert("Error", "No " + _Names.SerialNo + " was parsed from that query string.", "OK");
                    }
                }
                else
                {
                    DisplayAlert("Error", "The UID barcode could not be parsed, please check it and try again", "OK");
                }

            }
            else
            {
               // UIUDtype = false;
                PartNum = IUIDval.Substring(start_ + 7, end);
                start_ = IUIDval.IndexOf("~d029S");
                end = IUIDval.Substring(start_ + 6).IndexOf("~d030~d004");
                SerialNum_ = IUIDval.Substring(start_ = 6, end);
                start_ = IUIDval.IndexOf("~d03006~d029");
                end = IUIDval.Substring(start_ + 12).IndexOf("~d0291P");
                IAD = cutDown(IUIDval.Substring(start_ + 12, end));
                if (PartNum.Length > 0 && SerialNum_.Length > 0)
                {
                    var results = Database.Assets.GetDisplayeDataBySerailandPartNumberLike(_connection, SerialNum_, PartNum);
                    if (results.Count() > 0)
                    {
                        var test = new List<AssetDisplayClass>();
                        foreach (AssetDisplayClass Asset in results)
                        {
                            test.Add(AssetDisplayClass.FullDetails(Asset, _Names));
                        }
                        assetbarcodeListview.ItemsSource = test;
                    }
                    else
                    {
                        assetbarcodeListview.ItemsSource = defaultList;
                        DisplayAlert("Error", "No assets with that " + _Names.SerialNo + " were found", "OK");
                    }
                }
                else
                {
                    DisplayAlert("Error", "The UID barcode could not be parsed, please check it and try again", "OK");
                }
            }
        }
        
        private string cutDown(string IUIDval)
        {
            if (IUIDval.IndexOf("18S") == 0 || IUIDval.IndexOf("22S") == 0 || IUIDval.IndexOf("25S") == 0 || IUIDval.IndexOf("12V") == 0 || IUIDval.IndexOf("17V") == 0)
            {
                DataIden = IUIDval.Substring(0, 3);
                return (IUIDval.Substring(3));
            }
            else if (IUIDval.IndexOf("7L") == 0)
            {
                DataIden = IUIDval.Substring(0, 2);
                return (IUIDval.Substring(2));
            }
            else if (IUIDval.IndexOf("LH") == 0)
            {
                IAC_len = 2;
                IAC_val = IUIDval.Substring(0, 2);
                EIDCODE = "EHIBCC";
                return (IUIDval.Substring(2));
            }
            else if (IUIDval.IndexOf("LD") == 0)
            {
                IAC_val = IUIDval.Substring(0, 2);
                IAC_len = 6;
                EIDCODE = "DODACC";
                return (IUIDval.Substring(2));
            }
            else if (IUIDval.IndexOf("UN") == 0)
            {
                IAC_val = IUIDval.Substring(0, 2);
                IAC_len = 9;
                EIDCODE = "DUNS";
                return (IUIDval.Substring(2));
            }
            else if (IUIDval.IndexOf("LB") == 0)
            {
                IAC_val = IUIDval.Substring(0, 2);
                IAC_len = 3;
                EIDCODE = "ANSI";
                return (IUIDval.Substring(1));
            }
            else if (IUIDval.IndexOf("D") == 0)
            {
                IAC_val = IUIDval.Substring(0, 1);
                IAC_len = 5;
                EIDCODE = "CAGE";
                return (IUIDval.Substring(1));
            }
            else if (IUIDval.IndexOf("0") == 0 || IUIDval.IndexOf("1") == 0 || IUIDval.IndexOf("2") == 0 || IUIDval.IndexOf("3") == 0 || IUIDval.IndexOf("4") == 0 || IUIDval.IndexOf("5") == 0 || IUIDval.IndexOf("6") == 0 || IUIDval.IndexOf("7") == 0 || IUIDval.IndexOf("8") == 0 || IUIDval.IndexOf("9") == 0)
            {
                IAC_val = IUIDval.Substring(0, 1);
                IAC_len = 12;
                EIDCODE = "EAN.UCC" + IUIDval[0];
                return (IUIDval.Substring(1));
            }
            else
            {
                return (IUIDval);
            }
        }

        private bool IsIUID()
        {
            if (searchbar.Text.Trim().IndexOf("[)>") == 0)
                return true;
            else
                return false;
        }

        private void loadvalues()
        {
            try
            {
                AssetDetailNames Names = JsonConvert.DeserializeObject<AssetDetailNames>(Application.Current.Properties["DesignInformation"].ToString());
                _Names = Names;
                List<AssetDisplayClass> defaultList_ = new List<AssetDisplayClass>
                {
                AssetDisplayClass.FullDetails(
                new AssetDisplayClass
                {
                    AssetName = "Asset Name",
                    Barcode = "Example barcode",
                    LocationName = "Example Location",
                    SiteName ="Example Site",
                    Quantity = 0,
                    Asset_SublocationIdInternal = "Example sublocation",
                    AssetIdInternal = "00",
                    AssetSerialNo ="oo",
                    Cost =4,
                    AssetBarcode_ = "Barcode",
                    AssetCost_ = "Cost",
                    AssetLocationName_ = "Location",
                    AssetQuantity_ = "Qty",
                    AssetSerialNo_ = "Serial",
                    AssetSubLocationName_ = "SubLocation",
                    AssetSiteName_ = "Site"
                },_Names)
            };
                defaultList = defaultList_;
            }
            catch (Exception exc)
            {
                DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                DisplayAlert("Error", exc.Message, "OK");
            }
        }

        private void assetbarcodeListview_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            assetbarcodeListview.SelectedItem = null;
        }

        private async void assetbarcodeListview_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (!_PropertyPass)
            {
                try
                {
                    var asset = e.Item as AssetDisplayClass;
                    var asset_ = Database.Assets.GetAssetDataByAssetIDInternal(_connection, asset.AssetIdInternal);
                    if (_Person != null)
                        await Navigation.PushModalAsync(new AuditRecordPage(_Names, asset_.First(), asset_.First().Asset_SiteIdInternal, asset_.First().Asset_locationIdInternal, asset_.First().Asset_SublocationIdInternal, asset_.First().Asset_Department, _connection, _Person));
                    else
                        await Navigation.PushAsync(new ScanBarcodeUpdatePage(_Names, asset, _connection));
                }
                catch (Exception exc)
                {
                    await DisplayAlert("Error", exc.Message, "OK");
                }
                
            }
        }

        private async void CheckOutButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new SignaturePage(_PropertyPassTableID, _PropertyPassDisplayList, _connection));
            }
            catch(Exception exc)
            {
                await DisplayAlert("OK", exc.Message, "OK");
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
                        Navigation.PopModalAsync();
                        _result = result.Text;
                        searchbar.Text = _result;
                    });
                };

                // Navigate to our scanner page
                await Navigation.PushModalAsync(scanPage);
            }
            catch (Exception exc)
            {
                await DisplayAlert("Error", exc.Message, "OK");
            }
        }
    }
}