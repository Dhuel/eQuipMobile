using System;
using Xamarin.Forms;
using eQuipMobile.UWP;
using Mindscape.Raygun4Net;
using System.Collections.Generic;

[assembly: Dependency(typeof(Error))]
namespace eQuipMobile.UWP
{
    public class Error : IError
    {
        public void SendRaygunError(Exception e, string username, string url, object data)
        {
            RaygunClient.Current.User = username;
            RaygunClient.Current.ApplicationVersion = "1";
        }
    }
}