using System.Data;
using System.IO;
using System.Collections.Generic;
using ADF.Utility;

namespace ADF.IBusiness
{
    public interface IProfessionBussiness
    {
        DataSet GetXMLData();

        void UpdateProfession(Stream stream);
    }

    public class ProXmlNode : TreeModel
    {
        public List<SelectOption> Properties { get; set; }
    }
}