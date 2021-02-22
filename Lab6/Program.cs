using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Lab6;
using CsvProcessing;
using TableExtensions;

namespace Lab6
{
    class Program
    {
        static void Main(string[] args)
        {
            var moscowBuildings = "moscow-buildings.csv".ReadCsv().ToList();//new CsvHolder<MoscowBuildings>();
            //moscowBuildings.ReadScv("moscow-buildings.csv");

            //Console.WriteLine(moscowBuildings.Take(10).ToPlainTable().ToSingleRowString());
            Console.WriteLine("//////////////////////");

            moscowBuildings = moscowBuildings.Where(row => row.GetBy("house_year").FirstOrDefault()?.Value.ToString()!="н.д." ).ToList(); //.Process(list => list.Where(elem => elem.HouseYear != "н.д."));
            //Console.WriteLine(moscowBuildings.Take(10).ToPlainTable().ToSingleRowString());
            
            var max  = moscowBuildings
                .Where(row => row.GetBy("house_year").Any())
                .Where(row => row.GetBy("house_year").First().Value.ToString().All(char.IsDigit))
                .OrderByDescending(row => Convert.ToInt32(row.GetBy("house_year").First().Value)).First();//.Elements.OrderByDescending(elem => Convert.ToInt32(elem.HouseYear)).First();
            var min  = moscowBuildings
                .Where(row => row.GetBy("house_year").Any())
                .Where(row => row.GetBy("house_year").First().Value.ToString().All(char.IsDigit))
                .OrderBy(row => Convert.ToInt32(row.GetBy("house_year").First().Value)).First();

            Console.WriteLine(min.ToPlainTable(true).ToSingleRowString());
            Console.WriteLine(max.ToPlainTable(true).ToSingleRowString());

            var basmData = moscowBuildings
                .Where(row => row.GetBy("area_name").Any())
                .Where(row=>row.GetBy("area_name").First().Value.ToString().ToLower().Contains("муниципальный округ Басманный".ToLower()))
                .ToList();
                
            //Console.WriteLine(basmData);
            
            Console.WriteLine("//////////////////////");
            
            var grouped = moscowBuildings
                .Where(row => row.GetBy("area_name").Any())
                .GroupBy(row => row.GetBy("area_name").First().Value).ToList();//.Elements.GroupBy(elem => elem.AreaName).ToList();
            Console.WriteLine(string.Join("\n", grouped.Select(gr => (gr.Key, gr.Count()))));

            Console.WriteLine("//////////////////////");
            
            var groupedYear = grouped
                .Select(gr => (gr.Key, 2017 - gr.Sum(buildings => 
                    Convert.ToInt32(buildings.GetBy("house_year").First().Value)) / gr.Count())).ToList();
            Console.WriteLine(string.Join("\n", groupedYear));

            Console.WriteLine("//////////////////////");
            
            Console.WriteLine(min.GetBy("street_name").First().Value);

            var newTable = moscowBuildings.Select(row => new []
            {
                row.GetBy("city").FirstOrDefault(),
                row.GetBy("address").FirstOrDefault()
            }).ToList();
            /*
            var newCsv = new CsvHolder<MoscowBuildings>();
            Console.WriteLine(newCsv);
            
            Console.WriteLine("//////////////////////");
            
            newCsv.Header[0] = "Город";
            Console.WriteLine(newCsv);
            */
            Console.ReadKey();
        }
    }

    class MoscowArea 
    {
        private int Id { get; set; }
        private string Name { get; set; }
    }

    class MoscowBuildings
    {
        public int AreaId { get; set; }
        public string FullAddress { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string StreetPrefix { get; set; }
        public string StreetName { get; set; }
        public string Building { get; set; }
        public string HouseId { get; set; }
        public string AreaName { get; set; }
        public string HouseYear { get; set; }
    }

}
