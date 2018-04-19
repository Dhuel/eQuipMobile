using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace eQuipMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PropertyPassItemsList : ContentPage
    {
        
        SQLiteConnection _connection;
        private ObservableCollection<PropertyPassClass.PropertyPassDisplay> _allPropItemsObs;
        AssetDetailNames _Names = Newtonsoft.Json.JsonConvert.DeserializeObject<AssetDetailNames>(Application.Current.Properties["DesignInformation"].ToString());
        public PropertyPassItemsList(SQLiteConnection connection, PropertyPassDbTable PropertyPassTableID)
        {
            _connection = connection;
            InitializeComponent();
            var propertyPassItems = Database.PropertyPassItem.GetTableDetailsByPropertyPassTable(_connection, PropertyPassTableID.PropertyPassID_Internal);
            _allPropItemsObs  = new ObservableCollection<PropertyPassClass.PropertyPassDisplay>(PropertyPassClass.PropertyPassDisplay.GetData(propertyPassItems, _Names));
            //convert the propertyPass records to Asset displays with checkboxes GetTableDetailsByPropertyPassTable
            PropertyPassItemList.ItemsSource = propertyPassItems;
        }

        private void PropertyPassItemList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            PropertyPassItemList.SelectedItem = null;
        }

        private void PropertyPassItemList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                var PropertyPassItem_ = e.Item as PropertyPassClass.PropertyPassDisplay;
                var selected = _allPropItemsObs.Where(c => c.AssetIdInternal.Contains(PropertyPassItem_.AssetIdInternal)).First();
                if (selected.toggled_ == true)
                    selected.toggled_ = false;
                else
                    selected.toggled_ = true;
                _allPropItemsObs = new ObservableCollection<PropertyPassClass.PropertyPassDisplay>(PropertyPassClass.PropertyPassDisplay.GetData(_allPropItemsObs, _Names));
                PropertyPassItemList.ItemsSource = _allPropItemsObs;
            }
            catch (Exception exc)
            {
                DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                DisplayAlert("Error",  exc.Message, "OK");
            }
            
        }

        private async void CheckIn(object sender, EventArgs e)
        {
            try
            {
                var selectedPropertyPassItems = _allPropItemsObs.Where(c => c.toggled_ == true).ToList();
                var Date_ = DateTime.Now.ToString("s");
                foreach (PropertyPassClass.PropertyPassDisplay DisplayClass in selectedPropertyPassItems)
                {
                    Database.PropertyPassItem.Update(_connection, DisplayClass, Date_);
                }
                //if all property pass items have a termination date, add a termination date to the property pass table
                if (Database.PropertyPassItem.IsAllCheckedin(_connection, selectedPropertyPassItems.First().PropertyPassID_Internal))
                {
                    Database.PropertyPassTable.UpdateTermination(_connection, selectedPropertyPassItems.First().PropertyPassID_Internal, Date_);
                    await DisplayAlert("Complete", "All Assets have been checked out.", "OK");
                }  
                else
                {
                   
                    await DisplayAlert("Complete", "Assets have been checked out.", "OK");
                }
                await Navigation.PopToRootAsync();
            }
            catch(Exception exc)
            {
                DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                await DisplayAlert("Error", exc.Message, "OK");
            }
        }
    }
}