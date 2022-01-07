using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ReadExcel
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var file = new FileInfo(@"C:\Users\stefa\Documents\informatica\bachelorproef\Bestanden\DataTest.xlsx");
            var questionsFile = new FileInfo(@"C:\Users\stefa\Documents\informatica\bachelorproef\Bestanden\Vragen.xlsx");

            List<string> questions = await LoadExcelFile(questionsFile);
        }

        private static async Task<List<string>> LoadExcelFile(FileInfo file)
        {
            List<string> question = new List<string>();
            using var package = new ExcelPackage(file);
            await package.LoadAsync(file);
            var ws = package.Workbook.Worksheets[0];

            int col = 1;
            int row = 0;

            while (string.IsNullOrEmpty(ws.Cells[row, col].Value?.ToString()) == false)
            {
                var vraag = ws.Cells[row, col].Value.ToString();
                row++;
            }

            return new List<string>();
        }
    }
}
