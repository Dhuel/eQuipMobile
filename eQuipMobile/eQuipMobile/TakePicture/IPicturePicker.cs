using System;
using System.IO;
using System.Threading.Tasks;

namespace eQuipMobile
{
    public interface IPicturePicker
    {
        Task<Stream> GetImageStreamAsync();
    }
}
