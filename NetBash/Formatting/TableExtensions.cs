using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace NetBash.Formatting
{
    public static class TableExtensions
    {
        public static string ToConsoleTable<T>(this IEnumerable<T> data)
        {
            var type = typeof(T);
            var isEmpty = data == null || !data.Any();

            //Lets start with a string builder
            var sb = new StringBuilder();

            //If its a list of primatives just output them straight up, yo
            if (type.IsPrimitive || typeof(String) == type || typeof(Decimal) == type)
            {
                foreach (var row in data)
                {
                    sb.Append(row.ToString());
                }

                return sb.ToString();
            }
            
            //Headers
            var properties = type.GetProperties();

            //get the column widths
            var columnWidths = new Dictionary<string,int>();
            foreach (var prop in properties)
            {
                var max = prop.Name.Length;

                if (!isEmpty)
                {
                    max = data.Max(row => prop.GetValue(row, null).ToString().Length);

                    if (prop.Name.Length > max)
                        max = prop.Name.Length;
                }

                //Add some space
                columnWidths.Add(prop.Name, max + 3);
            }

            foreach (var prop in properties)
            {
                sb.AppendFormat("{0,-" + columnWidths[prop.Name] + "}", prop.Name.ToUpper());
            }

            //Bust out a linebreak after the headers
            sb.AppendLine();

            //Another dashed linebreak
            for (int i = 0; i < columnWidths.Sum(c => c.Value); i++)
            {
                sb.Append("-");
            }

            sb.AppendLine();
            
            if (!isEmpty)
            {
                foreach (var row in data)
                {
                    foreach (var prop in properties)
                    {
                        sb.AppendFormat("{0,-" + columnWidths[prop.Name] + "}", prop.GetValue(row, null));
                    }

                    sb.AppendLine();
                }
            }
            else
            {
                sb.AppendLine("NO RESULTS");
            }

            //Another dashed linebreak
            for (int i = 0; i < columnWidths.Sum(c => c.Value); i++)
            {
                sb.Append("-");
            }

            sb.AppendLine();

            return sb.ToString();
        }
    }
}
