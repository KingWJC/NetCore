using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using ADF.IBusiness;
using ADF.Entity;
using ADF.Utility;

namespace ADF.Business
{
    public class ProfessionBussiness : BaseBusiness<Profession>, IProfessionBussiness
    {
        public DataSet GetXMLData()
        {
            return null;
        }

        public void UpdateProfession(Stream stream)
        {
            DataSet dataSet = ExcelHelper.ExcelRead(stream);
            if (dataSet.Tables.Count == 3)
            {
                //导入病情观察的类别
                var dictList = dataSet.Tables[0].ToList<DictType>();
                Service.Update<DictType>(dictList);

                var proList = dataSet.Tables[1].ToList<Profession>();
                var proOptions = dataSet.Tables[2].ToList<ProfessionOption>();


                var nursingRecord = new List<ProXmlNode>();
                var nursingTemplate = new List<ProXmlNode>();

                //更新病情观察的每个数据项的配置
                ConfigInfo config = Service.GetEntity<ConfigInfo>("");
                using (Stream xmlStream = new MemoryStream(config.Data))
                {
                    XmlHelper xmlDoc = new XmlHelper(xmlStream);
                    if (xmlDoc.IsEmpty)
                    {
                        xmlDoc.AppendDeclaration();
                        var root = xmlDoc.AppendRootNode("NursingRecordConfig");
                        xmlDoc.AppendNode("NursingRecordConfig", "NursingRecords");
                        xmlDoc.AppendNode("NursingRecordConfig", "NursingTemplate");
                    }

                    proList.Where(predicate => predicate.PROFESSION_CONTEXT.IsMatch(@"[\S+]"))
                    .ForEach(p =>
                    {
                        var selectOptions = new List<SelectOption>
                        {
                        new SelectOption { name = "Code", value = p.PROFESSION_ID.ToString() },
                        new SelectOption { name = "Name", value = p.PROFESSION_NAME },
                        new SelectOption { name = "Caption", value = p.PROFESSION_NAME }
                            };
                        xmlDoc.AppendNode(@"NursingRecords", "NursingRecord", "", selectOptions);

                        xmlDoc.AppendNode(@"NursingTemplate", "NursingTemplate", p.PROFESSION_CONTEXT, new List<SelectOption> { new SelectOption { name = "Name", value = p.PROFESSION_NAME } });

                        proOptions.Where(o => o.PROFESSION_ID == p.PROFESSION_ID).ForEach(option =>
                        {
                            var itemOptions = new List<SelectOption>{
                        new SelectOption { name = "Code", value = option.OPTION_NAME },
                        new SelectOption { name = "Name", value = option.OPTION_NAME },
                        new SelectOption { name = "Caption", value = option.OPTION_NAME },
                        new SelectOption { name = "Option", value = option.OPTION_VALUE.IsNullOrEmpty()?"Text":"Check" },
                        new SelectOption { name = "Verify", value = "N" },
                        new SelectOption { name = "Source", value = "Local" },
                        new SelectOption { name = "Target", value = "["+option.OPTION_NAME+"]" },
                        new SelectOption { name = "Value", value = "" }};
                            xmlDoc.AppendNode($"NursingRecord[Code='{option.PROFESSION_ID.ToString()}']", "RecordProp", "", itemOptions);

                            if (!option.OPTION_VALUE.IsNullOrEmpty())
                            {
                                string[] splitList = option.OPTION_VALUE.Split(new char[] { '/' });
                                splitList.ForEach(p1 =>
                                {
                                    xmlDoc.AppendNode($@"NursingRecord[Code='{option.PROFESSION_ID.ToString()}']\RecordProp[Code='{option.OPTION_NAME}']", "Item", p1);
                                });
                            }
                        });

                        p.PROFESSION_CONTEXT = string.Empty;
                    });

                    //导入病情观察的数据项
                    Update(proList);

                }


                Service.UpdateAny<ConfigInfo>(config, new List<string> { "Data" });
            }
        }
    }
}