using Instagraph.Data;
using Instagraph.DataProcessor.DtoModels.ExportDtos;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Instagraph.DataProcessor
{
    public class Serializer
    {
        public static string ExportUncommentedPosts(InstagraphContext context)
        {
            var posts = context
                .Posts
                .Where(p => p.Comments.Count == 0)
                .OrderBy(p => p.Id)
                .Select(p => new
                {
                    Id = p.Id,
                    Picture = p.Picture.Path,
                    User = p.User.Username
                })
                .ToList();

            var json = JsonConvert.SerializeObject(posts, Newtonsoft.Json.Formatting.Indented);

            return json;
        }

        public static string ExportPopularUsers(InstagraphContext context)
        {
            var popularUsers = context
               .Users
               .Where(u => u.Posts
                   .Any(p => p.Comments
                       .Any(c => u.Followers
                           .Any(f => f.FollowerId == c.UserId))))
               .OrderBy(u => u.Id)
               .Select(u => new
               {
                   Username = u.Username,
                   Followers = u.Followers.Count
               })
               .ToList();

            var users = context
                .Users
                .Where(u => u.Posts.Count > 0 && u.Posts.Any(x => x.Comments.Count > 0))
                .OrderBy(u => u.Id)
                .Select(u => new
                {
                    Username = u.Username,
                    Followers = u.Followers.Count
                })
                .ToList();

            var json = JsonConvert.SerializeObject(users, Newtonsoft.Json.Formatting.Indented);

            return json;
        }

        public static string ExportCommentsOnPosts(InstagraphContext context)
        {
            var users = context
                .Users
                .Select(u => new
                {
                    Username = u.Username,
                    MostComments = u.Posts
                                     .Select(p => p.Comments.Count)
                                     .OrderByDescending(p => p)
                                     .ToList()
                })
                .ToList()
            .Select(u => new XmlUserDto()
             {
                 Username = u.Username,
                 MostComments = u.MostComments.Count > 0 ? u.MostComments.First() : 0
            })
            .OrderByDescending(u => u.MostComments)
            .ThenBy(u => u.Username)
            .ToArray();

            var serializer = new XmlSerializer(typeof(XmlUserDto[]), new XmlRootAttribute("users"));
            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName(string.Empty, string.Empty) });
            var sb = new StringBuilder();

            serializer.Serialize(new StringWriter(sb), users, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}
