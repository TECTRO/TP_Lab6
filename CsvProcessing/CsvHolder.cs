using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CsvProcessing
{
    public class CsvHolder<T>
    {
        public CsvHolder()
        {
            var names = typeof(T).GetRuntimeProperties().Select(prop => prop.Name).ToList();
            for (int i = 0; i < names.Count; i++)
            {
                Header[i] = names[i];
            }
        }

        const int Padding = -13;
        public string[] Header { get; private set; } = new string[typeof(T).GetRuntimeProperties().ToArray().Length];
        public List<T> Elements { get; private set; } = new List<T>();

        public string ElemToStr(T elem)
        {
            return string.Join(" ", typeof(T).GetRuntimeProperties().Select(prop => $"{prop.GetValue(elem),Padding}"));
        }
        public void Process(Func<IEnumerable<T>, IEnumerable<T>> func) => Elements = func(Elements).ToList();

        public CsvHolder<T> SubCsv(Func<IEnumerable<T>, IEnumerable<T>> func)
        {
            return new CsvHolder<T> { Elements = func(Elements).ToList(), Header = Header };
        }
        public override string ToString()
        {
            if (Header.All(s => !string.IsNullOrEmpty(s)))
                return string.Join("", Header.Select(h => $"{h,Padding}")) + "\n" + string.Join("\n", Elements.Select(element => string.Join("", element.GetType().GetRuntimeProperties().Select(prop => prop.GetValue(element)).Select(prop => $"{prop,Padding}"))));
            return string.Join("", typeof(T).GetRuntimeProperties().Select(h => $"{h.Name,Padding}")) + "\n" + string.Join("\n", Elements.Select(element => string.Join("", element.GetType().GetRuntimeProperties().Select(prop => prop.GetValue(element)).Select(prop => $"{prop,Padding}"))));

        }

        public void ReadScv(string path, char splitter = ',')
        {
            using (var sr = new StreamReader(path))
            {
                var title = sr.ReadLine()?.Split(splitter).ToList();
                for (int i = 0; i < title?.Count; i++)
                    Header[i] = title[i];

                while (!sr.EndOfStream)
                {
                    /*var list = new List<string>();
                    var rawLine = sr.ReadLine();
                    int first = -1;
                    int last = -1;
                    for (int i = 0; i < rawLine?.Length; i++)
                    {
                        if (rawLine[i] == '"')
                        {
                            if (first < 0)
                                first = i;
                            else
                            {
                                list.Add(rawLine.Substring(first,i-first));
                                rawLine = rawLine.Remove(0, i - first + 1);
                            }
                        }


                    }*/

                    var elements = new Queue<string>(sr.ReadLine()?.Split(splitter).Select(val => val.Trim()) ?? new string[0]);
                    if (elements.Count == typeof(T).GetRuntimeProperties().Count())
                    {
                        var element = Activator.CreateInstance<T>();
                        foreach (var property in element.GetType().GetRuntimeProperties())
                            property.SetValue(element, Convert.ChangeType(elements.Dequeue(), property.PropertyType));

                        Elements.Add(element);
                    }
                    else
                    {
                        var t = 0;
                    }
                }
            }
        }
    }
}
