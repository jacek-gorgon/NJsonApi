using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NJsonApi.Console.Katana
{
    class Program
    {
        static void Main(string[] args)
        {
            string uri = "http://localhost:1984";

            using (WebApp.Start<Startup>(uri))
            {
                System.Console.WriteLine("Server started on {0}", uri);
                System.Console.ReadKey();
            }
        }
    }
}
