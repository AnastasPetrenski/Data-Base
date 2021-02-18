using EF_03_Intro.Geography;
using EF_03_Intro.Models;
using EF_03_Intro.Services;

namespace EF_03_Intro
{
    class Program
    {
        static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();
            GeographyContext geography = new GeographyContext();

            SoftUniServices softServices = new SoftUniServices(context);
            GeographyServices geoServices = new GeographyServices(geography);

            //System.Console.WriteLine(softServices.GetAverageSalaryByDepartment());

            System.Console.WriteLine(geoServices.GetContinentsAndCurrencies());
        }
    }
}
