using System;
using System.Data;
using Aspose.Cells;

namespace ADF.Utility
{
    public class ExcelHelper
    {
         /// <summary>
        /// Aspose第三方组件生成Excel
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="saveFileName"></param>
        public static void WriteAspose(DataSet ds, string saveFileName)
        {
            Aspose.Cells.Workbook wb = new Aspose.Cells.Workbook();

            try
            {
                if (ds != null && ds.Tables.Count > 0)
                {
                    for (int p = 0; p < ds.Tables.Count; p++)
                    {
                        DataTable dtSource = ds.Tables[p];
                        Aspose.Cells.Worksheet ws = wb.Worksheets.Add(dtSource.TableName);

                        //表头样式
                        Style stHeadLeft = wb.Styles[wb.Styles.Add()];
                        stHeadLeft.HorizontalAlignment = TextAlignmentType.Left;       //文字居中
                        stHeadLeft.Font.Name = "宋体";
                        stHeadLeft.Font.IsBold = true;                                 //设置粗体
                        stHeadLeft.Font.Size = 14;                                     //设置字体大小
                        //设置背景颜色
                        stHeadLeft.ForegroundColor = System.Drawing.Color.FromArgb(153, 204, 0);
                        stHeadLeft.Pattern = Aspose.Cells.BackgroundType.Solid;
                        Style stHeadCenter = wb.Styles[wb.Styles.Add()];
                        stHeadCenter.HorizontalAlignment = TextAlignmentType.Center;       //文字居中
                        stHeadCenter.Font.Name = "宋体";
                        stHeadCenter.Font.IsBold = true;                                  //设置粗体
                        stHeadCenter.Font.Size = 14;                                      //设置字体大小
                        //设置背景颜色
                        stHeadCenter.ForegroundColor = System.Drawing.Color.FromArgb(153, 204, 0);
                        stHeadCenter.Pattern = Aspose.Cells.BackgroundType.Solid;

                        //内容样式
                        Style stContentLeft = wb.Styles[wb.Styles.Add()];
                        stContentLeft.HorizontalAlignment = TextAlignmentType.Left;
                        stContentLeft.Font.Size = 10;

                        WriteAspose(dtSource, ws, stHeadLeft, stHeadCenter, stContentLeft);

                        for (int k = 0; k < dtSource.Columns.Count; k++)
                        {
                            ws.AutoFitColumn(k, 0, 150);
                        }

                        ws.FreezePanes(1, 0, 1, dtSource.Columns.Count);
                    }
                }
                wb.Save(saveFileName);
            }
            catch (Exception e)
            {

            }
        }

