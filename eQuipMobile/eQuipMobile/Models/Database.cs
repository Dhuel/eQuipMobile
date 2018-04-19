using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
////TODO - can be used to reduce code in Database class
//public static Object CreateObjectBy(Type clazz)
//Complete DeleteRecords class which deletes assets based on the table name
//{
//    //Example - var test = CreateObjectBy(typeof(LoginDbTable));
//    Object theObject = Activator.CreateInstance(clazz);
//    return theObject;
//}
namespace eQuipMobile
{
    [Table("Login")]
    public class LoginDbTable
    {
        [PrimaryKey]
        public string UserID { get; set; }
        public string ServerTime { get; set; }
        public string RoleID { get; set; }
        public string MobileSites { get; set; }
        public string AllowDuplicateBarcode { get; set; }
        public string Permissions { get; set; }
        public string Framework { get; set; }
        public string eQuipVersion { get; set; }
        public string RequiredFields { get; set; }
        public string LastSyncDate { get; set; }
    }

    [Table("Settings")]
    public class SettingsDb
    {
        public bool FastAudit { get; set; }
        public bool BlindAudit { get; set; }
        public bool PriceLock { get; set; }
        public bool FrontCamera { get; set; }
        public bool FastAuditEntry { get; set; }
    }

    [Table("Sites")]
    public class SitesDbTable
    {
        [PrimaryKey]
        public string SiteIdInternal { get; set; }
        public string SiteName { get; set; }
        public string SiteCode { get; set; }
        public string SiteDescription { get; set; }
    }

    [Table("Locations")]
    public class LocationsDbTable
    {
        [PrimaryKey]
        public string LocationIdInternal { get; set; }
        public string LocationName { get; set; }
        public string LocationDescription { get; set; }
        public string LocationBarcode { get; set; }
        public string SiteIdInternal { get; set; }
    }

    [Table("SubLocations")]
    public class SubLocationsDbTable
    {
        [PrimaryKey]
        public string SubLocationID_Internal { get; set; }
        public string SubLocationName { get; set; }
        public string Location_ID_Internal { get; set; }
        
    }

    [Table("Departments")]
    public class DepartmentsDbTable
    {
        [PrimaryKey]
        public int ID { get; set; }
        public string DepartmentName { get; set; }
        public string LocationName { get; set; }
    }

    [Table("People")]
    public class PeopleDbTable
    {
        [PrimaryKey]
        public string PeopleIDInternal { get; set; }
        public string PeopleName { get; set; }
        public string PeopleDescription { get; set; }
        public string PeopleCategoryIDInternal { get; set; }
        public string LocationIDInternal { get; set; }
        public string SiteIDInternal { get; set; }
        public string AccessCardID { get; set; }
    }

    [Table("Usage")]
    public class UsageDbTable
    {
        [PrimaryKey]
        public string UsageIDInternal { get; set; }
        public int UsageID { get; set; }
        public string UsageLabel { get; set; }
    }

    [Table("Condition")]
    public class ConditionDbTable
    {
        [PrimaryKey]
        public string ConditionIDInternal { get; set; }
        public int ConditionID { get; set; }
        public string ConditionLabel { get; set; }
        
    }

    [Table("Vendor")]
    public class VendorDbTable
    {
        [PrimaryKey]
        public string VendorIDInternal { get; set; }
        public string Company { get; set; }

    }

    [Table("Manufacturer")]
    public class ManufacturerDbTable
    {
        [PrimaryKey]
        public string VendorIDInternal { get; set; }
        public string Company { get; set; }
    }

    [Table("AssetCategory")]
    public class CategoryDbTable
    {
        [PrimaryKey]
        public string AssetCategoryIDInternal { get; set; }
        public int AssetCategoryID { get; set; }
        public string AssetCategoryName { get; set; }
        public int ParentID { get; set; }
    }

    [Table("PropertyPassTable")]
    public class PropertyPassDbTable
    {
        [PrimaryKey]
        public string PropertyPassID_Internal { get; set; }
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
        public bool Synced { get; set; }
    }

    [Table("PropertyPassItemTable")]
    public class PropertyPassItemDbTable
    {
        public string Asset_ID_Internal { get; set; }
        public string AssetSerialNo { get; set; }
        public string Assigned_By { get; set; }
        public string CheckInDate { get; set; }
        public string CheckOutDate { get; set; }
        public string DueDate { get; set; }
        public string PropertyPassID_Internal { get; set; }
        public string PropertyPassItemID_Internal { get; set; }
        public int Quantity { get; set; }
        public string SiteIDInternal { get; set; }
        public string ModifiedOn { get; set; }
        public bool Synced { get; set; }
    }

    [Table("Assets")]
    public class AssetDbClass
    {
        //[PrimaryKey]
        public string AssetIdInternal { get; set; }
        public string Asset_SiteIdInternal { get; set; }
        public string Asset_locationIdInternal { get; set; }
        public string Asset_SublocationIdInternal { get; set; }
        public int Asset_Department { get; set; }
        public string Asset_PeopleIdInternal { get; set; }
        public string AssetName { get; set; }
        public string Barcode { get; set; }
        public string AssetSerialNumber { get; set; }
        public string OriginalPartNo { get; set; }
        public string AssetChangesDb { get; set; }
        public string AssetJSONDb { get; set; }
        public bool Synced { get; set; }
    }

