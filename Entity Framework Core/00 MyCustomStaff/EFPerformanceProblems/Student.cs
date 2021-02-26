using System;
using System.Collections.Generic;
using System.Text;

namespace EFPerformanceProblems
{
    public class Student
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public int CityID { get; set; }

        public virtual City City { get; set; }

        public byte[] Picture { get; set; }
    }
}
