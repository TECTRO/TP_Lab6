using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualBasic.FileIO;
using TableExtensions;

namespace CsvProcessing
{
    public static class Extensions
    {
        public static IEnumerable<IEnumerable<TableNode>> ReadCsv(this string path, char splitter = ',')
        {
            //this IEnumerable<IEnumerable<TableNode>>table
            IEnumerable<IEnumerable<TableNode>> table = new List<IEnumerable<TableNode>>();

            using (TextFieldParser parser = new TextFieldParser(path))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(splitter.ToString());
                parser.HasFieldsEnclosedInQuotes = true;
                parser.TrimWhiteSpace = true;
                var title = parser.ReadFields()?.ToList();
                if (title != null)
                {
                    while (!parser.EndOfData)
                    {
                        try
                        {
                            var elements = parser.ReadFields()?
                                .Zip(title, (val, tittle) => new TableNode {Title = tittle, Value = val})
                                .ToList();

                            if (!(elements is null))
                                table = table.Add(elements);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                }
            }
            return table;
        }
    }
}