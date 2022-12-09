using app_source.Data;
using app_source.Models;
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

        public static void InsertarEmpresasSemilla(ref List<CompanyModel> EmpresasSemilla, CompanyContext dbContext, string uniqueId)
        {
            // ingresamos empresas de prueba
            // creamos lista con empresas de prueba
            for (int i = 0; i < 4; ++i)
            {
                CompanyModel tempCompany = new CompanyModel
                {
                    // solo asignamos un nombre porque nos interesa probrar si se peude repetir nombres
                    Nombre = $"Testing {i} {uniqueId}",
                    TipoNegocio = $"Negocio de testing {i} {uniqueId}",
                    PaisBase = $"Pais de testing {i} {uniqueId}",
                    ValorEstimado = 45678.56m,
                    EsTransnacional = true
                };
                // ingresamos en la base y y actualizamos el modelo actual
                // la actualizacion es para recibir el numero de id que asigna la base
                tempCompany = dbContext.Add(tempCompany).Entity;

                EmpresasSemilla.Add(tempCompany);
            }

            // guardamos en la base los cambios realizados
            dbContext.SaveChanges();
        }
    }
}
