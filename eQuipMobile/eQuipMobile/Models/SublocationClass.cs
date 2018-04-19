using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eQuipMobile
{
    public class SublocationClass
    {
        public string SubLocationName { get; set; }
        public string SubLocationName_ { get; set; }
        public string Location_ID_Internal { get; set; }
        public string SubLocationID_Internal { get; set; }
        //used to show the location name on top of sublocation page
        public string LocationName { get; set; }
        public string SubLocationDisplayName_
        {
            get
            {
                return string.Format("{0} Name: {1} ", SubLocationName_, SubLocationName);
            }
        }
        public static List<SublocationClass> DbToSubLocation(IEnumerable<SubLocationsDbTable> SublocationDb, AssetDetailNames Names = null)
        {
            var SubLocationClassList = new List<SublocationClass>();
            foreach (SubLocationsDbTable SubLocationDbrecord in SublocationDb)
            {
                var _intSubLoc = new SublocationClass
                {
                    SubLocationName = SubLocationDbrecord.SubLocationName ?? "DefaultSublocationName",
                    Location_ID_Internal = SubLocationDbrecord.Location_ID_Internal,
                    SubLocationID_Internal = SubLocationDbrecord.SubLocationID_Internal
                };
                if (Names != null)
                {
                    _intSubLoc.SubLocationName_ = Names.SubLocation;
                }
                SubLocationClassList.Add(_intSubLoc);
            }
            return SubLocationClassList;
        }
    }
}
