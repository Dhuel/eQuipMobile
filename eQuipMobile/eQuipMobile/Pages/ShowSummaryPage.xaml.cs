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
    public partial class ShowSummaryPage : ContentPage
    {
        AssetDetailNames _Names;
        private SQLiteConnection _connection;
        public ShowSummaryPage(SQLiteConnection connection_, AssetDetailNames Names_, string Site_, string Location, string SubLocation, int Dep_ = 0)
        {
            _connection = connection_;
            _Names = Names_;
            InitializeComponent();
            var result = Database.Assets.GetAssetListForSearch(_connection, Site_, Location, SubLocation, Dep_);
            int Audited=0, Moved=0, Cust=0, Added=0, quant=0, dpt=0, emp = 0;
            foreach (AssetDbClass Assetrecord in result)
            {
                var Result = JsonConvert.DeserializeObject<AssetJsonObject>(Assetrecord.AssetJSONDb);
                switch (Result.AuditStatus)
                {
                    case "AUDITED":
                        Audited = Audited + 1;
                        break;
                    case "RECON-Moved":
                        Moved = Moved + 1;
                        break;
                    case "RECON-Added":
                        Added = Added + 1;
                        break;
                    case "RECON-Custodian":
                        Cust = Cust + 1;
                        break;
                    case "RECON-Quantity":
                        quant = quant + 1;
                        break;
                    case "RECON-Department":
                        dpt = dpt + 1;
                        break;
                    default:
                        emp = emp + 1;
                        break;
                }
                
            }
            var Workflow = new WorkflowClass
            {
                AuditedAssets = Audited.ToString(),
                NotAudited = emp.ToString(),
                ReconAdded = Added.ToString(),
                TotalAssets = result.Count().ToString(),
                ReconCustodian = Cust.ToString(),
                ReconDepartment = dpt.ToString(),
                ReconMoved = Moved.ToString(),
                ReconQuantity = quant.ToString()
            };
            WorkflowSummary.ItemsSource = new List<WorkflowClass>
            {
                Workflow
            };
        }

        private void WorkflowSummary_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            WorkflowSummary.SelectedItem = null;

        }
        private async void GoBack(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }

}