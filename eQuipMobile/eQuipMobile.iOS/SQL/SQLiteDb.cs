using System;
using System.IO;
using SQLite;
using Xamarin.Forms;
using eQuipMobile.iOS;

[assembly: Dependency(typeof(SQLiteDb))]

namespace eQuipMobile.iOS
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