using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace eQuipMobile
{
    public class PropertyPassClass
    {
        public class PropertyPassTable
        {
            public string Assigned_By { get; set; }
            public string AssignedToEmail { get; set; }
            public string AssignedToPhone { get; set; }
            public string Comments { get; set; }
            public DateTime CreationDate { get; set; }
            public DateTime DueDate { get; set; }
            public string PeopleID_Internal { get; set; }
            public string PropertyPassID_Internal { get; set; }
            public int PropertyPassLevel { get; set; }
            public string SiteIDInternal { get; set; }
            public DateTime TerminationDate { get; set; }
            public DateTime ModifiedOn { get; set; }
            public string SignaturePic { get; set; }

            public PropertyPassTable(PropertyPassDbTable PropertyPassDb = null)
            {
                if (PropertyPassDb != null)
                {
                    Assigned_By = PropertyPassDb.Assigned_By;
                    AssignedToEmail = PropertyPassDb.AssignedToEmail;
                    AssignedToPhone = PropertyPassDb.AssignedToPhone;
                    Comments = PropertyPassDb.Comments;
                    CreationDate = Convert.ToDateTime(PropertyPassDb.CreationDate);
                    DueDate = Convert.ToDateTime(PropertyPassDb.DueDate);
                    PeopleID_Internal = PropertyPassDb.PeopleID_Internal;
                    PropertyPassID_Internal = PropertyPassDb.PropertyPassID_Internal;
                    PropertyPassLevel = PropertyPassDb.PropertyPassLevel;
                    SiteIDInternal = PropertyPassDb.SiteIDInternal;
                    TerminationDate = Convert.ToDateTime(PropertyPassDb.TerminationDate);
                    ModifiedOn = Convert.ToDateTime(PropertyPassDb.ModifiedOn);
                    SignaturePic = PropertyPassDb.SignaturePic;
                }
            }
        }

        public class PropertyPassTableUpdateFormat
        {
            public string UserID { get; set; }
            public string PropertyPassID_Internal { get; set; }
            public PropertyPassTableSubClass asset { get; set; }

            public void SetUpdateTableAssetValues(PropertyPassTable PropertyPassrecords)
            {
                asset.Assigned_By = PropertyPassrecords.Assigned_By;
                asset.AssignedToEmail = PropertyPassrecords.AssignedToEmail;
                asset.AssignedToPhone = PropertyPassrecords.AssignedToPhone;
                asset.Comments = PropertyPassrecords.Comments;
                asset.CreationDate = PropertyPassrecords.CreationDate.ToString();
                asset.DueDate = PropertyPassrecords.DueDate.ToString();
                asset.PropertyPassLevel = PropertyPassrecords.PropertyPassLevel;
                asset.SiteIDInternal = PropertyPassrecords.SiteIDInternal;
                asset.TerminationDate = PropertyPassrecords.TerminationDate.ToString();
                asset.ModifiedOn = PropertyPassrecords.ModifiedOn.ToString();
                asset.SignaturePic = PropertyPassrecords.SignaturePic;
                asset.PeopleID_Internal = PropertyPassrecords.PeopleID_Internal;
            }
           
            public class PropertyPassTableSubClass
            {
                public string Assigned_By { get; set; }
                public string AssignedToEmail { get; set; }
                public string AssignedToPhone { get; set; }
                public string Comments { get; set; }
                public string CreationDate { get; set; }
                public string DueDate { get; set; }
                public string PeopleID_Internal { get; set; }
                public int PropertyPassLevel { get; set; }
                public string SiteIDInternal { get; set; }
                public string TerminationDate { get; set; }
                public string ModifiedOn { get; set; }
                public string SignaturePic { get; set; }
            }
           
        }
        public class PropertyPassTableCreateFormat
        {
            public string UserID { get; set; }
            public PropertyPassTableSubClassCreate asset { get; set; }

            public void SetCreateTableAssetValues(PropertyPassTable PropertyPassrecords)
            {
                if (asset == null)
                    asset = new PropertyPassTableSubClassCreate();
                asset.Assigned_By = PropertyPassrecords.Assigned_By;
                asset.AssignedToEmail = PropertyPassrecords.AssignedToEmail;
                asset.AssignedToPhone = PropertyPassrecords.AssignedToPhone;
                asset.Comments = PropertyPassrecords.Comments;
                asset.CreationDate = PropertyPassrecords.CreationDate.ToString();
                asset.DueDate = PropertyPassrecords.DueDate.ToString();
                asset.PropertyPassLevel = PropertyPassrecords.PropertyPassLevel;
                asset.SiteIDInternal = PropertyPassrecords.SiteIDInternal;
                asset.TerminationDate = PropertyPassrecords.TerminationDate.ToString();
                asset.ModifiedOn = PropertyPassrecords.ModifiedOn.ToString();
                asset.PropertyPassIDInternal = PropertyPassrecords.PropertyPassID_Internal;
                asset.SignaturePic = PropertyPassrecords.SignaturePic;
                asset.PeopleID_Internal = PropertyPassrecords.PeopleID_Internal;
            }
            public class PropertyPassTableSubClassCreate
            {
                public string Assigned_By { get; set; }
                public string AssignedToEmail { get; set; }
                public string AssignedToPhone { get; set; }
                public string Comments { get; set; }
                public string CreationDate { get; set; }
                public string DueDate { get; set; }
                public string PeopleID_Internal { get; set; }
                public int PropertyPassLevel { get; set; }
                public string SiteIDInternal { get; set; }
                public string TerminationDate { get; set; }
                public string ModifiedOn { get; set; }
                public string PropertyPassIDInternal { get; set; }
                public string SignaturePic { get; set; }
            }
        }

        public class PropertyPassDisplay : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            private bool _toggled;
            public string AssetName { get; set; }
            public string CheckInDate { get; set; }
            public string Barcode { get; set; }
            public string Display_Barcode { get; set; }
            public int Quantity { get; set; }
            public string Display_Quantity { get; set; }
            public string Comments { get; set; }
            public string SiteName { get; set; }
            public string AssignedToEmail { get; set; }
            public string AssignedToPhone { get; set; }
            public string PeopleID_Internal { get; set; }
            public string LocationName { get; set; }
            public string Asset_SublocationIdInternal { get; set; }
            public string AssetIdInternal { get; set; }
            public string AssetJSONDb { get; set; }
            public string PropertyPassItemID_Internal { get; set; }
            public string PropertyPassID_Internal { get; set; }
            public string Assigned_By { get; set; }
            public bool toggled_
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
            public string PropertyPassDisplayBarcode_
            {
                get
                {
                    return string.Format("{0}: {1} ", Display_Barcode, Barcode);
                }
            }
            public string PropertyPassDisplayQuantity_
            {
                get
                {
                    return string.Format("{0}: {1} ", Display_Quantity, Quantity);
                }
            }
            private void OnPropertyChanged([CallerMemberName]string PropertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
            }

            public static IEnumerable<PropertyPassDisplay> GetData(IEnumerable<PropertyPassDisplay> display, AssetDetailNames Names_)
            {
                var returnval = display;
                foreach (PropertyPassDisplay PPDisplay in display)
                {
                    PPDisplay.Display_Barcode = Names_.Barcode;
                    PPDisplay.Display_Quantity = Names_.Quantity;
                }
                return returnval;
            }

        }

        public class PropertyPassItem
        {
            public string Asset_ID_Internal { get; set; }
            public string AssetSerialNo { get; set; }
            public string Assigned_By { get; set; }
            public DateTime CheckInDate { get; set; }
            public DateTime CheckOutDate { get; set; }
            public DateTime DueDate { get; set; }
            public string PropertyPassID_Internal { get; set; }
            public string PropertyPassItemID_Internal { get; set; }
            public int Quantity { get; set; }
            public string SiteIDInternal { get; set; }
            public DateTime ModifiedOn { get; set; }
            public PropertyPassItem() { }
            public PropertyPassItem(PropertyPassItemDbTable PropertyPassItemDb = null)
            {
                Asset_ID_Internal = PropertyPassItemDb.Asset_ID_Internal;
                AssetSerialNo = PropertyPassItemDb.AssetSerialNo;
                Assigned_By = PropertyPassItemDb.Assigned_By;
                CheckInDate = Convert.ToDateTime(PropertyPassItemDb.CheckInDate);
                CheckOutDate = Convert.ToDateTime(PropertyPassItemDb.CheckOutDate);
                DueDate = Convert.ToDateTime(PropertyPassItemDb.DueDate);
                PropertyPassID_Internal = PropertyPassItemDb.PropertyPassID_Internal;
                PropertyPassItemID_Internal = PropertyPassItemDb.PropertyPassItemID_Internal;
                Quantity = PropertyPassItemDb.Quantity;
                SiteIDInternal = PropertyPassItemDb.SiteIDInternal;
                ModifiedOn = Convert.ToDateTime(PropertyPassItemDb.ModifiedOn);
            }
            public static IEnumerable<PropertyPassItemDbTable> ConvertListToDb(PropertyPassDbTable PropertyPassTable, List<AssetDisplayClass> AssetList)
            {
                var List = new List<PropertyPassItemDbTable>();
                foreach (AssetDisplayClass Asset in AssetList)
                {
                    var JsonObj = JsonConvert.DeserializeObject<AssetJsonObject>(Asset.AssetJSONDb);
                    var _PPI = new PropertyPassItemDbTable
                    {
                        Asset_ID_Internal = Asset.AssetIdInternal,
                        Assigned_By = PropertyPassTable.Assigned_By,
                        CheckOutDate = DateTime.Now.ToString(),
                        DueDate = PropertyPassTable.DueDate,
                        PropertyPassID_Internal = PropertyPassTable.PropertyPassID_Internal,
                        ModifiedOn = DateTime.Now.ToString(),
                        SiteIDInternal = JsonObj.SiteIDInternal,
                        Quantity = Asset.Quantity,
                        PropertyPassItemID_Internal = GUID.Generate(),
                        AssetSerialNo = JsonObj.AssetSerialNo,
                        CheckInDate = null,
                        Synced = true,
                    };
                    List.Add(_PPI);
                }
                return List;
            }
        }
        public class PropertyPassItemUpdateFormat
        {
            public PropertyPassItemUpdateFormat() { }
            public string UserID { get; set; }
            public string PropertyPassItemID_Internal { get; set; }
            public PropertyPassItemSubClassUpdate asset { get; set; }

            public void SetAssetValuesUpdate(PropertyPassItem PropertyPassrecords)
            {
                if (asset == null)
                {
                    asset = new PropertyPassItemSubClassUpdate();
                }
                asset.Asset_ID_Internal = PropertyPassrecords.Asset_ID_Internal;
                asset.AssetSerialNo = PropertyPassrecords.AssetSerialNo;
                asset.Assigned_By = PropertyPassrecords.Assigned_By;
                asset.CheckInDate = PropertyPassrecords.CheckInDate.ToString();
                asset.CheckOutDate = PropertyPassrecords.CheckOutDate.ToString();
                asset.DueDate = PropertyPassrecords.DueDate.ToString();
                asset.PropertyPassID_Internal = PropertyPassrecords.PropertyPassID_Internal;
                asset.SiteIDInternal = PropertyPassrecords.SiteIDInternal;
                asset.Quantity = PropertyPassrecords.Quantity;
                asset.ModifiedOn = PropertyPassrecords.ModifiedOn.ToString();
            }

            public class PropertyPassItemSubClassUpdate
            {
                public string Asset_ID_Internal { get; set; }
                public string AssetSerialNo { get; set; }
                public string Assigned_By { get; set; }
                public string CheckInDate { get; set; }
                public string CheckOutDate { get; set; }
                public string DueDate { get; set; }
                public string PropertyPassID_Internal { get; set; }
                public int Quantity { get; set; }
                public string SiteIDInternal { get; set; }
                public string ModifiedOn { get; set; }
            }

        }
        public class PropertyPassItemCreateFormat
        {
            public string UserID { get; set; }
            public string PropertyPassItemID_Internal { get; set; }
            public PropertyPassItemSubClassCreate asset { get; set; }

            public void SetAssetValuesCreate(PropertyPassItem PropertyPassrecords)
            {
                if (asset == null)
                {
                    asset = new PropertyPassItemSubClassCreate();
                }
                asset.Asset_ID_Internal = PropertyPassrecords.Asset_ID_Internal;
                asset.AssetSerialNo = PropertyPassrecords.AssetSerialNo;
                asset.Assigned_By = PropertyPassrecords.Assigned_By;
                asset.CheckInDate = PropertyPassrecords.CheckInDate.ToString();
                asset.CheckOutDate = PropertyPassrecords.CheckOutDate.ToString();
                asset.DueDate = PropertyPassrecords.DueDate.ToString();
                asset.PropertyPassID_Internal = PropertyPassrecords.PropertyPassID_Internal;
                asset.SiteIDInternal = PropertyPassrecords.SiteIDInternal;
                asset.Quantity = PropertyPassrecords.Quantity;
            }

            public class PropertyPassItemSubClassCreate
            {
                public string Asset_ID_Internal { get; set; }
                public string AssetSerialNo { get; set; }
                public string Assigned_By { get; set; }
                public string CheckInDate { get; set; }
                public string CheckOutDate { get; set; }
                public string DueDate { get; set; }
                public string PropertyPassID_Internal { get; set; }
                public int Quantity { get; set; }
                public string SiteIDInternal { get; set; }
            }

        }

    }

    
}
