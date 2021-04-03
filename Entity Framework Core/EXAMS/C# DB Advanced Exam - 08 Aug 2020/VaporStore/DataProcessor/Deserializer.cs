namespace VaporStore.DataProcessor
{
	using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.Data.Models.Enums;
    using VaporStore.DataProcessor.Dto.Import;

    public static class Deserializer
	{
		private const string ErrorMessage = "Invalid Data";

		public static string ImportGames(VaporStoreDbContext context, string jsonString)
		{
			var dtos = JsonConvert.DeserializeObject<JsonImportGameDto[]>(jsonString);

			var sb = new StringBuilder();
			
			foreach (var dto in dtos)
            {
                if (!IsValid(dto) || dto.Tags.Count() == 0)
                {
					sb.AppendLine(ErrorMessage);
					continue;
                }

                if (dto.Tags.Any(x => string.IsNullOrWhiteSpace(x)))
                {
					sb.AppendLine(ErrorMessage);
					continue;
				}

				//TODO: check isValidDate
				var date = DateTime.ParseExact(dto.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

				var developer = context.Developers.FirstOrDefault(d => d.Name == dto.Developer)
					?? new Developer() { Name = dto.Developer};

				var genre = context.Genres.FirstOrDefault(d => d.Name == dto.Genre)
					?? new Genre() { Name = dto.Genre };

				var game = new Game()
				{
					Name = dto.Name,
					Price = dto.Price,
					ReleaseDate = date,
					Developer = developer,
					Genre = genre,
				};

                foreach (var tagName in dto.Tags)
                {
					var tag = context.Tags.FirstOrDefault(x => x.Name == tagName)
						?? new Tag() { Name = tagName };
					game.GameTags.Add(new GameTag() { Tag = tag });                   
                }

				context.Games.Add(game);
				context.SaveChanges();

				sb.AppendLine($"Added {game.Name} ({game.Genre.Name}) with {game.GameTags.Count()} tags");
            }

            return sb.ToString().Trim();
		}

		public static string ImportUsers(VaporStoreDbContext context, string jsonString)
		{
			var dtos = JsonConvert.DeserializeObject<JsonImportUserDto[]>(jsonString);

			var sb = new StringBuilder();
			var users = new List<User>();

            foreach (var dto in dtos)
            {
                if (!IsValid(dto) || !dto.Cards.All(IsValid))
                {
					sb.AppendLine(ErrorMessage);
					continue;
				}

				var user = new User()
				{
					FullName = dto.FullName,
					Username = dto.Username,
					Email = dto.Email,
					Age = dto.Age,
					Cards = dto.Cards.Select(x => new Card()
					{
						Number = x.Number,
						Cvc = x.CVC,
						Type = Enum.Parse<CardType>(x.Type)
					})
					.ToList()
				};

				users.Add(user);
				sb.AppendLine($"Imported {user.Username} with {user.Cards.Count()} cards");
            }

			context.Users.AddRange(users);
			context.SaveChanges();

			return sb.ToString().Trim();
		}

		public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
		{
			var serializer = new XmlSerializer(typeof(XmlImportPurchases[]), new XmlRootAttribute("Purchases"));
			var dtos = serializer.Deserialize(new StringReader(xmlString)) as XmlImportPurchases[];

			var sb = new StringBuilder();
			var purchases = new List<Purchase>();

            foreach (var dto in dtos)
            {
                if (!IsValid(dto))
                {
					sb.AppendLine(ErrorMessage);
					continue;
				}

				var game = context.Games.FirstOrDefault(g => g.Name == dto.Title);
				var card = context.Cards.FirstOrDefault(c => c.Number == dto.Card);

                if (game is null || card is null)
                {
					sb.AppendLine(ErrorMessage);
					continue;
				}

				var user = context.Users.FirstOrDefault(x => x.Cards.Any(c => c.Number == dto.Card));

				purchases.Add(new Purchase()
				{
					Type = Enum.Parse<PurchaseType>(dto.Type),
					ProductKey = dto.Key,
					Date = DateTime.ParseExact(dto.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
					Card = card,
					Game = game
				});

				sb.AppendLine($"Imported {game.Name} for {user.Username}");
            }

			context.Purchases.AddRange(purchases);
			context.SaveChanges();

			return sb.ToString().Trim();
			}

		private static bool IsValid(object dto)
		{
			var validationContext = new ValidationContext(dto);
			var validationResult = new List<ValidationResult>();

			return Validator.TryValidateObject(dto, validationContext, validationResult, true);
		}
	}
}