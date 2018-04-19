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
    public partial class ScanBarcodeUpdatePage : ContentPage
    {
        private SQLiteConnection _connection;
        AssetJsonObject _Asset;
        bool _synced;
        AssetDetailNames _Names;
        public ScanBarcodeUpdatePage(AssetDetailNames Names_, AssetDisplayClass details, SQLiteConnection connection_)
        {
            InitializeComponent();
            BindingContext = details;
            _connection = connection_;
            var inermediate = Database.Assets.GetAssetDataByAssetIDInternal(_connection, details.AssetIdInternal);
            _synced = inermediate.First().Synced;
            _Asset = AssetClass.DbToAssetClass(inermediate).First().AssetJSON;
            details.Quantity = _Asset.Quantity;
            _Names = Names_;
            scanbarcodeupdateasset.ItemsSource = new List<AssetDisplayClass>() { AssetDisplayClass.GetDetails(details) };
            //if (Application.Current.MainPage.Height <= 568)
            //{
            //    Scan_scroll.HeightRequest = 550;
            //}
        }

        private void Scanbarcodeupdateasset_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            scanbarcodeupdateasset.SelectedItem = null;
        }
        protected override void OnAppearing()
        {
            var pricelock = Database.Settings.GetTableData(_connection);
            if (pricelock.PriceLock )
            {
                if (!_synced)
                {
                    price_.IsEnabled = true;
                    price_.BackgroundColor = Color.White;
                }
                else
                {
                    price_.IsEnabled = false;
                    price_.BackgroundColor = Color.LightGray;
                }

            }
            else
            {
                price_.IsEnabled = true;
                price_.BackgroundColor = Color.White;
                
            }
                
            base.OnAppearing();
        }

        private void Scanbarcodeupdateasset_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var Asset_list = e.Item as AssetDisplayClass;
            var AssetRecord = AssetClass.DbToAssetClass(Database.Assets.GetAssetDataByAssetIDInternal(_connection, Asset_list.AssetIdInternal)).First();
            Navigation.PushAsync(new AddAssetPage(_connection, AssetRecord));
        }

        private void Cancel(object sender, EventArgs e)
        {
            Navigation.PopToRootAsync();
        }

        private async void Update(object sender, EventArgs e)
        {
            try
            {
                bool quantityUsed = true;
                bool costUsed = true;
                bool updated = false;
                if (quantity_.Text == null)
                {
                    quantityUsed = false;
                }
                if (price_.Text == null)
                {
                    costUsed = false;
                }

                if (quantityUsed && int.TryParse(quantity_.Text, out var n))
                {
                    //quantity is being used and is a valid number
                    _Asset.Quantity = _Asset.Quantity + n;
                    updated = true;
                    //add n to quantity
                }
                if (costUsed && double.TryParse(price_.Text, out var o))
                {
                    _Asset.Price = o;
                    updated = true;
                }

                if (updated)
                {
                    updated = false;
                    var assetDetailsold = Database.Assets.GetAssetDataByAssetIDInternal(_connection, _Asset.AssetIDInternal);
                    var assetRecord = Database.Assets.Compare(assetDetailsold.First(), _Asset);
                    if (assetRecord.ChangesMade.Count < 1)
                    {
                        await DisplayAlert("Error", "Nothing was changed in this asset.", "OK");
                    }
                    else
                    {
                        var time = DateTime.Now;
                        assetRecord.AssetJSON.DateModified = time;
                        //Convert to be stored
                        var AssetForDb = AssetClass.AssetClassToDb(assetRecord, assetDetailsold.First().Synced);
                        //store to the database
                        Database.Assets.Update(_connection, AssetForDb);
                        // Update Record on List
                        var display = Database.Assets.GetDisplayeDataByAssetIDInternal(_connection, AssetForDb.AssetIdInternal);
                        display.First().Quantity = _Asset.Quantity;
                        display.First().Cost = _Asset.Price;
                        var __ = new List<AssetDisplayClass>() { AssetDisplayClass.FullDetails(display.First(), _Names) };
                        scanbarcodeupdateasset.ItemsSource = __ ;
                        await DisplayAlert("Complete", "Asset Updated", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Error", "There is no data to update.", "OK");
                }
            }
            catch(Exception exe)
            {
                DependencyService.Get<IError>().SendRaygunError(exe, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                await DisplayAlert("Error", exe.Message, "OK");
            }
        }
    }
}