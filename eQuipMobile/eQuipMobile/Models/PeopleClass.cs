using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eQuipMobile
{
    public class PeopleClass
    {
        public string PeopleName { get; set; }
        public string PeopleDescription { get; set; }
        public string PeopleIDInternal { get; set; }
        public string PeopleCategoryIDInternal { get; set; }
        public string LocationIDInternal { get; set; }
        public string SiteIDInternal { get; set; }
        public string AccessCardID { get; set; }
        public DateTime ModifiedOn { get; set; }

        public static List<PeopleClass> DbToPeople(IEnumerable<PeopleDbTable> PeopleDb, SQLiteConnection _connection)
        {
            var PeopleList = new List<PeopleClass>();
            foreach (PeopleDbTable PeopleDbRecord in PeopleDb)
            {

                PeopleList.Add(new PeopleClass
                {
                    PeopleName = PeopleDbRecord.PeopleName,
                    PeopleDescription = PeopleDbRecord.PeopleDescription,
                    PeopleIDInternal = PeopleDbRecord.PeopleIDInternal
                });
            }
            return PeopleList;
        }
    }
}
