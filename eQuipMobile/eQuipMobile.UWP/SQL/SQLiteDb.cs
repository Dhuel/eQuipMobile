using System.IO;
using SQLite;
using Xamarin.Forms;
using eQuipMobile.UWP;
using Windows.Storage;

[assembly: Dependency(typeof(SQLiteDb))]

namespace eQuipMobile.UWP
{
    public class SQLiteDb : ISQLiteDb
    {
        public SQLiteConnection GetConnection()
        {
            var documentsPath = ApplicationData.Current.LocalFolder.Path;
            var path = Path.Combine(documentsPath, "eQuipMobile.db3");
            return new SQLiteConnection(path);
        }
    }
}
