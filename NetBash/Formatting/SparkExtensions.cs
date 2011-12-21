using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetBash.Formatting
{
    public static class SparkExtensions
    {
        public static String Spark(this string input)
        {
            var numbers = new List<double>();

            foreach (var c in input)
            {
                //lol char graph
                numbers.Add((int)c);
            }

            return numbers.Spark();
        }

        public static String Spark(this IEnumerable<string> input)
        {
            var numbers = new List<double>();

            foreach (var c in input)
            {
                numbers.Add(Double.Parse(c.ToString()));
            }

            return numbers.Spark();
        }

        public static String Spark(this IEnumerable<int> input)
        {
            return input.Select(v => (double)v).Spark();
        }

        public static String Spark(this IEnumerable<decimal> input)
        {
            return input.Select(v => (double)v).Spark();
        }

        public static String Spark(this IEnumerable<double> input)
        {
            double min = input.Min();
            double max = input.Max();
            double intervalSize = max - min;

            StringBuilder sb = new StringBuilder(input.Count());
            String sparks = "▁▂▃▄▅▆▇";

            foreach (var d in input)
            {
                int sparkIndex = (int)((d - min) / intervalSize * (sparks.Length - 1));
                sb.Append(sparks[sparkIndex]);
            }

            return sb.ToString();
        }
    }
}
