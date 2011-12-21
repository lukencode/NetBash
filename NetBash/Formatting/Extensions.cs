using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBash.Formatting
{
    public static class Extensions
    {
        public static string ToConsoleTable<T>(this IEnumerable<T> data)
        {
            //Lets start with a string builder
            var sb = new StringBuilder();

            //If its a list of primatives just output them straight up, yo
            if (typeof(T).IsPrimitive || typeof(String) == typeof(T) || typeof(Decimal) == typeof(T))
            {
                foreach (var row in data)
                {
                    sb.Append(row.ToString());
                }
                return sb.ToString();
            }
            
            //Headers
            //TODO - Something that reads the class directly instead of the first item (So empty lists dont crash)
            var properties = data.First().GetType().GetProperties();

            //get the column widths
            var columnWidths = new Dictionary<string,int>();
            foreach (var prop in properties)
            {
                var max = data.Max(row => prop.GetValue(row, null).ToString().Length);
                if (prop.Name.Length > max)
                {
                    max = prop.Name.Length;
                }
                //Add some space
                max += 1;
                columnWidths.Add(prop.Name, max);
            }

            foreach (var prop in properties)
            {
                sb.AppendFormat("{0,-" + columnWidths[prop.Name] + "}", prop.Name);
            }

            //Bust out a linebreak after the headers
            sb.AppendLine();
            //Another dashed linebreak
            for (int i = 0; i < columnWidths.Sum(c => c.Value); i++)
            {
                sb.Append("-");
            }
            sb.AppendLine();

            foreach (var row in data)
            {
                foreach (var prop in properties)
                {
                    sb.AppendFormat("{0,-" + columnWidths[prop.Name] + "}", prop.GetValue(row, null));
                }
                sb.AppendLine();
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
