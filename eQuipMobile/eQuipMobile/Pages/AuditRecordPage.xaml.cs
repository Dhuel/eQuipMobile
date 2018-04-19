using Newtonsoft.Json;
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
    //TODO - dont start db connection each time, just pass it
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AuditRecordPage : ContentPage
    {
        string _SiteID, _LocationID, _Sublocation, _Person;
        int _Department;
        SQLiteConnection _connection;
        IEnumerable<AssetDbClass> IEnumAsset_;
        int _Actual;
        AssetDetailNames _Names;
        public AuditRecordPage(AssetDetailNames Names_, AssetDbClass Asset, string Site, string Location, string Sublocation, int Department, SQLiteConnection connection, string Person = null)
        {
            _Names = Names_;
            InitializeComponent();
            LoadNames();
            if (Site == null)
            {
                _SiteID = Asset.Asset_SiteIdInternal;
                _LocationID = Asset.Asset_locationIdInternal;
                _Sublocation = Asset.Asset_SublocationIdInternal;
                _Department = Asset.Asset_Department;
            }
            else
            {
                _SiteID = Site;
                _LocationID = Location;
                _Sublocation = Sublocation;
                _Department = Department;
            }
            if (Person != null)
            {
                _Person = Person;
                AuditButton_.Text = "Audit To Person";
            }
            _connection = connection;
            QuantityDisplay.IsVisible =  !Database.Settings.GetTableData(_connection).BlindAudit;
            IEnumAsset_ = new AssetDbClass[] { Asset };
            var AuditAsset_ = JsonConvert.DeserializeObject<AssetJsonObject>(Asset.AssetJSONDb);
            if (AuditAsset_.AuditDate == Convert.ToDateTime("01/01/1900 00:00:00"))
                auditDate_.Text = "";
            else
                auditDate_.Text = AuditAsset_.AuditDate.ToString("s");
            BindingContext = AuditAsset_;
        }

        private async void AuditButton(object sender, EventArgs e)
        {
            if (ActualQuantity_.Text == null)
                _Actual = -1;
            else
                _Actual = Convert.ToInt16(ActualQuantity_.Text);
            var _assetclass = AssetClass.DbToAssetClass(IEnumAsset_).First();
            AuditClass.Audit(_SiteID, _LocationID, _Sublocation, _Department, _assetclass, _connection, _Actual, _Person);
            Application.Current.Properties["AuditedSite"] = _SiteID;
            Application.Current.Properties["AuditedLocation"] = _LocationID;
            Application.Current.Properties["AuditedSublocation"] = _Sublocation;
            Application.Current.Properties["LastAssetID"] = _assetclass.AssetIdInternal;
            Application.Current.Properties["Department"] = _Department;
            await Application.Current.SavePropertiesAsync();
            await DisplayAlert("Complete", "Asset Audited", "OK");
            try
            {
                await Navigation.PopModalAsync();
                MessagingCenter.Send(this, "Pop", "");
            }
            catch (Exception exc)
            {
                DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
            }  
        }

        private void BackButton(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
        private void LoadNames()
        {
            AssetName_.Text = _Names.Name;
            AssetBarcode_.Text = _Names.Barcode;
            AuditStatus_.Text = _Names.AuditStatus;
            AssetSerial_.Text = _Names.SerialNo;
            AssetQty_.Text = _Names.Quantity;
            ActualAssetQty_.Text = "Actual " + _Names.Quantity;
        }
    }
}