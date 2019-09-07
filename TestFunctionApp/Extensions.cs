using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Reflection;
using Microsoft.AspNetCore.Http;

namespace TestFunctionApp
{
    public static class Extensions
    {
        /// <summary>
        /// Method that gets the CSV as a string and calls the ExportCSV method CSVHelper
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The generic list</param>
        /// <param name="filename">The filename for the generated CSV</param>
        public static void ExportCSV<T>(this List<T> list, string filename)
        {
            
            string csv = GetCSV(list);
            CSVHelper.ExportCSV(csv, filename);
        }

        /// <summary>
        /// Generate the CSV data as a string using reflection on the objects in the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The generic list</param>
        /// <returns></returns>
        public static string GetCSV<T>(this List<T> list)
        {
            StringBuilder sb = new StringBuilder();

            //Get the properties for type T for the headers
            PropertyInfo[] propInfos = typeof(T).GetProperties();
            for (int i = 0; i <= propInfos.Length - 1; i++)
            {
                sb.Append(propInfos[i].Name);

                if (i < propInfos.Length - 1)
                {
                    sb.Append(",");
                }
            }

            sb.AppendLine();

            //Loop through the collection, then the properties and add the values
            for (int i = 0; i <= list.Count - 1; i++)
            {
                T item = list[i];
                for (int j = 0; j <= propInfos.Length - 1; j++)
                {
                    object o = item.GetType().GetProperty(propInfos[j].Name).GetValue(item, null);
                    if (o != null)
                    {
                        string value = o.ToString();

                        //Check if the value contans a comma and place it in quotes if so
                        if (value.Contains(","))
                        {
                            value = string.Concat("\"", value, "\"");
                        }

                        //Replace any \r or \n special characters from a new line with a space
                        if (value.Contains("\r"))
                        {
                            value = value.Replace("\r", " ");
                        }
                        if (value.Contains("\n"))
                        {
                            value = value.Replace("\n", " ");
                        }

                        sb.Append(value);
                    }

                    if (j < propInfos.Length - 1)
                    {
                        sb.Append(",");
                    }
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
