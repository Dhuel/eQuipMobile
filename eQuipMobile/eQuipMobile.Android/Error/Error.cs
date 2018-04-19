using System;
using Xamarin.Forms;
using eQuipMobile.Droid;
using Mindscape.Raygun4Net;
using System.Collections.Generic;
using Newtonsoft.Json;

[assembly: Dependency(typeof(Error))]
namespace eQuipMobile.Droid
{
    public class Error : IError
    {
        public void SendRaygunError(Exception e, string username, string url, object data)
        {
            try
            {
                Exception error = new Exception(e.Message);
                RaygunClient.Current.User = username;
                RaygunClient.Current.ApplicationVersion = "1";
                RaygunClient.Current.Send(error, null, new Dictionary<string, object>() {
                    { "Username", username },
                    {"Url", url },
                    {"App Version", "1" },
                    {"Error",error },
                    {"Data", JsonConvert.SerializeObject(data)},
                    {"Stack trace", e.StackTrace }
            });
            }
            catch (Exception)
            {
            }

        }
    }
}