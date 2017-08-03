using DiR.EfChangeLog.Contexts;
using DiR.EfChangeLog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiR.EfChangeLog
{
    class Program
    {
        static void Main(string[] args)
        {
            using(OnlineShopContext ctx = new OnlineShopContext())
            {
                ctx.Customers.Add(new Customer
                {
                    Firstname = "Patrick",
                    Lastname = "Eberle"
                });

                ctx.SaveChanges();
            }

            Console.ReadLine();
        }
    }
}