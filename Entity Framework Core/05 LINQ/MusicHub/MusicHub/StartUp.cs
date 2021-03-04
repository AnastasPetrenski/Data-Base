namespace MusicHub
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context = 
                new MusicHubDbContext();

            //DbInitializer.ResetDatabase(context);

            //Test your solutions here
            Console.WriteLine(ExportSongsAboveDuration(context, 180)); 
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var sb = new StringBuilder();

            var albums = context.Albums
                                .Include(p => p.Songs)
                                .Where(p => p.ProducerId == producerId)
                                .Select(x => new
                                {
                                    AlbumName = x.Name,
                                    ReleaseDate = x.ReleaseDate,
                                    ProducerName = x.Producer.Name,
                                    TotalPrice = x.Price,
                                    AlbumSongs = x.Songs.Select(s => new
                                    {
                                        SongName = s.Name,
                                        Price = s.Price,
                                        SongWriter = s.Writer.Name
                                    })                                    
                                        .OrderByDescending(s => s.SongName)
                                        .ThenBy(s => s.SongWriter)
                                        .ToList()
                                })                                
                                .ToList();

            foreach (var album in albums.OrderByDescending(x => x.TotalPrice))
            {
                sb
                    .AppendLine($"-AlbumName: {album.AlbumName}")
                    .AppendLine($"-ReleaseDate: {album.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture)}")
                    .AppendLine($"-ProducerName: {album.ProducerName}")
                    .AppendLine($"-Songs:");

                for (int i = 0; i < album.AlbumSongs.Count; i++)
                {
                    sb
                        .AppendLine($"---#{i + 1}")
                        .AppendLine($"---SongName: {album.AlbumSongs[i].SongName}")
                        .AppendLine($"---Price: {album.AlbumSongs[i].Price:f2}")
                        .AppendLine($"---Writer: {album.AlbumSongs[i].SongWriter}");
                }

                sb.AppendLine($"-AlbumPrice: {album.AlbumSongs.Sum(x => x.Price):f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {

            // JUDGE ACCEPT THAT 100/100

            //var songs = context.Songs
            //    .Where(s => s.Duration.TotalSeconds > duration)
            //    .Select(s => new
            //    {
            //        SongName = s.Name,
            //        PerformerName = s.SongPerformers
            //            .Select(p => p.Performer.FirstName + " " + p.Performer.LastName)
            //            .FirstOrDefault(),
            //        WriterName = s.Writer.Name,
            //        AlbumProducerName = s.Album.Producer.Name,
            //        Duration = s.Duration
            //    })
            //    .ToList()
            //    .OrderBy(s => s.SongName)
            //    .ThenBy(s => s.WriterName)
            //    .ThenBy(s => s.PerformerName)
            //    .ToList();

            //StringBuilder sb = new StringBuilder();

            //int counter = 1;

            //foreach (var song in songs)
            //{
            //    sb.AppendLine($"-Song #{counter}");
            //    sb.AppendLine($"---SongName: {song.SongName}");
            //    sb.AppendLine($"---Writer: {song.WriterName}");
            //    sb.AppendLine($"---Performer: {song.PerformerName}");
            //    sb.AppendLine($"---AlbumProducer: {song.AlbumProducerName}");
            //    sb.AppendLine($"---Duration: {song.Duration.ToString("c")}");

            //    counter++;
            //}

            //return sb.ToString().TrimEnd();

            var sb = new StringBuilder();

            var songs = context.Songs
                               //.Where(x => x.Duration.TotalSeconds > duration)
                               .Select(x => new
                               {
                                   Name = x.Name,
                                   PerformerFullName = (x.SongPerformers.FirstOrDefault(p => p.SongId == x.Id).Performer.FirstName +
                                                    " " + x.SongPerformers.FirstOrDefault(p => p.SongId == x.Id).Performer.LastName),
                                   WriterName = x.Writer.Name,
                                   AlbumProducer = x.Album.Producer.Name,
                                   Duration = x.Duration
                               })
                               .OrderBy(x => x.Name)
                               .ThenBy(x => x.WriterName)
                               .ThenBy(x => x.PerformerFullName)
                               .ToList()
                               .Where(x => x.Duration.TotalSeconds > duration)
                               .ToList();

            int count = 0;
            foreach (var song in songs)
            {
                sb
                    .AppendLine($"-Song #{++count}")
                    .AppendLine($"---SongName: {song.Name}")
                    .AppendLine($"---Writer: {song.WriterName}")
                    .AppendLine($"---Performer: {song.PerformerFullName}")
                    .AppendLine($"---AlbumProducer: {song.AlbumProducer}")
                    .AppendLine($"---Duration: {song.Duration}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
