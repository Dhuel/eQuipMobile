using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eQuipMobile
{
    public class VendorClass
    {
        public string VendorIDInternal { get; set; }
        public string Company { get; set; }
        public static List<VendorClass> DbToVendor(IEnumerable<VendorDbTable> VendorDb, SQLiteConnection _connection)
        {
            var VendorsList = new List<VendorClass>();
            foreach (VendorDbTable VendorDbRecord in VendorDb)
            {

                VendorsList.Add(new VendorClass
                {
                    VendorIDInternal = VendorDbRecord.VendorIDInternal,
                    Company = VendorDbRecord.Company
                });
            }
            return VendorsList;
        }
    }

   public class ManufacturerClass
    {
        public string VendorIDInternal { get; set; }
        public string Company { get; set; }
        public static List<ManufacturerClass> DbToManufacturer(IEnumerable<ManufacturerDbTable> MfgDb, SQLiteConnection _connection)
        {
            var MfgList = new List<ManufacturerClass>();
            foreach (ManufacturerDbTable MfgDbRecord in MfgDb)
            {
                MfgList.Add(new ManufacturerClass
                {
                    VendorIDInternal = MfgDbRecord.VendorIDInternal,
                    Company = MfgDbRecord.Company
                });
            }
            return MfgList;
        }
    }
}
