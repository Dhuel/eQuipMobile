using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eQuipMobile
{
    public class DeletedAssetsClass
    {
        public string Table { get; set; }
        public string ID { get; set; }
        public DateTime ModifiedOn { get; set; }
        public DeletedAssetsClass()
        {}
        public DeletedAssetsClass(DeleteDbTable DeletedRecords)
        {
            Table = DeletedRecords.TableName;
            ID = DeletedRecords.IdInternal;
        }

    }
}
