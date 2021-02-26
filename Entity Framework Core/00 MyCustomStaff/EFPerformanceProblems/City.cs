using System;
using System.Collections.Generic;
using System.Text;

namespace EFPerformanceProblems
{
    public class City
    {
        public City()
        {
            Students = new List<Student>();
        }

        public int ID { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Student> Students { get; set; }
    }
}
