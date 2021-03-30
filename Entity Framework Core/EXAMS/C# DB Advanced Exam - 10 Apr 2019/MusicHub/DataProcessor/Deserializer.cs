namespace MusicHub.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using MusicHub.Data.Models;
    using MusicHub.Data.Models.Enums;
    using MusicHub.DataProcessor.ImportDtos;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data";

        private const string SuccessfullyImportedWriter 
            = "Imported {0}";
        private const string SuccessfullyImportedProducerWithPhone 
            = "Imported {0} with phone: {1} produces {2} albums";
        private const string SuccessfullyImportedProducerWithNoPhone
            = "Imported {0} with no phone number produces {1} albums";
        private const string SuccessfullyImportedSong 
            = "Imported {0} ({1} genre) with duration {2}";
        private const string SuccessfullyImportedPerformer
            = "Imported {0} ({1} songs)";

        public static string ImportWriters(MusicHubDbContext context, string jsonString)
        {
            var dtos = JsonConvert.DeserializeObject<JsonWriterDto[]>(jsonString);

            var sb = new StringBuilder();
            var writers = new List<Writer>();

            foreach (var dto in dtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                writers.Add(new Writer()
                {
                    Name = dto.Name,
                    Pseudonym = dto.Pseudonym
                });

                sb.AppendLine(string.Format(SuccessfullyImportedWriter, dto.Name));
            }

            context.Writers.AddRange(writers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportProducersAlbums(MusicHubDbContext context, string jsonString)
        {
            var dtos = JsonConvert.DeserializeObject<JsonProducerDto[]>(jsonString);
            var producers = new List<Producer>();
            var sb = new StringBuilder();

            foreach (var dto in dtos)
            {
                if(!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var producer = new Producer()
                {
                    Name = dto.Name,
                    Pseudonym = dto.Pseudonym,
                    PhoneNumber = dto.PhoneNumber
                };

                bool isValidAlbum = true;

                foreach (var album in dto.Albums)
                {
                    if (!IsValid(album))
                    {
                        sb.AppendLine(ErrorMessage);
                        isValidAlbum = false;
                        break;
                    }

                    producer.Albums.Add(new Album()
                    {
                        Name = album.Name,
                        ReleaseDate = DateTime.ParseExact(album.ReleaseDate, @"dd/MM/yyyy", CultureInfo.InvariantCulture)
                    });
                }

                if (!isValidAlbum)
                {
                    continue;
                }

                producers.Add(producer);
                sb.AppendLine(producer.PhoneNumber is null
                    ? string.Format(SuccessfullyImportedProducerWithNoPhone, producer.Name, producer.Albums.Count)
                    : string.Format(SuccessfullyImportedProducerWithPhone, producer.Name, producer.PhoneNumber, producer.Albums.Count));
            }

            context.Producers.AddRange(producers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportSongs(MusicHubDbContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(XmlSongDto[]), new XmlRootAttribute("Songs"));
            var dtos = serializer.Deserialize(new StringReader(xmlString)) as XmlSongDto[];

            var sb = new StringBuilder();
            var songs = new List<Song>();

            foreach (var dto in dtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var isValidGenre = Enum.TryParse<Genre>(dto.Genre, out Genre genre);

                if (!isValidGenre)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var album = context.Albums.FirstOrDefault(a => a.Id == dto.AlbumId);
                var writer = context.Writers.FirstOrDefault(w => w.Id == dto.WriterId);

                if (album is null || writer is null)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                songs.Add(new Song()
                {
                    Name = dto.Name,
                    Duration = TimeSpan.ParseExact(dto.Duration, "c", CultureInfo.InvariantCulture),
                    CreatedOn = DateTime.ParseExact(dto.CreatedOn, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Genre = genre,
                    Album = album,
                    Writer = writer,
                    Price = dto.Price
                });

                sb.AppendLine(string.Format(SuccessfullyImportedSong, dto.Name, genre, dto.Duration));
            }

            context.Songs.AddRange(songs);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportSongPerformers(MusicHubDbContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(XmlPerformerDto[]), new XmlRootAttribute("Performers"));
            var dtos = serializer.Deserialize(new StringReader(xmlString)) as XmlPerformerDto[];

            var sb = new StringBuilder();
            var performers = new List<Performer>();

            foreach (var dto in dtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var performer = new Performer()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Age = dto.Age,
                    NetWorth = dto.NetWorth
                };

                var isValidDto = true;

                foreach (var songDto in dto.PerformersSongs)
                {
                    var song = context.Songs.FirstOrDefault(s => s.Id == songDto.Id);

                    if (song is null)
                    {
                        isValidDto = false;
                        sb.AppendLine(ErrorMessage);
                        break;
                    }

                    performer.PerformerSongs.Add(new SongPerformer()
                    {
                        Song = song,
                        Performer = performer
                    });
                }

                if (!isValidDto)
                {
                    continue;
                }

                performers.Add(performer);
                sb.AppendLine(string.Format(SuccessfullyImportedPerformer, performer.FirstName, performer.PerformerSongs.Count));
            }

            context.Performers.AddRange(performers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new ValidationContext(obj);
            var validationResults = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);

            return isValid;
        }
    }
}