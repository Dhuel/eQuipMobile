using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eQuipMobile
{
    public class FormDesignerClass
    {
        public Stack<TabCollection> TabCollection { get; set; }
    }

    public class TabCollection
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public List<FieldCollection> FieldCollection { get; set; }
    }
    public class FieldCollection
    {
        public string Name { get; set; }
        public string Display { get; set; }
    }

    public class AssetDetailNames
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Barcode { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public string Site { get; set; }
        public string People { get; set; }
        public string Usage { get; set; }
        public string Condition { get; set; }
        public string AssetStatus { get; set; }
        public string SerialNo { get; set; }
        public string Price { get; set; }
        public string PurchaseDate { get; set; }
        public string PurchaseOrderNo { get; set; }
        public string Quantity { get; set; }
        public string Vendor { get; set; }
        public string AuditStatus { get; set; }
        public string Asset_UID { get; set; }
        public string Department { get; set; }
        public string Mfg { get; set; }
        public string POLine { get; set; }
        public string POStatus { get; set; }
        public string SubLocation { get; set; }
        public string OriginalPartNo { get; set; }
        public string ThumbnailImage { get; set; }
        public string Model { get; set; }
        public AssetDetailNames(){}
        public AssetDetailNames(FormDesignerClass FormDesignInfo)
        {
            foreach(TabCollection TabValue in FormDesignInfo.TabCollection)
            {
                foreach(FieldCollection FieldInfo in TabValue.FieldCollection)
                {
                    switch (FieldInfo.Name)
                    {
                        case "ASSETNAME":
                            Name = FieldInfo.Display;
                            break;
                        case "ASSETDESCRIPTION":
                            Description = FieldInfo.Display;
                            break;
                        case "BARCODE":
                            Barcode = FieldInfo.Display;
                            break;
                        case "ASSETCATEGORYID":
                            Category = FieldInfo.Display;
                            break;
                        case "LOCATIONID":
                            Location = FieldInfo.Display;
                            break;
                        case "SITEID":
                            Site = FieldInfo.Display;
                            break;
                        case "PEOPLEID":
                            People = FieldInfo.Display;
                            break;
                        case "ASSETUSAGEID":
                            Usage = FieldInfo.Display;
                            break;
                        case "ASSETCONDITIONID":
                            Condition = FieldInfo.Display;
                            break;
                        case "ASSETSTATUS":
                            AssetStatus = FieldInfo.Display;
                            break;
                        case "ASSETSERIALNO":
                            SerialNo = FieldInfo.Display;
                            break;
                        case "PRICE":
                            Price = FieldInfo.Display;
                            break;
                        case "DPPURCHASEDATE":
                            PurchaseDate = FieldInfo.Display;
                            break;
                        case "PURCHASEORDERNO":
                            PurchaseOrderNo = FieldInfo.Display;
                            break;
                        case "QUANTITY":
                            Quantity = FieldInfo.Display;
                            break;
                        case "VENDOR":
                            Vendor = FieldInfo.Display;
                            break;
                        case "AUDITSTATUS":
                            AuditStatus = FieldInfo.Display;
                            break;
                        case "ASSET_UID":
                            Asset_UID = FieldInfo.Display;
                            break;
                        case "DEPARTMENT":
                            Department = FieldInfo.Display;
                            break;
                        case "MFG":
                            Mfg = FieldInfo.Display;
                            break;
                        case "MODEL":
                            Model = FieldInfo.Display;
                            break;
                        case "POLINE":
                            POLine = FieldInfo.Display;
                            break;
                        case "POSTATUS":
                            POStatus = FieldInfo.Display;
                            break;
                        case "SUBLOCATION":
                            SubLocation = FieldInfo.Display;
                            break;
                        case "ORIGINALPARTNO":
                            OriginalPartNo = FieldInfo.Display;
                            break;
                        case "THUMBNAILIMAGE":
                            ThumbnailImage = FieldInfo.Display;
                            break;
                    }
                        
                }
            }
            Name = Name?? "Asset Name";
            Description = Description ??  "Description";
            Barcode = Barcode ?? "Barcode";
            Department = Department ?? "Department";
            if (Category == null)
                Category = "Category";
            if (Location == null)
                Location = "Location";
            if (Site == null)
                Site = "Site";
            if (People == null)
                People = "People";
            if (Usage == null)
                Usage = "Usage";
            if (Condition == null)
                Condition = "Condition";
            if (AssetStatus == null)
                AssetStatus = "AssetStatus";
            if (SerialNo == null)
                SerialNo = "SerialNo";
            if (Price == null)
                Price = "Price";
            if (PurchaseDate == null)
                PurchaseDate = "PurchaseDate";
            if (PurchaseOrderNo == null)
                PurchaseOrderNo = "PurchaseOrderNo";
            if (Quantity == null)
                Quantity = "Quantity";
            if (Vendor == null)
                Vendor = "Vendor";
            if (AuditStatus == null)
                AuditStatus = "AuditStatus";
            if (Asset_UID == null)
                Asset_UID = "Asset UID";
            if (Mfg == null)
                Mfg = "Manufacturer";
            if (Model == null)
                Model = "Model";
            if (POLine == null)
                POLine = "POLine";
            if (POStatus == null)
                POStatus = "POStatus";
            if (SubLocation == null)
                SubLocation = "SubLocation";
            if (OriginalPartNo == null)
                OriginalPartNo = "OriginalPartNo";
            if (ThumbnailImage == null)
                ThumbnailImage = "ThumbnailImage";

        }
    }

}
