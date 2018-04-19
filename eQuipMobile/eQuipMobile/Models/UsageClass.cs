using SQLite;
using System.Collections.Generic;

namespace eQuipMobile
{
    public class UsageClass
    {
        public int UsageID { get; set; }
        public string UsageLabel { get; set; }
        public string UsageIDInternal { get; set; }
        public static List<UsageClass> DbToUsage(IEnumerable<UsageDbTable> UsageDb, SQLiteConnection _connection)
        {
            var UsageList = new List<UsageClass>();
            foreach (UsageDbTable UsageDbRecord in UsageDb)
            {
                UsageList.Add(new UsageClass
                {
                    UsageID = UsageDbRecord.UsageID,
                    UsageLabel = UsageDbRecord.UsageLabel,
                    UsageIDInternal = UsageDbRecord.UsageIDInternal
                });
            }
            return UsageList;
        }
    }
}
