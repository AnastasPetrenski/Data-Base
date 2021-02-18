using EF_03_Intro.Geography;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public string GetHighestPeakInBulgaria()
        {
            var list = context
                .Peaks
                .Where(p => p.Mountain.MountainsCountries.Any(c => c.CountryCodeNavigation.CountryName == "Bulgaria"))
                .Select(p => new
                {
                    CountryCode = p.Mountain.MountainsCountries.Single().CountryCode,
                    MountainRange = p.Mountain.MountainRange,
                    PeakName = p.PeakName,
                    Elevation = p.Elevation
                })
                .Where(p => p.Elevation > 2835)
                .OrderByDescending(p => p.Elevation)
                .ToList();

            foreach (var p in list)
            {
                sb.AppendLine($"{p.CountryCode} {p.MountainRange} {p.PeakName} {p.Elevation}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
