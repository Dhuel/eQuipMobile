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
    public partial class TransferredAssets : ContentPage
    {
        List<TransferClassData> _transferred_records;
        private SQLiteConnection _connection;
        public TransferredAssets(SQLiteConnection _connection_, List<TransferClassData> transferred_records)
        {
            _connection = _connection_;
            _transferred_records = transferred_records;
            InitializeComponent();
            
            var grid = new Grid();
            var sampleSL = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Spacing = 0, // you can give more spacing for more space for borders
                BackgroundColor = Color.White
            };
            _scrollview.Content = renderTransferData(transferred_records, sampleSL);
        }

        public StackLayout renderTransferData(List<TransferClassData> TransferDataList_, StackLayout SL)
        {
            //get the current asset info from database
            var test = new List<Grid>();
            foreach (TransferClassData transferData_ in TransferDataList_)
            {
                var grid = new Grid();
                grid.BackgroundColor = Color.Black;

                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnSpacing = 1;
                grid.RowSpacing = 1;
                grid.Margin = new Thickness(0, 30, 0, 0);

                var AssetName = new Label { Text = transferData_.AssetName, BackgroundColor = Color.White, TextColor = Color.FromHex("#A0384C"),
                    Margin = new Thickness(1, 1, 1, 1), FontSize = 22 };
                var From = new Label { Text = "From", BackgroundColor = Color.White, Margin = new Thickness(1) };
                var To = new Label { Text = "To", BackgroundColor = Color.White, Margin = new Thickness(1) };
                var Site = new Label { Text = "Site", BackgroundColor = Color.White, Margin = new Thickness(1) };
                var Location = new Label { Text = "Location", BackgroundColor = Color.White, Margin = new Thickness(1) };
                var Sublocation = new Label { Text = "Sublocation", BackgroundColor = Color.White, Margin = new Thickness(1) };
                var Quantity = new Label { Text = "Quantity", BackgroundColor = Color.White, Margin = new Thickness(1) };
                var Site_from = new Label { Text = transferData_.SiteNameFrom, BackgroundColor = Color.White, Margin = new Thickness(1) };
                var Site_To = new Label { Text = transferData_.SiteNameTo, BackgroundColor = Color.White, Margin = new Thickness(1) };
                var Location_from = new Label { Text = transferData_.LocationNameFrom, BackgroundColor = Color.White, Margin = new Thickness(1) };
                var Location_to= new Label { Text = transferData_.LocationNameTo, BackgroundColor = Color.White, Margin = new Thickness(1) };
                var Sublocation_from = new Label { Text = transferData_.FromSublocation, BackgroundColor = Color.White, Margin = new Thickness(1) };
                var Sublocation_to = new Label { Text = transferData_.ToSublocation, BackgroundColor = Color.White, Margin = new Thickness(1) };
                var Quantity_from = new Label { Text = transferData_.Quantityprev.ToString(), BackgroundColor = Color.White, Margin = new Thickness(1) };
                var Quantity_to = new Label { Text = transferData_.Quantitymoved.ToString(), BackgroundColor = Color.White, Margin = new Thickness(1) };


                grid.Children.Add(AssetName, 0, 0);
                grid.Children.Add(Site, 0, 1);
                grid.Children.Add(Location, 0, 2);
                grid.Children.Add(Sublocation, 0, 3);
                grid.Children.Add(Quantity, 0, 4);

                grid.Children.Add(From, 1, 0);
                grid.Children.Add(Site_from, 1, 1);
                grid.Children.Add(Location_from, 1, 2);
                grid.Children.Add(Sublocation_from, 1, 3);
                grid.Children.Add(Quantity_from, 1, 4);

                grid.Children.Add(To, 2, 0);
                grid.Children.Add(Site_To, 2, 1);
                grid.Children.Add(Location_to, 2, 2);
                grid.Children.Add(Sublocation_to, 2, 3);
                grid.Children.Add(Quantity_to, 2, 4);

                SL.Children.Add(grid);
            }
            return SL;
        }

        private async void Complete_Transfer(object sender, EventArgs e)
        {
            var completeList = new List<TransferDbTable>();
            foreach (TransferClassData transferdata in _transferred_records)
            {
                completeList.Add(new TransferDbTable
                {
                    AssetIdInternal = transferdata.AssetIdInternal,
                    DateMoved = transferdata.DateMoved.ToString(),
                    Quantitymoved = transferdata.Quantitymoved,
                    ToLocation = transferdata.ToLocation,
                    ToSite = transferdata.ToSite,
                    ToSublocation = transferdata.ToSublocation,
                    TransactionName = "TRANSFERS"
                });
            }
            Database.Transfers.InsertAll(_connection, completeList);
            await DisplayAlert("Complete", "Transfers Completed", "OK");
            await Navigation.PopToRootAsync();
        }
    }
}