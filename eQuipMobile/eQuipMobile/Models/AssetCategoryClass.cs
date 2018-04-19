using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eQuipMobile
{
    public class AssetCategoryClass
    {
        public int AssetCategoryID { get; set; }
        public string AssetCategoryIDInternal { get; set; }
        public string AssetCategoryName { get; set; }
        public int ParentID { get; set; }
        public static List<AssetCategoryClass> DbToCategory(IEnumerable<CategoryDbTable> CategoryDb, SQLiteConnection _connection)
        {
            var CategoryList = new List<AssetCategoryClass>();
            foreach (CategoryDbTable CategoryDbRecord in CategoryDb)
            {
                CategoryList.Add(new AssetCategoryClass
                {
                    AssetCategoryID = CategoryDbRecord.AssetCategoryID,
                    AssetCategoryIDInternal = CategoryDbRecord.AssetCategoryIDInternal,
                    AssetCategoryName = CategoryDbRecord.AssetCategoryName,
                    ParentID = CategoryDbRecord.ParentID
                });
            }
            return CategoryList;
        }
    }
}
