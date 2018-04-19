using System;
using System.IO;
using SQLite;
using Xamarin.Forms;
using eQuipMobile.Droid;
using System.Threading.Tasks;
using Android.Content;

[assembly: Dependency(typeof(SQLiteDb))]
namespace eQuipMobile.Droid
{
    public class SQLiteDb : ISQLiteDb
    {
        public SQLiteConnection GetConnection()
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(documentsPath, "eQuipMobile.db3");
            return new SQLiteConnection(path);
        }
    }
}