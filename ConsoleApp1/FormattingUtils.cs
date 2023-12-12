using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public static class FormattingUtils
    {
        public static string FormatLine(ICollection<string> values, Columns columns)
        {
            if (values.Count != columns.ColumnDefinitions.Count)
            {
                throw new Exception("Number of columns does not match the number of values");
            }

            string separator = new StringBuilder(columns.ColumnGap).Insert(0, " ", columns.ColumnGap).ToString();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < values.Count; i++)
            {
                var column = columns.ColumnDefinitions.ElementAt(i);
                var value = values.ElementAt(i);
                int toPad = column.Size - value.Length;
                if (column.Alignment.ToLower() == "right")
                {
                    for (int j = 0; j < toPad; j++)
                    {
                        sb.Append(" ");
                    }
                    sb.Append(value);
                }
                else
                {
                    sb.Append(value);
                    for (int j = 0; j < toPad; j++)
                    {
                        sb.Append(" ");
                    }
                }

                if (i < values.Count - 1)
                {
                    sb.Append(separator);
                }
            }

            return sb.ToString();
        }

        public static string GetDashedLine(int length)
        {
            return new StringBuilder(length).Insert(0, "-", length).ToString();
        }
    }
    
    public record ColumnDefinition(int Size, string Alignment = "left");
    public record Columns(ICollection<ColumnDefinition> ColumnDefinitions, int ColumnGap = 4)
    {
        public int GetTotalSize()
        {
            int total = 0;
            for (int i = 0; i < ColumnDefinitions.Count; i++)
            {
                total += ColumnDefinitions.ElementAt(i).Size;
                if (i < ColumnDefinitions.Count - 1)
                {
                    total += ColumnGap;
                }
            }
            return total;
        }
    }
}
