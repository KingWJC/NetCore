using System;
using System.Data;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Aspose.Cells;

namespace ADF.Utility
{
    public class ExcelHelper
    {
        #region 生成Excel(Aspose.Cells)
        public static void WriteAsposeFile(DataSet dataSet, string saveFileName)
        {
            Workbook workbook = WriteAspose(dataSet);
            workbook.Save(saveFileName);
        }

        public static Stream WriteAsposeStream(DataSet dataSet)
        {
            Workbook workbook = WriteAspose(dataSet);
            return workbook.SaveToStream();
        }

        /// <summary>
        /// 生成Excel的Workbook
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="saveFileName"></param>
        public static Workbook WriteAspose(DataSet ds)
        {
            Aspose.Cells.Workbook wb = new Aspose.Cells.Workbook();
            wb.Worksheets.Clear();

            Style stHeadCenter = CreateHeaderStyle(wb);
            //内容样式
            Style stContentLeft = wb.CreateStyle();
            stContentLeft.HorizontalAlignment = TextAlignmentType.Left;
            stContentLeft.Font.Size = 10;

            try
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    for (int p = 0; p < ds.Tables.Count; p++)
                    {
                        DataTable dtSource = ds.Tables[p];
                        Aspose.Cells.Worksheet ws = wb.Worksheets.Add(dtSource.TableName);
                        WriteAspose(dtSource, ws, stHeadCenter, stContentLeft);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return wb;
        }

        private static void WriteAspose(DataTable dtList, Worksheet ws, Style stHeadCenter, Style stContentLeft)
        {
            Cells cell = ws.Cells;
            //设置行高
            cell.SetRowHeight(0, 20);

            //赋值给Excel内容
            for (int col = 0; col < dtList.Columns.Count; col++)
            {
                //设置表头
                Style stHead = stHeadCenter;
                Style stContent = stContentLeft;

                putValue(cell, dtList.Columns[col].ColumnName, 0, col, stHead);

                for (int row = 0; row < dtList.Rows.Count; row++)
                {
                    putValue(cell, dtList.Rows[row][col], row + 1, col, stContent);
                }

                cell.SetColumnWidth(col, 10);
                ws.AutoFitColumn(col, 0, 150);
            }

            ws.FreezePanes(1, 0, 1, dtList.Columns.Count);
        }

        private static Style CreateHeaderStyle(Workbook wb)
        {
            //表头样式
            Style stHeadCenter = wb.CreateStyle(); ;
            stHeadCenter.HorizontalAlignment = TextAlignmentType.Center;       //文字居中
            stHeadCenter.Font.Name = "宋体";
            stHeadCenter.Font.IsBold = true;                                  //设置粗体
            stHeadCenter.Font.Size = 14;                                      //设置字体大小
                                                                              //设置背景颜色
            stHeadCenter.ForegroundColor = System.Drawing.Color.FromArgb(153, 204, 0);
            stHeadCenter.Pattern = Aspose.Cells.BackgroundType.Solid;
            return stHeadCenter;
        }

        private static void putValue(Cells cell, object value, int row, int column, Style st)
        {
            //填充数据到excel中
            cell[row, column].PutValue(value);
            cell[row, column].SetStyle(st);
        }

        /// <summary>
        /// Aspose第三方组件生成Excel  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">Ilist集合</param>
        /// <param name="filepath">保存的地址</param>
        public static void WriteAspose<T>(IList<T> data, string fileName, string filepath)
        {
            try
            {
                Workbook wb = new Workbook();
                Worksheet ws = (Worksheet)wb.Worksheets[0];

                //表头样式
                Style stHeadCenter = CreateHeaderStyle(wb);
                //内容样式
                Style stContentLeft = wb.CreateStyle();
                stContentLeft.HorizontalAlignment = TextAlignmentType.Left;
                stContentLeft.Font.Size = 10;

                System.Reflection.PropertyInfo[] ps = typeof(T).GetProperties();
                var colIndex = "A";
                foreach (var p in ps)
                {
                    ws.Cells[colIndex + 1].PutValue(p.Name);//设置表头名称  要求表头为中文所以不用 p.name 为字段名称 可在list第一条数据为表头名称
                    ws.Cells[colIndex + 1].SetStyle(stHeadCenter);
                    int i = 1;
                    foreach (var d in data)
                    {
                        ws.Cells[colIndex + i].PutValue(p.GetValue(d, null));
                        ws.Cells[colIndex + i].SetStyle(stContentLeft);
                        i++;
                    }
                    colIndex = getxls_top(colIndex); //((char)(colIndex[0] + 1)).ToString();//表头  A1/A2/
                }
                //workbook.Shared = true;
                wb.Save(filepath);
                GC.Collect();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 生成新的对应的列  A-Z  AA-ZZ
        /// </summary>
        /// <param name="top">当前列</param>
        /// <returns></returns>
        private static string getxls_top(string top)
        {
            char[] toplist = top.ToArray();
            var itemtop = top.Last();
            string topstr = string.Empty;
            if ((char)itemtop == 90)//最后一个是Z
            {
                if (toplist.Count() == 1)
                {
                    topstr = "AA";
                }
                else
                {
                    toplist[0] = (char)(toplist[0] + 1);
                    toplist[toplist.Count() - 1] = 'A';
                    foreach (var item in toplist)
                    {
                        topstr += item.ToString();
                    }
                }
            }
            else//最后一个不是Z  包括top为两个字符
            {
                itemtop = (char)(itemtop + 1);
                toplist[toplist.Count() - 1] = itemtop;

                foreach (var item in toplist)
                {
                    topstr += item.ToString();
                }
            }
            return topstr;
        }
        #endregion

        #region 读取EXCEL(Aspose.Cells)
        /// <summary>
        /// 读取EXCEL(Aspose.Cells)
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static DataSet ExcelRead(Stream stream)
        {
            DataSet ds = new DataSet();
            try
            {
                Workbook wk = new Workbook(stream);
                for (int t = 0; t < wk.Worksheets.Count; t++)
                {
                    Cells cells = wk.Worksheets[t].Cells;

                    // int firstRow = cells.FirstCell.IsMerged ? 1 : 0;
                    int firstRow = GetFirstRowNo(cells);
                    int firstColumn = cells.FirstCell.Column;
                    int totalRows = cells.MaxDataRow + 1;
                    int totalColumns = cells.MaxDataColumn + 1;
                    DataTable dataTable = new DataTable(wk.Worksheets[t].Name);

                    if (totalRows != 0 && totalColumns != 0)
                    {
                        Row row = cells.Rows.GetRowByIndex(firstRow);
                        SetDataColumns(row, firstColumn, totalColumns, dataTable);
                        for (int k = firstRow + 1; k < firstRow + totalRows; k++)
                        {
                            DataRow dataRow = dataTable.NewRow();
                            if (k == cells.Rows.Count)
                            {
                                break;
                            }
                            row = cells.Rows.GetRowByIndex(k);
                            if (row != null && !row.IsBlank)
                            {
                                //Cell cellTem = row.GetCellOrNull(firstColumn);
                                //if (cellTem == null || cellTem.Value == null || string.IsNullOrEmpty(cellTem.Value.ToString()))
                                //    continue;

                                for (int l = firstColumn; l < firstColumn + totalColumns; l++)
                                {
                                    Cell cellOrNull2 = row.GetCellOrNull(l);
                                    if (cellOrNull2 != null)
                                    {
                                        if (cellOrNull2.IsMerged)
                                        {
                                            var range = cellOrNull2.GetMergedRange();
                                            var rowIndex = range.FirstRow;
                                            var columnIndex = range.FirstColumn;
                                            cellOrNull2 = cells.GetCell(rowIndex, columnIndex);
                                        }
                                        SetDataValue(cellOrNull2, dataRow, l - firstColumn);
                                    }
                                }
                                if (!dataRow.ItemArray.All(p => p.GetType() == typeof(DBNull)))
                                    dataTable.Rows.Add(dataRow);
                            }
                        }
                    }
                    ds.Tables.Add(dataTable);
                }
            }
            catch (Exception ex)
            {
                throw ex;
                // LogHelper.WriteLog_LocalTxt("Excel读取错误！" + ex.ToString());
            }
            return ds;
        }

        /// <summary>
        /// 获取Excel中，导入数据的起始行号（排除标题）
        /// </summary>
        /// <param name="cells"></param>
        /// <returns></returns>
        private static int GetFirstRowNo(Cells cells)
        {
            int firstRow = 0;
            for (int row = 0; row < cells.Count; row++)
            {
                if (!cells[row, 0].IsMerged)
                {
                    firstRow = row;
                    break;
                }
            }
            return firstRow;
        }

        /// <summary>
        /// 设置数据的列配置
        /// </summary>
        /// <param name="row"></param>
        /// <param name="firstColumn"></param>
        /// <param name="totalColumns"></param>
        /// <param name="dataTable"></param>
        private static void SetDataColumns(Row row, int firstColumn, int totalColumns, DataTable dataTable)
        {
            if (row != null)
            {
                for (int i = firstColumn; i < totalColumns + firstColumn; i++)
                {
                    Cell cellOrNull = row.GetCellOrNull(i);

                    if (cellOrNull != null && !string.IsNullOrEmpty(cellOrNull.StringValue))
                    {
                        DataColumn dc = dataTable.Columns.Add();
                        SetColumnType(dataTable, i - firstColumn, cellOrNull);
                        dc.ColumnName = cellOrNull.StringValue.Replace("\n", "").Trim();
                    }
                    else
                    {
                        totalColumns = i - firstColumn;
                        break;
                    }
                }
            }
            else
            {
                for (int j = 0; j < totalColumns; j++)
                {
                    dataTable.Columns.Add();
                    dataTable.Columns[j].DataType = typeof(string);
                }
            }
        }

        /// <summary>
        /// 设置列类型 
        /// </summary>
        /// <param name="dataTable_0"></param>
        /// <param name="int_0"></param>
        /// <param name="cell_0"></param>
        private static void SetColumnType(DataTable dataTable_0, int int_0, Cell cell_0)
        {
            Style style = cell_0.GetStyle();
            switch (cell_0.Type)
            {
                case CellValueType.IsBool:
                    dataTable_0.Columns[int_0].DataType = typeof(bool);
                    return;
                case CellValueType.IsDateTime:
                    dataTable_0.Columns[int_0].DataType = typeof(DateTime);
                    return;
                case CellValueType.IsError:
                    break;
                case CellValueType.IsNull:
                    if (style.IsDateTime)
                    {
                        dataTable_0.Columns[int_0].DataType = typeof(DateTime);
                        return;
                    }
                    break;
                case CellValueType.IsNumeric:
                    if (cell_0.Value is double)
                    {
                        dataTable_0.Columns[int_0].DataType = typeof(double);
                        return;
                    }
                    dataTable_0.Columns[int_0].DataType = typeof(int);
                    break;
                case CellValueType.IsString:
                    dataTable_0.Columns[int_0].DataType = typeof(string);
                    return;
                default:
                    return;
            }
        }

        /// <summary>
        /// 在DataTable中填充数据 
        /// </summary>
        /// <param name="dataTable_0"></param>
        /// <param name="int_0"></param>
        /// <param name="cell_0"></param>
        private static void SetDataValue(Cell cellOrNull2, DataRow dataRow, int columnIndex)
        {

            switch (cellOrNull2.Type)
            {
                case CellValueType.IsBool:
                    dataRow[columnIndex] = cellOrNull2.BoolValue;
                    break;
                case CellValueType.IsDateTime:
                    if (dataRow.Table.Columns[columnIndex].DataType == typeof(DateTime))
                    {
                        dataRow[columnIndex] = cellOrNull2.DateTimeValue;
                    }
                    else
                    {
                        dataRow[columnIndex] = cellOrNull2.StringValue;
                    }
                    break;
                case CellValueType.IsError:
                case CellValueType.IsString:
                case CellValueType.IsUnknown:
                    {
                        string stringValue = cellOrNull2.StringValue;
                        if (stringValue != null && stringValue != "")
                        {
                            dataRow[columnIndex] = stringValue;
                        }
                        break;
                    }
                case CellValueType.IsNumeric:
                    if (dataRow.Table.Columns[columnIndex].DataType == typeof(double))
                    {
                        dataRow[columnIndex] = cellOrNull2.DoubleValue;
                    }
                    else
                    {
                        if (dataRow.Table.Columns[columnIndex].DataType == typeof(int))
                        {
                            dataRow[columnIndex] = cellOrNull2.IntValue;
                        }
                        else
                        {
                            if (dataRow.Table.Columns[columnIndex].DataType == typeof(string))
                            {
                                dataRow[columnIndex] = cellOrNull2.DoubleValue.ToString().Trim();
                            }
                            else
                            {
                                dataRow[columnIndex] = cellOrNull2.DateTimeValue;
                            }
                        }
                    }
                    break;
            }
        }
        #endregion

        #region 文件格式转换
        /// <summary>
        /// Excel转换html
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ExcelToHtml(string path)
        {
            string htmlPath = string.Empty;
            try
            {
                Workbook wk = new Workbook(path);
                htmlPath = path.Replace(".xlsx", ".html").Replace(".xls", ".html");
                wk.Save(htmlPath, SaveFormat.Html);
            }
            catch (Exception ex)
            {
                throw ex;
                // LogHelper.WriteLog_LocalTxt("Excel读取错误！" + ex.ToString());
            }

            return htmlPath;
        }

        /// <summary>
        /// Excel转换pdf
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string ExcelToPDF(string path)
        {
            var htmlPath = string.Empty;
            try
            {
                Workbook wk = new Workbook(path);

                htmlPath = path.Replace(".xlsx", ".pdf").Replace(".xls", ".pdf");
                htmlPath = htmlPath.Insert(htmlPath.LastIndexOf('\\'), "\\pdf");

                Aspose.Cells.PdfSaveOptions xlsSaveOption = new Aspose.Cells.PdfSaveOptions();
                xlsSaveOption.SecurityOptions = new Aspose.Cells.Rendering.PdfSecurity.PdfSecurityOptions();
                //Disable extracting content permission
                xlsSaveOption.SecurityOptions.ExtractContentPermission = false;
                //Disable print permission
                xlsSaveOption.SecurityOptions.PrintPermission = false;
                xlsSaveOption.OnePagePerSheet = true;
                //xlsSaveOption.DefaultFont = "Microsoft YaHei";
                wk.Save(htmlPath, xlsSaveOption);

                //PdfSaveOptions pdfOption = new PdfSaveOptions();
                //pdfOption.OnePagePerSheet = true;
                //FileStream fs = new FileStream(htmlPath, FileMode.Create);
                //wk.Save(fs, pdfOption);

                // var dire = htmlPath.Remove(htmlPath.LastIndexOf('\\'));
                // if (!Directory.Exists(dire))
                //     Directory.CreateDirectory(dire);

                // if (!File.Exists(htmlPath))
                // {

                // }
            }
            catch (Exception ex)
            {
                throw ex;
                // LogHelper.WriteLog_LocalTxt("Excel读取错误！" + ex.ToString());
            }
            return htmlPath;
        }

        #endregion

        // /// <summary>
        // /// 按模板导出数据
        // /// </summary>
        // /// <param name="appPath"></param>
        // /// <param name="tempID"></param>
        // /// <returns></returns>
        // public static string TempTableToExcel(DataSet dtList, string appPath, string tempID, string tableName = "", string title = "", string hosp = "")
        // {
        //     try
        //     {
        //         string tempPath = Path.Combine(appPath + "ImportTemplate.xml");
        //         if (File.Exists(tempPath))
        //         {
        //             XmlDocument doc = new XmlDocument();
        //             doc.Load(tempPath);
        //             XmlNode node = doc.SelectSingleNode($"doc/table[@id='{tempID}']");
        //             if (node != null)
        //             {
        //                 string tempName = node.Attributes["name"].Value.Replace("模板", "");
        //                 if (!string.IsNullOrWhiteSpace(tableName))
        //                     tempName = tableName;
        //                 string path = !string.IsNullOrWhiteSpace(tableName) ? tableName : tempName + " - " + DateTime.Now.Year.ToString();
        //                 string SavePath = Path.Combine(appPath + $"File\\{path}.xlsx");

        //                 if (!Directory.Exists(appPath + "\\File"))
        //                 {
        //                     Directory.CreateDirectory(appPath + "\\File");
        //                 }

        //                 Workbook wb = new Workbook();
        //                 Worksheet ws = wb.Worksheets[0];
        //                 //ws.Name = tempName;
        //                 Cells cell = ws.Cells;
        //                 int firstRow = 0;

        //                 XmlNodeList rowList = node.SelectNodes("row");
        //                 XmlNodeList fieldList = node.SelectNodes("field");
        //                 if (fieldList.Count > 0 && dtList != null && dtList.Tables.Count > 0)
        //                 {
        //                     var tableData = dtList.Tables[0];

        //                     //设置行高
        //                     cell.SetRowHeight(0, 20);

        //                     //设置字体样式
        //                     Style style1 = wb.CreateStyle();
        //                     style1.HorizontalAlignment = TextAlignmentType.Center;//文字居中
        //                     style1.Font.Name = "宋体";
        //                     style1.Font.IsBold = true;//设置粗体
        //                     style1.Font.Size = 16;//设置字体大小

        //                     Style style2 = wb.CreateStyle();
        //                     style2.HorizontalAlignment = TextAlignmentType.Center;
        //                     style2.Font.Size = 12;
        //                     style2.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);
        //                     style2.SetBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black);
        //                     style2.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
        //                     style2.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);

        //                     Style style3 = wb.CreateStyle();
        //                     style3.HorizontalAlignment = TextAlignmentType.Left;

        //                     //设置标题
        //                     if (rowList.Count > 0)
        //                     {
        //                         for (int rowIndex = 0; rowIndex < rowList.Count; rowIndex++)
        //                         {
        //                             switch (rowList[rowIndex].Attributes["id"].Value)
        //                             {
        //                                 case "TITLE":
        //                                     cell[rowIndex, 0].PutValue(title);
        //                                     break;
        //                                 case "HOSPTIAL":
        //                                     cell[rowIndex, 0].PutValue(hosp);
        //                                     break;
        //                                 default:
        //                                     cell[rowIndex, 0].PutValue(rowList[rowIndex].InnerText.Trim());
        //                                     break;
        //                             }

        //                             //合并单元格
        //                             Range range = cell.CreateRange(rowIndex, 0, 1, fieldList.Count);
        //                             range.Merge();
        //                             switch (rowList[rowIndex].Attributes["style"].Value)
        //                             {
        //                                 case "1":
        //                                     range.SetStyle(style1);
        //                                     break;
        //                                 case "2":
        //                                     range.SetStyle(style3);
        //                                     break;
        //                             }
        //                             firstRow++;
        //                         }
        //                     }

        //                     //数据符合模板
        //                     for (int j = 0; j < tableData.Columns.Count;)
        //                     {
        //                         var flag = false;
        //                         var nodeList = node.SelectSingleNode($"field[@id='{tableData.Columns[j].ColumnName}']");
        //                         if (nodeList != null)
        //                         {
        //                             if (tableData.Rows.Count > 0)
        //                             {
        //                                 if (tableData.Columns[j].ColumnName == "DYN_GRAD" && string.IsNullOrWhiteSpace(tableData.Rows[0][tableData.Columns[j].ColumnName].ToString()))
        //                                 {
        //                                     tableData.Columns.RemoveAt(j);
        //                                     flag = true;
        //                                 }
        //                             }
        //                             if (flag) continue;
        //                             tableData.Columns[j].ColumnName = nodeList.InnerText.Trim();
        //                             //tableData.Columns[j].SetOrdinal(Convert.ToInt32(nodeList.Attributes["order"].Value));
        //                             j++;
        //                         }
        //                         else
        //                         {
        //                             tableData.Columns.RemoveAt(j);
        //                         }
        //                     }


        //                     string[,] _ReportDt = new string[tableData.Rows.Count, tableData.Columns.Count];
        //                     for (int i = 0; i < tableData.Rows.Count; i++)
        //                     {
        //                         for (int j = 0; j < tableData.Columns.Count; j++)
        //                         {
        //                             _ReportDt[i, j] = tableData.Rows[i]["" + tableData.Columns[j].ColumnName.ToString().Trim() + ""].ToString().Trim();
        //                         }
        //                     }

        //                     //设置Execl列名
        //                     for (int i = 0; i < tableData.Columns.Count; i++)
        //                     {
        //                         var nodeList = node.SelectSingleNode($"field[text()='{tableData.Columns[i].ColumnName}']");

        //                         cell[firstRow, i].PutValue(tableData.Columns[i].ColumnName.ToString().Trim());
        //                         cell[firstRow, i].SetStyle(style2);

        //                         if (nodeList.Attributes["width"] != null)
        //                             cell.SetColumnWidth(i, Convert.ToInt32(nodeList.Attributes["width"].Value));
        //                         else
        //                             ws.AutoFitColumn(i);
        //                     }
        //                     //赋值给Excel内容
        //                     for (int i = 0; i < _ReportDt.Length / tableData.Columns.Count; i++)
        //                     {
        //                         for (int j = 0; j < tableData.Columns.Count; j++)
        //                         {
        //                             cell[i + firstRow + 1, j].PutValue(_ReportDt[i, j].ToString().Trim());
        //                             cell[i + firstRow + 1, j].SetStyle(style3);
        //                         }
        //                     }


        //                     if (File.Exists(SavePath))
        //                     {
        //                         File.Delete(SavePath);
        //                     }
        //                     wb.Save(SavePath);


        //                 }
        //             }
        //         }

        //     }
        //     catch (Exception ex)
        //     {
        //         LogHelper.WriteLog_LocalTxt("Excel结果导出错误！" + ex.ToString());
        //     }
        //     return string.Empty;
        // }

        // /// <summary>
        // /// 下载模板
        // /// </summary>
        // /// <param name="tempID">模板ID，表名称</param>
        // /// <param name="typeId">指标类别 1 材料指标 2（脓毒症专病）|3（ICU专科） 数据指标 4 现场指标</param>
        // /// <returns>文件的完整路径</returns>
        // public static bool TemplateToExcel(string savePath, string tempName, XmlNodeList rowList, XmlNodeList fieldList, XmlNodeList nodeList)
        // {
        //     try
        //     {

        //         Workbook wb = new Workbook();
        //         Worksheet ws = wb.Worksheets[0];
        //         ws.Name = tempName;
        //         Cells cell = ws.Cells;
        //         int firstRow = 0;

        //         if (fieldList.Count > 0)
        //         {
        //             //设置行高
        //             cell.SetRowHeight(0, 30);

        //             //设置字体样式
        //             Style style1 = wb.CreateStyle();
        //             style1.HorizontalAlignment = TextAlignmentType.Center;//文字居中
        //             style1.Font.Name = "宋体";
        //             style1.Font.IsBold = true;//设置粗体
        //             style1.Font.Size = 16;//设置字体大小

        //             Style style2 = wb.CreateStyle();
        //             style2.HorizontalAlignment = TextAlignmentType.Left;
        //             style2.Font.Size = 12;
        //             style2.Font.IsBold = true;
        //             style2.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);
        //             style2.SetBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black);
        //             style2.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
        //             style2.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);
        //             style2.Pattern = BackgroundType.Solid;
        //             style2.ForegroundColor = Color.LightBlue;



        //             Style style3 = wb.CreateStyle();
        //             style3.HorizontalAlignment = TextAlignmentType.Left;

        //             Style styleValue = wb.CreateStyle();
        //             styleValue.HorizontalAlignment = TextAlignmentType.Center;
        //             styleValue.Font.Size = 12;
        //             styleValue.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);
        //             styleValue.SetBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black);
        //             styleValue.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
        //             styleValue.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);

        //             //数据验证
        //             ValidationCollection validations = ws.Validations;

        //             //设置标题
        //             if (rowList.Count > 0)
        //             {
        //                 for (int rowIndex = 0; rowIndex < rowList.Count; rowIndex++)
        //                 {
        //                     cell[rowIndex, 0].PutValue(rowList[rowIndex].InnerText.Trim());
        //                     //合并单元格
        //                     Range range = cell.CreateRange(rowIndex, 0, 1, fieldList.Count);
        //                     range.Merge();
        //                     switch (rowList[rowIndex].Attributes["style"].Value)
        //                     {
        //                         case "1":
        //                             range.SetStyle(style1);
        //                             break;
        //                         case "2":
        //                             range.SetStyle(style3);
        //                             break;
        //                     }
        //                     firstRow++;
        //                 }
        //             }

        //             //设置Execl列名
        //             for (int i = 0; i < fieldList.Count; i++)
        //             {
        //                 cell[firstRow, i].PutValue(fieldList[i].InnerText.Trim());
        //                 cell[firstRow, i].SetStyle(style2);
        //                 if (fieldList[i].Attributes["type"] != null)
        //                 {
        //                     if (fieldList[i].Attributes["type"].Value == "option")
        //                     {
        //                         Validation validation = validations[validations.Add()];
        //                         validation.Type = Aspose.Cells.ValidationType.List;
        //                         validation.InCellDropDown = true;

        //                         var dict = GetDictList(fieldList[i].Attributes["dictcode"].Value);
        //                         validation.Formula1 = String.Join(",", dict.Select(d => d.Value));
        //                         CellArea area;
        //                         area.StartRow = 1;
        //                         area.EndRow = 100;
        //                         area.StartColumn = i;
        //                         area.EndColumn = i;
        //                         validation.AreaList.Add(area);
        //                     }
        //                 }

        //                 if (fieldList[i].Attributes["width"] != null)
        //                     cell.SetColumnWidth(i, Convert.ToInt32(fieldList[i].Attributes["width"].Value));
        //                 else
        //                     ws.AutoFitColumn(i);
        //             }

        //             //赋值给Excel内容
        //             for (int i = 0; i < nodeList.Count; i++)
        //             {
        //                 cell[firstRow + 1, i].PutValue(nodeList[i].InnerText.Trim());
        //                 cell[firstRow + 1, i].SetStyle(styleValue);
        //                 if (i == nodeList.Count - 1)
        //                     cell[firstRow + 1, i + 1].PutValue("上传前请删除示意行");

        //                 //if (fieldList[i].Attributes["width"] != null)
        //                 //    cell.SetColumnWidth(i, Convert.ToInt32(nodeList[i].Attributes["width"].Value));
        //                 //else
        //                 //    ws.AutoFitColumn(i);
        //             }


        //             if (File.Exists(savePath))
        //             {
        //                 File.Delete(savePath);
        //             }
        //             wb.Save(savePath);

        //             //savePath.Replace(appPath, "/").Replace("\\", "/");
        //         }
        //         return true;
        //     }
        //     catch (Exception ex)
        //     {
        //         LogHelper.WriteLog_LocalTxt("Excel结果导出错误！" + ex.ToString());
        //         return false;
        //     }
        // }

        // /// <summary>
        // /// 获取Excel的实体集合
        // /// </summary>
        // /// <typeparam name="T"></typeparam>
        // /// <param name="filePath"></param>
        // /// <param name="temName"></param>
        // /// <returns></returns>
        // public static List<T> GetExcelDataList<T>(string filePath, string tempID)
        // {
        //     try
        //     {
        //         string appPath = FileHelper.GetCurrentDir();
        //         string tempPath = Path.Combine(appPath + "ImportTemplate.xml");
        //         if (File.Exists(tempPath))
        //         {
        //             DataSet ds = ExcelRead(filePath);
        //             if (ds != null && ds.Tables.Count > 0)
        //             {
        //                 DataTable dt = ds.Tables[0];
        //                 if (dt != null && dt.Rows.Count > 0)
        //                 {
        //                     XmlDocument doc = new XmlDocument();
        //                     doc.Load(tempPath);
        //                     XmlNode node = doc.DocumentElement.SelectSingleNode($"//table[@id='{tempID}']");
        //                     if (node != null && node.HasChildNodes)
        //                     {
        //                         for (int i = 0; i < dt.Columns.Count;)
        //                         {
        //                             XmlNode nodeField = node.SelectSingleNode($"field[text()='{dt.Columns[i].ColumnName.Trim()}']");
        //                             if (nodeField != null && nodeField.Attributes["id"].Value != "-")
        //                             {
        //                                 dt.Columns[i].ColumnName = nodeField.Attributes["id"].Value;
        //                                 i++;
        //                             }
        //                             else
        //                             {
        //                                 dt.Columns.RemoveAt(i);
        //                             }
        //                         }
        //                         if (dt.Columns.Count > 0)
        //                             return ToEntityList<T>(dt);
        //                     }
        //                 }
        //             }
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         LogHelper.WriteLog_LocalTxt("获取Excel的实体集合错误！" + ex.ToString());
        //     }
        //     return null;
        // }

        // /// <summary>
        // /// 获取Excel的实体集合,字典数据转换。
        // /// </summary>
        // /// <typeparam name="T"></typeparam>
        // /// <param name="templatePath"></param>
        // /// <param name="filePath"></param>
        // /// <param name="tempID"></param>
        // /// <param name="func"></param>
        // /// <returns></returns>
        // public static List<T> GetExcelDataList<T>(string templatePath, string filePath, string tempID, out string message, Func<string, Dictionary<string, string>> func = null)
        // {
        //     message = string.Empty;
        //     try
        //     {
        //         string tempPath = Path.Combine(templatePath + "\\ImportTemplate.xml");
        //         if (File.Exists(tempPath))
        //         {
        //             DataSet ds = ExcelRead(filePath);
        //             if (ds != null && ds.Tables.Count > 0)
        //             {
        //                 DataTable dt = ds.Tables[0];
        //                 if (dt != null && dt.Rows.Count > 0)
        //                 {
        //                     XmlDocument doc = new XmlDocument();
        //                     doc.Load(tempPath);
        //                     XmlNode node = doc.DocumentElement.SelectSingleNode($"//table[@id='{tempID}']");
        //                     if (node != null && node.HasChildNodes)
        //                     {
        //                         for (int i = 0; i < dt.Columns.Count;)
        //                         {
        //                             XmlNode nodeField = node.SelectSingleNode($"field[text()='{dt.Columns[i].ColumnName.Trim()}']");
        //                             if (nodeField != null && nodeField.Attributes["id"].Value != "-")
        //                             {
        //                                 dt.Columns[i].ColumnName = nodeField.Attributes["id"].Value.Trim();
        //                                 i++;
        //                             }
        //                             else
        //                             {
        //                                 dt.Columns.RemoveAt(i);
        //                             }
        //                         }

        //                         var optionNodes = node.SelectNodes($"field[@type='option']");
        //                         if (func != null && optionNodes.Count > 0)
        //                         {
        //                             foreach (XmlNode option in optionNodes)
        //                             {
        //                                 var optionList = func(option.Attributes["dictcode"].Value);
        //                                 for (int i = 0; i < dt.Rows.Count; i++)
        //                                 {
        //                                     if (!string.IsNullOrWhiteSpace(dt.Rows[i][option.Attributes["id"].Value].ToString().Trim()))
        //                                     {
        //                                         var value = optionList.Where(p => p.Value == dt.Rows[i][option.Attributes["id"].Value].ToString().Trim()).Select(p => p.Key).FirstOrDefault();
        //                                         if (string.IsNullOrEmpty(value))
        //                                             message += $"字典中没有这条“{dt.Rows[i][option.Attributes["id"].Value].ToString()}”数据;";
        //                                         dt.Rows[i][option.Attributes["id"].Value] = value.Trim();
        //                                     }
        //                                 }
        //                             }
        //                         }
        //                         if (dt.Columns.Count > 0)
        //                             return ToEntityList<T>(dt);
        //                     }
        //                 }
        //             }
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         LogHelper.WriteLog_LocalTxt("获取Excel的实体集合错误！" + ex.ToString());
        //     }
        //     return null;
        // }
    }
}