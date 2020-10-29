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
                if (dataSet.Tables.Contains("DictType"))
                {
                    var dictList = dataSet.Tables["DictType"].ToList<DictType>();
                    // string sql = "INSERT INTO JHICU_DICT_MICDICT_DETAIL(\"MICDICT_ID\",\"TYPE_ID\",\"DICT_ID\",\"DICT_NAME\",\"NOTE\",\"PYM\",\"WBM\",\"HOSPITAL_NO\",\"STATE\")  VALUES(7000,71,15,'生命体征','病情观察','SMTZ','','400011489',0)";
                    // Service.ExecuteSql(sql);
                    Service.Delete<DictType>(dictList.Select(p => p.DICT_ID.ToString()).ToList());
                    Service.Insert<DictType>(dictList);
                }

                //更新病情观察的每个数据项的配置
                ConfigInfo config = Service.GetEntity<ConfigInfo>("5");
                using (Stream xmlStream = new MemoryStream(config.CONFIG_CONTEXT))
                {
                    XmlHelper xmlDoc = new XmlHelper(xmlStream);
                    if (xmlDoc.IsEmpty)
                    {
                        xmlDoc.AppendDeclaration();
                        var root = xmlDoc.AppendRootNode("NursingRecordConfig");
                        xmlDoc.AppendNode("NursingRecordConfig", "NursingRecords");
                        xmlDoc.AppendNode("NursingRecordConfig", "NursingTemplate");
                    }

                    UpdateProfessionItem(dataSet.Tables["Profession"], xmlDoc);

                    UpdateOptionItem(dataSet.Tables["ProfessionOption"], xmlDoc);

                    config.CONFIG_CONTEXT = (byte[])xmlDoc.Document.OuterXml.ChangeType_ByConvert(typeof(byte[]));
                    Service.UpdateAny<ConfigInfo>(config, new List<string> { "CONFIG_CONTEXT" });

                    FileHelper.WriteTxt(xmlDoc.Document.OuterXml, @"E:\data.xml");
                }

                Service.UpdateAny<ConfigInfo>(config, new List<string> { "Data" });
            }

            void UpdateProfessionItem(DataTable dataTable, XmlHelper xmlDoc)
            {
                var proList = dataTable != null ? dataTable.ToList<Profession>() : new List<Profession>();
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

                    p.PROFESSION_CONTEXT = string.Empty;
                });

                //导入病情观察的数据项
                Delete(proList.Select(p => p.PROFESSION_ID.ToString()).ToList());
                Insert(proList);
            }

            void UpdateOptionItem(DataTable dataTable, XmlHelper xmlDoc)
            {
                var proOptions = !dataTable.IsNullOrEmpty() ? dataTable.ToList<ProfessionOption>() : new List<ProfessionOption>();
                proOptions.ForEach(option =>
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
                    xmlDoc.AppendNode($"NursingRecords/NursingRecord[@Code='{option.PROFESSION_ID.ToString()}']", "RecordProp", "", itemOptions);

                    if (!option.OPTION_VALUE.IsNullOrEmpty())
                    {
                        string[] splitList = option.OPTION_VALUE.Split(new char[] { '/' });
                        splitList.ForEach(p1 =>
                        {
                            xmlDoc.AppendNode($"NursingRecords/NursingRecord[@Code='{option.PROFESSION_ID.ToString()}']/RecordProp[@Code='{option.OPTION_NAME}']", "Item", p1);
                        });
                    }
                });
            }
        }
    }
}