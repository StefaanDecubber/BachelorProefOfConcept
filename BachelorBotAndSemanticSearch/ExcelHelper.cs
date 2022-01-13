using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BachelorBotAndSemanticSearch
{
    public static class ExcelHelper
    {
        public static async Task SaveDataInNewExcelAsync(List<QuestionAnswer> qas, FileInfo file, string deel)
        {
            //Write qas in excel
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            await SaveExcelFile(qas, file, deel);
        }

        private static async Task SaveExcelFile(List<QuestionAnswer> qas, FileInfo file, string deel)
        {
            DeleteIfExists(file);
            using var package = new ExcelPackage(file);

            var ws = package.Workbook.Worksheets.Add(deel);

            var range = ws.Cells["A2"].LoadFromCollection(qas, true);
            range.AutoFitColumns();
            await package.SaveAsync();
        }

        private static void DeleteIfExists(FileInfo file)
        {
            if (file.Exists)
            {
                file.Delete();
            }
        }
    }
}
