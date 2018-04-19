using System.Net.Http;
using ModernHttpClient;
using Xamarin.Forms;
using eQuipMobile.iOS.CreateClient;

[assembly: Dependency(typeof(Client))]

namespace eQuipMobile.iOS.CreateClient
{
    public class Client : IClient
    {
        public HttpClient GetClient()
        {
            return new HttpClient(new NativeMessageHandler());
        }
    }
}