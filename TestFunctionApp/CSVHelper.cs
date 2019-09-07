using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace TestFunctionApp
{
    public static class CSVHelper
    {
        private static IHttpContextAccessor _httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static HttpContext Current => _httpContextAccessor.HttpContext;

        //public CSVHelper(IHttpContextAccessor contextAccessor)
        //{
        //    _contextAccessor = contextAccessor;
        //}
        /// <summary>
        /// Exports a CSV by writing to the current HttpResponse
        /// </summary>
        /// <param name="csv">The CSV data as a string</param>
        /// <param name="filename">The filename for the exported file</param>
        public static IActionResult ExportCSV(string csv, string filename)
        {
            //HttpContext.Current.Response.Clear();
            //HttpContext Current =_httpContextAccessor.HttpContext;

            //_httpContextAccessor.HttpContext.Response.Clear();
            //_httpContextAccessor.HttpContext.Response.Headers.Add("content-disposition", string.Format("attachment; filename={0}.csv", filename));
            //_httpContextAccessor.HttpContext.Response.ContentType = "text/csv";
            //_httpContextAccessor.HttpContext.Response.Headers.Add("Pragma", "public");
            //_httpContextAccessor.HttpContext.Response.WriteAsync(csv);
            //_httpContextAccessor.HttpContext.Response.End();


            //var stream = new MemoryStream();
            //var writeFile = new StreamWriter(stream);
            //var csv = new CsvWriter(writeFile);
            //csv.Configuration.RegisterClassMap<GroupReportCSVMap>();

            //csv.WriteRecords(reportCSVModels);

            //stream.Position = 0; //reset stream
            //return File(stream, "application/octet-stream", "Reports.csv");


            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
            var result = new FileStreamResult(stream, "text/plain");
            result.FileDownloadName = "export_" + DateTime.Now + ".csv";
            return result;
        }

    }
}
