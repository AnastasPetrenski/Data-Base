using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using P03_FootballBetting.Data;
using P03_FootballBetting.Data.Models;
using P03_FootballBetting.Web.Common;
using P03_FootballBetting.Web.Models;

namespace P03_FootballBetting.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly FootballBettingContext context;
        
        //new
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly IConfiguration config;

        public HomeController(ILogger<HomeController> logger,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IConfiguration config)
        {
            this.logger = logger;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.config = config;
        }

        public IActionResult Index()
        {
            if (Session["UserId"] != null)
            {
                return this.View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

        }

        public IActionResult Register()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                var check = context.Users.FirstOrDefault(u => u.Email == user.Email);
                if (check == null)
                {
                    user.Password = Sha512Generator.Sha512(user.Password);
                    context.Users.Add(user);
                    context.SaveChanges();

                    return RedirectToAction("Index");
                }
            }

            return this.View();
        }

        public IActionResult Login()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (ModelState.IsValid)
            {
                var pass = Sha512Generator.Sha512(password);
                var data = context.Users.Where(u => u.Email.Equals(email) && u.Password.Equals(pass)).ToList();
                if (data.Count == 1)
                {
                    Session["FullName"] = data.FirstOrDefault().Username;
                }

            }
            return this.View();
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //create a string MD5 => Sha512
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }
    }
}