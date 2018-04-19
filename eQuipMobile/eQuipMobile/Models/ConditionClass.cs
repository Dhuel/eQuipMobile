using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eQuipMobile
{
    public class ConditionClass
    {
        public int ConditionID { get; set; }
        public string ConditionLabel { get; set; }
        public string ConditionIDInternal { get; set; }
        public static List<ConditionClass> DbToCondition(IEnumerable<ConditionDbTable> ConditionDb, SQLiteConnection _connection)
        {
            var ConditionList = new List<ConditionClass>();
            foreach (ConditionDbTable ConditionDbRecord in ConditionDb)
            {

                ConditionList.Add(new ConditionClass
                {
                    ConditionID = ConditionDbRecord.ConditionID,
                    ConditionLabel = ConditionDbRecord.ConditionLabel,
                    ConditionIDInternal = ConditionDbRecord.ConditionIDInternal
                });
            }
            return ConditionList;
        }
    }
}
