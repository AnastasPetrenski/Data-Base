using EFDemo.MusicX;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace EFDemo
{
    class Program
    {
        public static readonly ILoggerFactory factory
            = LoggerFactory.Create(builder => { builder.AddConsole(); });

        static void Main()
        {
            var optionsBuilder = new DbContextOptionsBuilder<MusicXRestoredContext>();
            optionsBuilder.UseSqlServer("Server=.;Database=MusicXRestored;Integrated Security=true;");
            optionsBuilder.UseLoggerFactory(factory);

            using var db = new MusicXRestoredContext(optionsBuilder.Options);
            System.Console.WriteLine(db.Songs.Count());

            System.Console.WriteLine(db.SongArtists.Count() > 1);
            db.Songs.Count(x => x.SongArtists.Count() > 1);

            var artists = db.Artists.Select(x =>
            new
            {
                x.Name,
                Count = x.SongArtists.Count(),
                DoubleCount = x.ArtistMetadata
            }).OrderByDescending(x => x.Count).Take(10).ToList();

            foreach (var artist in artists)
            {
                System.Console.WriteLine($"Name:{artist.Name} Songs:{artist.Count} atributes:{artist.DoubleCount}");
            }

            System.Console.WriteLine(string.Join(Environment.NewLine, artists));
        }

    }
}
