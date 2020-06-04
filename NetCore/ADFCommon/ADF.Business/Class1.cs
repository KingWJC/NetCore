using System;

namespace ADF.Business
{
    public class Class1
    {
        public int testconnect()
        {
            return  new ADF.DataAccess.BaseService().GetCount();
        }
    }
}
