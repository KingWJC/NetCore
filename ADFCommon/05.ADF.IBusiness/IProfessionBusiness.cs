using System.IO;
using System.Collections.Generic;
using ADF.Utility;

namespace ADF.IBusiness
{
    public interface IProfessionBussiness
    {
        Stream ExportProfession();

        void UpdateProfession(Stream stream);
    }

    public class ProXmlNode : TreeModel
    {
        public List<SelectOption> Properties { get; set; }
    }
}