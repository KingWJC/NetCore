namespace ADF.DataAccess
{
    public class DbFactory
    {
        public static SQLHelper SQLServer(string connectionStr)
        {
            return new SQLHelper(connectionStr);
        }

        public static OracleHelper Oracle(string connectionStr)
        {
            return new OracleHelper(connectionStr);
        }
    }
}