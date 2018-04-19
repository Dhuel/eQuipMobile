using System;

namespace eQuipMobile
{
    public interface IError
    {
        void SendRaygunError(Exception e, string username, string url, object data);
    }
}
