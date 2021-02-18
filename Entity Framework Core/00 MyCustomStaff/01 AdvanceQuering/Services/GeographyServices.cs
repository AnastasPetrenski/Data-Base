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

        public string GetContinentsAndCurrencies()
        {
            var currencyByCountriesAndContinents = context
                .Currencies
                .Select(c => new
                {
                    ContinentCode = c.Countries.Select(x => x.ContinentCode).Single(),
                    CurrencyCode = c.CurrencyCode,
                    CurrencyInCountries = c.Countries.Count(r => r.CurrencyCode == c.CurrencyCode),
                    CurrencyInCountriesByContinents = c.Countries.Count(r => r.ContinentCode == c.Countries.Select(z => z.ContinentCode).Single())
                })
                .Where(c => c.CurrencyInCountriesByContinents > 1)
                .OrderBy(c => c.ContinentCode)
                .ThenByDescending(c => c.CurrencyInCountriesByContinents);

            var continentsCode = context
                .Continents
                .Select(c => new 
                {
                    continentCode = c.ContinentCode
                })
                .ToList();

            var countCountriesByContinent = context
                .Continents
                .Select(c => new
                {
                    ContinentCode = c.ContinentCode,
                    ContinentCountries = c.Countries.Count(x => x.ContinentCode == c.ContinentCode)
                });

            var currencyConcret = context
                .Countries
                .Count(c => c.ContinentCode == "EU" && c.CurrencyCode == "EUR");

            var currency = context
                .Countries
                .Select(x => new
                {
                    CurrencyCode = x.CurrencyCode,
                    CountryCode = x.CountryCode,
                    ContinentCode = x.ContinentCode,
                    CountCurrency = x.CurrencyCodeNavigation.Countries.Count(c => c.ContinentCode == x.ContinentCode)
                })
                .Where(x => x.CountCurrency > 1)
                .ToList();

            foreach (var c in currencyByCountriesAndContinents)
            {
                sb.AppendLine($"{c.ContinentCode} {c.CurrencyCode} {c.CurrencyInCountriesByContinents}");
            }

            return sb.ToString().TrimEnd();
        }

        public string GetCountOfMountainRanges()
        {
            var list = context
                .Countries
                .Select(c => new
                {
                    CountryCode = c.CountryCode,
                    CountryName = c.CountryName,
                    MountainRanges = c.MountainsCountries.Count(m => m.CountryCode == c.CountryCode),
                    Countinent = c.ContinentCodeNavigation.ContinentName
                })
                .Where(c => c.MountainRanges > 0)
                .OrderByDescending(c => c.MountainRanges)
                .ToList();

            sb.AppendLine("CountryCode Name MountainRanges Continent");
            foreach (var c in list)
            {
                sb.AppendLine($"{c.CountryCode} {c.CountryName}: {c.MountainRanges} - {c.Countinent}");
            }

            return sb.ToString().TrimEnd();
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
