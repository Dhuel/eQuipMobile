using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eQuipMobile
{
    public class AuditClass
    {
        public static void Audit(string SiteID, string LocationID, string SublocationID, int DepartmentID, AssetClass asset, SQLiteConnection _connection, int actualQuantity = -1, string Person = null)
        {
            
            var assetDetails = Database.Assets.GetAssetDataByAssetIDInternal(_connection, asset.AssetIdInternal).First();
            var UpdatedValue = JsonConvert.DeserializeObject<AssetJsonObject>(assetDetails.AssetJSONDb);
            var time = DateTime.Now;
            if (actualQuantity != -1)
                UpdatedValue.ShortageOverage = actualQuantity;
            else
            {
                UpdatedValue.ShortageOverage = asset.AssetJSON.Quantity;
                actualQuantity = asset.AssetJSON.Quantity;
            }
            UpdatedValue.DateModified = time;
            UpdatedValue.AuditDate = time;
            if (asset.Asset_SiteIdInternal == SiteID && asset.Asset_locationIdInternal == LocationID)
            {
                //site and location match, continue
                if (SublocationID != null)
                {
                    //sublocation was used
                    if (SublocationID == asset.Asset_SublocationIdInternal)
                    {
                        //sublocation matched, continue
                        if (DepartmentID != 0)
                        {
                            //department was used
                            if (DepartmentID == asset.Asset_Department)
                            {
                                //department matches, check Quantity
                                if (actualQuantity == -1 ||actualQuantity == asset.AssetJSON.Quantity)
                                {
                                    //Quantity isnt used or matches
                                    if (Person == null || asset.AssetJSON.PeopleIDInternal == Person)
                                    {
                                        //person isnt used or matches
                                        UpdatedValue.AuditStatus = "AUDITED";
                                    }
                                    else
                                    {
                                        UpdatedValue.AuditStatus = "RECON-Custodian";
                                        UpdatedValue.PeopleIDInternal = Person;
                                    }
                                }
                                else
                                {
                                    //Quantity doesn't match
                                    if (Person == null || asset.AssetJSON.PeopleIDInternal == Person)
                                    {
                                        //person isnt used or matches
                                        UpdatedValue.AuditStatus = "RECON-Quantity";
                                    }
                                    else
                                    {
                                        //Person doesnt match
                                        UpdatedValue.AuditStatus = "RECON-Custodian";
                                        UpdatedValue.PeopleIDInternal = Person;
                                    }
                                }
                            }
                            else
                            {
                                //Department doesnt match
                                if (Person == null || asset.AssetJSON.PeopleIDInternal == Person)
                                {
                                    //person isnt used or matches
                                    UpdatedValue.DataGatherID = DepartmentID;
                                    UpdatedValue.AuditStatus = "RECON-Department";
                                }
                                else
                                {
                                    //Person doesnt match
                                    UpdatedValue.AuditStatus = "RECON-Custodian";
                                    UpdatedValue.PeopleIDInternal = Person;
                                }
                            }
                        }
                        else
                        {
                            //department was not used
                            if (actualQuantity == -1 || actualQuantity == asset.AssetJSON.Quantity)
                            {
                                //Quantity isn't used or matches
                                if (Person == null || asset.AssetJSON.PeopleIDInternal == Person)
                                {
                                    //person isnt used or matches
                                    UpdatedValue.AuditStatus = "AUDITED";
                                }
                                else
                                {
                                    //Person doesnt match
                                    UpdatedValue.AuditStatus = "RECON-Custodian";
                                    UpdatedValue.PeopleIDInternal = Person;
                                }
                            }
                            else
                            {
                                //Quantity used, doesnt match
                                if (Person == null || asset.AssetJSON.PeopleIDInternal == Person)
                                {
                                    //person isnt used or matches
                                    UpdatedValue.AuditStatus = "RECON-Quantity";
                                }
                                else
                                {
                                    //Person doesnt match
                                    UpdatedValue.AuditStatus = "RECON-Custodian";
                                    UpdatedValue.PeopleIDInternal = Person;
                                }
                            }
                        }
                    }
                    else
                    {
                        //sublocation did not match, recon moved. but still check department to see if it changed
                        if (DepartmentID != 0)
                        {
                            //Department changed
                            UpdatedValue.SubLocation = SublocationID;
                            UpdatedValue.DataGatherID = DepartmentID;
                            if (Person == null || asset.AssetJSON.PeopleIDInternal == Person)
                            {
                                //person isnt used or matches
                                UpdatedValue.AuditStatus = "RECON-Moved";
                            }
                            else
                            {
                                //Person doesnt match
                                UpdatedValue.AuditStatus = "RECON-Custodian";
                                UpdatedValue.PeopleIDInternal = Person;
                            }
                        }
                    }
                }
                else
                {
                    if (asset.Asset_SublocationIdInternal != "" || asset.Asset_SublocationIdInternal != null)
                        UpdatedValue.SubLocation = "";
                    //sublocation was not used
                    if (DepartmentID != 0)
                    {
                        //department was used
                        if (DepartmentID == asset.Asset_Department)
                        {
                            //department matches
                            if (actualQuantity == -1 || actualQuantity == asset.AssetJSON.Quantity)
                            {
                                if (Person == null || asset.AssetJSON.PeopleIDInternal == Person)
                                {
                                    //person isnt used or matches
                                    UpdatedValue.AuditStatus = "AUDITED";
                                }
                                else
                                {
                                    //Person doesnt match
                                    UpdatedValue.AuditStatus = "RECON-Custodian";
                                    UpdatedValue.PeopleIDInternal = Person;
                                }
                            }
                            else
                            {
                                if (Person == null || asset.AssetJSON.PeopleIDInternal == Person)
                                {
                                    //person isnt used or matches
                                    UpdatedValue.AuditStatus = "RECON-Quantity";
                                }
                                else
                                {
                                    //Person doesnt match
                                    UpdatedValue.AuditStatus = "RECON-Custodian";
                                    UpdatedValue.PeopleIDInternal = Person;
                                }
                            }
                        }
                        else
                        {
                            if (Person == null || asset.AssetJSON.PeopleIDInternal == Person)
                            {
                                //person isnt used or matches
                                UpdatedValue.AuditStatus = "RECON-Department";
                                UpdatedValue.DataGatherID = DepartmentID;
                            }
                            else
                            {
                                //Person doesnt match
                                UpdatedValue.AuditStatus = "RECON-Custodian";
                                UpdatedValue.PeopleIDInternal = Person;
                            }
                        }
                    }
                    else
                    {
                        //department was not used
                        if (actualQuantity == -1 || actualQuantity == asset.AssetJSON.Quantity)
                        {
                            if (Person == null || asset.AssetJSON.PeopleIDInternal == Person)
                            {
                                //person isnt used or matches
                                UpdatedValue.AuditStatus = "AUDITED";
                            }
                            else
                            {
                                //Person doesnt match
                                UpdatedValue.AuditStatus = "RECON-Custodian";
                                UpdatedValue.PeopleIDInternal = Person;
                            }
                        }
                        else
                        {
                            if (Person == null || asset.AssetJSON.PeopleIDInternal == Person)
                            {
                                //person isnt used or matches
                                UpdatedValue.AuditStatus = "RECON-Quantity";
                            }
                            else
                            {
                                //Person doesnt match
                                UpdatedValue.AuditStatus = "RECON-Custodian";
                                UpdatedValue.PeopleIDInternal = Person;
                            }
                        }
                    }
                }
            }
            else
            {
                //Site and location do not match, recon moved
                UpdatedValue.AuditDate = time;
                UpdatedValue.AuditStatus = "RECON-Moved";
                UpdatedValue.SiteIDInternal = SiteID;
                UpdatedValue.LocationIDInternal = LocationID;
                if (SublocationID != null)
                {
                    //set sublocation to whatever SublocationID is (still recon Moved)
                    UpdatedValue.SubLocation = SublocationID;
                }
                else
                {
                    if (asset.Asset_SublocationIdInternal != "" || asset.Asset_SublocationIdInternal != null)
                        UpdatedValue.SubLocation = "";
                }
                if (DepartmentID != 0)
                {
                    //set department to whatever dpt is(still mark as RECON Moved)
                    UpdatedValue.DataGatherID = DepartmentID;
                }
            }
            var assetRecord = Database.Assets.Compare(assetDetails, UpdatedValue);
            var AssetForDb = AssetClass.AssetClassToDb(assetRecord, assetDetails.Synced);
            Database.Assets.Update(_connection, AssetForDb);
        }
    }
}
