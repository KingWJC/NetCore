using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using ADF.IBusiness;
using ADF.Business;


namespace ADF.Web.Pages.ExcelXmlTransform
{
    public class IndexModel : PageModel
    {
        public IActionResult OnPostImport(IFormFile excelfile)
        {
            using (Stream stream = new MemoryStream())
            {
                excelfile.CopyTo(stream);
                stream.Flush();

                IProfessionBussiness profession = new ProfessionBussiness();
                profession.UpdateProfession(stream);
            }
            return Content("OK");

            // string sWebRootFolder = _hostingEnvironment.WebRootPath;
            // string sFileName = $"{Guid.NewGuid()}.xlsx";
            // FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            // try
            // {
            //     using (FileStream fs = new FileStream(file.ToString(), FileMode.Create))
            //     {
            //         excelfile.CopyTo(fs);
            //         fs.Flush();
            //     }
            //     using (ExcelPackage package = new ExcelPackage(file))
            //     {
            //         StringBuilder sb = new StringBuilder();
            //         ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
            //         int rowCount = worksheet.Dimension.Rows;
            //         int ColCount = worksheet.Dimension.Columns;
            //         bool bHeaderRow = true;
            //         for (int row = 1; row <= rowCount; row++)
            //         {
            //             for (int col = 1; col <= ColCount; col++)
            //             {
            //                 if (bHeaderRow)
            //                 {
            //                     sb.Append(worksheet.Cells[row, col].Value.ToString() + "\t");
            //                 }
            //                 else
            //                 {
            //                     sb.Append(worksheet.Cells[row, col].Value.ToString() + "\t");
            //                 }
            //             }
            //             sb.Append(Environment.NewLine);
            //         }
            //         return Content(sb.ToString());
            //     }
            // }
            // catch (Exception ex)
            // {
            //     return Content(ex.Message);
            // }
        }

        public IActionResult OnGetExport()
        {
            IProfessionBussiness profession = new ProfessionBussiness();
            var stream = profession.ExportProfession();
            stream.Position = 0;
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}
