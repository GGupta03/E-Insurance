using E_Insurance_App.Helpers;

namespace E_Insurance_App.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly DbHelper _db;

        protected BaseRepository(DbHelper db)
        {
            _db = db;
        }
    }
}
