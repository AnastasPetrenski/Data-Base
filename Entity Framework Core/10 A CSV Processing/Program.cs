using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using CsvHelper;

namespace CSV
{
    public class Program
    {
        static void Main(string[] args)
        {
            var data = File.ReadAllText("../../../Cars.csv");
            
            using (CsvReader csvReader = new CsvReader(new StringReader(data), CultureInfo.InvariantCulture))
            {
                var cars = csvReader.GetRecords<Car>().ToList();
            }

            using (CsvReader reader = new CsvReader(new StreamReader("../../../Cars.csv"), CultureInfo.InvariantCulture))
            {
                var cars = reader.GetRecords<Car>().ToList();
            }

            //Split result by NewLine
            var splitData = data.Split(Environment.NewLine).ToList();
            List<List<string>> separatedData = new List<List<string>>(splitData.Count);
            for (int i = 0; i < splitData.Count; i++)
            {
                //Initialize new list add it 
                var list = new List<string>();
                separatedData.Add(list);
                //Converte string to char array
                var row = splitData[i].ToCharArray();
                Queue<char> queue = new Queue<char>(row);
                var item = string.Empty;
                bool inQuotedField = false;

                while (queue.Count > 0)
                {
                    //escape '\'
                    var current = queue.Dequeue();
                    if (current == '\\')
                    {
                        continue;
                    }
                    //build string
                    if (current != ',' && current != '"') 
                    {
                        item += current;
                    }
                    //add string to list
                    else if (current == ',')
                    {
                        separatedData[i].Add(item);
                        item = string.Empty;
                    }
                    //nested string 
                    else if (current == '"' && !inQuotedField)
                    {
                        inQuotedField = true;
                        var quatedItem = string.Empty;
                        while (inQuotedField)
                        {
                            current = queue.Dequeue();

                            if (current == '\\')
                            {
                                continue;
                            }

                            if (current != '"')
                            {
                                quatedItem += current;
                            }
                            else if (current == '"' && queue.Peek() == '\\')
                            {
                                queue.Dequeue();
                                if (queue.Peek() == '"')
                                {
                                    current = queue.Dequeue();
                                    quatedItem += current;
                                    continue;
                                }
                            }
                            else if (current == '"' && queue.Peek() == ',')
                            {
                                if (quatedItem.Length == 0)
                                {
                                    separatedData[i].Add(quatedItem);
                                    inQuotedField = false;
                                    current = queue.Dequeue();
                                }
                                else
                                {
                                    //quatedItem += current; <== if string contains quates
                                    separatedData[i].Add(quatedItem);
                                    quatedItem = string.Empty;
                                    inQuotedField = false;
                                    current = queue.Dequeue();
                                }
                                
                            }
                        }
                    }
                   
                }
                //add last item to list
                if (item.Length > 0)
                {
                    separatedData[i].Add(item);
                }
            }

            List<Car> carsList = new List<Car>();
            int count = 0;
            foreach (var carParts in separatedData)
            {
                count++;
                if (count == 1)
                {
                    continue;
                }

                var entity = new Car()
                {
                    Year = int.Parse(carParts[0]),
                    Make = carParts[1],
                    Model = carParts[2],
                    Description = carParts[3],
                    Price = decimal.Parse(carParts[4])
                };

                carsList.Add(entity);
                
            }

            Console.WriteLine($"Cars available: {carsList.Count}");
            foreach (var car in carsList)
            {
                Console.WriteLine(car.ToString());
            }

            Thread.Sleep(10000);

        }
    }
}
