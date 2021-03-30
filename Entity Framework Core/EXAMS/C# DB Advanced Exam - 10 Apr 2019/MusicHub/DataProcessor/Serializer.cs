namespace MusicHub.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using MusicHub.DataProcessor.ImportDtos;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albums = context.Albums.Where(a => a.ProducerId == producerId)
                .Select(a => new
                {
                    AlbumName = a.Name,
                    ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                    ProducerName = a.Producer.Name,
                    Songs = a.Songs.Select(s => new
                    {
                        SongName = s.Name,
                        Price = s.Price.ToString("f2"),
                        Writer = s.Writer.Name
                    })
                        .OrderByDescending(s => s.SongName)
                        .ThenBy(s => s.Writer)
                        .ToList(),
                    AlbumPrice = a.Songs.Sum(s => s.Price).ToString()
                })
                .OrderByDescending(a => a.AlbumPrice)
                .ToList();

            var json = JsonConvert.SerializeObject(albums, Formatting.Indented);

            return json;
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var songsDto = context.Songs.Where(s => s.Duration.TotalSeconds > duration)
                .Select(s => new XmlExportSongDto()
                {
                    SongName = s.Name,
                    WriterName = s.Writer.Name,
                    PerformerFullName = s.SongPerformers.Select(p => p.Performer.FirstName + " " + p.Performer.LastName).FirstOrDefault(),
                    ProducerName = s.Album.Producer.Name,
                    Duration = s.Duration.ToString("c")
                })
                .OrderBy(s => s.SongName)
                .ThenBy(s => s.WriterName)
                .ThenBy(s => s.PerformerFullName)
                .ToArray();

            var serializer = new XmlSerializer(typeof(XmlExportSongDto[]), new XmlRootAttribute("Songs"));
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            var sb = new StringBuilder();

            serializer.Serialize(new Utf8StringWriter(sb, Encoding.UTF8), songsDto, namespaces);

            return sb.ToString().TrimEnd();
        }
    }

    public sealed class Utf8StringWriter : StringWriter
    {
        private readonly Encoding _encoding;

        public Utf8StringWriter(StringBuilder builder, Encoding encoding) 
            : base(builder)
        {
            this._encoding = encoding;
        }

        public override Encoding Encoding
        {
            get { return _encoding; }
        }
    }
}