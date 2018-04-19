using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace eQuipMobile
{
    public class AssetClass
    {
        public string AssetIdInternal { get; set; }
        public string Asset_SiteIdInternal { get; set; }
        public string Asset_locationIdInternal { get; set; }
        public string Asset_SublocationIdInternal { get; set; }
        public string Asset_PeopleIdInternal { get; set; }
        public string AssetName { get; set; }
        public int Asset_Department { get; set; }
        public string Barcode { get; set;}
        public string AssetSerialNumber { get; set; }
        public AssetJsonObject AssetJSON { get; set; }
        public string AssetJSONDb { get; set; }
        public bool Synced { get; set; }
        public List<Changesmade> ChangesMade { get; set; }
        public string AssetChangesDb { get; set; }
        public string BarcodeDisplay_ { get; set; }
        public string AssetSerialNumberDisplay_ { get; set; }
        public string Asset_SublocationIdInternalDisplay_ { get; set; }
        public string QuantityDisplay_ { get; set; }
        public string OriginalPartDisplay_ { get; set; }
        public string AuditStatusDisplay_ { get; set; }
        public string UIDDisplay_ { get; set; }
        public string SiteDisplay_ { get; set; }
        public string SiteName_ { get; set; }
        public string LocationName_ { get; set; }
        public string LocationDisplay_ { get; set; }
        public string AssetDisplayBarcode_
        {
            get
            {
                return string.Format("{0}: {1} ", BarcodeDisplay_, Barcode);
            }
        }
        public string AssetDisplaySite_
        {
            get
            {
                return string.Format("{0}: {1} ", SiteDisplay_, SiteName_);
            }
        }
        public string AssetDisplayLocation_
        {
            get
            {
                return string.Format("{0}: {1} ", LocationDisplay_, LocationName_);
            }
        }
        public string AssetDisplayQty_
        {
            get
            {
                return string.Format("{0}: {1} ", QuantityDisplay_, AssetJSON.Quantity);
            }
        }
        public string AssetDisplaySubLoc_
        {
            get
            {
                return string.Format("{0}: {1} ", Asset_SublocationIdInternalDisplay_, AssetJSON.SubLocation);
            }
        }
        public string AssetDisplayOrig_
        {
            get
            {
                return string.Format("{0}: {1} ", OriginalPartDisplay_, AssetJSON.OriginalPartNo);
            }
        }
        public string AssetDisplaySerial_
        {
            get
            {
                return string.Format("{0}: {1} ", AssetSerialNumberDisplay_, AssetJSON.AssetSerialNo);
            }
        }
        public string AssetDisplayUID_
        {
            get
            {
                return string.Format("{0}: {1} ", UIDDisplay_, AssetJSON.Asset_UID);
            }
        }
        public string AssetDisplayAuditStatus_
        {
            get
            {
                return string.Format("{0}: {1} ", AuditStatusDisplay_, AssetJSON.AuditStatus);
            }
        }
        public AssetClass() { }
        public AssetClass (AssetJsonObject AssetJSON_ = null, bool Synced_ = false)
        {
            if (AssetJSON_ != null)
            {
                AssetIdInternal = AssetJSON_.AssetIDInternal;
                Asset_Department = AssetJSON_.DataGatherID;
                Asset_SiteIdInternal = AssetJSON_.SiteIDInternal;
                Asset_locationIdInternal = AssetJSON_.LocationIDInternal;
                Asset_SublocationIdInternal = AssetJSON_.SubLocation;
                AssetName = AssetJSON_.AssetName;
                AssetSerialNumber = AssetJSON_.AssetSerialNo;
                Asset_PeopleIdInternal = AssetJSON_.PeopleIDInternal;
                Barcode = AssetJSON_.Barcode;
                AssetJSON = AssetJSON_;
                Synced = Synced_;
                AssetChangesDb = JsonConvert.SerializeObject(ChangesMade);
                AssetJSONDb = JsonConvert.SerializeObject(AssetJSON_);
            }
        }
        public AssetClass(AssetDbClass AssetJSON_, AssetJsonObject AssetJsonObj, List<Changesmade> ChangesMade_, bool Synced_ = false)
        {
            AssetIdInternal = AssetJSON_.AssetIdInternal;
            Asset_Department = AssetJSON_.Asset_Department;
            Asset_SiteIdInternal = AssetJSON_.Asset_SiteIdInternal;
            Asset_locationIdInternal = AssetJSON_.Asset_locationIdInternal;
            Asset_SublocationIdInternal = AssetJSON_.Asset_SublocationIdInternal;
            AssetName = AssetJSON_.AssetName;
            AssetSerialNumber = AssetJSON_.AssetSerialNumber;
            Asset_PeopleIdInternal = AssetJSON_.Asset_PeopleIdInternal;
            Barcode = AssetJSON_.Barcode;
            AssetJSON = AssetJsonObj;
            Synced = AssetJSON_.Synced;
            ChangesMade = ChangesMade_;
            AssetChangesDb = JsonConvert.SerializeObject(ChangesMade);
            AssetJSONDb = JsonConvert.SerializeObject(AssetJSON_);
         }
        public static List<AssetClass> DbToAssetClass(IEnumerable<AssetDbClass> AssetJSON_, AssetDetailNames Names = null, SQLiteConnection _connection = null)
        {
            var AssetList = new List<AssetClass>();
            foreach ( AssetDbClass AssetDBData in AssetJSON_)
            {
                var intermediateAsset = new AssetClass
                {
                    AssetIdInternal = AssetDBData.AssetIdInternal,
                    Asset_SiteIdInternal = AssetDBData.Asset_SiteIdInternal,
                    Asset_locationIdInternal = AssetDBData.Asset_locationIdInternal,
                    Asset_SublocationIdInternal = AssetDBData.Asset_SublocationIdInternal,
                    AssetName = AssetDBData.AssetName,
                    Asset_Department = AssetDBData.Asset_Department,
                    AssetSerialNumber = AssetDBData.AssetSerialNumber,
                    Asset_PeopleIdInternal = AssetDBData.Asset_PeopleIdInternal,
                    Barcode = AssetDBData.Barcode,
                    AssetJSON = JsonConvert.DeserializeObject<AssetJsonObject>(AssetDBData.AssetJSONDb),
                    ChangesMade = JsonConvert.DeserializeObject<List<Changesmade>>(AssetDBData.AssetChangesDb),
                    Synced = AssetDBData.Synced
                };
                if (_connection != null)
                {
                    try
                    {
                        intermediateAsset.SiteName_ = Database.Sites.GetTableData(_connection).Single(cm => cm.SiteIdInternal == AssetDBData.Asset_SiteIdInternal).SiteName;
                        intermediateAsset.LocationName_ = Database.Locations.GetTableDataFromSites(_connection, AssetDBData.Asset_SiteIdInternal).Single(cm => cm.LocationIdInternal == AssetDBData.Asset_locationIdInternal).LocationName;
                    }
                    catch(Exception exc)
                    {
                        DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                    }
                }
                if (Names != null)
                {
                    intermediateAsset.BarcodeDisplay_ = Names.Barcode;
                    intermediateAsset.AssetSerialNumberDisplay_ = Names.SerialNo;
                    intermediateAsset.Asset_SublocationIdInternalDisplay_ = Names.SubLocation;
                    intermediateAsset.QuantityDisplay_ = Names.Quantity;
                    intermediateAsset.OriginalPartDisplay_ = Names.OriginalPartNo;
                    intermediateAsset.AuditStatusDisplay_ = Names.AuditStatus;
                    intermediateAsset.UIDDisplay_ = Names.Asset_UID;
                    intermediateAsset.SiteDisplay_ = Names.Site;
                    intermediateAsset.LocationDisplay_ = Names.Location;
                    
                }
                AssetList.Add(intermediateAsset);  
            }
            return AssetList;
        }

        public static AssetDbClass AssetClassToDb(AssetClass asset, bool synced)
        {
            var returnedAssetDb = new AssetDbClass { 
                AssetIdInternal = asset.AssetIdInternal,
                Barcode = asset.Barcode,
                AssetName = asset.AssetName,
                AssetSerialNumber = asset.AssetSerialNumber,
                Asset_Department = asset.Asset_Department,
                Asset_locationIdInternal = asset.Asset_locationIdInternal,
                Asset_PeopleIdInternal = asset.Asset_PeopleIdInternal,
                Asset_SiteIdInternal = asset.Asset_SiteIdInternal,
                Synced = synced,
                Asset_SublocationIdInternal = asset.Asset_SublocationIdInternal,
                AssetChangesDb = JsonConvert.SerializeObject(asset.ChangesMade),
                AssetJSONDb = JsonConvert.SerializeObject(asset.AssetJSON),
                OriginalPartNo = asset.AssetJSON.OriginalPartNo,
            };
            return returnedAssetDb;
        }
    }

    public class AssetDisplayClass
    {
        public string AssetName { get; set; }
        public string Barcode { get; set; }
        public string AssetBarcode_ { get; set; }
        public int Quantity { get; set; }
        public string AssetQuantity_ { get; set; }
        public string SiteName { get; set; }
        public string AssetSiteName_ { get; set; }
        public string LocationName { get; set; }
        public string AssetLocationName_ { get; set; }
        public string Asset_SublocationIdInternal { get; set; }
        public string AssetSubLocationName_ { get; set; }
        //public string DepartmentName { get; set; }
        public string AssetIdInternal { get; set; }
        public string AssetJSONDb { get; set; }
        public string AssetSerialNo { get; set; }
        public string AssetSerialNo_ { get; set; }
        public double Cost { get; set; }
        public string AssetCost_ { get; set; }
        public AssetDisplayClass() { }
        public string AsseDetailBarcode_
        {
            get
            {
                return string.Format("{0}: {1} ", AssetBarcode_, Barcode);
            }
        }
        public string AsseDetailSite_
        {
            get
            {
                return string.Format("{0}: {1} ", AssetSiteName_, SiteName);
            }
        }
        public string AsseDetailLocation_
        {
            get
            {
                return string.Format("{0}: {1} ", AssetLocationName_, LocationName);
            }
        }
        public string AsseDetailSubLocation_
        {
            get
            {
                return string.Format("{0}: {1} ", AssetSubLocationName_, Asset_SublocationIdInternal);
            }
        }
        public string AsseDetailSerialNo_
        {
            get
            {
                return string.Format("{0}: {1} ", AssetSerialNo_, AssetSerialNo);
            }
        }
        public string AsseDetailQuantity_
        {
            get
            {
                return string.Format("{0}: {1} ", AssetQuantity_, Quantity);
            }
        }
        public string AsseDetailCost_
        {
            get
            {
                return string.Format("{0}: {1} ", AssetCost_, Cost);
            }
        }
        public static AssetDisplayClass FullDetails(AssetDisplayClass display_, AssetDetailNames Name)
        {
            AssetJsonObject details = new AssetJsonObject();
            if (display_.AssetJSONDb != null)
                details = JsonConvert.DeserializeObject<AssetJsonObject>(display_.AssetJSONDb);
            else
            {
                details.Quantity = 0;
                details.AssetSerialNo = "000000";
                details.Price = 0;
            }
            var returnedAssetDb = new AssetDisplayClass
            {
                AssetIdInternal = display_.AssetIdInternal,
                Barcode = display_.Barcode,
                AssetName = display_.AssetName,
                Quantity = details.Quantity,
                AssetSerialNo = details.AssetSerialNo,
                Cost = details.Price,
                SiteName = display_.SiteName,
                LocationName = display_.LocationName,
                Asset_SublocationIdInternal = display_.Asset_SublocationIdInternal,
                AssetJSONDb = display_.AssetJSONDb,
                AssetBarcode_ = Name.Barcode,
                AssetSiteName_ = Name.Site,
                AssetLocationName_ = Name.Location,
                AssetSerialNo_ = Name.SerialNo,
                AssetSubLocationName_ = Name.SubLocation,
                AssetQuantity_ = Name.Quantity,
                AssetCost_ = Name.Price
            };
            return returnedAssetDb;
        }
        public static IEnumerable<AssetDisplayClass> FullDetails(IEnumerable<AssetDisplayClass> AssetDisplay, AssetDetailNames Name)
        {
            List<AssetDisplayClass> returnedAssetDb = new List<AssetDisplayClass>();
            foreach (AssetDisplayClass display_ in AssetDisplay)
            {
                var details = JsonConvert.DeserializeObject<AssetJsonObject>(display_.AssetJSONDb);
                var singledisplay = new AssetDisplayClass
                {
                    AssetIdInternal = display_.AssetIdInternal,
                    Quantity = details.Quantity,
                    AssetSerialNo = details.AssetSerialNo,
                    Cost = details.Price,
                    Barcode = display_.Barcode,
                    AssetName = display_.AssetName,
                    SiteName = display_.SiteName,
                    LocationName = display_.LocationName,
                    Asset_SublocationIdInternal = display_.Asset_SublocationIdInternal,
                    AssetJSONDb = display_.AssetJSONDb,
                    AssetBarcode_ = Name.Barcode,
                    AssetSiteName_ = Name.Site,
                    AssetLocationName_ = Name.Location,
                    AssetSerialNo_ = Name.SerialNo,
                    AssetSubLocationName_ = Name.SubLocation,
                    AssetQuantity_ = Name.Quantity,
                    AssetCost_ = Name.Price
                };
                returnedAssetDb.Add(singledisplay);
            }
            
            return returnedAssetDb;
        }
        public static AssetDisplayClass GetDetails( AssetDisplayClass display_)
        {
            var details = JsonConvert.DeserializeObject<AssetJsonObject>(display_.AssetJSONDb);
            var returnedAssetDb = display_;
            returnedAssetDb.AssetSerialNo = details.AssetSerialNo;
            returnedAssetDb.Cost = details.Price;
            returnedAssetDb.Quantity = details.Quantity;
            return returnedAssetDb;
        }
    }


    public class AssetJsonObject
    {
        public string AssetName { get; set; }
        public string AssetDescription { get; set; }
        public string Barcode { get; set; }
        public string AssetIDInternal { get; set; }
        public string AssetCategoryIDInternal { get; set; }
        public string LocationIDInternal { get; set; }
        public string SiteIDInternal { get; set; }
        public string PeopleIDInternal { get; set; }
        public int AssetUsageID { get; set; }
        public int AssetConditionID { get; set; }
        public string AssetStatus { get; set; }
        public string AssetSerialNo { get; set; }
        public double Price { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string PurchaseOrderNo { get; set; }
        public int Quantity { get; set; }
        public int ShortageOverage { get; set; }
        public string Vendor { get; set; }
        public bool IsActive { get; set; }
        public DateTime AuditDate { get; set; }
        public string AuditStatus { get; set; }
        public string Asset_UID { get; set; }
        public int DataGatherID { get; set; }
        public string Mfg { get; set; }
        public string Model { get; set; }
        public int POLine { get; set; }
        public string POStatus { get; set; }
        public DateTime DateModified { get; set; }
        public string SubLocation { get; set; }
        public string OriginalPartNo { get; set; }
        public string ThumbnailImage { get; set; }
        public AssetJsonObject() { }
    }

    public class Changesmade
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
    }

    public class AssetsChange
    {
        public string AssetIDInternal { get; set; }
        public DateTime DateModified { get; set; }
        public List<Changesmade> Changesmade { get; set; }
    }

    public class AssetUpdateClass
    {
        public string UserID { get; set; }
        public List<AssetsChange> AssetsChanges { get; set; }
    }
    public class CreateAssetClass
    {
        public string UserID { get; set; }
        public string AssetIDInternal { get; set; }
        public AssetJsonObject Asset { get; set; }
    }
}


