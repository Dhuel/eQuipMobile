using System;
using SQLite;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace eQuipMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddPropertyPassPage : ContentPage
    {
        private SQLiteConnection _connection;

        public AddPropertyPassPage(SQLiteConnection connection)
        {
            _connection = connection;
            InitializeComponent();
            if (Device.RuntimePlatform == Device.iOS)
                Padding = new Thickness(0, 20, 20, 0);
            LoadSites();
            LoadPerson();
            PropertyPassDueDate.Date = DateTime.Now;
        }

        private void LoadSites()
        {
            var sitesList = SitesClass.DbToSite(Database.Sites.GetTableData(_connection), _connection);
            foreach (SitesClass siteClass_ in sitesList)
            {
                PropertyPassSite.Items.Add(siteClass_.SiteName);
            }
        }

        private void LoadPerson()
        {
            var PersonList = PeopleClass.DbToPeople(Database.People.GetTableData(_connection), _connection);
            foreach (PeopleClass person in PersonList)
            {
                PropertyPassPerson.Items.Add(person.PeopleName);
            }
        }

        private async void AddPropertyPass(object sender, EventArgs e)
        {
            string siteName, personName;
            SitesClass siteInfo = new SitesClass();
            PeopleClass personInfo = new PeopleClass();
            var field = CheckValues();
            if (field == "")
            {
                //gets site
                if (PropertyPassSite.SelectedIndex >= 0)
                {
                    siteName = PropertyPassSite.Items[PropertyPassSite.SelectedIndex];
                    siteInfo = SitesClass.DbToSite(Database.Sites.GetTableData(_connection), _connection).Single(cm => cm.SiteName == siteName);
                }

                //gets people
                if (PropertyPassPerson.SelectedIndex >= 0)
                {
                    personName = PropertyPassPerson.Items[PropertyPassPerson.SelectedIndex];
                    personInfo = PeopleClass.DbToPeople(Database.People.GetTableData(_connection), _connection).Single(cm => cm.PeopleName == personName);
                }
                var PPdata = new PropertyPassDbTable()
                {
                    AssignedToEmail = PropertyPassEmail.Text,
                    CreationDate = DateTime.Now.ToString("s"),
                    AssignedToPhone = PropertyPassPhone.Text,
                    ModifiedOn = DateTime.Now.ToString("s"),
                    Comments = PropertyPassComments.Text,
                    DueDate = PropertyPassDueDate.Date.ToString("s"),
                    PeopleID_Internal = personInfo.PeopleIDInternal,
                    SiteIDInternal = siteInfo.SiteIdInternal,
                    PropertyPassID_Internal = GUID.Generate(),
                    Assigned_By = Application.Current.Properties["user"].ToString(),
                    Synced = true
                };

                Database.PropertyPassTable.Insert(_connection, PPdata);
                await DisplayAlert("Complete", "The property pass table has been added.", "OK");
                await Navigation.PopAsync();
            }
            else
                await DisplayAlert("Error", "The " + field + " field is required", "OK");
        }

        private async void Cancel(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
        private string CheckValues()
        {
            if (PropertyPassPerson.SelectedIndex < 0)
                return "Person";
            else if (PropertyPassEmail.Text == null)
                return "Email";
            else if (PropertyPassPhone.Text == null)
                return "Telephone";
            else if (PropertyPassComments.Text == null)
                return "Comments";
            else if (PropertyPassSite.SelectedIndex < 0)
                return "Site";
            else return "";
        }
    }
}