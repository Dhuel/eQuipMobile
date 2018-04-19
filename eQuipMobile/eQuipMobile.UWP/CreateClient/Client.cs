using eQuipMobile.UWP.CreateClient;
using ModernHttpClient;
using System.Net.Http;
using System.Runtime.CompilerServices;

using System.IO;
using SQLite;
using Xamarin.Forms;
using eQuipMobile.UWP;
using Windows.Storage;

[assembly: Xamarin.Forms.Dependency(typeof(Client))]

namespace eQuipMobile.UWP.CreateClient
{
    public class Client : IClient
    {
        public HttpClient GetClient()
        {
            return new HttpClient(new NativeMessageHandler());
        }
    }
}
