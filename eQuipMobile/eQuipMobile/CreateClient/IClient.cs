using System.Net.Http;

namespace eQuipMobile
{
    public interface IClient
    {
        HttpClient GetClient();
    }
}
