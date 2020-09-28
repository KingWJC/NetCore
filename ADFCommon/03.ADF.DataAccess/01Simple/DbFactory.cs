using ADF.Utility;

namespace ADF.DataAccess.Simple
{
    public class DbFactory
    {
        public static SqlserverHelper SQLServer(string connectionStr)
        {
            return new SqlserverHelper(connectionStr);
        }

        public static OracleHelper Oracle(string connectionStr)
        {
            return new OracleHelper(connectionStr);
        }
    }
}