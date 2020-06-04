namespace ADF.DataAccess
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