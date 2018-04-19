using SQLite;


namespace eQuipMobile
{
    public interface ISQLiteDb
    {
        SQLiteConnection GetConnection();
    }
}
