using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace eQuipMobile
{
    public class SitesClass
    {
        public string SiteIdInternal { get; set; }
        public string LocationName { get; set; }
        public string SiteName { get; set; }
        public string SiteCode { get; set; }
        public string SiteDescription { get; set; }
        public int Count { get; set; }
        public string SiteName_ { get; set; }
        public string SiteDisplayName_
        {
            get
            {
                return string.Format("{0} Name: {1} ", SiteName_, SiteName);
            }
        }
        public string SiteDisplayCode_
        {
            get
            {
                return string.Format("{0} Code: {1} ", SiteName_, SiteCode);
            }
        }
        public string SiteDisplayDescription_
        {
            get
            {
                return string.Format("{0} Code: {1} ", SiteName_, SiteDescription);
            }
        }
        public string LocationNameDisplay_
        {
            get
            {
                return string.Format("No. of {0}(s): {1} ", LocationName, Count);
            }
        }

        public static List<SitesClass> DbToSite(IEnumerable<SitesDbTable> SitesDb, SQLiteConnection _connection, AssetDetailNames Names = null)
        {
            var SitesList = new List<SitesClass>();
            foreach (SitesDbTable SiteDbRecord in SitesDb)
            {
                var intermediateSite = new SitesClass
                {
                    SiteIdInternal = SiteDbRecord.SiteIdInternal,
                    SiteName = SiteDbRecord.SiteName,
                    SiteCode = SiteDbRecord.SiteCode,
                    SiteDescription = SiteDbRecord.SiteDescription,
                    Count = Database.Locations.GetTableDataFromSites(_connection, SiteDbRecord.SiteIdInternal).Count()
                };
                if (Names != null)
                {
                    intermediateSite.SiteName_ = Names.Site;
                    intermediateSite.LocationName = Names.Location;
                }
                SitesList.Add(intermediateSite);
            }
            return SitesList;
        }
    }
    public class MobileSitesSender
    {
        public string UserID { get; set; }
        public List<SiteIDs> mobileSites { get; set; }

        public MobileSitesSender(List<MobileSitesClass> MobileSitesList, string _UserID)
        {
            mobileSites = new List<SiteIDs>();
            UserID = _UserID;
            foreach (MobileSitesClass mobileSite in MobileSitesList)
            {
                mobileSites.Add(new SiteIDs { SiteID = mobileSite.SiteIdInternal });
            }
        }
    }
    public class SiteIDs
    {
        public string SiteID { get; set; }
    }
    public class MobileSitesClass : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private bool _toggled;
        public string SiteIdInternal { get; set; }
        public string SiteName { get; set; }
        public bool Toggled_
        {
            get
            {
                return _toggled;
            }
            set
            {
                if (_toggled = value)
                    return;

                _toggled = value;
                OnPropertyChanged();
            }
        }
        private void OnPropertyChanged([CallerMemberName]string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

    }
}
