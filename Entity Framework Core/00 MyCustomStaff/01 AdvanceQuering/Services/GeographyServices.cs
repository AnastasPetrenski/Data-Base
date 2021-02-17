using EF_03_Intro.Geography;
using System;
using System.Collections.Generic;
using System.Text;

namespace EF_03_Intro.Services
{
    public class GeographyServices
    {
        private static StringBuilder sb = new StringBuilder();

        private readonly GeographyContext context;

        public GeographyServices(GeographyContext context)
        {
            this.context = context;
        }


    }
}
