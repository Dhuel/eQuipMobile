using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eQuipMobile
{
    public class DepartmentClass
    {
        public int ID { get; set; }
        public string DepartmentName { get; set; }

        public static List<DepartmentClass> DbToDepartment(IEnumerable<DepartmentsDbTable> DepartmentsDB)
        {
            var DepartmentsClassList = new List<DepartmentClass>();
            foreach (DepartmentsDbTable DepartmentDb in DepartmentsDB)
            {
                DepartmentsClassList.Add(new DepartmentClass{
                    ID = DepartmentDb.ID,
                    DepartmentName = DepartmentDb.DepartmentName,
                });
            }
            return DepartmentsClassList;
        }
    }
}
