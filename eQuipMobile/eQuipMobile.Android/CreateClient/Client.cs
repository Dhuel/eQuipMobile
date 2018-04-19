using Xamarin.Forms;
using eQuipMobile.Droid;
using System.Net.Http;
using System;
using ModernHttpClient;

[assembly: Dependency(typeof(Client))]
namespace eQuipMobile.Droid
{
    public class Client : IClient
    {
        public HttpClient GetClient()
        {
            try
            {
                if (Android.OS.Build.VERSION.SdkInt <= Android.OS.BuildVersionCodes.Lollipop)
                    return new HttpClient(new AndroidCustomClientHandler());
                else
                    return new HttpClient(new NativeMessageHandler());
            }
            catch (Exception exc)
            {
                throw (new Exception(exc.Message));
            }
            
        }
    }
}