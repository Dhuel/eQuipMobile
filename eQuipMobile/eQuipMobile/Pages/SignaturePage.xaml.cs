using SignaturePad.Forms;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace eQuipMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignaturePage : ContentPage
    {
        SQLiteConnection _connection;
        PropertyPassDbTable _PropertyPassTableID;
        List<AssetDisplayClass> _PropertyPassDisplayList;
        public SignaturePage(PropertyPassDbTable PropertyPassTableID_, List <AssetDisplayClass> PropertyPassDisplayList_, SQLiteConnection connection_)
        {
            InitializeComponent();
            _PropertyPassTableID = PropertyPassTableID_;
            _PropertyPassDisplayList = PropertyPassDisplayList_;
            _connection = connection_;
        }

        private async void OnGetImage(object sender, EventArgs e)
        {
            var settings = new ImageConstructionSettings
            {
                Padding = 12,
                StrokeColor = Color.FromRgb(25, 25, 25),
                BackgroundColor = Color.FromRgb(225, 225, 225),
                DesiredSizeOrScale = 2f
            };
            var image = await padView.GetImageStreamAsync(SignatureImageFormat.Png, settings);
            if (image == null)
            {
                await DisplayAlert("Error", "A signature is required to check assets out.", "OK");
                return;
            }

            var base64_ = AppImages.StreamToBase64String(image);

            IEnumerable<PropertyPassItemDbTable> PropertyPass = PropertyPassClass.PropertyPassItem.ConvertListToDb(_PropertyPassTableID, _PropertyPassDisplayList);
            Database.PropertyPassItem.InsertAll(_connection, PropertyPass);
            var sigupdate = Database.PropertyPassTable.UpdateSignature(_connection, base64_, _PropertyPassTableID.PropertyPassID_Internal);
            if (sigupdate == "")
                await DisplayAlert("Complete!", "The asset(s) have been checked out.", "OK");
            else
                await DisplayAlert("Error", sigupdate, "OK");
            await Navigation.PopToRootAsync();
        }

    }

}