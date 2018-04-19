using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eQuipMobile
{
    public class LocationClass
    {
        public string LocationIdInternal { get; set; }
        public string LocationName { get; set; }
        public string LocationDescription { get; set; }
        public string LocationBarcode { get; set; }
        public string SiteIdInternal { get; set; }
        public int Count { get; set; }
        public string LocationName_ { get; set; }
        public int SubCount { get; set; }
        public string SublocationName_ { get; set; }
        public string LocationDisplayName_
        {
            get
            {
                return string.Format("{0} Name: {1} ", LocationName_, LocationName);
            }
        }
        public string LocationDisplayBarcode_
        {
            get
            {
                return string.Format("{0} barcode: {1} ", LocationName_, LocationBarcode);
            }
        }
        public string LocationDisplayDescription_
        {
            get
            {
                return string.Format("{0} description: {1} ", LocationName_, LocationDescription);
            }
        }
        public string SubLocationCount_
        {
            get
            {
                return string.Format("No. of {0}(s): {1} ", SublocationName_, Count);
            }
        }

        public static List<LocationClass> DbToLocation(IEnumerable<LocationsDbTable> LocationDb, SQLiteConnection _connection, AssetDetailNames Names = null)
        {
            var LocationClassList = new List<LocationClass>();
            foreach (LocationsDbTable LocationDbrecord in LocationDb)
            {
                var _intLoc = new LocationClass
                {
                    LocationIdInternal = LocationDbrecord.LocationIdInternal,
                    LocationBarcode = LocationDbrecord.LocationBarcode,
                    LocationDescription = LocationDbrecord.LocationDescription,
                    LocationName = LocationDbrecord.LocationName,
                    SiteIdInternal = LocationDbrecord.SiteIdInternal,
                    SubCount = Database.SubLocations.GetTableDataFromLocations(_connection, LocationDbrecord.LocationIdInternal).Count(),
                    Count = Database.Assets.GetTableDataFromSiteLocation(_connection, LocationDbrecord.SiteIdInternal, LocationDbrecord.LocationIdInternal).Count()
                };
                if (Names != null)
                {
                    _intLoc.LocationName_ = Names.Location;
                    _intLoc.SublocationName_ = Names.SubLocation;
                }
                LocationClassList.Add(_intLoc);
            }
            return LocationClassList;
        }
    }
}