        private static void WriteAspose(DataTable dtList, Worksheet ws, Style stHeadLeft, Style stHeadCenter, Style stContentLeft)
        {
            Cells cell = ws.Cells;
            //设置行高
            cell.SetRowHeight(0, 20);

            //赋值给Excel内容
            for (int col = 0; col < dtList.Columns.Count; col++)
            {
                Style stHead = null;
                Style stContent = null;
                //设置表头
                string columnType = dtList.Columns[col].DataType.ToString();
                switch (columnType.ToLower())
                {
                    //如果类型是string，则靠左对齐(对齐方式看项目需求修改)
                    case "system.string":
                        stHead = stHeadLeft;
                        break;
                    default:
                        stHead = stHeadCenter;
                        break;
                }
                stContent = stContentLeft;
                putValue(cell, dtList.Columns[col].ColumnName, 0, col, stHead);

                for (int row = 0; row < dtList.Rows.Count; row++)
                {
                    putValue(cell, dtList.Rows[row][col], row + 1, col, stContent);
                }
            }
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
                Style stHeadCenter = wb.Styles[wb.Styles.Add()];
                stHeadCenter.HorizontalAlignment = TextAlignmentType.Center;       //文字居中
                stHeadCenter.Font.Name = "宋体";
                stHeadCenter.Font.IsBold = true;                                  //设置粗体
                stHeadCenter.Font.Size = 14;                                      //设置字体大小
                //设置背景颜色
                stHeadCenter.ForegroundColor = System.Drawing.Color.FromArgb(153, 204, 0);
                stHeadCenter.Pattern = Aspose.Cells.BackgroundType.Solid;

                //内容样式
                Style stContentLeft = wb.Styles[wb.Styles.Add()];
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

                /// <summary>
        /// 第三方组件Aspose
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static DataSet ReadAspose(string filePath)
        {
            Workbook wk = new Workbook(filePath);
            DataSet ds = new DataSet();
            for (int t = 0; t < wk.Worksheets.Count; t++)
            {
                Cells cells = wk.Worksheets[t].Cells;
                int firstRow = cells.FirstCell.IsMerged ? 1 : 0;
                int firstColumn = cells.FirstCell.Column; ;
                int totalRows = cells.MaxDataRow;
                int totalColumns = cells.MaxDataColumn + 1;
                DataTable dataTable = new DataTable(wk.Worksheets[t].Name);

                if (totalRows != 0 && totalColumns != 0)
                {
                    Row row = cells.Rows.GetRowByIndex(firstRow);
                    if (row != null)
                    {
                        for (int i = firstColumn; i < totalColumns + firstColumn; i++)
                        {
                            Cell cellOrNull = row.GetCellOrNull(i);
                            if (cellOrNull != null && !string.IsNullOrEmpty(cellOrNull.StringValue))
                            {
                                DataColumn dc = dataTable.Columns.Add();
                                SetColumnType(dataTable, i - firstColumn, cellOrNull);
                                dc.ColumnName = cellOrNull.StringValue.Trim();
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
                    for (int k = firstRow + 1; k < firstRow + totalRows; k++)
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataTable.Rows.Add(dataRow);
                        row = cells.Rows.GetRowByIndex(k);
                        if (row != null)
                        {
                            for (int l = firstColumn; l < firstColumn + totalColumns; l++)
                            {
                                Cell cellOrNull2 = row.GetCellOrNull(l);
                                if (cellOrNull2 != null)
                                {
                                    switch (cellOrNull2.Type)
                                    {
                                        case CellValueType.IsBool:
                                            dataRow[l - firstColumn] = cellOrNull2.BoolValue;
                                            break;
                                        case CellValueType.IsDateTime:
                                            if (dataTable.Columns[l - firstColumn].DataType == typeof(DateTime))
                                            {
                                                dataRow[l - firstColumn] = cellOrNull2.DateTimeValue;
                                            }
                                            else
                                            {
                                                dataRow[l - firstColumn] = cellOrNull2.StringValue;
                                            }
                                            break;
                                        case CellValueType.IsError:
                                        case CellValueType.IsString:
                                        case CellValueType.IsUnknown:
                                            {
                                                string stringValue = cellOrNull2.StringValue;
                                                if (stringValue != null && stringValue != "")
                                                {
                                                    dataRow[l - firstColumn] = stringValue;
                                                }
                                                break;
                                            }
                                        case CellValueType.IsNumeric:
                                            if (dataTable.Columns[l - firstColumn].DataType == typeof(double))
                                            {
                                                dataRow[l - firstColumn] = cellOrNull2.DoubleValue;
                                            }
                                            else
                                            {
                                                if (dataTable.Columns[l - firstColumn].DataType == typeof(int))
                                                {
                                                    dataRow[l - firstColumn] = cellOrNull2.IntValue;
                                                }
                                                else
                                                {
                                                    if (dataTable.Columns[l - firstColumn].DataType == typeof(string))
                                                    {
                                                        dataRow[l - firstColumn] = cellOrNull2.DoubleValue.ToString();
                                                    }
                                                    else
                                                    {
                                                        dataRow[l - firstColumn] = cellOrNull2.DateTimeValue;
                                                    }
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
                ds.Tables.Add(dataTable);
            }
            return ds;
        }

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

        #region 
         /// <summary>
        /// EXCEL读取操作
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        public static DataSet ExcelRead(string path)
        {
            DataSet ds = new DataSet();
            try
            {
                Workbook wk = new Workbook(path);
                for (int t = 0; t < wk.Worksheets.Count; t++)
                {
                    Cells cells = wk.Worksheets[t].Cells;

                    int firstRow = 0;
                    for (int row = 0; row < cells.Count; row++)
                    {
                        if (!cells[row, 0].IsMerged)
                        {
                            firstRow = row;
                            break;
                        }
                    }
                    int firstColumn = cells.FirstCell.Column;
                    int totalRows = cells.MaxDataRow + 1;
                    int totalColumns = cells.MaxDataColumn + 1;
                    DataTable dataTable = new DataTable(wk.Worksheets[t].Name);

                    if (totalRows != 0 && totalColumns != 0)
                    {
                        Row row = cells.Rows.GetRowByIndex(firstRow);
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
                                        switch (cellOrNull2.Type)
                                        {
                                            case CellValueType.IsBool:
                                                dataRow[l - firstColumn] = cellOrNull2.BoolValue;
                                                break;
                                            case CellValueType.IsDateTime:
                                                if (dataTable.Columns[l - firstColumn].DataType == typeof(DateTime))
                                                {
                                                    dataRow[l - firstColumn] = cellOrNull2.DateTimeValue;
                                                }
                                                else
                                                {
                                                    dataRow[l - firstColumn] = cellOrNull2.StringValue;
                                                }
                                                break;
                                            case CellValueType.IsError:
                                            case CellValueType.IsString:
                                            case CellValueType.IsUnknown:
                                                {
                                                    string stringValue = cellOrNull2.StringValue;
                                                    if (stringValue != null && stringValue != "")
                                                    {
                                                        dataRow[l - firstColumn] = stringValue;
                                                    }
                                                    break;
                                                }
                                            case CellValueType.IsNumeric:
                                                if (dataTable.Columns[l - firstColumn].DataType == typeof(double))
                                                {
                                                    dataRow[l - firstColumn] = cellOrNull2.DoubleValue;
                                                }
                                                else
                                                {
                                                    if (dataTable.Columns[l - firstColumn].DataType == typeof(int))
                                                    {
                                                        dataRow[l - firstColumn] = cellOrNull2.IntValue;
                                                    }
                                                    else
                                                    {
                                                        if (dataTable.Columns[l - firstColumn].DataType == typeof(string))
                                                        {
                                                            dataRow[l - firstColumn] = cellOrNull2.DoubleValue.ToString().Trim();
                                                        }
                                                        else
                                                        {
                                                            dataRow[l - firstColumn] = cellOrNull2.DateTimeValue;
                                                        }
                                                    }
                                                }
                                                break;
                                        }
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
                LogHelper.WriteLog_LocalTxt("Excel读取错误！" + ex.ToString());
            }
            return ds;
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
        /// 结果导出
        /// </summary>
        /// <param name="dtList"></param>
        /// <param name="SavePath"></param>
        public static void TableToExcel(DataSet dtList, string SavePath)
        {
            try
            {
                Workbook wb = new Workbook();
                foreach (DataTable _NewDt in dtList.Tables)
                {
                    if (_NewDt.Rows.Count > 0)
                    {
                        Worksheet ws = wb.Worksheets.Add(_NewDt.TableName);
                        Cells cell = ws.Cells;

                        //定义并获取导出的数据源(以传递的数组列名为依据)
                        string[,] _ReportDt = new string[_NewDt.Rows.Count, _NewDt.Columns.Count];

                        for (int i = 0; i < _NewDt.Rows.Count; i++)
                        {
                            for (int j = 0; j < _NewDt.Columns.Count; j++)
                            {
                                _ReportDt[i, j] = _NewDt.Rows[i]["" + _NewDt.Columns[j].ColumnName.ToString().Trim() + ""].ToString().Trim();
                            }
                        }

                        //设置行高
                        cell.SetRowHeight(0, 20);

                        //设置字体样式
                        Style style1 = wb.Styles[wb.Styles.Add()];
                        style1.HorizontalAlignment = TextAlignmentType.Center;//文字居中
                        style1.Font.Name = "宋体";
                        style1.Font.IsBold = true;//设置粗体
                        style1.Font.Size = 16;//设置字体大小

                        Style style2 = wb.Styles[wb.Styles.Add()];
                        style2.HorizontalAlignment = TextAlignmentType.Center;
                        style2.Font.Size = 12;

                        Style style3 = wb.Styles[wb.Styles.Add()];
                        style3.HorizontalAlignment = TextAlignmentType.Right;

                        //设置Execl列名
                        for (int i = 0; i < _NewDt.Columns.Count; i++)
                        {
                            cell[0, i].PutValue(_NewDt.Columns[i].ColumnName.ToString().Trim());
                            cell[0, i].SetStyle(style2);
                        }
                        //赋值给Excel内容
                        for (int i = 0; i < _ReportDt.Length / _NewDt.Columns.Count; i++)
                        {
                            for (int j = 0; j < _NewDt.Columns.Count; j++)
                            {
                                cell[i + 1, j].PutValue(_ReportDt[i, j].ToString().Trim());
                                cell[i + 1, j].SetStyle(style3);
                            }
                        }
                        //设置列宽
                        for (int i = 0; i < _NewDt.Columns.Count; i++)
                        {
                            cell.SetColumnWidth(i, 10);
                        }
                    }
                }
                wb.Worksheets.RemoveAt(0);
                if (File.Exists(SavePath))
                {
                    File.Delete(SavePath);
                }
                wb.Save(SavePath);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog_LocalTxt("Excel结果导出错误！" + ex.ToString());
            }
        }

        /// <summary>
        /// 按模板导出数据
        /// </summary>
        /// <param name="appPath"></param>
        /// <param name="tempID"></param>
        /// <returns></returns>
        public static string TempTableToExcel(DataSet dtList, string appPath, string tempID, string tableName = "", string title = "", string hosp = "")
        {
            try
            {
                string tempPath = Path.Combine(appPath + "ImportTemplate.xml");
                if (File.Exists(tempPath))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(tempPath);
                    XmlNode node = doc.SelectSingleNode($"doc/table[@id='{tempID}']");
                    if (node != null)
                    {
                        string tempName = node.Attributes["name"].Value.Replace("模板", "");
                        if (!string.IsNullOrWhiteSpace(tableName))
                            tempName = tableName;
                        string path = !string.IsNullOrWhiteSpace(tableName) ? tableName : tempName + " - " + DateTime.Now.Year.ToString();
                        string SavePath = Path.Combine(appPath + $"File\\{path}.xlsx");

                        if (!Directory.Exists(appPath + "\\File"))
                        {
                            Directory.CreateDirectory(appPath + "\\File");
                        }

                        Workbook wb = new Workbook();
                        Worksheet ws = wb.Worksheets[0];
                        //ws.Name = tempName;
                        Cells cell = ws.Cells;
                        int firstRow = 0;

                        XmlNodeList rowList = node.SelectNodes("row");
                        XmlNodeList fieldList = node.SelectNodes("field");
                        if (fieldList.Count > 0 && dtList != null && dtList.Tables.Count > 0)
                        {
                            var tableData = dtList.Tables[0];

                            //设置行高
                            cell.SetRowHeight(0, 20);

                            //设置字体样式
                            Style style1 = wb.Styles[wb.Styles.Add()];
                            style1.HorizontalAlignment = TextAlignmentType.Center;//文字居中
                            style1.Font.Name = "宋体";
                            style1.Font.IsBold = true;//设置粗体
                            style1.Font.Size = 16;//设置字体大小

                            Style style2 = wb.Styles[wb.Styles.Add()];
                            style2.HorizontalAlignment = TextAlignmentType.Center;
                            style2.Font.Size = 12;
                            style2.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);
                            style2.SetBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black);
                            style2.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
                            style2.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);

                            Style style3 = wb.Styles[wb.Styles.Add()];
                            style3.HorizontalAlignment = TextAlignmentType.Left;

                            //设置标题
                            if (rowList.Count > 0)
                            {
                                for (int rowIndex = 0; rowIndex < rowList.Count; rowIndex++)
                                {
                                    switch (rowList[rowIndex].Attributes["id"].Value)
                                    {
                                        case "TITLE":
                                            cell[rowIndex, 0].PutValue(title);
                                            break;
                                        case "HOSPTIAL":
                                            cell[rowIndex, 0].PutValue(hosp);
                                            break;
                                        default:
                                            cell[rowIndex, 0].PutValue(rowList[rowIndex].InnerText.Trim());
                                            break;
                                    }

                                    //合并单元格
                                    Range range = cell.CreateRange(rowIndex, 0, 1, fieldList.Count);
                                    range.Merge();
                                    switch (rowList[rowIndex].Attributes["style"].Value)
                                    {
                                        case "1":
                                            range.SetStyle(style1);
                                            break;
                                        case "2":
                                            range.SetStyle(style3);
                                            break;
                                    }
                                    firstRow++;
                                }
                            }

                            //数据符合模板
                            for (int j = 0; j < tableData.Columns.Count;)
                            {
                                var flag = false;
                                var nodeList = node.SelectSingleNode($"field[@id='{tableData.Columns[j].ColumnName}']");
                                if (nodeList != null)
                                {
                                    if (tableData.Rows.Count > 0)
                                    {
                                        if (tableData.Columns[j].ColumnName == "DYN_GRAD" && string.IsNullOrWhiteSpace(tableData.Rows[0][tableData.Columns[j].ColumnName].ToString()))
                                        {
                                            tableData.Columns.RemoveAt(j);
                                            flag = true;
                                        }
                                    }
                                    if (flag) continue;
                                    tableData.Columns[j].ColumnName = nodeList.InnerText.Trim();
                                    //tableData.Columns[j].SetOrdinal(Convert.ToInt32(nodeList.Attributes["order"].Value));
                                    j++;
                                }
                                else
                                {
                                    tableData.Columns.RemoveAt(j);
                                }
                            }


                            string[,] _ReportDt = new string[tableData.Rows.Count, tableData.Columns.Count];
                            for (int i = 0; i < tableData.Rows.Count; i++)
                            {
                                for (int j = 0; j < tableData.Columns.Count; j++)
                                {
                                    _ReportDt[i, j] = tableData.Rows[i]["" + tableData.Columns[j].ColumnName.ToString().Trim() + ""].ToString().Trim();
                                }
                            }

                            //设置Execl列名
                            for (int i = 0; i < tableData.Columns.Count; i++)
                            {
                                var nodeList = node.SelectSingleNode($"field[text()='{tableData.Columns[i].ColumnName}']");

                                cell[firstRow, i].PutValue(tableData.Columns[i].ColumnName.ToString().Trim());
                                cell[firstRow, i].SetStyle(style2);

                                if (nodeList.Attributes["width"] != null)
                                    cell.SetColumnWidth(i, Convert.ToInt32(nodeList.Attributes["width"].Value));
                                else
                                    ws.AutoFitColumn(i);
                            }
                            //赋值给Excel内容
                            for (int i = 0; i < _ReportDt.Length / tableData.Columns.Count; i++)
                            {
                                for (int j = 0; j < tableData.Columns.Count; j++)
                                {
                                    cell[i + firstRow + 1, j].PutValue(_ReportDt[i, j].ToString().Trim());
                                    cell[i + firstRow + 1, j].SetStyle(style3);
                                }
                            }


                            if (File.Exists(SavePath))
                            {
                                File.Delete(SavePath);
                            }
                            wb.Save(SavePath);


                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.WriteLog_LocalTxt("Excel结果导出错误！" + ex.ToString());
            }
            return string.Empty;
        }

        /// <summary>
        /// 下载模板
        /// </summary>
        /// <param name="tempID">模板ID，表名称</param>
        /// <param name="typeId">指标类别 1 材料指标 2（脓毒症专病）|3（ICU专科） 数据指标 4 现场指标</param>
        /// <returns>文件的完整路径</returns>
        public static bool TemplateToExcel(string savePath, string tempName, XmlNodeList rowList, XmlNodeList fieldList, XmlNodeList nodeList)
        {
            try
            {

                Workbook wb = new Workbook();
                Worksheet ws = wb.Worksheets[0];
                ws.Name = tempName;
                Cells cell = ws.Cells;
                int firstRow = 0;

                if (fieldList.Count > 0)
                {
                    //设置行高
                    cell.SetRowHeight(0, 30);

                    //设置字体样式
                    Style style1 = wb.Styles[wb.Styles.Add()];
                    style1.HorizontalAlignment = TextAlignmentType.Center;//文字居中
                    style1.Font.Name = "宋体";
                    style1.Font.IsBold = true;//设置粗体
                    style1.Font.Size = 16;//设置字体大小

                    Style style2 = wb.Styles[wb.Styles.Add()];
                    style2.HorizontalAlignment = TextAlignmentType.Left;
                    style2.Font.Size = 12;
                    style2.Font.IsBold = true;
                    style2.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);
                    style2.SetBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black);
                    style2.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
                    style2.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);
                    style2.Pattern = BackgroundType.Solid;
                    style2.ForegroundColor = Color.LightBlue;



                    Style style3 = wb.Styles[wb.Styles.Add()];
                    style3.HorizontalAlignment = TextAlignmentType.Left;

                    Style styleValue = wb.Styles[wb.Styles.Add()];
                    styleValue.HorizontalAlignment = TextAlignmentType.Center;
                    styleValue.Font.Size = 12;
                    styleValue.SetBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);
                    styleValue.SetBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black);
                    styleValue.SetBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
                    styleValue.SetBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);

                    //数据验证
                    ValidationCollection validations = ws.Validations;

                    //设置标题
                    if (rowList.Count > 0)
                    {
                        for (int rowIndex = 0; rowIndex < rowList.Count; rowIndex++)
                        {
                            cell[rowIndex, 0].PutValue(rowList[rowIndex].InnerText.Trim());
                            //合并单元格
                            Range range = cell.CreateRange(rowIndex, 0, 1, fieldList.Count);
                            range.Merge();
                            switch (rowList[rowIndex].Attributes["style"].Value)
                            {
                                case "1":
                                    range.SetStyle(style1);
                                    break;
                                case "2":
                                    range.SetStyle(style3);
                                    break;
                            }
                            firstRow++;
                        }
                    }

                    //设置Execl列名
                    for (int i = 0; i < fieldList.Count; i++)
                    {
                        cell[firstRow, i].PutValue(fieldList[i].InnerText.Trim());
                        cell[firstRow, i].SetStyle(style2);
                        if (fieldList[i].Attributes["type"] != null)
                        {
                            if (fieldList[i].Attributes["type"].Value == "option")
                            {
                                Validation validation = validations[validations.Add()];
                                validation.Type = Aspose.Cells.ValidationType.List;
                                validation.InCellDropDown = true;

                                var dict = GetDictList(fieldList[i].Attributes["dictcode"].Value);
                                validation.Formula1 = String.Join(",", dict.Select(d => d.Value));
                                CellArea area;
                                area.StartRow = 1;
                                area.EndRow = 100;
                                area.StartColumn = i;
                                area.EndColumn = i;
                                validation.AreaList.Add(area);
                            }
                        }

                        if (fieldList[i].Attributes["width"] != null)
                            cell.SetColumnWidth(i, Convert.ToInt32(fieldList[i].Attributes["width"].Value));
                        else
                            ws.AutoFitColumn(i);
                    }

                    //赋值给Excel内容
                    for (int i = 0; i < nodeList.Count; i++)
                    {
                        cell[firstRow + 1, i].PutValue(nodeList[i].InnerText.Trim());
                        cell[firstRow + 1, i].SetStyle(styleValue);
                        if (i == nodeList.Count - 1)
                            cell[firstRow + 1, i + 1].PutValue("上传前请删除示意行");

                        //if (fieldList[i].Attributes["width"] != null)
                        //    cell.SetColumnWidth(i, Convert.ToInt32(nodeList[i].Attributes["width"].Value));
                        //else
                        //    ws.AutoFitColumn(i);
                    }


                    if (File.Exists(savePath))
                    {
                        File.Delete(savePath);
                    }
                    wb.Save(savePath);

                    //savePath.Replace(appPath, "/").Replace("\\", "/");
                }
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog_LocalTxt("Excel结果导出错误！" + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 获取Excel的实体集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="temName"></param>
        /// <returns></returns>
        public static List<T> GetExcelDataList<T>(string filePath, string tempID)
        {
            try
            {
                string appPath = FileHelper.GetCurrentDir();
                string tempPath = Path.Combine(appPath + "ImportTemplate.xml");
                if (File.Exists(tempPath))
                {
                    DataSet ds = ExcelRead(filePath);
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            XmlDocument doc = new XmlDocument();
                            doc.Load(tempPath);
                            XmlNode node = doc.DocumentElement.SelectSingleNode($"//table[@id='{tempID}']");
                            if (node != null && node.HasChildNodes)
                            {
                                for (int i = 0; i < dt.Columns.Count;)
                                {
                                    XmlNode nodeField = node.SelectSingleNode($"field[text()='{dt.Columns[i].ColumnName.Trim()}']");
                                    if (nodeField != null && nodeField.Attributes["id"].Value != "-")
                                    {
                                        dt.Columns[i].ColumnName = nodeField.Attributes["id"].Value;
                                        i++;
                                    }
                                    else
                                    {
                                        dt.Columns.RemoveAt(i);
                                    }
                                }
                                if (dt.Columns.Count > 0)
                                    return ToEntityList<T>(dt);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog_LocalTxt("获取Excel的实体集合错误！" + ex.ToString());
            }
            return null;
        }
        /// <summary>
        /// Excel转换pdf
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Tuple<string, string> ExcelToHtml(string path)
        {
            try
            {
                Workbook wk = new Workbook(path);
                var htmlPath = path.Replace(".xlsx", ".html").Replace(".xls", ".html");
                wk.Save(htmlPath, SaveFormat.Html);
                var file = File.ReadAllText(htmlPath);
                return Tuple.Create(htmlPath, file);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog_LocalTxt("Excel读取错误！" + ex.ToString());
            }
            return Tuple.Create("", "");
        }

        public static bool ExcelToPDF(string path)
        {
            var result = false;
            try
            {
                Workbook wk = new Workbook(path);

                var htmlPath = path.Replace(".xlsx", ".pdf").Replace(".xls", ".pdf");
                htmlPath = htmlPath.Insert(htmlPath.LastIndexOf('\\'), "\\pdf");

                var dire = htmlPath.Remove(htmlPath.LastIndexOf('\\'));
                if (!Directory.Exists(dire))
                    Directory.CreateDirectory(dire);

                if (!File.Exists(htmlPath))
                {
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
                    //result = true;
                }
                result = true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog_LocalTxt("Excel读取错误！" + ex.ToString());
            }
            return result;
        }

        /// <summary>
        /// 获取Excel的实体集合,字典数据转换。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="templatePath"></param>
        /// <param name="filePath"></param>
        /// <param name="tempID"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static List<T> GetExcelDataList<T>(string templatePath, string filePath, string tempID, out string message, Func<string, Dictionary<string, string>> func = null)
        {
            message = string.Empty;
            try
            {
                string tempPath = Path.Combine(templatePath + "\\ImportTemplate.xml");
                if (File.Exists(tempPath))
                {
                    DataSet ds = ExcelRead(filePath);
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        DataTable dt = ds.Tables[0];
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            XmlDocument doc = new XmlDocument();
                            doc.Load(tempPath);
                            XmlNode node = doc.DocumentElement.SelectSingleNode($"//table[@id='{tempID}']");
                            if (node != null && node.HasChildNodes)
                            {
                                for (int i = 0; i < dt.Columns.Count;)
                                {
                                    XmlNode nodeField = node.SelectSingleNode($"field[text()='{dt.Columns[i].ColumnName.Trim()}']");
                                    if (nodeField != null && nodeField.Attributes["id"].Value != "-")
                                    {
                                        dt.Columns[i].ColumnName = nodeField.Attributes["id"].Value.Trim();
                                        i++;
                                    }
                                    else
                                    {
                                        dt.Columns.RemoveAt(i);
                                    }
                                }

                                var optionNodes = node.SelectNodes($"field[@type='option']");
                                if (func != null && optionNodes.Count > 0)
                                {
                                    foreach (XmlNode option in optionNodes)
                                    {
                                        var optionList = func(option.Attributes["dictcode"].Value);
                                        for (int i = 0; i < dt.Rows.Count; i++)
                                        {
                                            if (!string.IsNullOrWhiteSpace(dt.Rows[i][option.Attributes["id"].Value].ToString().Trim()))
                                            {
                                                var value = optionList.Where(p => p.Value == dt.Rows[i][option.Attributes["id"].Value].ToString().Trim()).Select(p => p.Key).FirstOrDefault();
                                                if (string.IsNullOrEmpty(value))
                                                    message += $"字典中没有这条“{dt.Rows[i][option.Attributes["id"].Value].ToString()}”数据;";
                                                dt.Rows[i][option.Attributes["id"].Value] = value.Trim();
                                            }
                                        }
                                    }
                                }
                                if (dt.Columns.Count > 0)
                                    return ToEntityList<T>(dt);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog_LocalTxt("获取Excel的实体集合错误！" + ex.ToString());
            }
            return null;
        }

        private static List<T> ToEntityList<T>(DataTable table)
        {
            if (table == null)
                return null;
            List<DataRow> rows = new List<DataRow>();
            foreach (DataRow row in table.Rows)
                rows.Add(row);
            return ConvertTo<T>(rows);

        }

        private static List<T> ConvertTo<T>(IList<DataRow> rows, string timeFormat = "")
        {
            List<T> list = null;
            if (rows != null)
            {
                list = new List<T>();
                foreach (DataRow row in rows)
                {
                    T item = CreateItem<T>(row, timeFormat);
                    list.Add(item);
                }
            }
            return list;
        }

        private static T CreateItem<T>(DataRow row, string timeFormat = "")
        {
            string columnName;
            T obj = default(T);
            if (row != null)
            {
                obj = Activator.CreateInstance<T>();
                foreach (DataColumn column in row.Table.Columns)
                {
                    columnName = column.ColumnName;
                    //Get property with same columnName
                    PropertyInfo prop = obj.GetType().GetProperty(columnName);
                    try
                    {
                        if (prop == null) continue;
                        //Get value for the column
                        object value = (row[columnName].GetType() == typeof(DBNull))
                        ? null : row[columnName];
                        //Set property value
                        if (prop.CanWrite)    //判断其是否可写
                        {
                            if (string.IsNullOrEmpty(timeFormat))
                            {
                                // prop.SetValue(obj, value!=null?Convert.ChangeType(value, prop.PropertyType):null, null);
                                if (value != null && value.GetType().FullName == "System.DateTime")
                                {
                                    prop.SetValue(obj, value != null ? Convert.ChangeType(value, prop.PropertyType) : null, null);
                                }
                                else if (value != null && (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?)))
                                {
                                    prop.SetValue(obj, Int32.Parse(value.ToString()), null);
                                }
                                else if (value != null && prop.PropertyType.Name == "Decimal")
                                {
                                    prop.SetValue(obj, Decimal.Parse(value.ToString()), null);
                                }
                                else
                                {
                                    //prop.SetValue(obj, value != null ? Convert.ChangeType(value, prop.PropertyType) : null, null);
                                    prop.SetValue(obj, value, null);
                                }
                            }
                            else
                            {
                                DateTime dt;
                                if (value != null
                                    && value.GetType().FullName == "System.DateTime"
                                    && !string.IsNullOrEmpty(value.ToString())
                                    && DateTime.TryParse(value.ToString(), out dt))
                                {
                                    prop.SetValue(obj, Convert.ChangeType(dt.ToString(timeFormat), prop.PropertyType), null);
                                }
                                else
                                {
                                    //prop.SetValue(obj, value, null);
                                    prop.SetValue(obj, value != null ? Convert.ChangeType(value, prop.PropertyType) : null, null);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                        //Catch whatever here
                    }
                }
            }
            return obj;
        }

        #endregion
    }
}