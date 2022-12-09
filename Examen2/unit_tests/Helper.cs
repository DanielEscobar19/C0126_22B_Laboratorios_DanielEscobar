using app_source.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace unit_tests
{
    public static class Helper
    {
        private static string Connectionstring = "Data Source=172.16.202.209;Initial Catalog=C02748;TrustServerCertificate=True;Persist Security Info=True;User ID=C02748;Password=c02748";

        public static CompanyContext DbContext
        {
            get
            {
                var optionsBuilder = new DbContextOptionsBuilder<CompanyContext>();
                optionsBuilder.UseSqlServer(Connectionstring);

                return new CompanyContext(optionsBuilder.Options);
            }

        }
    }
}
