using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace eQuipMobile
{
    //TODO - Find out how to interchange between DateType and regular datetime and vice versa
    // Change new sync to keep object deserialized
    //When data isnt sent, store it to error data correctly
    public class SyncClass
    {
        //private static HttpClient _client = new HttpClient(new NativeMessageHandler()); 
        private static HttpClient _client = DependencyService.Get<IClient>().GetClient();
        public static async Task<string> API_Login(string username, string password, string url)
        {
            string returnedData = "[]";
            var url_ = url + "/eQuipAPI/eQuipLoginAD?format=json";
            var username_ = username;
            var password_ = password;
            var LoginContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("username", username_),
                    new KeyValuePair<string, string>("password", Convert.ToBase64String(Encoding.UTF8.GetBytes(password)))
                });

            try
            {
                HttpResponseMessage response = await _client.PostAsync(url_, LoginContent);
            
                if (response.IsSuccessStatusCode)
                    returnedData = '[' + response.Content.ReadAsStringAsync().Result + ']';
                else
                {
                    var loginError = JsonConvert.DeserializeObject<LoginResponse>(response.Content.ReadAsStringAsync().Result);
                    throw (new LoginException(loginError.ResponseStatus.Message + " Please check the username or password."));
                }

            }
            catch (HttpRequestException e)
            {
                DependencyService.Get<IError>().SendRaygunError(e, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", e.Message);
                return ("Error, " + e.ToString());
            }
            return returnedData;
        }
        public static async Task<bool> SetMobileSites(MobileSitesSender mobileSites, string url)
        {
            var url_ = url + "/eQuipAPI/SetMobileSites?format=json";
            var SerializedSites = JsonConvert.SerializeObject(mobileSites);
            var httpContent = new StringContent(SerializedSites, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync(url_, httpContent);
            try
            {
                if (response.IsSuccessStatusCode)
                    return true;
                else
                    return false;
            }
            catch (HttpRequestException e)
            {
                DependencyService.Get<IError>().SendRaygunError(e, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", response.Content.ReadAsStringAsync().Result??"nothing");
                throw (new Exception("Error, " + e.Message));
            }
        }
        public static class NewSyncClass
        {
            public static async Task<string> API_GetAllSites(string UserID, string url)
            {
                string returnedSites = "[]";
                var url_ = url + "/eQuipAPI/GetAllSites/"+UserID+"?format=json";
                HttpResponseMessage response = await _client.GetAsync(url_);
                if (response.IsSuccessStatusCode)
                    returnedSites = response.Content.ReadAsStringAsync().Result;
                else
                    throw new Exception("Error getting Sites.");
                return returnedSites;
            }

            public static async Task<string> API_GetLocation(string UserID, string url, Stack<SitesClass> SiteIDInternal = null, string LastLocationIDInternal = null)
            {
                string returnedLocation = "[]";
                if (SiteIDInternal == null)
                {
                    returnedLocation = await GetAllLocationRecursive(UserID, url, LastLocationIDInternal);
                }
                else
                {
                    returnedLocation = await GetAllLocationBySiteRecursive(UserID, url, SiteIDInternal);
                }
                    

                return returnedLocation;
            }

            private static async Task<string> GetAllLocationRecursive(string UserID, string url, string LastLocationIDInternal = null)
            {
                var returnedLocationList = "[]";
                if (LastLocationIDInternal == null)
                    LastLocationIDInternal = "00000000-0000-0000-0000-000000000000";
                var url_ = url + "/eQuipAPI/GetAllLocations?UserID=" + UserID + "&LastLocationIDInternal=" + LastLocationIDInternal + "&LastSyncDate=2001-01-01&format=json";
                HttpResponseMessage response = await _client.GetAsync(url_);
                if (response.IsSuccessStatusCode)
                {
                    var returnedLocations = response.Content.ReadAsStringAsync().Result;
                    var JSONLocationList = JsonConvert.DeserializeObject<List<LocationClass>>(returnedLocations);
                    if (JSONLocationList.Count > 0)
                    {
                        var recursiveLocations = await GetAllLocationRecursive(UserID, url, JSONLocationList[JSONLocationList.Count - 1].LocationIdInternal);
                        var recursiveJSON = JsonConvert.DeserializeObject<List<LocationClass>>(recursiveLocations);
                        JSONLocationList.AddRange(recursiveJSON);
                        returnedLocationList = JsonConvert.SerializeObject(JSONLocationList);
                    }
                    else
                    {
                        returnedLocationList = JsonConvert.SerializeObject(JSONLocationList);
                    }
                }
                else
                    throw new Exception("Error getting Locations.");
                return returnedLocationList;
            }
            private static async Task<string> GetAllLocationBySiteRecursive(string UserID, string url, Stack<SitesClass> SiteIdInternal)
            {
                var returnedLocationList = "[]";
                var url_ = url + "/eQuipAPI/GetAllLocationsBySite/" + UserID + "/" + SiteIdInternal.Pop().SiteIdInternal + "?format=json";
                try
                {
                    HttpResponseMessage response = await _client.GetAsync(url_);
                    if (response.IsSuccessStatusCode)
                    {
                        var returnedLocations = response.Content.ReadAsStringAsync().Result;
                        var JSONLocationList = JsonConvert.DeserializeObject<List<LocationClass>>(returnedLocations);
                        if (SiteIdInternal.Count > 0)
                        {
                            var recursiveLocations = await GetAllLocationBySiteRecursive(UserID, url, SiteIdInternal);
                            var recursiveJSON = JsonConvert.DeserializeObject<List<LocationClass>>(recursiveLocations);
                            JSONLocationList.AddRange(recursiveJSON);
                            returnedLocationList = JsonConvert.SerializeObject(JSONLocationList);
                        }
                        else
                        {
                            returnedLocationList = JsonConvert.SerializeObject(JSONLocationList);
                        }
                    }
                    else
                        throw new Exception(response.Content.ReadAsStringAsync().Result);
                }
                catch (Exception exc)
                {
                    throw exc;
                }
                

                return returnedLocationList;
            }

            public static async Task<string> API_GetAllSublocations(string UserID, string url)
            {
                string returnedSubLocations = "[]";
                var url_ = url + "/eQuipAPI/GetAllSubLocation/" + UserID + "?format=json";
                try
                {
                    HttpResponseMessage response = await _client.GetAsync(url_);
                    if (response.IsSuccessStatusCode)
                        returnedSubLocations = response.Content.ReadAsStringAsync().Result;
                    else
                        return "Error getting SubLocations.";
                    return returnedSubLocations;
                }
                catch (Exception exc){
                    throw exc;
                }  
            }

            public static async Task<string> API_GetDepartments(string UserID, string url)
            {
                string returnedDepartments = "[]";
                var url_ = url + "/eQuipAPI/GetTemplateData?UserID="+UserID+"&TableName=DepartmentMaster&format=json";
                try
                {
                    HttpResponseMessage response = await _client.GetAsync(url_);
                    if (response.IsSuccessStatusCode)
                        returnedDepartments = response.Content.ReadAsStringAsync().Result;
                    else
                        throw new Exception("Error getting Departments.");
                    return returnedDepartments;
                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }
            public static async Task<string> API_GetPeople(string UserID, string url, string LastPersonID = null)
            {
                string returnedPeople = "[]";
                try
                {
                    returnedPeople = await GetPeopleRecursive(UserID, url, LastPersonID);
                }
                catch (Exception exc)
                {
                    throw exc;
                }
                
                return returnedPeople;
            }

            private static async Task<string> GetPeopleRecursive(string UserID, string url, string LastPersonID = null)
            {
                var returnedPeopleList = "[]";
                
                if (LastPersonID == null)
                    LastPersonID = "00000000-0000-0000-0000-000000000000";
                var url_ = url + "/eQuipAPI/GetAllPeople?UserID=" + UserID + "&LastPeopleIDInternal=" + LastPersonID + "&LastSyncDate=2001-01-01&format=json";
                try
                {
                    HttpResponseMessage response = await _client.GetAsync(url_);
                    if (response.IsSuccessStatusCode)
                    {
                        var returnedPeople = response.Content.ReadAsStringAsync().Result;
                        var JSONPeopleList = JsonConvert.DeserializeObject<List<PeopleClass>>(returnedPeople);
                        if (JSONPeopleList.Count > 0)
                        {
                            var recursivePeople = await GetPeopleRecursive(UserID, url, JSONPeopleList[JSONPeopleList.Count - 1].PeopleIDInternal);
                            var recursiveJSON = JsonConvert.DeserializeObject<List<PeopleClass>>(recursivePeople);
                            JSONPeopleList.AddRange(recursiveJSON);
                            returnedPeopleList = JsonConvert.SerializeObject(JSONPeopleList);
                        }
                        else
                        {
                            returnedPeopleList = JsonConvert.SerializeObject(JSONPeopleList);
                        }
                    }
                    else
                        throw new Exception("Error getting People.");
                    return returnedPeopleList;
                }
                catch (Exception exc)
                {
                    throw exc;
                }
                
            }

            public static async Task<string> API_GetUsages(string UserID, string url)
            {
                string returnedUsages = "[]";
                var url_ = url + "/eQuipAPI/GetAllUsages/" + UserID + "?format=json";
                try
                {
                    HttpResponseMessage response = await _client.GetAsync(url_);
                    if (response.IsSuccessStatusCode)
                        returnedUsages = response.Content.ReadAsStringAsync().Result;
                    else
                        throw new Exception("Error getting Usages.");
                    return returnedUsages;
                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }

            public static async Task<string> API_GetConditions(string UserID, string url)
            {
                string returnedCondition = "[]";
                var url_ = url + "/eQuipAPI/GetAllConditions/" + UserID + "?format=json";
                try
                {
                    HttpResponseMessage response = await _client.GetAsync(url_);
                    if (response.IsSuccessStatusCode)
                        returnedCondition = response.Content.ReadAsStringAsync().Result;
                    else
                        throw new Exception("Error getting Conditions.");
                    return returnedCondition;
                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }

            public static async Task<string> API_GetVendors(string UserID, string url)
            {
                string returnedVendors = "[]";
                var url_ = url + "/eQuipAPI/GetAllVendors/" + UserID + "?format=json";
                try
                {
                    HttpResponseMessage response = await _client.GetAsync(url_);
                    if (response.IsSuccessStatusCode)
                        returnedVendors = response.Content.ReadAsStringAsync().Result;
                    else
                        throw new Exception("Error getting Vendors.");
                    return returnedVendors;
                }
                catch(Exception exc)
                {
                    throw exc;
                }
            }

            public static async Task<string> API_GetMfg(string UserID, string url)
            {
                string returnedMfg = "[]";
                var url_ = url + "/eQuipAPI/GetAllMfg/" + UserID + "?format=json";
                try
                {
                    HttpResponseMessage response = await _client.GetAsync(url_);
                    if (response.IsSuccessStatusCode)
                        returnedMfg = response.Content.ReadAsStringAsync().Result;
                    else
                        throw new Exception("Error getting Manufacturers.");
                    return returnedMfg;
                }
                catch(Exception exc)
                {
                    throw exc;
                }
                
            }

            public static async Task<string> API_GetAssetCategory(string UserID, string url)
            {
                string returnedAssetCategory = "[]";
                var url_ = url + "/eQuipAPI/GetAllAssetCategories/" + UserID + "?format=json";
                try
                {
                    HttpResponseMessage response = await _client.GetAsync(url_);
                    if (response.IsSuccessStatusCode)
                        returnedAssetCategory = response.Content.ReadAsStringAsync().Result;
                    else
                        throw new Exception("Error getting Categories.");
                    return returnedAssetCategory;
                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }

            public static async Task<string> API_GetPropertyPassTable(string UserID, string url, string PropertyPassIDInternal = null)
            {
                string returnedPropertyPassTable = "[]";
                try
                {
                    returnedPropertyPassTable = await GetPropertyPassTableRecursive(UserID, url, PropertyPassIDInternal);
                }
                catch (Exception exc)
                {
                    throw exc;
                }
                return returnedPropertyPassTable;
            }

            private static async Task<string> GetPropertyPassTableRecursive(string UserID, string url, string PropertyPassIDInternal = null)
            {
                var returnedPropertyPassList = "[]";
                if (PropertyPassIDInternal == null)
                    PropertyPassIDInternal = "00000000-0000-0000-0000-000000000000";
                var url_ = url + "/eQuipAPI/GetAllPropertyPassTable?UserID=" + UserID + "&PropertyPassIDInternal=" + PropertyPassIDInternal + "&LastSyncDate=2001-01-01&format=json";
                try
                {
                    HttpResponseMessage response = await _client.GetAsync(url_);
                    if (response.IsSuccessStatusCode)
                    {
                        var returnedPrpopertyPass = response.Content.ReadAsStringAsync().Result;
                        try
                        {
                            var JSONPropertyPassList = JsonConvert.DeserializeObject<List<PropertyPassClass.PropertyPassTable>>(returnedPrpopertyPass);
                            if (JSONPropertyPassList.Count > 0)
                            {
                                var recursivePropertyPassTable = await GetPropertyPassTableRecursive(UserID, url, JSONPropertyPassList[JSONPropertyPassList.Count - 1].PropertyPassID_Internal);
                                var recursiveJSON = JsonConvert.DeserializeObject<List<PropertyPassClass.PropertyPassTable>>(recursivePropertyPassTable);
                                JSONPropertyPassList.AddRange(recursiveJSON);
                                returnedPropertyPassList = JsonConvert.SerializeObject(JSONPropertyPassList);
                            }
                            else
                            {
                                returnedPropertyPassList = JsonConvert.SerializeObject(JSONPropertyPassList);
                            }
                        }
                        catch (Exception e)
                        {
                            //DependencyService.Get<IError>().SendRaygunError(e, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", returnedPrpopertyPass ?? "");
                            throw e;
                        }
                    }
                    else
                        throw new Exception("Error getting Property Pass Tables.");
                    return returnedPropertyPassList;
                }
                catch (Exception exc)
                {
                    throw exc;
                }
                
            }

            public static async Task<string> API_GetPropertyPassItem(string UserID, string url, string PropertyPassItemIDInternal = null)
            {
                string returnedPropertyPassItem = "[]";
                try
                {
                    returnedPropertyPassItem = await GePropertyPassItemRecursive(UserID, url, PropertyPassItemIDInternal);
                }
                catch(Exception exc)
                {
                    throw exc;
                }
                return returnedPropertyPassItem;
            }

            private static async Task<string> GePropertyPassItemRecursive(string UserID, string url, string PropertyPassItemIDInternal = null)
            {
                var returnedPropertyPassItemList = "[]";
                if (PropertyPassItemIDInternal == null)
                    PropertyPassItemIDInternal = "00000000-0000-0000-0000-000000000000";
                var url_ = url + "/eQuipAPI/GetAllPropertyPassItems?UserID=" + UserID + "&LastPropertyItemIDInternal=" + PropertyPassItemIDInternal + "&LastSyncDate=2001-01-01&format=json";
                try
                {
                    HttpResponseMessage response = await _client.GetAsync(url_);
                    if (response.IsSuccessStatusCode)
                    {
                        var returnedPrpopertyPassItems = response.Content.ReadAsStringAsync().Result;
                        var JSONPropertyPassItemsList = JsonConvert.DeserializeObject<List<PropertyPassClass.PropertyPassItem>>(returnedPrpopertyPassItems);
                        if (JSONPropertyPassItemsList.Count > 0)
                        {
                            var recursivePropertyPassItem = await GePropertyPassItemRecursive(UserID, url, JSONPropertyPassItemsList[JSONPropertyPassItemsList.Count - 1].PropertyPassItemID_Internal);
                            var recursiveJSON = JsonConvert.DeserializeObject<List<PropertyPassClass.PropertyPassItem>>(recursivePropertyPassItem);
                            JSONPropertyPassItemsList.AddRange(recursiveJSON);
                            returnedPropertyPassItemList = JsonConvert.SerializeObject(JSONPropertyPassItemsList);
                        }
                        else
                        {
                            returnedPropertyPassItemList = JsonConvert.SerializeObject(JSONPropertyPassItemsList);
                        }
                    }
                    else
                        throw new Exception("Error getting Property Pass items.");
                    return returnedPropertyPassItemList;
                }
                catch (Exception exc)
                {
                    throw exc;
                }  
            }

            public static async Task<string> API_GetAssets(string UserID, string url, Stack<SitesClass> SiteIDInternal = null, string AssetIDInternal = null, String LastSyncDate = null)
            {
                string returnedAssets= "[]";
                List<AssetJsonObject> JSONrecursive = new List<AssetJsonObject>();
                try
                {
                    if (SiteIDInternal == null)
                        returnedAssets = await GetAssetsRecursive(UserID, url, AssetIDInternal, LastSyncDate);
                    else
                    {
                        while (SiteIDInternal.Count > 0)
                        {
                            try
                            {
                                var sitePulled = SiteIDInternal.Pop();
                                returnedAssets = await GetAssetsBySiteRecursive(UserID, url, sitePulled.SiteIdInternal, AssetIDInternal, LastSyncDate);
                                JSONrecursive.AddRange(JsonConvert.DeserializeObject<List<AssetJsonObject>>(returnedAssets));
                                returnedAssets = JsonConvert.SerializeObject(JSONrecursive);
                            }
                            catch (Exception exc)
                            {
                                throw new Exception(exc.Message);
                            }
                        }
                    }
                    return returnedAssets;
                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }

            private static async Task<string> GetAssetsRecursive(string UserID, string url, string AssetIDInternal = null, string LastSyncDate = null)
            {
                var returnedAssetList = "[]";
                if (AssetIDInternal == null)
                    AssetIDInternal = "00000000-0000-0000-0000-000000000000";
                if (LastSyncDate == null)
                    LastSyncDate = "2001-01-01";
                var url_ = url + "/eQuipAPI/GetAllAssets?UserID=" + UserID + "&LastSyncDate="+LastSyncDate+"&LastAssetIDInternal=" + AssetIDInternal + "&format=json";
                try
                {
                    HttpResponseMessage response = await _client.GetAsync(url_);
                    if (response.IsSuccessStatusCode)
                    {
                        var returnedAssets = response.Content.ReadAsStringAsync().Result;
                        var JSONAssetList = JsonConvert.DeserializeObject<List<AssetJsonObject>>(returnedAssets);
                        if (JSONAssetList.Count > 0)
                        {
                            var recursiveAsset = await GetAssetsRecursive(UserID, url, JSONAssetList[JSONAssetList.Count - 1].AssetIDInternal);
                            var recursiveJSON = JsonConvert.DeserializeObject<List<AssetJsonObject>>(recursiveAsset);
                            JSONAssetList.AddRange(recursiveJSON);
                            returnedAssetList = JsonConvert.SerializeObject(JSONAssetList);
                        }
                        else
                        {
                            returnedAssetList = JsonConvert.SerializeObject(JSONAssetList);
                        }
                    }
                    else
                        throw new Exception(response.Content.ReadAsStringAsync().Result);
                    return returnedAssetList;
                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }

            private static async Task<string> GetAssetsBySiteRecursive(string UserID, string url, string SiteIDInternal, string AssetIDInternal = null, string LastSyncDate = null, bool V2 = true)
            {
                var url_ = "";
                var returnedAssetList = "[]";
                if (AssetIDInternal == null)
                    AssetIDInternal = "00000000-0000-0000-0000-000000000000";
                if (LastSyncDate == null)
                    LastSyncDate = "2001-01-01";
                if (V2)
                    url_ = url + "/eQuipAPI/GetAllAssetsBySite_V2?UserID=" + UserID + "&SiteIDInternal="+ SiteIDInternal+ "&LastAssetIDInternal=" + AssetIDInternal + "&LastSyncDate = "+LastSyncDate+"&format=json";
                else
                    url_ = url + "/eQuipAPI/GetAllAssetsBySite?UserID=" + UserID + "&SiteIDInternal=" + SiteIDInternal + "&LastAssetIDInternal=" + AssetIDInternal + "&LastSyncDate = " + LastSyncDate + "&format=json";
                try
                {
                    HttpResponseMessage response = await _client.GetAsync(url_);
                    if (response.IsSuccessStatusCode)
                    {
                        var returnedAssets = response.Content.ReadAsStringAsync().Result;
                        var JSONAssetList = JsonConvert.DeserializeObject<List<AssetJsonObject>>(returnedAssets);
                        if (JSONAssetList.Count > 0)
                        {
                            var recursiveAsset = await GetAssetsBySiteRecursive(UserID, url, SiteIDInternal, JSONAssetList[JSONAssetList.Count - 1].AssetIDInternal, LastSyncDate, V2);
                            var recursiveJSON = JsonConvert.DeserializeObject<List<AssetJsonObject>>(recursiveAsset);
                            JSONAssetList.AddRange(recursiveJSON);
                            returnedAssetList = JsonConvert.SerializeObject(JSONAssetList);
                        }
                        else
                        {
                            returnedAssetList = JsonConvert.SerializeObject(JSONAssetList);
                        }
                    }
                    else
                    {
                        //var error = new Exception(response.Content.ReadAsStringAsync().Result);
                        if (V2)
                            return await GetAssetsBySiteRecursive(UserID, url, SiteIDInternal, AssetIDInternal, LastSyncDate, false);
                        else
                            throw new Exception(response.Content.ReadAsStringAsync().Result);
                    }

                    return returnedAssetList;
                }
                catch(Exception exc)
                {
                    throw exc;
                }
            }

            public static async Task<string> API_GeFormDesigner(string UserID, string url)
            {
                string returnedFormDesigner = "[]";
                var url_ = url + "/eQuipAPI/GetFormDesignerData/" + UserID + "/0/AssetDetail/0/Designer/0?format=json";
                try
                {
                    HttpResponseMessage response = await _client.GetAsync(url_);
                    if (response.IsSuccessStatusCode)
                        returnedFormDesigner = response.Content.ReadAsStringAsync().Result;
                    else
                        throw new Exception("Error getting Form Designer.");
                    return returnedFormDesigner;
                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }

            //Possible move to Audit class when created

        //    public static async Task<string> API_GetAssetByBarcode(string UserID, string url, string barcode)
        //    {
        //        string returnedAsset = "[]";
        //        var url_ = url + "/eQuipAPI/GetAssetDetailByBarcode/" + UserID + "/" + barcode + "?format=json";
        //        try
        //        {
        //            HttpResponseMessage response = await _client.GetAsync(url_);
        //            if (response.IsSuccessStatusCode)
        //                returnedAsset = response.Content.ReadAsStringAsync().Result;
        //            else
        //                throw new Exception("Error getting Asset by Barcode.");
        //            return returnedAsset;
        //        }
        //        catch (Exception exc)
        //        {
        //            throw exc;
        //        }
        //    }
        //}
        public static async Task<string> API_GetAssetByBarcode(string UserID, string url, string barcode, bool attempt_2 = false)
            {
                string returnedAsset = "[]";
                string url_ = "";
                if (!attempt_2)
                {
                    url_ = url + "/eQuipAPI/GetAssetDetailByBarcode_V2/" + UserID + "/" + barcode + "?format=json";
                }
                else
                    url_ = url + "/eQuipAPI/GetAssetDetailByBarcode/" + UserID + "/" + barcode + "?format=json";
                try
                {
                    HttpResponseMessage response = await _client.GetAsync(url_);
                    if (response.IsSuccessStatusCode)
                        returnedAsset = response.Content.ReadAsStringAsync().Result;
                    else
                    {
                        if (!attempt_2)
                        {
                            returnedAsset = await API_GetAssetByBarcode(UserID, url, barcode, true);
                            
                        }
                        else
                            throw new Exception("Error getting Asset by Barcode.");
                    }
                    return returnedAsset;
                }
                catch (Exception exc)
                {
                    throw exc;
                } 
            }
        }
        public static class UpdateSyncClass
        {
            public static void API_GetAssetToUpdate(string UserID, string url, ref Stack<AssetClass> Assets, SQLiteConnection db)
            {
                var AssetToSync = new AssetClass();
                while (Assets.Count > 0)
                {
                    AssetToSync = Assets.Pop();
                    var listOfChanges = new List<AssetsChange>
                    {
                        new AssetsChange
                        {
                            AssetIDInternal = AssetToSync.AssetIdInternal,
                            DateModified = AssetToSync.AssetJSON.DateModified,
                            Changesmade = AssetToSync.ChangesMade
                        }
                    };
                    try
                    {
                        API_UpdateAsset(UserID, url, listOfChanges, 0, db);
                    }
                    catch(Exception exc)
                    {
                        throw exc;
                    }
                }
            }

            static async void API_UpdateAsset(string UserID, string url, List<AssetsChange> AssetsChangeList, int cntr, SQLiteConnection db)
            {
                if (cntr < 2)
                {
                    var AssetUpdater = new AssetUpdateClass
                    {
                        UserID = UserID,
                        AssetsChanges = AssetsChangeList
                    };
                    var SerializedUpdateValues = JsonConvert.SerializeObject(AssetUpdater);
                    var url_ = url + "/eQuipAPI/UpdateAssets?format=json";
                    var httpContent = new StringContent(SerializedUpdateValues, Encoding.UTF8, "application/json");
                    try
                    {
                        HttpResponseMessage response = await _client.PostAsync(url_, httpContent);
                        try
                        {
                            if (!response.IsSuccessStatusCode)
                            {
                                cntr++;
                                API_UpdateAsset(UserID, url, AssetsChangeList, cntr, db);
                            }
                        }
                        catch (HttpRequestException e)
                        {
                            //DependencyService.Get<IError>().SendRaygunError(e, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", AssetsChangeList ?? null);
                            throw (new Exception("Error, " + e.Message));
                        }
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                    }
                }
                else
                {
                    API_SendErrorData(UserID, url, new ErrorDataClass()
                    {
                        UserID = UserID,
                        TableName = "Asset_Update",
                        IncorrectData = Database.Assets.GetAssetDataByAssetIDInternal(db, AssetsChangeList.First().AssetIDInternal).First().AssetJSONDb,
                        LastSyncDate = Convert.ToDateTime(Database.Login.GetTableData(db).First().LastSyncDate)
                    }, "Asset update");
                }
            }

            public static void API_GetCreatedAssets(string _userID, string _url, ref Stack<AssetJsonObject> CreatedAssetsList, SQLiteConnection db)
            {
                try
                {
                    while (CreatedAssetsList.Count > 0)
                    {
                        AssetJsonObject CreateAsset = CreatedAssetsList.Pop();
                        API_CreateAsset(_userID, _url, CreateAsset, 0, db);
                    }
                }
                catch (Exception exc) {
                    throw exc;
                    //DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", null);
                }
            }

            static async void API_CreateAsset(string _userID, string _url, AssetJsonObject CreatedAsset, int cntr, SQLiteConnection db)
            {
                if (cntr < 2)
                {
                    var AssetCreateFormat = new CreateAssetClass
                    {
                        UserID = _userID,
                        AssetIDInternal = CreatedAsset.AssetIDInternal,
                        Asset = CreatedAsset
                    };
                    var serializedCreateAsset = JsonConvert.SerializeObject(AssetCreateFormat);
                    var url_ = _url + "/eQuipAPI/CreateAsset?format=json";
                    var httpContent = new StringContent(serializedCreateAsset, Encoding.UTF8, "application/json");
                    try {
                        HttpResponseMessage response = await _client.PostAsync(url_, httpContent);
                        try
                        {
                            if (!response.IsSuccessStatusCode)
                            {
                                cntr++;
                                var test = response.Content.ReadAsStringAsync().Result;
                                API_CreateAsset(_userID, _url, CreatedAsset, cntr, db);
                            }
                            else
                                Database.Assets.UpdateSynced(db, CreatedAsset);
                        }
                        catch (HttpRequestException e)
                        {
                            //DependencyService.Get<IError>().SendRaygunError(e, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", CreatedAsset ?? null);
                            throw (new Exception("Error, " + e.Message));
                        }
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                    }
                }
                else
                {
                    try
                    {
                        API_SendErrorData(_userID, _url, new ErrorDataClass()
                        {
                            UserID = _userID,
                            TableName = "Create_Asset",
                            IncorrectData = JsonConvert.SerializeObject(CreatedAsset),
                            LastSyncDate = Convert.ToDateTime(Database.Login.GetTableData(db).First().LastSyncDate)
                        }, "Create Asset");
                    }
                    catch (Exception exc)
                    {
                        //DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", CreatedAsset??null);
                        throw (new Exception(exc.Message));
                    }
                    
                }
            }

            public static void API_GetTransferAssets(string UserID, string url, ref Stack<TransferClass> Transfers, SQLiteConnection db)
            {
                while(Transfers.Count > 0)
                {
                    var AssetToTransfer = Transfers.Pop();
                    API_TransferAsset(UserID, url, AssetToTransfer, 0, db);
                }
            }

            static async void API_TransferAsset(string UserID, string url, TransferClass TransferData, int cntr, SQLiteConnection db, bool V2 = true)
            {
                string url_ = "";
                if (cntr < 2)
                {
                    var AssetTransferFormat = new AssetTransferFormatClass
                    {
                        UserID = UserID
                    };
                    AssetTransferFormat.SetUsingTransfer(TransferData);
                    var serializedTransferAsset = JsonConvert.SerializeObject(AssetTransferFormat);
                    if(V2)
                        url_ = url + "/eQuipAPI/AssetTransfer_V2?format=json";
                    else
                        url_ = url + "/eQuipAPI/AssetTransfer?format=json";
                    var httpContent = new StringContent(serializedTransferAsset, Encoding.UTF8, "application/json");
                    try
                    {
                        HttpResponseMessage response = await _client.PostAsync(url_, httpContent);
                        try
                        {
                            if (!response.IsSuccessStatusCode)
                            {
                                cntr++;
                                API_TransferAsset(UserID, url, TransferData, cntr, db, V2);
                            }
                            else
                            {
                                Database.Transfers.DeleteWhere(db, TransferData.AssetIdInternal);
                            }
                        }
                        catch (HttpRequestException e)
                        {
                            DependencyService.Get<IError>().SendRaygunError(e, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", TransferData ?? null);
                            throw (new Exception("Error, " + e.Message));
                        }
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                    }
                }
                else
                {
                    if (V2)
                    {
                        API_TransferAsset(UserID, url, TransferData, 0, db, false);
                    }
                    else
                    {
                        API_SendErrorData(UserID, url, new ErrorDataClass()
                        {
                            UserID = UserID,
                            TableName = "Transfer_Update",
                            IncorrectData = JsonConvert.SerializeObject(TransferData),
                            LastSyncDate = Convert.ToDateTime(Database.Login.GetTableData(db).First().LastSyncDate)
                        }, "Transfer update");
                    }
                    
                }
            }

            public static void API_GetDeleteAssetList(string UserID, string url, ref Stack<DeletedAssetsClass> DeletedAssetsList, SQLiteConnection db)
            {
                try
                {
                    while (DeletedAssetsList.Count > 0)
                    {
                        var AssetToDelete = DeletedAssetsList.Pop();
                        API_DeleteAsset(UserID, url, AssetToDelete, 0, db);
                    }
                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }
            static async void API_DeleteAsset(string UserID, string url, DeletedAssetsClass DeletedData, int cntr, SQLiteConnection db)
            {
                if (cntr < 2)
                {
                    try
                    {
                        var url_ = url + "/eQuipAPI/DeleteAssetByID/" + UserID + "/" + DeletedData.ID + "?format=json";
                        var response = await _client.GetAsync(url_);

                        if (!response.IsSuccessStatusCode)
                        {
                            cntr++;
                            API_DeleteAsset(UserID, url, DeletedData, cntr, db);
                        }
                        else
                        {
                            Database.Deletes.DeleteWhere(db, DeletedData.ID);
                        }
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                        //DependencyService.Get<IError>().SendRaygunError(exc, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", DeletedData);
                        //throw (e); TODO
                        //Send Exception to raygun or something but dont stop
                        //create a log in the code that sends this data to wherever we want to send it
                    }
                } 
            }

            public static void API_GetPropertyPassTablesToUpdateOrCreate(SQLiteConnection db, string UserID, string url, ref Stack<PropertyPassClass.PropertyPassTable> PropertyPassTableList, bool Update_0_Create_1_)
            {
                try
                {
                    if (Update_0_Create_1_ == false)
                    {
                        while (PropertyPassTableList.Count > 0)
                        {
                            var PropertyPassRecord = PropertyPassTableList.Pop();
                            API_UpdatePropertyPassTable(db, UserID, url, PropertyPassRecord, 0);
                        }
                    }
                    else
                    {
                        while (PropertyPassTableList.Count > 0)
                        {
                            var PropertyPassRecord = PropertyPassTableList.Pop();
                            API_CreatePropertyPassTable(db, UserID, url, PropertyPassRecord, 0);
                        }
                    }
                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }

            static async void API_UpdatePropertyPassTable(SQLiteConnection db, string UserID, string url, PropertyPassClass.PropertyPassTable PropertyPassTableRecord, int cntr)
            {
                var PropertyPassSendingFormat = new PropertyPassClass.PropertyPassTableUpdateFormat
                {
                    UserID = UserID,
                    PropertyPassID_Internal = PropertyPassTableRecord.PropertyPassID_Internal
                };
                PropertyPassSendingFormat.SetUpdateTableAssetValues(PropertyPassTableRecord);
                var serializedPropertyPassTable = JsonConvert.SerializeObject(PropertyPassSendingFormat);
                if (cntr < 2)
                {
                    var url_ = url + "/eQuipAPI/UpdatePropertyPassTable?format=json";
                    var httpContent = new StringContent(serializedPropertyPassTable, Encoding.UTF8, "application/json");
                    try
                    {
                        HttpResponseMessage response = await _client.PostAsync(url_, httpContent);
                        try
                        {
                            if (!response.IsSuccessStatusCode)
                            {
                                cntr++;
                                API_UpdatePropertyPassTable(db, UserID, url, PropertyPassTableRecord, cntr);
                            }
                        }
                        catch (HttpRequestException e)
                        {
                            //DependencyService.Get<IError>().SendRaygunError(e, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", PropertyPassTableRecord ?? null);
                            throw (new Exception("Error, " + e.Message));
                        }
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                    }
                    
                }
                else
                {
                    API_SendErrorData(UserID, url, new ErrorDataClass()
                    {
                        UserID = UserID,
                        TableName = "PropertyPassTable",
                        IncorrectData = serializedPropertyPassTable,
                        LastSyncDate = Convert.ToDateTime(Database.Login.GetTableData(db).First().LastSyncDate)
                    }, " Update Property Pass Table");
                }
            }

            static async void API_CreatePropertyPassTable (SQLiteConnection db, string UserID, string url, PropertyPassClass.PropertyPassTable PropertyPassTableRecord, int cntr)
            {
                try
                {
                    var PropertyPassSendingFormat = new PropertyPassClass.PropertyPassTableCreateFormat
                    {
                        UserID = UserID
                    };
                    PropertyPassSendingFormat.SetCreateTableAssetValues(PropertyPassTableRecord);
                    var serializedPropertyPassTable = JsonConvert.SerializeObject(PropertyPassSendingFormat);

                    if (cntr < 2)
                    {
                        var url_ = url + "/eQuipAPI/CreatePropertyPass?format=json";
                        var httpContent = new StringContent(serializedPropertyPassTable, Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await _client.PostAsync(url_, httpContent);
                        try
                        {
                            if (!response.IsSuccessStatusCode)
                            {
                                cntr++;
                                API_CreatePropertyPassTable(db, UserID, url, PropertyPassTableRecord, cntr);
                            }
                        }
                        catch (HttpRequestException e)
                        {
                            DependencyService.Get<IError>().SendRaygunError(e, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", PropertyPassTableRecord ?? null);
                            throw (new Exception("Error, " + e.Message));
                        }
                    }
                    else
                    {
                        API_SendErrorData(UserID, url, new ErrorDataClass()
                        {
                            UserID = UserID,
                            TableName = "PropertyPassTable",
                            IncorrectData = serializedPropertyPassTable,
                            LastSyncDate = Convert.ToDateTime(Database.Login.GetTableData(db).First().LastSyncDate)
                        }, " Create Property Pass Table");
                    }
                }
                catch(Exception exc)
                {
                    throw exc;
                }
            }

            public static void API_GetPropertyPassItemsToUpdateOrCreate(SQLiteConnection db, string UserID, string url, ref Stack<PropertyPassClass.PropertyPassItem> PropertyPassItemList, bool Update_0_Create_1_)
            {
                try
                {
                    if (Update_0_Create_1_ == false)
                    {
                        while (PropertyPassItemList.Count > 0)
                        {
                            var PropertyPassRecord = PropertyPassItemList.Pop();
                            API_UpdatePropertyPassItem(db, UserID, url, PropertyPassRecord, 0);
                        }
                    }
                    else
                    {
                        while (PropertyPassItemList.Count > 0)
                        {
                            var PropertyPassRecord = PropertyPassItemList.Pop();
                            API_CreatePropertyPassItem(db, UserID, url, PropertyPassRecord, 0);
                        }
                    }
                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }

            static async void API_UpdatePropertyPassItem(SQLiteConnection db, string UserID, string url, PropertyPassClass.PropertyPassItem PropertyPassItemRecord, int cntr, bool V2 = true)
            {
                string url_ = "";
                try
                {
                    var PropertyPassSendingFormat = new PropertyPassClass.PropertyPassItemUpdateFormat()
                    {
                        UserID = UserID,
                        PropertyPassItemID_Internal = PropertyPassItemRecord.PropertyPassItemID_Internal,
                    };
                    PropertyPassSendingFormat.SetAssetValuesUpdate(PropertyPassItemRecord);
                    var serializedPropertyPassItem = JsonConvert.SerializeObject(PropertyPassSendingFormat);
                    if (cntr < 2)
                    {
                        if(V2)
                            url_ = url + "/eQuipAPI/UpdatePropertyPassItem_V2?format=json";
                        else
                            url_ = url + "/eQuipAPI/UpdatePropertyPassItem?format=json";
                        var httpContent = new StringContent(serializedPropertyPassItem, Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await _client.PostAsync(url_, httpContent);
                        try
                        {
                            if (!response.IsSuccessStatusCode)
                            {
                                cntr++;
                                API_UpdatePropertyPassItem(db, UserID, url, PropertyPassItemRecord, cntr, V2);
                            }
                            else
                            {
                                if(V2)
                                    API_UpdatePropertyPassItem(db, UserID, url, PropertyPassItemRecord, cntr, false);
                                else
                                    throw new Exception(response.Content.ReadAsStringAsync().Result);
                            }
                        }
                        catch (HttpRequestException e)
                        {
                           if (V2)

                            DependencyService.Get<IError>().SendRaygunError(e, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", PropertyPassItemRecord ?? null);
                            throw (new Exception("Error, " + e.Message));
                        }
                    }
                    else
                    {
                        API_SendErrorData(UserID, url, new ErrorDataClass()
                        {
                            UserID = UserID,
                            TableName = "PropertyPassTable",
                            IncorrectData = serializedPropertyPassItem,
                            LastSyncDate = Convert.ToDateTime(Database.Login.GetTableData(db).First().LastSyncDate)
                        }, " Update Property Pass Item");
                    }
                }
                catch (Exception exc)
                {
                    throw exc;
                }
                
            }

            static async void API_CreatePropertyPassItem(SQLiteConnection db, string UserID, string url, PropertyPassClass.PropertyPassItem PropertyPassItemRecord, int cntr)
            {
                try
                {
                    var PropertyPassSendingFormat = new PropertyPassClass.PropertyPassItemCreateFormat
                    {
                        UserID = UserID,
                        PropertyPassItemID_Internal = PropertyPassItemRecord.PropertyPassItemID_Internal
                    };
                    PropertyPassSendingFormat.SetAssetValuesCreate(PropertyPassItemRecord);
                    var serializedPropertyPassItem = JsonConvert.SerializeObject(PropertyPassSendingFormat);

                    if (cntr < 2)
                    {
                        var url_ = url + "/eQuipAPI/CreatePropertyPassItem?format=json";
                        var httpContent = new StringContent(serializedPropertyPassItem, Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await _client.PostAsync(url_, httpContent);
                        try
                        {
                            if (!response.IsSuccessStatusCode)
                            {
                                cntr++;
                                API_CreatePropertyPassItem(db, UserID, url, PropertyPassItemRecord, cntr);
                            }
                        }
                        catch (HttpRequestException e)
                        {
                            DependencyService.Get<IError>().SendRaygunError(e, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", PropertyPassItemRecord ?? null);
                            throw (new Exception("Error, " + e.Message));
                        }
                    }
                    else
                    {
                        API_SendErrorData(UserID, url, new ErrorDataClass()
                        {
                            UserID = UserID,
                            TableName = "PropertyPassTable",
                            IncorrectData = serializedPropertyPassItem,
                            LastSyncDate = Convert.ToDateTime(Database.Login.GetTableData(db).First().LastSyncDate)
                        }, " Create Property Pass Item");
                    }
                }
                catch(Exception exc)
                {
                    throw exc;
                }
            }

            static async void API_SendErrorData(string UserID, string url, ErrorDataClass ErrorData, string incoming)
            {
                try
                {
                    var ErrorDataSending = JsonConvert.SerializeObject(ErrorData);
                    var url_ = url + "/eQuipAPI/StoreErrorData?format=json";
                    var httpContent = new StringContent(ErrorDataSending, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await _client.PostAsync(url_, httpContent);
                    Debug.WriteLine(response.Content.ReadAsStringAsync().Result);
                    try
                    {
                        if (!response.IsSuccessStatusCode && ErrorData.TableName != "General_Error")
                        {
                            ErrorData.TableName = "General_Error";
                            API_SendErrorData(UserID, url, ErrorData, incoming);
                        }
                        else if (!response.IsSuccessStatusCode && ErrorData.TableName == "General_Error")
                        {
                            Application.Current.Properties["Error"] = "general";
                            //Failed as general error
                            //Store data to log file
                            //throw (new Exception(response.Content.ReadAsStringAsync().Result));
                        }
                        else
                        {
                            if (ErrorData.TableName == "General_Error")
                                Application.Current.Properties["Error"] = "general";
                            else
                                Application.Current.Properties["Error"] = "recon";
                            Debug.WriteLine(response.Content.ReadAsStringAsync().Result);
                            //TODO write this error data to the log and wed api
                        }
                    }
                    catch (Exception e)
                    {
                        DependencyService.Get<IError>().SendRaygunError(e, Application.Current.Properties["user"].ToString() ?? "unsynced", Application.Current.Properties["url"].ToString() ?? "unsynced", ErrorData ?? null);
                        throw (new Exception(e.Message));
                    }
                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }
        }

    }
}