    [Table("Transfers")]
    public class TransferDbTable
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string TransactionName { get; set; }
        public string ToSite { get; set; }
        public string ToLocation { get; set; }
        public string ToPeople { get; set; }
        public string ToSublocation { get; set; }
        public int Quantitymoved { get; set; }
        public string DateMoved { get; set; }
        public string AssetIdInternal { get; set; }
    }

    [Table("DeletedRecords")]
    public class DeleteDbTable
    {
        [PrimaryKey]
        public string IdInternal { get; set; }
        public string TableName { get; set; }
    }

    [Table("MobileSites")]
    public class MobileSitesData
    {
        [PrimaryKey]
        public string SiteIdInternal { get; set; }
        public string SiteName { get; set; }
    }
    class Database
    {
        public static void Reinitialize(SQLiteConnection db)
        {
            Sites.DeleteAll(db);
            Locations.DeleteAll(db);
            SubLocations.DeleteAll(db);
            Department.DeleteAll(db);
            People.DeleteAll(db);
            Usage.DeleteAll(db);
            Condition.DeleteAll(db);
            Vendor.DeleteAll(db);
            Manufacturer.DeleteAll(db);
            Category.DeleteAll(db);
            PropertyPassTable.DeleteAll(db);
            PropertyPassItem.DeleteAll(db);
            Assets.DeleteAll(db);
            Transfers.DeleteAll(db);
            Deletes.DeleteAll(db);
            MobileSites.DeleteAll(db);
            Login.DeleteAll(db);
        }
       // private SQLiteConnection _connection = DependencyService.Get<ISQLiteDb>().GetConnection();
        public class Login
        {
            public static IEnumerable<LoginDbTable> GetTableData(SQLiteConnection db)
            {
                db.CreateTable<LoginDbTable>();
                return db.Query<LoginDbTable>("select * from Login");
            }
            public static IEnumerable<LoginDbTable>UpdateSync(SQLiteConnection db, string LastSyncDate)
            {
                db.CreateTable<LoginDbTable>();
                return db.Query<LoginDbTable>("Update Login set LastSyncDate = ?", LastSyncDate);
            }
            
            public static void Insert(SQLiteConnection db, LoginDbTable _data)
            {
                db.Insert(_data);
            }
            public static void Update(SQLiteConnection db, LoginDbTable _data)
            {
                db.Update(_data);
            }
            public static void Delete(SQLiteConnection db, LoginDbTable _data)
            {
                db.Delete(_data);
            }
            public static void DeleteAll(SQLiteConnection db)
            {
                db.CreateTable<LoginDbTable>();
                db.DeleteAll<LoginDbTable>();
            }
        }
        public class MobileSites
        {
            public static IEnumerable<MobileSitesData> GetTableData(SQLiteConnection db)
            {
                db.CreateTable<MobileSitesData>();
                return db.Query<MobileSitesData>("select * from MobileSites");
            }
            public static void DeleteAndInsert(SQLiteConnection db, List<MobileSitesData> _data)
            {
                db.CreateTable<MobileSitesData>();
                db.DeleteAll<MobileSitesData>();
                db.InsertAll(_data);
            }
            public static void DeleteAll(SQLiteConnection db)
            {
                db.CreateTable<MobileSitesData>();
                db.DeleteAll<MobileSitesData>();
            }
        }

        public class Sites
        {
            public static IEnumerable<SitesDbTable> GetTableData(SQLiteConnection db)
            {
                db.CreateTable<SitesDbTable>();
                return db.Query<SitesDbTable>("select * from Sites ORDER BY SiteName ASC");
            }
            public static void Insert(SQLiteConnection db, SitesDbTable _data)
            {
                db.Insert(_data);
            }
            public static void InsertAll(SQLiteConnection db, IEnumerable<SitesDbTable> _data)
            {
                db.CreateTable<SitesDbTable>();
                db.InsertAll(_data);
            }
            public static void Update(SQLiteConnection db, SitesDbTable _data)
            {
                db.Update(_data);
            }
            public static void Delete(SQLiteConnection db, SitesDbTable _data)
            {
                db.Delete(_data);
            }
            public static void DeleteAndInsert(SQLiteConnection db, IEnumerable<SitesDbTable> _data)
            {
                db.CreateTable<SitesDbTable>();
                db.DeleteAll<SitesDbTable>();
                db.InsertAll(_data);
            }
            public static void DeleteAll(SQLiteConnection db)
            {
                db.CreateTable<SitesDbTable>();
                db.DeleteAll<SitesDbTable>();
            }
        }

        public class Locations
        {
            public static IEnumerable<LocationsDbTable> GetTableDataFromSites(SQLiteConnection db, string siteID)
            {
                db.CreateTable<LocationsDbTable>();
                return db.Query<LocationsDbTable>("select * from Locations where SiteIdInternal = ? ORDER BY LocationName ASC", siteID);
            }
            public static void Insert(SQLiteConnection db, LocationsDbTable _data)
            {
                db.Insert(_data);
            }
            public static void InsertAll(SQLiteConnection db, IEnumerable<LocationsDbTable> _data)
            {
                db.CreateTable<LocationsDbTable>();
                db.InsertAll(_data);
            }
            public static void Update(SQLiteConnection db, LocationsDbTable _data)
            {
                db.Update(_data);
            }
            public static void Delete(SQLiteConnection db, LocationsDbTable _data)
            {
                db.Delete(_data);
            }
            public static void DeleteAndInsert(SQLiteConnection db, IEnumerable<LocationsDbTable> _data)
            {
                db.CreateTable<LocationsDbTable>();
                db.DeleteAll<LocationsDbTable>();
                db.InsertAll(_data);
            }
            public static void DeleteAll(SQLiteConnection db)
            {
                db.CreateTable<LocationsDbTable>();
                db.DeleteAll<LocationsDbTable>();
            }
        }

        public class SubLocations
        {
            public static IEnumerable<SubLocationsDbTable> GetTableData(SQLiteConnection db)
            {
                db.CreateTable<SubLocationsDbTable>();
                return db.Query<SubLocationsDbTable>("select * from SubLocations");
            }
            public static IEnumerable<SubLocationsDbTable> GetTableDataFromLocations(SQLiteConnection db, string LocationID)
            {
                db.CreateTable<SubLocationsDbTable>();
                return db.Query<SubLocationsDbTable>("select * from SubLocations where Location_ID_Internal = ? ORDER BY SubLocationName ASC", LocationID);
            }
            public static void Insert(SQLiteConnection db, SubLocationsDbTable _data)
            {
                db.Insert(_data);
            }
            public static void InsertAll(SQLiteConnection db, IEnumerable<SubLocationsDbTable> _data)
            {
                db.CreateTable<SubLocationsDbTable>();
                db.InsertAll(_data);
            }
            public static void Update(SQLiteConnection db, SubLocationsDbTable _data)
            {
                db.Update(_data);
            }
            public static void Delete(SQLiteConnection db, SubLocationsDbTable _data)
            {
                db.Delete(_data);
            }
            public static void DeleteAndInsert(SQLiteConnection db, IEnumerable<SubLocationsDbTable> _data)
            {
                db.CreateTable<SubLocationsDbTable>();
                db.DeleteAll<SubLocationsDbTable>();
                db.InsertAll(_data);
            }
            public static void DeleteAll(SQLiteConnection db)
            {
                db.CreateTable<SubLocationsDbTable>();
                db.DeleteAll<SubLocationsDbTable>();
            }
        }

        public class Department
        {
            public static IEnumerable<DepartmentsDbTable> GetTableData(SQLiteConnection db)
            {
                db.CreateTable<DepartmentsDbTable>();
                return db.Query<DepartmentsDbTable>("select Distinct * from Departments");
            }
            public static void Insert(SQLiteConnection db, DepartmentsDbTable _data)
            {
                db.Insert(_data);
            }
            public static void InsertAll(SQLiteConnection db, IEnumerable<DepartmentsDbTable> _data)
            {
                db.CreateTable<DepartmentsDbTable>();
                db.InsertAll(_data);
            }
            public static void Update(SQLiteConnection db, DepartmentsDbTable _data)
            {
                db.Update(_data);
            }
            public static void Delete(SQLiteConnection db, DepartmentsDbTable _data)
            {
                db.Delete(_data);
            }
            public static void DeleteAndInsert(SQLiteConnection db, IEnumerable<DepartmentsDbTable> _data)
            {
                db.CreateTable<DepartmentsDbTable>();
                db.DeleteAll<DepartmentsDbTable>();
                db.InsertAll(_data);
                
            }
            public static void DeleteAll(SQLiteConnection db)
            {
                db.CreateTable<DepartmentsDbTable>();
                db.DeleteAll<DepartmentsDbTable>();
            }
        }

        public class People
        {
            public static IEnumerable<PeopleDbTable> GetTableData(SQLiteConnection db)
            {
                db.CreateTable<PeopleDbTable>();
                return db.Query<PeopleDbTable>("select * from People ORDER BY Peoplename ASC");
            }
            public static IEnumerable<PeopleDbTable> GetTableDataByIdOrAccess(SQLiteConnection db, string ID)
            {
                db.CreateTable<PeopleDbTable>();
                return db.Query<PeopleDbTable>("select * from  People where AccessCardID like ? or PeopleName like ? ORDER BY Peoplename ASC", "%" + ID + "%", "%" + ID + "%");
            }
            public static void Insert(SQLiteConnection db, PeopleDbTable _data)
            {
                db.Insert(_data);
            }
            public static void InsertAll(SQLiteConnection db, IEnumerable<PeopleDbTable> _data)
            {
                db.CreateTable<PeopleDbTable>();
                db.InsertAll(_data);
            }
            public static void Update(SQLiteConnection db, PeopleDbTable _data)
            {
                db.Update(_data);
            }
            public static void Delete(SQLiteConnection db, PeopleDbTable _data)
            {
                db.Delete(_data);
            }
            public static void DeleteAndInsert(SQLiteConnection db, IEnumerable<PeopleDbTable> _data)
            {
                db.CreateTable<PeopleDbTable>();
                db.DeleteAll<PeopleDbTable>();
                db.InsertAll(_data);
            }
            public static void DeleteAll(SQLiteConnection db)
            {
                db.CreateTable<PeopleDbTable>();
                db.DeleteAll<PeopleDbTable>();
            }
        }

        public class Usage
        {
            public static IEnumerable<UsageDbTable> GetTableData(SQLiteConnection db)
            {
                db.CreateTable<UsageDbTable>();
                return db.Query<UsageDbTable>("select * from Usage order by UsageLabel ASC");
            }
            public static void Insert(SQLiteConnection db, UsageDbTable _data)
            {
                db.Insert(_data);
            }
            public static void InsertAll(SQLiteConnection db, IEnumerable<UsageDbTable> _data)
            {
                db.CreateTable<UsageDbTable>();
                db.InsertAll(_data);
            }
            public static void Update(SQLiteConnection db, UsageDbTable _data)
            {
                db.Update(_data);
            }
            public static void Delete(SQLiteConnection db, UsageDbTable _data)
            {
                db.Delete(_data);
            }
            public static void DeleteAndInsert(SQLiteConnection db, IEnumerable<UsageDbTable> _data)
            {
                db.CreateTable<UsageDbTable>();
                db.DeleteAll<UsageDbTable>();
                db.InsertAll(_data);
            }
            public static void DeleteAll(SQLiteConnection db)
            {
                db.CreateTable<UsageDbTable>();
                db.DeleteAll<UsageDbTable>();
            }
        }

        public class Condition
        {
            public static IEnumerable<ConditionDbTable> GetTableData(SQLiteConnection db)
            {
                db.CreateTable<ConditionDbTable>();
                return db.Query<ConditionDbTable>("select * from Condition order by ConditionLabel ASC");
            }
            public static void Insert(SQLiteConnection db, ConditionDbTable _data)
            {
                db.Insert(_data);
            }
            public static void InsertAll(SQLiteConnection db, IEnumerable<ConditionDbTable> _data)
            {
                db.CreateTable<ConditionDbTable>();
                db.InsertAll(_data);
            }
            public static void Update(SQLiteConnection db, ConditionDbTable _data)
            {
                db.Update(_data);
            }
            public static void Delete(SQLiteConnection db, ConditionDbTable _data)
            {
                db.Delete(_data);
            }
            public static void DeleteAndInsert(SQLiteConnection db, IEnumerable<ConditionDbTable> _data)
            {
                db.CreateTable<ConditionDbTable>();
                db.DeleteAll<ConditionDbTable>();
                db.InsertAll(_data);
            }
            public static void DeleteAll(SQLiteConnection db)
            {
                db.CreateTable<ConditionDbTable>();
                db.DeleteAll<ConditionDbTable>();
            }
        }

        public class Vendor
        {
            public static IEnumerable<VendorDbTable> GetTableData(SQLiteConnection db)
            {
                db.CreateTable<VendorDbTable>();
                return db.Query<VendorDbTable>("select * from Vendor ORDER BY Company ASC");
            }
            public static void Insert(SQLiteConnection db, VendorDbTable _data)
            {
                db.Insert(_data);
            }
            public static void InsertAll(SQLiteConnection db, IEnumerable<VendorDbTable> _data)
            {
                db.CreateTable<VendorDbTable>();
                db.InsertAll(_data);
            }
            public static void Update(SQLiteConnection db, VendorDbTable _data)
            {
                db.Update(_data);
            }
            public static void Delete(SQLiteConnection db, VendorDbTable _data)
            {
                db.Delete(_data);
            }
            public static void DeleteAndInsert(SQLiteConnection db, IEnumerable<VendorDbTable> _data)
            {
                db.CreateTable<VendorDbTable>();
                db.DeleteAll<VendorDbTable>();
                db.InsertAll(_data);
            }
            public static void DeleteAll(SQLiteConnection db)
            {
                db.CreateTable<VendorDbTable>();
                db.DeleteAll<VendorDbTable>();
            }
        }

        public class Manufacturer
        {
            public static IEnumerable<ManufacturerDbTable> GetTableData(SQLiteConnection db)
            {
                db.CreateTable<ManufacturerDbTable>();
                return db.Query<ManufacturerDbTable>("select * from Manufacturer ORDER BY Company ASC");
            }
            public static void Insert(SQLiteConnection db, ManufacturerDbTable _data)
            {
                db.Insert(_data);
            }
            public static void InsertAll(SQLiteConnection db, IEnumerable<ManufacturerDbTable> _data)
            {
                db.CreateTable<ManufacturerDbTable>();
                db.InsertAll(_data);
            }
            public static void Update(SQLiteConnection db, ManufacturerDbTable _data)
            {
                db.Update(_data);
            }
            public static void Delete(SQLiteConnection db, ManufacturerDbTable _data)
            {
                db.Delete(_data);
            }
            public static void DeleteAndInsert(SQLiteConnection db, IEnumerable<ManufacturerDbTable> _data)
            {
                db.CreateTable<ManufacturerDbTable>();
                db.DeleteAll<ManufacturerDbTable>();
                db.InsertAll(_data);
            }
            public static void DeleteAll(SQLiteConnection db)
            {
                db.CreateTable<ManufacturerDbTable>();
                db.DeleteAll<ManufacturerDbTable>();
            }
        }

        public class Category
        {
            public static IEnumerable<CategoryDbTable> GetTableData(SQLiteConnection db)
            {
                db.CreateTable<CategoryDbTable>();
                return db.Query<CategoryDbTable>("select * from AssetCategory order by AssetCategoryName ASC");
            }
            public static IEnumerable<CategoryDbTable> GetCategoryWithParents(SQLiteConnection db, int parentID)
            {
                db.CreateTable<CategoryDbTable>();
                return db.Query<CategoryDbTable>("select * from AssetCategory where ParentID = ? order by AssetCategoryName ASC", parentID);
            }
            public static IEnumerable<CategoryDbTable> GetMyvalueByCategoryIDInternal(SQLiteConnection db, string AssetCategoryIDInternal)
            {
                db.CreateTable<CategoryDbTable>();
                return db.Query<CategoryDbTable>("select * from AssetCategory where AssetCategoryIDInternal = ? order by AssetCategoryName ASC ", AssetCategoryIDInternal);
            }
            public static IEnumerable<CategoryDbTable> GetMyvalueByCategoryID(SQLiteConnection db, int AssetCategoryID)
            {
                db.CreateTable<CategoryDbTable>();
                return db.Query<CategoryDbTable>("select * from AssetCategory where AssetCategoryID = ? order by AssetCategoryName ASC ", AssetCategoryID);
            }
            public static void Insert(SQLiteConnection db, CategoryDbTable _data)
            {
                db.Insert(_data);
            }
            public static void InsertAll(SQLiteConnection db, IEnumerable<CategoryDbTable> _data)
            {
                db.CreateTable<CategoryDbTable>();
                db.InsertAll(_data);
            }
            public static void Update(SQLiteConnection db, CategoryDbTable _data)
            {
                db.Update(_data);
            }
            public static void Delete(SQLiteConnection db, CategoryDbTable _data)
            {
                db.Delete(_data);
            }
            public static void DeleteAndInsert(SQLiteConnection db, IEnumerable<CategoryDbTable> _data)
            {
                db.CreateTable<CategoryDbTable>();
                db.DeleteAll<CategoryDbTable>();
                db.InsertAll(_data);
            }
            public static void DeleteAll(SQLiteConnection db)
            {
                db.CreateTable<CategoryDbTable>();
                db.DeleteAll<CategoryDbTable>();
            }
        }

        public class PropertyPassTable
        {
            public static IEnumerable<PropertyPassDbTable> GetTableData(SQLiteConnection db)
            {
                db.CreateTable<PropertyPassDbTable>();
                return db.Query<PropertyPassDbTable>("select * from PropertyPassTable where TerminationDate == '1900-01-01T00:00:00-05:00' or TerminationDate='' or TerminationDate is 'null' or TerminationDate is null ");
            }
            public static string UpdateSignature (SQLiteConnection db, string signature, string PropertyPassID)
            {
                try
                {
                    db.Query<PropertyPassDbTable>("update PropertyPassTable set SignaturePic=? where PropertyPassID_Internal=?", signature, PropertyPassID);
                    return "";
                }
                catch (Exception exe)
                {
                    DependencyService.Get<IError>().SendRaygunError(exe, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                    return exe.Message;
                }
                
            }
            public static PropertyPassDbTable GetTableDataByID(SQLiteConnection db, string PropertyPassID)
            {
                db.CreateTable<PropertyPassDbTable>();
                return db.Query<PropertyPassDbTable>("select * from PropertyPassTable where PropertyPassID_Internal = ?", PropertyPassID).First();
            }
            public static void UpdateTermination(SQLiteConnection db, string PropertyPassID, string Date)
            {
                db.Query<PropertyPassDbTable>("Update PropertyPassTable set TerminationDate=?, ModifiedOn=? where PropertyPassID_Internal=?", Date, Date, PropertyPassID);
            }
            public static IEnumerable<PropertyPassDbTable> GetUpdatedTableData(SQLiteConnection db, string ModifiedOn)
            {
                db.CreateTable<PropertyPassDbTable>();
                //ToDO convert Modified on to a database date type if the data is being stored in that format
                return db.Query<PropertyPassDbTable>("select * from PropertyPassTable where ModifiedOn > ? and Synced = 0", ModifiedOn);
            }
            public static IEnumerable<PropertyPassDbTable> GetCreatedTableData(SQLiteConnection db)
            {
                db.CreateTable<PropertyPassDbTable>();
                //ToDO convert Modified on to a database date type if the data is being stored in that format
                return db.Query<PropertyPassDbTable>("select * from PropertyPassTable where Synced = 1");
            }
            public static Stack<PropertyPassClass.PropertyPassTable> ConvertDbToStack(IEnumerable<PropertyPassDbTable> PropertyPassDbRecord)
            {
                var PropertyPassStack = new Stack<PropertyPassClass.PropertyPassTable>();
                foreach (PropertyPassDbTable PropertyPass in PropertyPassDbRecord)
                {
                    PropertyPassStack.Push(new PropertyPassClass.PropertyPassTable(PropertyPass));
                }
                return PropertyPassStack;
            }
            public static void Insert(SQLiteConnection db, PropertyPassDbTable _data)
            {
                db.Insert(_data);
            }
            public static void Update(SQLiteConnection db, PropertyPassDbTable _data)
            {
                db.Update(_data);
            }
            public static void Delete(SQLiteConnection db, PropertyPassDbTable _data)
            {
                db.Delete(_data);
            }
            public static void DeleteAndInsert(SQLiteConnection db, IEnumerable<PropertyPassDbTable> _data)
            {
                db.CreateTable<PropertyPassDbTable>();
                db.DeleteAll<PropertyPassDbTable>();
                db.InsertAll(_data);
            }
            public static void DeleteAll(SQLiteConnection db)
            {
                db.CreateTable<PropertyPassDbTable>();
                db.DeleteAll<PropertyPassDbTable>();
            }
        }

        public class PropertyPassItem
        {
            public static IEnumerable<PropertyPassItemDbTable> GetTableData(SQLiteConnection db)
            {
                db.CreateTable<PropertyPassItemDbTable>();
                return db.Query<PropertyPassItemDbTable>("select DISTINCT * from PropertyPassItemTable");
            }
            public static bool IsAllCheckedin(SQLiteConnection db, string PropertyPassID)
            {
                db.CreateTable<PropertyPassItemDbTable>();
                var returns = db.Query<PropertyPassItemDbTable>("select * from PropertyPassItemTable where PropertyPassID_Internal = ? and (CheckInDate='/Date(-2208970800000-0000)/' or CheckInDate='' or CheckInDate is 'null' or CheckInDate is null) ", PropertyPassID);
                if (returns.Count() > 0)
                    return false;
                else
                    return true;
            }
            public static IEnumerable<PropertyPassItemDbTable> GetTableDataByPropertyPassTable(SQLiteConnection db, string PropertyPassTableID)
            {
                db.CreateTable<PropertyPassItemDbTable>();
                return db.Query<PropertyPassItemDbTable>("select * from PropertyPassItemTable where PropertyPassID_Internal = ?", PropertyPassTableID);
            }
            public static IEnumerable<PropertyPassClass.PropertyPassDisplay> GetTableDetailsByPropertyPassTable(SQLiteConnection db, string PropertyPassTableID)
            {
                return db.Query<PropertyPassClass.PropertyPassDisplay>("select distinct PropertyPassItemTable.PropertyPassItemID_Internal, PropertyPassItemTable.PropertyPassID_Internal, PropertyPassItemTable.Assigned_By, Sites.SiteName, Assets.AssetJSONDb, Assets.AssetName, Assets.AssetIdInternal, Assets.Barcode, Locations.LocationName, PropertyPassTable.AssignedToEmail, PropertyPassTable.AssignedToPhone, PropertyPassTable.PeopleID_Internal from Assets inner Join Locations ON Assets.Asset_locationIdInternal = Locations.LocationIdInternal inner join Sites on Assets.Asset_SiteIdInternal = Sites.SiteIdInternal inner join PropertyPassItemTable on Assets.AssetIdInternal = PropertyPassItemTable.Asset_ID_Internal inner join PropertyPassTable on PropertyPassTable.PropertyPassID_Internal = PropertyPassItemTable.PropertyPassID_Internal where PropertyPassItemTable.PropertyPassID_Internal = ? and (CheckInDate = '1900-01-01T00:00:00-05:00' or CheckInDate = '' or CheckInDate is 'null' or CheckInDate is null)", PropertyPassTableID);
             }
            public static IEnumerable<PropertyPassItemDbTable> GetTableDataByAssetID(SQLiteConnection db, string Asset_ID_Internal_)
            {
                db.CreateTable<PropertyPassItemDbTable>();
                return db.Query<PropertyPassItemDbTable>("select * from PropertyPassItemTable where Asset_ID_Internal = ? and (CheckInDate = '1900-01-01T00:00:00-05:00' or CheckInDate = '' or CheckInDate is 'null' or CheckInDate is null)", Asset_ID_Internal_);
            }
            public static IEnumerable<PropertyPassItemDbTable> GetUpdatedTableData(SQLiteConnection db, string ModifiedOn)
            {
                db.CreateTable<PropertyPassItemDbTable>();
                //ToDO convert Modified on to a database date type if the data is being stored in that format
                return db.Query<PropertyPassItemDbTable>("select * from PropertyPassItemTable where ModifiedOn > ? and Synced = 0", ModifiedOn);
            }
            public static IEnumerable<PropertyPassItemDbTable> GetCreatedTableData(SQLiteConnection db)
            {
                db.CreateTable<PropertyPassItemDbTable>();
                //ToDO convert Modified on to a database date type if the data is being stored in that format
                return db.Query<PropertyPassItemDbTable>("select * from PropertyPassItemTable where Synced = 1");
            }
            public static Stack<PropertyPassClass.PropertyPassItem> ConvertDbToStack(IEnumerable<PropertyPassItemDbTable> PropertyPassItemDbRecord)
            {
                Stack<PropertyPassClass.PropertyPassItem> PropertyPassItemStack = new Stack<PropertyPassClass.PropertyPassItem>();
                foreach (PropertyPassItemDbTable PropertyPassItem in PropertyPassItemDbRecord)
                {
                    PropertyPassItemStack.Push(new PropertyPassClass.PropertyPassItem(PropertyPassItem));
                }
                return PropertyPassItemStack;
            }
            public static void InsertAll(SQLiteConnection db, IEnumerable<PropertyPassItemDbTable> _data)
            {
                db.CreateTable<PropertyPassItemDbTable>();
                db.InsertAll(_data);
            }
            public static void Update(SQLiteConnection db, PropertyPassClass.PropertyPassDisplay _data, string Date)
            {
                db.Query<PropertyPassItemDbTable>("Update PropertyPassItemTable set  CheckInDate=?, ModifiedOn=? where PropertyPassID_Internal=? and Asset_ID_Internal=?", Date, Date, _data.PropertyPassID_Internal, _data.AssetIdInternal);
            }
            public static void DeleteAndInsert(SQLiteConnection db, IEnumerable<PropertyPassItemDbTable> _data)
            {
                db.CreateTable<PropertyPassItemDbTable>();
                db.DeleteAll<PropertyPassItemDbTable>();
                db.InsertAll(_data);
            }
            public static void DeleteAll(SQLiteConnection db)
            {
                db.CreateTable<PropertyPassItemDbTable>();
                db.DeleteAll<PropertyPassItemDbTable>();
            }
        }

        public class Assets
        {
            public static IEnumerable<AssetDbClass> GetTableData(SQLiteConnection db)
            {
                db.CreateTable<AssetDbClass>();
                return db.Query<AssetDbClass>("select DISTINCT * from Assets");
            }
            public static IEnumerable<AssetDbClass> GetDuplicates(SQLiteConnection db)
            {
                db.CreateTable<AssetDbClass>();
                return db.Query<AssetDbClass>("Select *, Count(*) from Assets GROUP BY AssetIdInternal HAVING COUNT(*) > 1 ");
            }
            public static IEnumerable<AssetDbClass> GetTableDataByBarcode(SQLiteConnection db, string Barcode)
            {
                db.CreateTable<AssetDbClass>();
                return db.Query<AssetDbClass>("select DISTINCT * from Assets where Barcode = ?", Barcode);
            }
            public static IEnumerable<AssetDbClass> GetTableDataByBarcodeLike(SQLiteConnection db, string Barcode)
            {
                db.CreateTable<AssetDbClass>();
                return db.Query<AssetDbClass>("select DISTINCT * from Assets where Barcode like ?", "%" + Barcode + "%");
            }
            public static IEnumerable<AssetDisplayClass> GetDisplayeDataByBarcodeLike(SQLiteConnection db, string Barcode)
            {
                db.CreateTable<AssetDbClass>();
                var result = db.Query<AssetDisplayClass>("select DISTINCT Sites.SiteName, Assets.Asset_SublocationIdInternal, Assets.AssetJSONDb, Assets.AssetName, Assets.AssetIdInternal, Assets.Barcode, Locations.LocationName from Assets inner Join Locations ON Assets.Asset_locationIdInternal = Locations.LocationIdInternal inner join Sites on Assets.Asset_SiteIdInternal = Sites.SiteIdInternal where Barcode like ?", "%" + Barcode + "%");
                return result;
            }
            public static IEnumerable<AssetDisplayClass> GetDisplayeDataBySerailNumberLike(SQLiteConnection db, string Serial)
            {
                db.CreateTable<AssetDbClass>();
                var result = db.Query<AssetDisplayClass>("select DISTINCT Sites.SiteName, Assets.Asset_SublocationIdInternal, Assets.AssetJSONDb, Assets.AssetName, Assets.AssetIdInternal, Assets.Barcode, Locations.LocationName from Assets inner Join Locations ON Assets.Asset_locationIdInternal = Locations.LocationIdInternal inner join Sites on Assets.Asset_SiteIdInternal = Sites.SiteIdInternal where AssetSerialNumber like ?", "%" + Serial + "%");
                return result;
            }
            public static IEnumerable<AssetDisplayClass> GetDisplayeDataBySerailandPartNumberLike(SQLiteConnection db, string Serial, string PartNo)
            {
                db.CreateTable<AssetDbClass>();
                var result = db.Query<AssetDisplayClass>("select DISTINCT Sites.SiteName, Assets.Asset_SublocationIdInternal, Assets.AssetJSONDb, Assets.AssetName, Assets.AssetIdInternal, Assets.Barcode, Locations.LocationName from Assets inner Join Locations ON Assets.Asset_locationIdInternal = Locations.LocationIdInternal inner join Sites on Assets.Asset_SiteIdInternal = Sites.SiteIdInternal where AssetSerialNumber like ? and OriginalPartNo = ?", "%" + Serial + "%", "%" + PartNo + "%");
                return result;
            }

            public static IEnumerable<AssetDisplayClass> GetDisplayeDataByBarcode(SQLiteConnection db, string Barcode)
            {
                db.CreateTable<AssetDbClass>();
                var result = db.Query<AssetDisplayClass>("select DISTINCT Sites.SiteName, Assets.AssetJSONDb, Assets.AssetName, Assets.AssetIdInternal, Assets.Barcode, Locations.LocationName from Assets inner Join Locations ON Assets.Asset_locationIdInternal = Locations.LocationIdInternal inner join Sites on Assets.Asset_SiteIdInternal = Sites.SiteIdInternal where Barcode = ?", Barcode);
                return result;
            }
            public static IEnumerable<AssetDisplayClass> GetDisplayeDataByAssetIDInternal(SQLiteConnection db, string AssetID)
            {
                db.CreateTable<AssetDbClass>();
                var result = db.Query<AssetDisplayClass>("select DISTINCT Sites.SiteName, Assets.AssetJSONDb, Assets.AssetName, Assets.AssetIdInternal, Assets.Barcode, Locations.LocationName from Assets inner Join Locations ON Assets.Asset_locationIdInternal = Locations.LocationIdInternal inner join Sites on Assets.Asset_SiteIdInternal = Sites.SiteIdInternal where AssetIdInternal = ?", AssetID);
                return result;
            }

            public static IEnumerable<AssetDbClass> GetAssetDataByAssetIDInternal(SQLiteConnection db, string AssetIDInternal)
            {
                db.CreateTable<AssetDbClass>();
                return db.Query<AssetDbClass>("select DISTINCT * from Assets where AssetIdInternal = ?", AssetIDInternal);
            }
            public static IEnumerable<AssetDbClass> GetAssetDataByPeople(SQLiteConnection db, string PeopleIDInternal)
            {
                db.CreateTable<AssetDbClass>();
                return db.Query<AssetDbClass>("select DISTINCT * from Assets where Asset_PeopleIdInternal = ?", PeopleIDInternal);
            }
            public static IEnumerable<AssetDbClass> GetUpdatedAssets(SQLiteConnection db)
            {
                db.CreateTable<AssetDbClass>();
                //and changesMade is not equal to [] and date modified > last syncdate
                //return db.Query<AssetDbClass>("select DISTINCT * from Assets where Synced = 1 and AssetChangesDb IS NOT NULL  and AssetChangesDb != NULL");
                return db.Query<AssetDbClass>("select DISTINCT * from Assets where Synced = 1 and AssetChangesDb != 'null'");
            }
            public static IEnumerable<AssetDbClass> GetInsertedAssets(SQLiteConnection db)
            {
                db.CreateTable<AssetDbClass>();
                return db.Query<AssetDbClass>("select DISTINCT * from Assets where Synced = 0");
            }
            public static IEnumerable<AssetDbClass> GetAssetListForSearch(SQLiteConnection db, LocationClass Location, SublocationClass sublocation, DepartmentClass department)
            {
                db.CreateTable<AssetDbClass>();
                if (sublocation != null)
                {
                    if (department!= null)
                    {
                        //sublocation and department
                        return db.Query<AssetDbClass>("select DISTINCT * from Assets where Asset_SiteIdInternal = ? and Asset_locationIdInternal = ? and Asset_SublocationIdInternal= ? and Asset_Department = ?", Location.SiteIdInternal, Location.LocationIdInternal, sublocation.SubLocationName, department.ID);
                    }
                    else
                    {
                        //sublocation no department
                        return db.Query<AssetDbClass>("select DISTINCT * from Assets where Asset_SiteIdInternal = ? and Asset_locationIdInternal = ? and Asset_SublocationIdInternal= ?", Location.SiteIdInternal, Location.LocationIdInternal, sublocation.SubLocationName);
                    }
                }
                else
                {
                    if (department != null)
                    {
                        //no sublocation and department
                        return db.Query<AssetDbClass>("select DISTINCT * from Assets where Asset_SiteIdInternal = ? and Asset_locationIdInternal = ? and Asset_Department = ?", Location.SiteIdInternal, Location.LocationIdInternal, department.ID);
                    }
                    else
                    {
                        //no sublocation, no department
                        return db.Query<AssetDbClass>("select DISTINCT * from Assets where Asset_SiteIdInternal = ? and Asset_locationIdInternal = ?", Location.SiteIdInternal, Location.LocationIdInternal);
                    }
                }
            }
            public static IEnumerable<AssetDbClass> GetAssetListForSearch(SQLiteConnection db,string siteID, string Location, string sublocation, int department)
            {
                db.CreateTable<AssetDbClass>();
                if (sublocation != null)
                {
                    if (department != 0)
                    {
                        //sublocation and department
                        return db.Query<AssetDbClass>("select DISTINCT * from Assets where Asset_SiteIdInternal = ? and Asset_locationIdInternal = ? and Asset_SublocationIdInternal= ? and Asset_Department = ?", siteID, Location, sublocation, department);
                    }
                    else
                    {
                        //sublocation no department
                        return db.Query<AssetDbClass>("select DISTINCT * from Assets where Asset_SiteIdInternal = ? and Asset_locationIdInternal = ? and Asset_SublocationIdInternal= ?", siteID, Location, sublocation);
                    }
                }
                else
                {
                    if (department != 0)
                    {
                        //no sublocation and department
                        return db.Query<AssetDbClass>("select DISTINCT * from Assets where Asset_SiteIdInternal = ? and Asset_locationIdInternal = ? and Asset_Department = ?", siteID, Location, department);
                    }
                    else
                    {
                        //no sublocation, no department
                        return db.Query<AssetDbClass>("select DISTINCT * from Assets where Asset_SiteIdInternal = ? and Asset_locationIdInternal = ?", siteID, Location);
                    }
                }
            }



            public static IEnumerable<LocationsDbTable> GetTableDataFromSiteLocation(SQLiteConnection db, string siteID, string LocationId)
            {
                db.CreateTable<LocationsDbTable>();
                return db.Query<LocationsDbTable>("select DISTINCT * from Assets where Asset_SiteIdInternal = ? and Asset_locationIdInternal = ?", siteID, LocationId);
            }
            public static void Deletewhere(SQLiteConnection db, string ID)
            {
                db.CreateTable<AssetDbClass>();
                db.Query<AssetDbClass>("Delete from Assets where AssetIdInternal = ?", ID);
            }
            public static void Insert(SQLiteConnection db, AssetDbClass _data)
            {
                db.Insert(_data);
            }
            public static void Update(SQLiteConnection db, AssetDbClass _data)
            {
                db.Query<AssetDbClass>("Update Assets set AssetIdInternal = ?, Asset_SiteIdInternal = ?, Asset_locationIdInternal= ?,Asset_SublocationIdInternal = ?, Asset_Department = ?, Asset_PeopleIdInternal= ?, AssetName = ?, Barcode = ?, AssetSerialNumber = ?, AssetChangesDb = ?, AssetJSONDb = ?, Synced = ?, OriginalPartNo = ? where AssetIdInternal = ?",
                    _data.AssetIdInternal, _data.Asset_SiteIdInternal, _data.Asset_locationIdInternal, _data.Asset_SublocationIdInternal, _data.Asset_Department, _data.Asset_PeopleIdInternal, _data.AssetName, _data.Barcode, _data.AssetSerialNumber, _data.AssetChangesDb, _data.AssetJSONDb, _data.Synced, _data.OriginalPartNo, _data.AssetIdInternal);
                //db.Update(_data);
            }
            public static void UpdateSynced(SQLiteConnection db, AssetJsonObject _data)
            {
                db.Query<AssetDbClass>("Update Assets set Synced = 1 where AssetIdInternal = ?",_data.AssetIDInternal);
                //db.Update(_data);
            }
            public static void DeleteUsingAssetID(SQLiteConnection db, string _data)
            {
                db.Query<AssetDbClass>("DELETE from Assets where AssetIdInternal = ? ", _data);
            }
            
            public static void DeleteAndInsert(SQLiteConnection db, IEnumerable<AssetDbClass> _data)
            {
                db.CreateTable<AssetDbClass>();
                db.DeleteAll<AssetDbClass>();
                db.InsertAll(_data);
            }
            public static void DeleteAll(SQLiteConnection db)
            {
                db.CreateTable<AssetDbClass>();
                db.DeleteAll<AssetDbClass>();
            }
            public static void UpdateAll(SQLiteConnection db, IEnumerable<AssetDbClass> updates)
            {
                db.CreateTable<AssetDbClass>();
                db.UpdateAll(updates);
            }
            public static Stack<AssetClass> ConvertDbToStack(IEnumerable<AssetDbClass> AssetDbRecord)
            {
                Stack<AssetClass> UpdateAssetsStack = new Stack<AssetClass>();
                foreach (AssetDbClass AssetfromDatabase in AssetDbRecord)
                {
                    var AssetJSONToStore = JsonConvert.DeserializeObject<AssetJsonObject>(AssetfromDatabase.AssetJSONDb);
                    var AssetChanges = JsonConvert.DeserializeObject<List<Changesmade>>(AssetfromDatabase.AssetChangesDb);
                    UpdateAssetsStack.Push(new AssetClass(AssetfromDatabase, AssetJSONToStore, AssetChanges));
                }
                return UpdateAssetsStack;
            }
            public static Stack<AssetJsonObject> ConvertDbToStackInsert(IEnumerable<AssetDbClass> AssetDbRecord)
            {
                Stack<AssetJsonObject> InsertAssetsStack = new Stack<AssetJsonObject>();
                foreach (AssetDbClass AssetfromDatabase in AssetDbRecord)
                {
                    var intermediate = JsonConvert.DeserializeObject<AssetJsonObject>(AssetfromDatabase.AssetJSONDb);
                    intermediate.AssetDescription = intermediate.AssetDescription ?? "";
                    intermediate.AssetSerialNo = intermediate.AssetSerialNo ?? "";
                    intermediate.Asset_UID = intermediate.Asset_UID ?? "";
                    intermediate.AuditStatus = intermediate.AuditStatus ?? "";
                    intermediate.Mfg = intermediate.Mfg ?? "";
                    intermediate.Model = intermediate.Model ?? "";
                    intermediate.OriginalPartNo = intermediate.OriginalPartNo ?? "";
                    intermediate.POStatus = intermediate.POStatus ?? "N";
                    intermediate.PeopleIDInternal = intermediate.PeopleIDInternal ?? "00000000-0000-0000-0000-000000000000";
                    intermediate.PurchaseOrderNo = intermediate.PurchaseOrderNo ?? "";
                    intermediate.SubLocation = intermediate.PurchaseOrderNo ?? "";
                    intermediate.ThumbnailImage = intermediate.ThumbnailImage ?? "";
                    intermediate.Vendor = intermediate.Vendor ?? "";
                    intermediate.IsActive = true;
                    InsertAssetsStack.Push(intermediate);
                }
                return InsertAssetsStack;
            }
            public static AssetClass Compare(AssetDbClass CompareAssetDB, AssetJsonObject newAssetJSON)
            {
                //Compare JSON is the new data value that we want
                var newrecord = new AssetClass(newAssetJSON)
                {
                    AssetIdInternal = CompareAssetDB.AssetIdInternal,
                    Synced = CompareAssetDB.Synced
                };
                var old_AssetJSON = JsonConvert.DeserializeObject<AssetJsonObject>(CompareAssetDB.AssetJSONDb);
                newrecord.ChangesMade = JsonConvert.DeserializeObject<List<Changesmade>>(CompareAssetDB.AssetChangesDb);
                bool found = false;

                //TODO - change this so that it updates with the ccorrect data
                if (newrecord.ChangesMade == null)
                    newrecord.ChangesMade = new List<Changesmade>();
                //checks asset name and adds to changesmade
                if (old_AssetJSON.AssetName != newAssetJSON.AssetName)
                {
                    foreach(Changesmade change in newrecord.ChangesMade)
                    {
                        if (change.FieldName == "AssetName")
                        {
                            found = true;
                            change.FieldValue = newAssetJSON.AssetName;
                            break;
                        }
                            
                    };
                    if (!found)
                    {
                        newrecord.ChangesMade.Add(new Changesmade
                        {
                            FieldName = "AssetName",
                            FieldValue = newAssetJSON.AssetName
                        });
                    }
                    found = false;
                }

                //checks asset category and adds to changes made
                if ((old_AssetJSON.AssetCategoryIDInternal != newAssetJSON.AssetCategoryIDInternal) && newAssetJSON.AssetCategoryIDInternal != "Select Asset Category")
                {
                    foreach (Changesmade change in newrecord.ChangesMade)
                    {
                        if (change.FieldName == "AssetCategoryID_Internal")
                        {
                            found = true;
                            change.FieldValue = newAssetJSON.AssetCategoryIDInternal;
                            break;
                        }
                    };
                    if (!found)
                    {
                        newrecord.ChangesMade.Add(new Changesmade
                        {
                            FieldName = "AssetCategoryID_Internal",
                            FieldValue = newAssetJSON.AssetCategoryIDInternal
                        });
                    }
                    found = false;
                }

                //checks asset condition and adds to changes made
                if (old_AssetJSON.AssetConditionID != newAssetJSON.AssetConditionID)
                {
                    foreach (Changesmade change in newrecord.ChangesMade)
                    {
                        if (change.FieldName == "AssetConditionID")
                        {
                            found = true;
                            change.FieldValue = Convert.ToString(newAssetJSON.AssetConditionID);
                            break;
                        }
                    };
                    if (!found)
                    {
                        newrecord.ChangesMade.Add(new Changesmade
                        {
                            FieldName = "AssetConditionID",
                            FieldValue = Convert.ToString(newAssetJSON.AssetConditionID)
                        });
                    }
                    found = false;
                }

                //asset description
                if (old_AssetJSON.AssetDescription != newAssetJSON.AssetDescription)
                {
                    foreach (Changesmade change in newrecord.ChangesMade)
                    {
                        if (change.FieldName == "AssetDescription")
                        {
                            found = true;
                            change.FieldValue = newAssetJSON.AssetDescription;
                            break;
                        }
                    };
                    if (!found)
                    {
                        newrecord.ChangesMade.Add(new Changesmade
                        {
                            FieldName = "AssetDescription",
                            FieldValue = newAssetJSON.AssetDescription
                        });
                    }
                    found = false;
                }

                //Asset serial No
                if (old_AssetJSON.AssetSerialNo != newAssetJSON.AssetSerialNo)
                {
                    foreach (Changesmade change in newrecord.ChangesMade)
                    {
                        if (change.FieldName == "AssetSerialNo")
                        {
                            found = true;
                            change.FieldValue = newAssetJSON.AssetSerialNo;
                            break;
                        }
                    };
                    if (!found)
                    {
                        newrecord.ChangesMade.Add(new Changesmade
                        {
                            FieldName = "AssetSerialNo",
                            FieldValue = newAssetJSON.AssetSerialNo
                        });
                    }
                    found = false;
                }

                //Asset status
                if (old_AssetJSON.AssetStatus != newAssetJSON.AssetStatus)
                {
                    foreach (Changesmade change in newrecord.ChangesMade)
                    {
                        if (change.FieldName == "AssetStatus")
                        {
                            found = true;
                            change.FieldValue = newAssetJSON.AssetStatus;
                            break;
                        }
                    };
                    if (!found)
                    {
                        newrecord.ChangesMade.Add(new Changesmade
                        {
                            FieldName = "AssetStatus",
                            FieldValue = newAssetJSON.AssetStatus
                        });
                    }
                    found = false;
                }

                //assetUsage ID
                if (old_AssetJSON.AssetUsageID != newAssetJSON.AssetUsageID)
                {
                    foreach (Changesmade change in newrecord.ChangesMade)
                    {
                        if (change.FieldName == "AssetUsageID")
                        {
                            found = true;
                            change.FieldValue = Convert.ToString(newAssetJSON.AssetUsageID);
                            break;
                        }
                    };
                    if (!found)
                    {
                        newrecord.ChangesMade.Add(new Changesmade
                        {
                            FieldName = "AssetUsageID",
                            FieldValue = Convert.ToString(newAssetJSON.AssetUsageID)
                        });
                    }
                    found = false;
                }

                //AssetUID
                if (old_AssetJSON.Asset_UID != newAssetJSON.Asset_UID)
                {
                    foreach (Changesmade change in newrecord.ChangesMade)
                    {
                        if (change.FieldName == "Asset_UID")
                        {
                            found = true;
                            change.FieldValue = Convert.ToString(newAssetJSON.Asset_UID);
                            break;
                        }
                    };
                    if (!found)
                    {
                        newrecord.ChangesMade.Add(new Changesmade
                        {
                            FieldName = "Asset_UID",
                            FieldValue = Convert.ToString(newAssetJSON.Asset_UID)
                        });
                    }
                    found = false;
                }

                //Audit date
                if (old_AssetJSON.AuditDate != newAssetJSON.AuditDate)
                {
                    foreach (Changesmade change in newrecord.ChangesMade)
                    {
                        if (change.FieldName == "AuditDate")
                        {
                            found = true;
                            change.FieldValue = newAssetJSON.AuditDate.ToString();
                            break;
                        }
                    };
                    if (!found)
                    {
                        newrecord.ChangesMade.Add(new Changesmade
                        {
                            FieldName = "AuditDate",
                            FieldValue = newAssetJSON.AuditDate.ToString()
                        });
                    }
                    found = false;
                }

                //AuditStatus
                if (old_AssetJSON.AuditStatus != newAssetJSON.AuditStatus)
                {
                    foreach (Changesmade change in newrecord.ChangesMade)
                    {
                        if (change.FieldName == "AuditStatus")
                        {
                            found = true;
                            change.FieldValue = newAssetJSON.AuditStatus;
                            break;
                        }
                    };
                    if (!found)
                    {
                        newrecord.ChangesMade.Add(new Changesmade
                        {
                            FieldName = "AuditStatus",
                            FieldValue = newAssetJSON.AuditStatus
                        });
                    }
                    found = false;
                }

                //Barcode
                if (old_AssetJSON.Barcode != newAssetJSON.Barcode)
                {
                    foreach (Changesmade change in newrecord.ChangesMade)
                    {
                        if (change.FieldName == "Barcode")
                        {
                            found = true;
                            change.FieldValue = newAssetJSON.Barcode;
                            break;
                        }
                    };
                    if (!found)
                    {
                        newrecord.ChangesMade.Add(new Changesmade
                        {
                            FieldName = "Barcode",
                            FieldValue = newAssetJSON.Barcode
                        });
                    }
                    found = false;
                }

                //DataGatherID
                if (old_AssetJSON.DataGatherID != newAssetJSON.DataGatherID)
                {
                    foreach (Changesmade change in newrecord.ChangesMade)
                    {
                        if (change.FieldName == "DataGatherID")
                        {
                            found = true;
                            change.FieldValue = newAssetJSON.DataGatherID.ToString();
                            break;
                        }
                    };
                    if (!found)
                    {
                        newrecord.ChangesMade.Add(new Changesmade
                        {
                            FieldName = "DataGatherID",
                            FieldValue = newAssetJSON.DataGatherID.ToString()
                        });
                    }
                    found = false;
                }

                //location ID Internal
                if (old_AssetJSON.LocationIDInternal != newAssetJSON.LocationIDInternal)
                {
                    foreach (Changesmade change in newrecord.ChangesMade)
                    {
                        if (change.FieldName == "Location_ID_Internal")
                        {
                            found = true;
                            change.FieldValue = newAssetJSON.LocationIDInternal;
                            break;
                        }
                    };
                    if (!found)
                    {
                        newrecord.ChangesMade.Add(new Changesmade
                        {
                            FieldName = "Location_ID_Internal",
                            FieldValue = newAssetJSON.LocationIDInternal
                        });
                    }
                    found = false;
                }

                //Manufacturer
                if (old_AssetJSON.Mfg != newAssetJSON.Mfg && (newAssetJSON.Mfg!= null))
                {
                    foreach (Changesmade change in newrecord.ChangesMade)
                    {
                        if (change.FieldName == "Mfg")
                        {
                            found = true;
                            change.FieldValue = newAssetJSON.Mfg;
                            break;
                        }
                    };
                    if (!found)
                    {
                        newrecord.ChangesMade.Add(new Changesmade
                        {
                            FieldName = "Mfg",
                            FieldValue = newAssetJSON.Mfg
                        });
                    }
                    found = false;
                }

                //Model
                if (old_AssetJSON.Model != newAssetJSON.Model)
                {
                    foreach (Changesmade change in newrecord.ChangesMade)
                    {
                        if (change.FieldName == "Model")
                        {
                            found = true;
                            change.FieldValue = newAssetJSON.Model;
                            break;
                        }
                    };
                    if (!found)
                    {
                        newrecord.ChangesMade.Add(new Changesmade
                        {
                            FieldName = "Model",
                            FieldValue = newAssetJSON.Model
                        });
                    }
                    found = false;
                }

                //original part No
                if (old_AssetJSON.OriginalPartNo != newAssetJSON.OriginalPartNo)
                {
                    foreach (Changesmade change in newrecord.ChangesMade)
                    {
                        if (change.FieldName == "OriginalPartNo")
                        {
                            found = true;
                            change.FieldValue = newAssetJSON.OriginalPartNo;
                            break;
                        }
                    };
                    if (!found)
                    {
                        newrecord.ChangesMade.Add(new Changesmade
                        {
                            FieldName = "OriginalPartNo",
                            FieldValue = newAssetJSON.OriginalPartNo
                        });
                    }
                    found = false;
                }

                //POLine
                if (old_AssetJSON.POLine != newAssetJSON.POLine)
                {
                    foreach (Changesmade change in newrecord.ChangesMade)
                    {
                        if (change.FieldName == "POLine")
                        {
                            found = true;
                            change.FieldValue = newAssetJSON.POLine.ToString();
                            break;
                        }
                    };
                    if (!found)
                    {
                        newrecord.ChangesMade.Add(new Changesmade
                        {
                            FieldName = "POLine",
                            FieldValue = newAssetJSON.POLine.ToString()
                        });
                    }
                    found = false;
                }

                //POStatus

                if (old_AssetJSON.POStatus != newAssetJSON.POStatus)
                {
                    foreach (Changesmade change in newrecord.ChangesMade)
                    {
                        if (change.FieldName == "POStatus")
                        {
                            found = true;
                            change.FieldValue = newAssetJSON.POStatus;
                            break;
                        }
                    };
                    if (!found)
                    {
                        newrecord.ChangesMade.Add(new Changesmade
                        {
                            FieldName = "POStatus",
                            FieldValue = newAssetJSON.POStatus
                        });
                    }
                    found = false;
                }


                //POStatus
                if (old_AssetJSON.PeopleIDInternal != newAssetJSON.PeopleIDInternal)
                {
                    foreach (Changesmade change in newrecord.ChangesMade)
                    {
                        if (change.FieldName == "People_ID_Internal")
                        {
                            found = true;
                            change.FieldValue = newAssetJSON.PeopleIDInternal;
                            break;
                        }
                    };
                    if (!found)
                    {
                        newrecord.ChangesMade.Add(new Changesmade
                        {
                            FieldName = "People_ID_Internal",
                            FieldValue = newAssetJSON.PeopleIDInternal
                        });
                    }
                    found = false;
                }

                //Price
                if (old_AssetJSON.Price != newAssetJSON.Price)
                {
                    foreach (Changesmade change in newrecord.ChangesMade)
                    {
                        if (change.FieldName == "Price")
                        {
                            found = true;
                            change.FieldValue = newAssetJSON.Price.ToString();
                            break;
                        }
                    };
                    if (!found)
                    {
                        newrecord.ChangesMade.Add(new Changesmade
                        {
                            FieldName = "Price",
                            FieldValue = newAssetJSON.Price.ToString()
                        });
                    }
                    found = false;
                }

                //purchase date
                if (old_AssetJSON.PurchaseDate != newAssetJSON.PurchaseDate)
                {
                    foreach (Changesmade change in newrecord.ChangesMade)
                    {
                        if (change.FieldName == "PurchaseDate")
                        {
                            found = true;
                            change.FieldValue = newAssetJSON.PurchaseDate.ToString();
                            break;
                        }
                    };
                    if (!found)
                    {
                        newrecord.ChangesMade.Add(new Changesmade
                        {
                            FieldName = "PurchaseDate",
                            FieldValue = newAssetJSON.PurchaseDate.ToString()
                        });
                    }
                    found = false;
                }

                //purhase order No
                if (old_AssetJSON.PurchaseOrderNo != newAssetJSON.PurchaseOrderNo)
                {
                    foreach (Changesmade change in newrecord.ChangesMade)
                    {
                        if (change.FieldName == "PurchaseOrderNo")
                        {
                            found = true;
                            change.FieldValue = newAssetJSON.PurchaseOrderNo;
                            break;
                        }
                    };
                    if (!found)
                    {
                        newrecord.ChangesMade.Add(new Changesmade
                        {
                            FieldName = "PurchaseOrderNo",
                            FieldValue = newAssetJSON.PurchaseOrderNo
                        });
                    }
                    found = false;
                }

                //Quantity
                if (old_AssetJSON.Quantity != newAssetJSON.Quantity)
                {
                    foreach (Changesmade change in newrecord.ChangesMade)
                    {
                        if (change.FieldName == "Quantity")
                        {
                            found = true;
                            change.FieldValue = newAssetJSON.Quantity.ToString();
                            break;
                        }
                    };
                    if (!found)
                    {
                        newrecord.ChangesMade.Add(new Changesmade
                        {
                            FieldName = "Quantity",
                            FieldValue = newAssetJSON.Quantity.ToString()
                        });
                    }
                    found = false;
                }

                //ShortageOverage
                if (old_AssetJSON.ShortageOverage != newAssetJSON.ShortageOverage)
                {
                    foreach (Changesmade change in newrecord.ChangesMade)
                    {
                        if (change.FieldName == "ShortageOverage")
                        {
                            found = true;
                            change.FieldValue = newAssetJSON.ShortageOverage.ToString();
                            break;
                        }
                    };
                    if (!found)
                    {
                        newrecord.ChangesMade.Add(new Changesmade
                        {
                            FieldName = "ShortageOverage",
                            FieldValue = newAssetJSON.ShortageOverage.ToString()
                        });
                    }
                    found = false;
                }

                //Site ID Internal
                if (old_AssetJSON.SiteIDInternal != newAssetJSON.SiteIDInternal)
                {
                    foreach (Changesmade change in newrecord.ChangesMade)
                    {
                        if (change.FieldName == "SiteID_Internal")
                        {
                            found = true;
                            change.FieldValue = newAssetJSON.SiteIDInternal;
                            break;
                        }
                    };
                    if (!found)
                    {
                        newrecord.ChangesMade.Add(new Changesmade
                        {
                            FieldName = "SiteID_Internal",
                            FieldValue = newAssetJSON.SiteIDInternal
                        });
                    }
                    found = false;
                }

                //Sublocation
                if (old_AssetJSON.SubLocation != newAssetJSON.SubLocation && (newAssetJSON.SubLocation!= null))
                {
                    foreach (Changesmade change in newrecord.ChangesMade)
                    {
                        if (change.FieldName == "SubLocation")
                        {
                            found = true;
                            change.FieldValue = newAssetJSON.SubLocation;
                            break;
                        }
                    };
                    if (!found)
                    {
                        newrecord.ChangesMade.Add(new Changesmade
                        {
                            FieldName = "SubLocation",
                            FieldValue = newAssetJSON.SubLocation
                        });
                    }
                    found = false;
                }

                //Thumbnail
                if (old_AssetJSON.ThumbnailImage != newAssetJSON.ThumbnailImage)
                {
                    foreach (Changesmade change in newrecord.ChangesMade)
                    {
                        if (change.FieldName == "ThumbNailImage")
                        {
                            found = true;
                            change.FieldValue = newAssetJSON.ThumbnailImage;
                            break;
                        }
                    };
                    if (!found)
                    {
                        newrecord.ChangesMade.Add(new Changesmade
                        {
                            FieldName = "ThumbNailImage",
                            FieldValue = newAssetJSON.ThumbnailImage
                        });
                    }
                    found = false;
                }

                //Vendor
                if (old_AssetJSON.Vendor != newAssetJSON.Vendor && (newAssetJSON.Vendor != null))
                {
                    foreach (Changesmade change in newrecord.ChangesMade)
                    {
                        if (change.FieldName == "Vendor")
                        {
                            found = true;
                            change.FieldValue = newAssetJSON.Vendor;
                            break;
                        }
                    };
                    if (!found)
                    {
                        newrecord.ChangesMade.Add(new Changesmade
                        {
                            FieldName = "Vendor",
                            FieldValue = newAssetJSON.Vendor
                        });
                    }
                    found = false;
                }


                return newrecord;
            }
        }

        public class Transfers
        {
            public static IEnumerable<TransferDbTable> GetTableData(SQLiteConnection db)
            {
                db.CreateTable<TransferDbTable>();
                return db.Query<TransferDbTable>("select * from Transfers");
            }

            public static IEnumerable<TransferDbTable> GetTableDataByAssetID(SQLiteConnection db, string AssetIDInternal)
            {
                db.CreateTable<TransferDbTable>();
                return db.Query<TransferDbTable>("select * from Transfers where AssetIdInternal = ? ", AssetIDInternal);
            }
            public static Stack<TransferClass> ConvertDbToStack(IEnumerable<TransferDbTable> TransferDbRecord)
            {
                Stack<TransferClass> TransferAssetsStack = new Stack<TransferClass>();
                foreach (TransferDbTable TransferfromDatabase in TransferDbRecord)
                {
                    TransferAssetsStack.Push(new TransferClass(TransferfromDatabase));
                }
                return TransferAssetsStack;
            }
            public static void Insert(SQLiteConnection db, TransferDbTable _data)
            {
                db.Insert(_data);
            }
            public static void InsertAll(SQLiteConnection db, IEnumerable<TransferDbTable> _data)
            {
                db.CreateTable<TransferDbTable>();
                db.InsertAll(_data);
            }
            public static void Update(SQLiteConnection db, TransferDbTable _data)
            {
                db.Update(_data);
            }
            public static void Delete(SQLiteConnection db, TransferDbTable _data)
            {
                db.Delete(_data);
            }
            public static void DeleteWhere(SQLiteConnection db, string AssetIDInternal)
            {
                db.Query<TransferDbTable>("Delete from Transfers where AssetIdInternal = ? ", AssetIDInternal);
            }
            public static void DeleteAndInsert(SQLiteConnection db, IEnumerable<TransferDbTable> _data)
            {
                db.CreateTable<TransferDbTable>();
                db.DeleteAll<TransferDbTable>();
                db.InsertAll(_data);
            }
            public static void DeleteAll(SQLiteConnection db)
            {
                db.CreateTable<TransferDbTable>();
                db.DeleteAll<TransferDbTable>();
            }
        }

        public class Deletes
        {
            public static IEnumerable<DeleteDbTable> GetTableData(SQLiteConnection db)
            {
                db.CreateTable<DeleteDbTable>();
                return db.Query<DeleteDbTable>("select * from DeletedRecords");
            }
            public static Stack<DeletedAssetsClass> ConvertDbToStack(IEnumerable<DeleteDbTable> DeletedDbRecord)
            {
                Stack<DeletedAssetsClass> DeletedAssetsStack = new Stack<DeletedAssetsClass>();
                foreach (DeleteDbTable DeletedAsset in DeletedDbRecord)
                {
                    DeletedAssetsStack.Push(new DeletedAssetsClass(DeletedAsset));
                }
                return DeletedAssetsStack;
            }
            public static void Insert(SQLiteConnection db, DeleteDbTable _data)
            {
                db.CreateTable<DeleteDbTable>();
                db.Insert(_data);
            }
            public static void InsertAll(SQLiteConnection db, IEnumerable<DeleteDbTable> _data)
            {
                db.CreateTable<DeleteDbTable>();
                db.InsertAll(_data);
            }
            public static void Update(SQLiteConnection db, DeleteDbTable _data)
            {
                db.CreateTable<DeleteDbTable>();
                db.Update(_data);
            }
            public static void Delete(SQLiteConnection db, DeleteDbTable _data)
            {
                db.CreateTable<DeleteDbTable>();
                db.Delete(_data);
            }
            public static void DeleteWhere(SQLiteConnection db, string _data)
            {
                db.CreateTable<DeleteDbTable>();
                db.Query<DeleteDbTable>("delete from DeletedRecords where IdInternal = ?", _data);
            }
            public static void DeleteAndInsert(SQLiteConnection db, IEnumerable<DeleteDbTable> _data)
            {
                db.CreateTable<DeleteDbTable>();
                db.DeleteAll<DeleteDbTable>();
                db.InsertAll(_data);
            }
            public static void DeleteAll(SQLiteConnection db)
            {
                db.CreateTable<DeleteDbTable>();
                db.DeleteAll<DeleteDbTable>();
            }
            public static void DeleteRecords(SQLiteConnection db, List<DeletedAssetsClass> DeletedAssetsList)
            {
                foreach(DeletedAssetsClass DeletedRecord in DeletedAssetsList)
                {
                    try
                    {
                        switch (DeletedRecord.Table)
                        {
                            case "Assets":
                                Assets.Deletewhere(db, DeletedRecord.ID);
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception exc) {
                        DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                    }  
                }
            }
        }

       
        public class Settings
        {
            public static SettingsDb GetTableData(SQLiteConnection db)
            {
                db.CreateTable<SettingsDb>();
                var test = db.Query<SettingsDb>("select * from Settings");
                if (test.Count > 0)
                    return test.First();
                else
                    return new SettingsDb { FastAudit = true, BlindAudit = false, PriceLock = true}; //default values
            }
            public static void UpdateData(SQLiteConnection db, SettingsDb settings)
            {
                db.CreateTable<SettingsDb>();
                var test = db.Query<SettingsDb>("select * from Settings");
                if (test.Count > 0)
                    db.Query<SettingsDb>("Update Settings set FastAudit = ?, BlindAudit = ?, PriceLock= ?, FrontCamera= ?, FastAuditEntry = ?", settings.FastAudit, settings.BlindAudit, settings.PriceLock, settings.FrontCamera, settings.FastAuditEntry);
                else
                    db.Insert(settings);
            }
           
            public static void DeleteAll(SQLiteConnection db)
            {
                db.CreateTable<SettingsDb>();
                db.DeleteAll<SettingsDb>();
            }
        }
    }
}
