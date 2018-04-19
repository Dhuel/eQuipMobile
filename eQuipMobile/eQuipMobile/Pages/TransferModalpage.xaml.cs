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
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TransferModalpage : ContentPage
    {
        int _globalcount;
        string _Site, _Location, _Sublocation, _People;
        AssetDisplayClass _assetData;
        private SQLiteConnection _connection;
        AssetDetailNames _Names;

        public TransferModalpage(AssetDetailNames Names_, SQLiteConnection _connection_, int count, AssetDisplayClass assetData, string Site, string Location, string Sublocation = null, string Person = null)
        {
            _connection = _connection_;
            _Names = Names_;
            _globalcount = count;
            _Site = Site;
            _Location = Location;
            _Sublocation = Sublocation;
            _assetData = assetData;
            _People = Person;
            BindingContext = _assetData;
            InitializeComponent();
            AssetName_.Text = _Names.Name;
            Barcode.Text = _Names.Barcode;
            SiteName_.Text = _Names.Site;
            LocationName_.Text = _Names.Location;
            SublocationName_.Text = _Names.SubLocation;
            QuantityName_.Text = _Names.Quantity;
            if (_People != null)
            {
                TransferButton.Text = "Assign";
                QtyName_to_assign.Text = _Names.Quantity + " to assign";
            }
            else
                QtyName_to_assign.Text = _Names.Quantity + " to move";
        }

        protected override void OnAppearing()
        {
            if (_assetData.Quantity == 0)
            {
                DisplayAlert("Error", "This asset has a quantity of 0, it cannot be moved", "OK");
                Cancel(null, null);
            }
            if (_assetData.Quantity == 1)
            {
                TransferAmount.Text = "1";
            }
            base.OnAppearing();
        }

        private async void Cancel(object sender, EventArgs e)
        {
            if (_globalcount == 1)
            {
                await Navigation.PopModalAsync();
            }
            else
            {
                int numModals = Application.Current.MainPage.Navigation.ModalStack.Count;
                for (int currModal = 0; currModal < numModals; currModal++)
                {
                    await Application.Current.MainPage.Navigation.PopModalAsync();
                }
            }
        }

        private async void Transfer(object sender, EventArgs e)
        {
            var transferringAmnt = Convert.ToInt16(TransferAmount.Text);
            if (transferringAmnt < 1)
            {
                await DisplayAlert("Warning", "Please enter a valid amount to transfer", "OK");
            }
            else
            {
                var Assetrecords = Database.Assets.GetAssetDataByAssetIDInternal(_connection, _assetData.AssetIdInternal).First();
                var AssetDetails = JsonConvert.DeserializeObject<AssetJsonObject>(Assetrecords.AssetJSONDb);
                if (AssetDetails.Quantity < transferringAmnt)
                    await DisplayAlert("Error", "The transferring quantity is more than the available quantity.", "OK");
                else
                {
                    var transferCheck = Database.Transfers.GetTableDataByAssetID(_connection, Assetrecords.AssetIdInternal);
                    if (transferCheck.Count() > 0)
                    {
                        var totalQuantity = 0;
                        foreach(TransferDbTable transferRecord in transferCheck)
                        {
                            totalQuantity = totalQuantity + transferRecord.Quantitymoved;
                        }
                        if ((totalQuantity + transferringAmnt) > AssetDetails.Quantity)
                        {
                            var available = (AssetDetails.Quantity - totalQuantity).ToString();
                            await DisplayAlert("Error", "That value is more than is available to transfer. You have "+ available+ " available for transfer.", "OK");
                        }
                        else
                        {
                            EnterTransferData(Assetrecords, transferringAmnt);
                        }
                    }
                    else
                    {
                        EnterTransferData(Assetrecords, transferringAmnt);
                    }
                }
            }
        }

        private async void EnterTransferData(AssetDbClass Asset_data, int transferAmnt)
        {
            if (_People != null)
            {
                var Data = new TransferClassData
                {
                    AssetName = Asset_data.AssetName,
                    FromSite = Asset_data.Asset_SiteIdInternal,
                    AssetIdInternal = Asset_data.AssetIdInternal,
                    DateMoved = DateTime.Now,
                    FromLocation = Asset_data.Asset_locationIdInternal,
                    ToLocation = _Location,
                    FromSublocation = Asset_data.Asset_SublocationIdInternal,
                    ToSublocation = _Sublocation,
                    ToSite = _Site,
                    Quantityprev = JsonConvert.DeserializeObject<AssetJsonObject>(Asset_data.AssetJSONDb).Quantity,
                    Quantitymoved = transferAmnt,
                    TransactionName = "TRANSFER",
                    FromPeople = Asset_data.Asset_PeopleIdInternal,
                    ToPeople = _People
                };
                MessagingCenter.Send(this, "TransferAsset", JsonConvert.SerializeObject(Data));
                await DisplayAlert("Completed", "Asset has been added to assign list.", "OK");
            }
            else
            {
                var Data = new TransferClassData
                {
                    AssetName = Asset_data.AssetName,
                    FromSite = Asset_data.Asset_SiteIdInternal,
                    AssetIdInternal = Asset_data.AssetIdInternal,
                    DateMoved = DateTime.Now,
                    FromLocation = Asset_data.Asset_locationIdInternal,
                    ToLocation = _Location,
                    FromSublocation = Asset_data.Asset_SublocationIdInternal,
                    ToSublocation = _Sublocation,
                    ToSite = _Site,
                    Quantityprev = JsonConvert.DeserializeObject<AssetJsonObject>(Asset_data.AssetJSONDb).Quantity,
                    Quantitymoved = transferAmnt,
                    TransactionName = "TRANSFER",
                };
                MessagingCenter.Send(this, "TransferAsset", JsonConvert.SerializeObject(Data));
                await DisplayAlert("Completed", "Asset has been added to transfer list.", "OK");
            }
            Cancel(this, null);
        }
    }
}