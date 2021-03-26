using Instagraph.Data;
using Instagraph.DataProcessor.DtoModels.ImportDtos;
using Instagraph.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Instagraph.DataProcessor
{

    public class Deserializer
    {
        private static string successMessage = "Successfully imported {0}.";
        private static string errorMessage = "Error: Invalid data.";

        public static string ImportPictures(InstagraphContext context, string jsonString)
        {
            var dtos = JsonConvert.DeserializeObject<JsonPictureDto[]>(jsonString);

            var pictures = new List<Picture>();

            var sb = new StringBuilder();

            foreach (var dto in dtos)
            {
                var picture = new Picture()
                {
                    Path = dto.Path,
                    Size = dto.Size
                };

                if (IsValid(picture) && !pictures.Any(x => x.Path == picture.Path))
                {
                    pictures.Add(picture);
                    sb.AppendLine(String.Format(successMessage, $"Picture {picture.Path}"));
                }
                else
                {
                    sb.AppendLine(errorMessage);
                }
            }

            context.Pictures.AddRange(pictures);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportUsers(InstagraphContext context, string jsonString)
        {
            var dtos = JsonConvert.DeserializeObject<JsonUserDto[]>(jsonString);
            var pictures = context.Pictures.ToList();
            var users = new List<User>();
            var sb = new StringBuilder();

            foreach (var dto in dtos)
            {
                var picture = pictures.FirstOrDefault(p => p.Path == dto.ProfilePicture);

                if (picture is null)
                {
                    sb.AppendLine(errorMessage);
                    continue;
                }

                var user = new User()
                {
                    Username = dto.Username,
                    Password = dto.Password,
                    ProfilePicture = picture
                };

                if (IsValid(user))
                {
                    users.Add(user);
                    sb.AppendLine(string.Format(successMessage, $"User {user.Username}"));
                }
                else
                {
                    sb.AppendLine(errorMessage);
                }
            }

            Console.WriteLine(sb.ToString().TrimEnd());
            context.Users.AddRange(users);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportFollowers(InstagraphContext context, string jsonString)
        {
            var dtos = JsonConvert.DeserializeObject<JsonUserFollowerDto[]>(jsonString);
            var users = context.Users.AsNoTracking().ToList();
            var usersFollowers = new List<UserFollower>();
            var sb = new StringBuilder();

            foreach (var dto in dtos)
            {
                var user = users.FirstOrDefault(u => u.Username == dto.User);

                if (user is null)
                {
                    sb.AppendLine(errorMessage);
                    continue;
                }

                var follower = users.FirstOrDefault(u => u.Username == dto.Follower);

                if (follower is null)
                {
                    sb.AppendLine(errorMessage);
                    continue;
                }

                var isFollowing = usersFollowers.Any(u => u.User == user && u.Follower == follower);

                if (isFollowing)
                {
                    sb.AppendLine(errorMessage);
                    continue;
                }

                var userFollower = new UserFollower()
                {
                    User = user,
                    Follower = follower
                };

                usersFollowers.Add(userFollower);
                sb.AppendLine(string.Format(successMessage, $"Follower {follower.Username} to User {user.Username}"));
            }

            context.UsersFollowers.AddRange(usersFollowers);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportPosts(InstagraphContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(XmlPostDto[]), new XmlRootAttribute("posts"));
            var dtos = serializer.Deserialize(new StringReader(xmlString)) as XmlPostDto[];

            var sb = new StringBuilder();
            var posts = new List<Post>();

            var users = context.Users.ToList();
            var pictures = context.Pictures.ToList();

            foreach (var dto in dtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(errorMessage);
                    continue;
                }

                var user = users.FirstOrDefault(u => u.Username == dto.Username);

                if (user is null)
                {
                    sb.AppendLine(errorMessage);
                    continue;
                }

                var picture = pictures.FirstOrDefault(p => p.Path == dto.Picture);

                if (picture is null)
                {
                    sb.AppendLine(errorMessage);
                    continue;
                }

                var post = new Post()
                {
                    Caption = dto.Caption,
                    User = user,
                    Picture = picture
                };

                posts.Add(post);
                sb.AppendLine(string.Format(successMessage, $"Post {dto.Caption}"));
            }

            context.Posts.AddRange(posts);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportComments(InstagraphContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(XmlCommentDto[]), new XmlRootAttribute("comments"));
            var dtos = serializer.Deserialize(new StringReader(xmlString)) as XmlCommentDto[];

            var sb = new StringBuilder();
            var comments = new List<Comment>();

            foreach (var dto in dtos)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(errorMessage);
                    continue;
                }

                var user = context.Users.FirstOrDefault(u => u.Username == dto.Username);

                var post = context.Posts.FirstOrDefault(p => p.Id == dto.Post.Id);

                if (user is null || post is null)
                {
                    sb.AppendLine(errorMessage);
                    continue;
                }

                var comment = new Comment()
                {
                    Content = dto.Content,
                    User = user,
                    Post = post
                };

                comments.Add(comment);
                sb.AppendLine(string.Format(successMessage, $"Comment {dto.Content}"));
            }

            context.Comments.AddRange(comments);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new ValidationContext(obj);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);

            return isValid;
        }
    }
}
