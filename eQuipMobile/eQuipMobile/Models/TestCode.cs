using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eQuipMobile
{
    public static class TestCode
    {
        
        public static string testAssetupdate(string _userID, string _url)
        {
            //TODO - Find out how to interchange between DateType and regular datetime and vice versa
            //Add Updates for other code parts
            var wtest = Convert.ToDateTime("11/24/2017");
            var TestAsset = new List<AssetClass>
            {
                new AssetClass
                {
                    AssetIdInternal = "34baab1e-5dce-43aa-8860-80000d0db8ec",
                    AssetJSON = new AssetJsonObject{
                        DateModified = wtest,
                    },
                    ChangesMade = new List<Changesmade>
                    {
                        new Changesmade
                        {
                            FieldName = "Location_ID_Internal",
                            FieldValue = "00d447b5-5711-4155-94e7-2f90b0f3e65b"
                        }
                    }
                }
            };
            var TestStack = new Stack<AssetClass>(TestAsset);
            //SyncClass.UpdateSyncClass.API_GetAssetToUpdate(_userID, _url, ref TestStack);
            return "complete";
        }

        public static string testCreateAsset(string _userID, string _url)
        {
            var wtest = Convert.ToDateTime("11/24/2017");
            //String.Format("{0:MM/dd/yyyy HH:mm:ss}", wtest),
            var TestAsset = new List<AssetJsonObject>
            {
               new AssetJsonObject
               {
                   AssetCategoryIDInternal = "0119963b-0609-42df-9f64-e10c3988fb21",
                   AssetConditionID = 30,
                   AssetDescription = "Created using C#",
                   AssetIDInternal = "dfdaeceb-4c8f-40a0-99d0-0927c31306af",
                   AssetName = "Dhuel C# test",
                   AssetSerialNo = "",
                   AssetStatus = "INVENTORY",
                   AssetUsageID = 13,
                   Asset_UID = "",
                   AuditDate = wtest,
                   AuditStatus = "",
                   Barcode = "Dhuel_BarcodeTest",
                   DataGatherID = 0,
                   DateModified = wtest,
                   IsActive = true,
                   LocationIDInternal = "0f054e31-8213-45f0-b522-d5480ec10d14",
                   Mfg = "",
                   Model = "",
                   OriginalPartNo = "",
                   POLine = 0,
                   POStatus = "N",
                   PeopleIDInternal = "00000000-0000-0000-0000-000000000000",
                   Price = 0,
                   PurchaseDate =wtest,
                   PurchaseOrderNo = "",
                   Quantity = 1,
                   ShortageOverage = 0,
                   SiteIDInternal = "33a091ab-b9fa-4b98-abda-9e07ed1c985d",
                   SubLocation = "",
                   ThumbnailImage = "",
                   Vendor = ""
               }
            };
            var TestStack = new Stack<AssetJsonObject>(TestAsset);
            //SyncClass.UpdateSyncClass.API_GetCreatedAssets(_userID, _url, ref TestStack);
            return "complete";
        }


    }
}
