using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eQuipMobile
{
    public class ErrorDataClass
    {
        public string UserID { get; set; }
        public string TableName { get; set; }
        public DateTime LastSyncDate { get; set; }
        public string IncorrectData { get; set; }
        public ErrorDataClass()
        { }
    }
}
