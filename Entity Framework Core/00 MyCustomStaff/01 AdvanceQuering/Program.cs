using EF_03_Intro.Models;
using EF_03_Intro.Services;

namespace EF_03_Intro
{
    class Program
    {
        static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();

            SoftUniServices service = new SoftUniServices(context);

            System.Console.WriteLine(service.GetEmployee24Projects()); 
        }
    }
}
