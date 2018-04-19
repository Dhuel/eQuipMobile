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
    public partial class AuditList : ContentPage
    {
        public IEnumerable<AssetDbClass> _AssetList;
        public bool _Auditing;
        string _Site, _Location, _Sublocation;
        int _Department;
        string _PersonID;
        SQLiteConnection _connection;
        AssetDetailNames _Names;
        public AuditList(AssetDetailNames Names_, IEnumerable<AssetDbClass> AssetList, SQLiteConnection connection, bool Auditing, string Site, string Location, string Sublocation, int Department, string Person = null, string PersonName = null)
        {
            try
            {
                MessagingCenter.Subscribe<AuditRecordPage, string>(this, "Pop", (sender, arg) =>
                {
                    Navigation.PopAsync();
                });
                _Names = Names_;
                InitializeComponent();
                _AssetList = AssetList;
                _Auditing = Auditing;
                _Site = Site;
                _Location = Location;
                _Sublocation = Sublocation;
                _Department = Department;
                _connection = connection;
                if (Database.Settings.GetTableData(_connection).BlindAudit)
                {
                    AuditListView2.IsVisible = true;
                    AuditListView.IsVisible = false;
                    AuditListView2.ItemsSource = AssetClass.DbToAssetClass(_AssetList, _Names, _connection);
                }
                else
                {
                    AuditListView.IsVisible = true;
                    AuditListView2.IsVisible = false;
                    AuditListView.ItemsSource = AssetClass.DbToAssetClass(_AssetList, _Names, _connection);
                }
                if (Person != null)
                {
                    PersonButton.IsVisible = true;
                    _PersonID = Person;
                    PersonName_.Text = PersonName + "'s Assets";
                }
            }
            catch (Exception exe)
            {
                DependencyService.Get<IError>().SendRaygunError(exe, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                throw (new Exception(exe.Message));
            }
        }

        private async void PersonButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ScanBarcodePage(_connection, _PersonID));
        }

        private async void AuditListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (_Auditing)
            {
                //Get asset class
                var asset_class = e.Item as AssetClass;
                //convert class to DB
                var asset = AssetClass.AssetClassToDb(asset_class, asset_class.Synced);
                if (_PersonID != null)
                {
                    await Navigation.PushModalAsync(new AuditRecordPage(_Names, asset, _Site, _Location, _Sublocation, _Department, _connection, _PersonID));
                }
                else
                {
                    var SettingsData = Database.Settings.GetTableData(_connection);
                    if (SettingsData.FastAudit && !SettingsData.FastAuditEntry)
                    {
                        try
                        {
                            var _assetclass = AssetClass.DbToAssetClass(new List<AssetDbClass> { asset }, _Names).First();
                            AuditClass.Audit(_Site, _Location, _Sublocation, _Department, _assetclass, _connection);
                            Application.Current.Properties["AuditedSite"] = _Site;
                            Application.Current.Properties["AuditedLocation"] = _Location;
                            Application.Current.Properties["AuditedSublocation"] = _Sublocation;
                            Application.Current.Properties["LastAssetID"] = _assetclass.AssetIdInternal;
                            Application.Current.Properties["Department"] = _Department;
                            await Application.Current.SavePropertiesAsync();
                            await DisplayAlert("Complete", "Asset Audited", "OK");
                            await Navigation.PopAsync();
                        }
                        catch (Exception exc)
                        {
                            DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", asset_class);
                            await DisplayAlert("Error", exc.Message, "OK");
                        }
                    }
                    else
                        await Navigation.PushModalAsync(new AuditRecordPage(_Names, asset, _Site, _Location, _Sublocation, _Department, _connection));
                } 
            }
        }

        private void AuditListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            AuditListView.SelectedItem = null;
            AuditListView2.SelectedItem = null;

        }
    }
}