using System.Data;

namespace API.TrenAmbiental.Data.Mysql.Interfaces
{
    public interface IConnectionString
    {
        IDbConnection Connection();
    }
}
