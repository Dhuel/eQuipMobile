using SQLite;
using System;
using System.Collections.ObjectModel;
using System.Linq;


using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace eQuipMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AssetCategoryPage : ContentPage
    {
        private SQLiteConnection _connection;
        AssetClass _Asset;
        private ObservableCollection<CategoryDbTable> _AssetCategoryTable;
        string _categoryIDInternal = null;
        CategoryDbTable _globalCategoryID;


        public AssetCategoryPage(SQLiteConnection connection_, AssetClass asset = null)
        {
            if (asset != null)
            {
                _Asset = asset;
                _categoryIDInternal = _Asset.AssetJSON.AssetCategoryIDInternal;
            }
            _connection = connection_;
            InitializeComponent();
            if (Device.RuntimePlatform == Device.iOS)
                Padding = new Thickness(0, 20, 20, 0);
            //load root categories
            var assetCategories_ = Database.Category.GetCategoryWithParents(_connection, 0);
            CategoryList.ItemsSource = assetCategories_;
            base.OnAppearing();
        }

        private void CategoryList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            CategoryList.SelectedItem = null;
        }

        private void CategoryList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var assetCategory = e.Item as CategoryDbTable;
            var newAssetCategoriesDb = Database.Category.GetCategoryWithParents(_connection, assetCategory.AssetCategoryID);
            _globalCategoryID = assetCategory;
            if (newAssetCategoriesDb.Count() > 0)
            {
                _AssetCategoryTable = new ObservableCollection<CategoryDbTable>(newAssetCategoriesDb);
                CategoryList.ItemsSource = _AssetCategoryTable;
                _categoryIDInternal = assetCategory.AssetCategoryIDInternal;
                _categoryHeader.Text = assetCategory.AssetCategoryName;
                base.OnAppearing();
            }
            else
            {
                //select it
                MessagingCenter.Send(this, "AssetCategoryID", _globalCategoryID.AssetCategoryIDInternal);
                Navigation.PopModalAsync();

            }
        }

        private void Selected(object sender, EventArgs e)
        {
            if (_globalCategoryID != null)
                MessagingCenter.Send(this, "AssetCategoryID", _globalCategoryID.AssetCategoryIDInternal);
            Navigation.PopModalAsync();
        }

        private void Cancel(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }

        private void parent(object sender, EventArgs e)
        {
            try
            {
                var newAssetCategoriesDb = Database.Category.GetCategoryWithParents(_connection, _globalCategoryID.ParentID);
                _AssetCategoryTable = new ObservableCollection<CategoryDbTable>(newAssetCategoriesDb);
                CategoryList.ItemsSource = _AssetCategoryTable;
                var assetCategory = Database.Category.GetMyvalueByCategoryID(_connection, _globalCategoryID.ParentID);
                if (_globalCategoryID.ParentID == 0)
                {
                    _categoryHeader.Text = "Root Categories";
                    _globalCategoryID = null;
                    base.OnAppearing();
                }
                else
                {
                    _categoryHeader.Text = assetCategory.First().AssetCategoryName;
                    _categoryIDInternal = assetCategory.First().AssetCategoryIDInternal;
                    _globalCategoryID = assetCategory.First();
                    base.OnAppearing();
                }
            }
            catch (Exception exc)
            {
                //throw (exc);
                DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                DisplayAlert("Error", "Unable to do that.", "OK");
            }
        }
    }
}