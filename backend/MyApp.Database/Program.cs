using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MyApp.Database
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = "~/../ss_license.txt".MapHostAbsolutePath();
            Console.ReadKey();
        }
    }
}
