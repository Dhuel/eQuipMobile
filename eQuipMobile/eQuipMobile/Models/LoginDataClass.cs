using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace eQuipMobile
{
    
    public class LoginDataClass
    {
        public DateTime ServerTime { get; set; }
        public string UserID { get; set; }
        public int RoleID { get; set; }
        public List<MobileSite> MobileSites { get; set; }
        public bool AllowDuplicateBarcode { get; set; }
        public Permissions Permissions { get; set; }
        public string Framework { get; set; }
        public string eQuipVersion { get; set; }
        public List<string> RequiredFields { get; set; }
        public string GetMobileSites()
        {
            return JsonConvert.SerializeObject(MobileSites);
        }

        //TODO - Write code to get permissions
        public string GetPermissions()
        {
            return "";
        }
    }
    public class MobileSite
    {
        public string SiteCode { get; set; }
        public string SiteDescription { get; set; }
        public string SiteIDInternal { get; set; }
        public string SiteName { get; set; }
    }

    public class Permissions
    {
        public bool DeleteAsset { get; set; }
        public bool AddEditAssets { get; set; }
        public bool AddEditDeleteFloors { get; set; }
        public bool RelocateAssets { get; set; }
        public bool AddEditDeletePermissions { get; set; }
        public bool AddEditDeleteReports { get; set; }
        public bool FromDesign { get; set; }
        public bool EditFloorplanImages { get; set; }
        public bool ChangeOptions { get; set; }
        public bool ChangePerferences { get; set; }
        public bool ModifyValueLists { get; set; }
        public bool AddEditDeleteCategories { get; set; }
        public bool OpenProjects { get; set; }
        public bool CreateProjects { get; set; }
        public bool WorkstationAuditEmployeeImport { get; set; }
        public bool BarcodeScanningPrinting { get; set; }
        public bool AssetCheckOut { get; set; }
        public bool ScannerAdmin { get; set; }
        public bool MoveLocations { get; set; }
        public bool AddEditLocations { get; set; }
        public bool DeleteLocations { get; set; }
        public bool AddEditPeople { get; set; }
        public bool DeletePeople { get; set; }
        public bool ImportData { get; set; }
        public bool CreateWorkOrder { get; set; }
        public bool ApproveWorkOrder { get; set; }
        public bool DeleteWorkOrder { get; set; }
        public bool ExportWorkOrder { get; set; }
        public bool CreateMessage { get; set; }
        public bool ViewAllSitesReadOnly { get; set; }
        public bool ViewAdminMenu { get; set; }
        public bool Prohibitchangeofassetsbyotherusers { get; set; }
        public bool ViewUnApprovedAssets { get; set; }
        public bool ViewRemoteSites { get; set; }
        public bool ViewReportsMenu { get; set; }
        public bool ViewAssetReservation { get; set; }
    }

    public class ResponseStatus
    {
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public List<object> Errors { get; set; }
    }

    public class LoginResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class LoginException : Exception
    {
        public LoginException(string message)
           : base(message)
        {
            
        }
    }

}


