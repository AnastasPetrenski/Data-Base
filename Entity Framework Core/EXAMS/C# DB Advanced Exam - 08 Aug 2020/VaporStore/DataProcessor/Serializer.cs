namespace VaporStore.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.DataProcessor.Dto.Export;

    public static class Serializer
    {
        public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
        {
            var dtos = context.Genres.Where(g => genreNames.Contains(g.Name)) // && g.Games.Any(x => x.Purchases.Any()))
                .ToList()
                .Select(g => new
                {
                    Id = g.Id,
                    Genre = g.Name,
                    Games = g.Games.Select(x => new
                    {
                        Id = x.Id,
                        Title = x.Name,
                        Developer = x.Developer.Name,
                        Tags = string.Join(", ", x.GameTags.Select(t => t.Tag.Name)),
                        Players = x.Purchases.Count()
                    })
                    .Where(x => x.Players > 0)
                    .OrderByDescending(x => x.Players)
                    .ThenBy(x => x.Id),
                    TotalPlayers = g.Games.Sum(x => x.Purchases.Count())
                })
                .OrderByDescending(g => g.TotalPlayers)
                .ThenBy(g => g.Id)
                .ToList();

            var json = JsonConvert.SerializeObject(dtos, Formatting.Indented);

            return json;
        }

        public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
        {
            var dtos = context.Users.ToList()
                .Where(x => x.Cards.Any(c => c.Purchases.Any()))
                .Select(x => new XmlExportUserDto
                {
                    Username = x.Username,
                    Purchases = x.Cards.SelectMany(c => c.Purchases)
                        .Where(p => p.Type.ToString() == storeType && p.Card.User.Username == x.Username)
                        .OrderBy(x => x.Date)
                        .Select(p => new XmlExportPurchaseDto
                        {
                            Card = p.Card.Number,
                            Cvc = p.Card.Cvc,
                            Date = p.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                            Game = new XmlExportGameDto
                            {
                                Title = p.Game.Name,
                                Price = p.Game.Price,
                                Genre = p.Game.Genre.Name,
                            }
                        })
                        .ToArray(),
                    TotalSpent = x.Cards.Sum(
                        c => c.Purchases.Where(p => p.Type.ToString() == storeType)
                              .Sum(p => p.Game.Price)),
                })
                .Where(u => u.Purchases.Length > 0)
                .OrderByDescending(x => x.TotalSpent).ThenBy(x => x.Username).ToArray();


            var serializer = new XmlSerializer(typeof(XmlExportUserDto[]), new XmlRootAttribute("Users"));
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            var sb = new StringBuilder();

            serializer.Serialize(new StringWriter(sb), dtos, namespaces);

            return sb.ToString().Trim();
        }
    }
}