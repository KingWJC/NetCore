using System.Data;

namespace ADF.DataAccess.ORM
{
    public class CusDbParameter
    {
        private string _parameterName;
        public string ParameterName
        {
            get { return _parameterName; }
        }
        private object _value;
        public object Value
        {
            get { return _value; }
            set { _value = Value; }
        }
        private DbType _dbType;
        public DbType DbType
        {
            get { return _dbType; }
        }
        private int _size;
        public int Size
        {
            get { return _size; }
        }
        private ParameterDirection _direction;
        public ParameterDirection Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }
        public CusDbParameter(string parameterName, object value)
        {
            _parameterName = parameterName;
            _value = value;
        }
        public CusDbParameter(string parameterName, object value, DbType dbType)
        {
            _parameterName = parameterName;
            _value = value;
            _dbType = dbType;
        }
        public CusDbParameter(string parameterName, object value, DbType dbType, int size)
        {
            _parameterName = parameterName;
            _value = value;
            _dbType = dbType;
            _size = size;
        }
    }
}