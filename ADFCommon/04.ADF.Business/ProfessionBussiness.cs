using System;
using System.Text;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Collections.Generic;
using ADF.IBusiness;
using ADF.Entity;
using ADF.Utility;
using ADF.DataAccess.ORM;

namespace ADF.Business
{
    public class ProfessionBussiness : BaseBusiness<Profession>, IProfessionBussiness
    {
        public void UpdateProfession(Stream stream)
        {
            DataSet dataSet = ExcelHelper.ExcelRead(stream);
            if (dataSet.Tables.Count > 0)
            {
                //导入病情观察的类别
                if (dataSet.Tables.Contains("DictType"))
                {
                    var dictList = dataSet.Tables["DictType"].ToList<DictType>();
                    Service.Delete<DictType>(dictList.Select(p => p.MICDICT_ID.ToString()).ToList());
                    Service.Insert<DictType>(dictList);
                }

                //更新病情观察的每个数据项的配置
                ConfigInfo config = Service.GetEntity<ConfigInfo>("6");
                using (Stream xmlStream = new MemoryStream((byte[])config.CONFIG_CONTEXT.ChangeType_ByConvert(typeof(byte[]))))
                {
                    XmlHelper xmlDoc = new XmlHelper(xmlStream);
                    if (xmlDoc.IsEmpty)
                    {
                        xmlDoc.AppendDeclaration();
                        var root = xmlDoc.AppendRootNode("NursingRecordConfig");
                        xmlDoc.AppendNode("NursingRecordConfig", "NursingRecords");
                        xmlDoc.AppendNode("NursingRecordConfig", "NursingTemplate");
                    }

                    //导入病情观察项目
                    if (dataSet.Tables.Contains("Profession"))
                        UpdateProfessionItem(dataSet.Tables["Profession"]);
                    //生成病情观察项目和选择项的xml
                    if (dataSet.Tables.Contains("ProfessionOption"))
                        UpdateOptionItem(dataSet.Tables["ProfessionOption"], xmlDoc);
                    //生成病情观察结果描述的xml
                    if (dataSet.Tables.Contains("ProfessionContext"))
                        UpdateProfessionContext(dataSet.Tables["ProfessionContext"], xmlDoc);

                    config.CONFIG_CONTEXT = xmlDoc.Document.OuterXml;
                    Service.UpdateAny<ConfigInfo>(config, new List<string> { "CONFIG_CONTEXT" });

                    FileHelper.WriteTxt(xmlDoc.Document.OuterXml, @"E:\data.xml");
                }
            }

            void UpdateProfessionItem(DataTable dataTable)
            {
                var proList = dataTable != null ? dataTable.ToList<Profession>() : new List<Profession>();
                //导入病情观察的数据项
                Delete(proList.Select(p => p.PROFESSION_ID.ToString()).ToList());
                Insert(proList);
            }

            void UpdateOptionItem(DataTable dataTable, XmlHelper xmlDoc)
            {
                var proOptions = !dataTable.IsNullOrEmpty() ? dataTable.ToList<ProfessionOption>() : new List<ProfessionOption>();

                var list = proOptions.Select(p => new { p.PROFESSION_ID, p.PROFESSION_NAME }).Distinct();
                list.ForEach(x =>
                     {
                         xmlDoc.RemoveNode($"NursingRecords/NursingRecord[@Code='{x.PROFESSION_ID.ToString()}']");

                         var selectOptions = new List<SelectOption>
                             {
                                new SelectOption { name = "Code", value = x.PROFESSION_ID.ToString() },
                                new SelectOption { name = "Name", value = x.PROFESSION_NAME },
                                new SelectOption { name = "Caption", value = x.PROFESSION_NAME }
                             };
                         xmlDoc.AppendNode("NursingRecords", "NursingRecord", "", selectOptions);
                     });

                proOptions.ForEach(option =>
                {
                    var itemOptions = new List<SelectOption>{
                        new SelectOption { name = "Code", value = option.OPTION_NAME },
                        new SelectOption { name = "Name", value = option.OPTION_NAME },
                        new SelectOption { name = "Caption", value = option.OPTION_NAME },
                        new SelectOption { name = "Option", value = option.OPTION_TYPE },
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

            void UpdateProfessionContext(DataTable dataTable, XmlHelper xmlDoc)
            {
                var contexts = !dataTable.IsNullOrEmpty() ? dataTable.ToList<ProfessionContext>() : new List<ProfessionContext>();

                contexts.ForEach(p =>
                {
                    xmlDoc.RemoveNode($"NursingTemplate/NursingTemplate[@Name='{p.PROFESSION_NAME}']");
                    
                    xmlDoc.AppendNode("NursingTemplate", "NursingTemplate", p.PROFESSION_CONTEXT, new List<SelectOption> { new SelectOption { name = "Name", value = p.PROFESSION_NAME } });

                });
            }
        }

        public Stream ExportProfession()
        {
            DataSet dataSet = new DataSet();

            //病情观察类别
            var professionTypeList = Service.GetDataTableWithSql("select * from JHICU_DICT_MICDICT_DETAIL where type_id =:type_id", new List<CusDbParameter>() { new CusDbParameter(":type_id", 71) });
            professionTypeList.TableName = "DictType";
            dataSet.Tables.Add(professionTypeList.Copy());

            //病情观察项目
            var professionList = Service.GetList<Profession>().ToDataTable();
            professionList.TableName = "Profession";
            dataSet.Tables.Add(professionList.Copy());

            //病情观察项目模板
            DataTable dtOptions = new DataTable("ProfessionOption");
            dtOptions.Columns.AddRange(new DataColumn[] { new DataColumn("PROFESSION_ID"), new DataColumn("PROFESSION_NAME"), new DataColumn("OPTION_NAME"), new DataColumn("OPTION_TYPE"), new DataColumn("OPTION_VALUE") });

            DataTable dtContext = new DataTable("ProfessionContent");
            dtContext.Columns.AddRange(new DataColumn[] { new DataColumn("PROFESSION_NAME"), new DataColumn("PROFESSION_CONTEXT") });
            ConfigInfo config = Service.GetEntity<ConfigInfo>("6");
            using (Stream stream = new MemoryStream((byte[])config.CONFIG_CONTEXT.ChangeType_ByConvert(typeof(byte[]))))
            {
                XmlHelper xmldoc = new XmlHelper(stream);

                var records = xmldoc.GetNode("NursingRecords");
                foreach (XmlNode node in records.ChildNodes)
                {
                    var contents = xmldoc.GetNode($"NursingTemplate/NursingTemplate[@Name='{node.Attributes["Name"].Value}']");

                    if (dtContext.Select("PROFESSION_NAME='" + node.Attributes["Name"].Value + "'").Count() == 0)
                    {
                        DataRow drNew = dtContext.NewRow();
                        drNew["PROFESSION_NAME"] = node.Attributes["Name"].Value;
                        drNew["PROFESSION_CONTEXT"] = contents.InnerText;
                        dtContext.Rows.Add(drNew);
                    }

                    foreach (XmlNode proNode in node.ChildNodes)
                    {
                        DataRow dr = dtOptions.NewRow();
                        dr["PROFESSION_ID"] = node.Attributes["Code"].Value;
                        dr["PROFESSION_NAME"] = node.Attributes["Name"].Value;
                        dr["OPTION_NAME"] = proNode.Attributes["Code"].Value;
                        dr["OPTION_TYPE"] = proNode.Attributes["Option"].Value;
                        if (proNode.Attributes["Option"].Value.IsMatch("Option|Check"))
                        {
                            StringBuilder stringBuilder = new StringBuilder();
                            foreach (XmlNode itemNode in proNode.ChildNodes)
                            {
                                stringBuilder.Append(itemNode.InnerText).Append("/");
                            }
                            dr["OPTION_VALUE"] = stringBuilder.ToString().TrimEnd('/');
                        }
                        dtOptions.Rows.Add(dr);
                    }
                }
            }

            dataSet.Tables.Add(dtOptions);
            dataSet.Tables.Add(dtContext);
            return ExcelHelper.WriteAsposeStream(dataSet);
        }
    }
}