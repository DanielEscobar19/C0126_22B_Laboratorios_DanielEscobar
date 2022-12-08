using app_source.Data;
using app_source.Models;
using functional_tests.Pages;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Security.Policy;

namespace functional_tests
{
    public class Tests
    {
        private WebDriver Driver;
        private CompanyContext _DbContext;
        private List<CompanyModel> EmpresasSemilla;

        [SetUp]
        public void Setup()
        {
            Driver = new ChromeDriver();
            Driver.Url = Helper.URL;
            Driver.Manage().Window.Maximize();
            Driver.Navigate().GoToUrl(Helper.URL);
            _DbContext = Helper.DbContext;
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var item in EmpresasSemilla)
            {
                _DbContext.Remove(item); 
            }
            _DbContext.SaveChanges();

            Driver.Close();
        }

        [Test]
        public void ReadOperation_DisplaysAllCompanies()
        {
            // arrange 
            // creamos lista con empresas de prueba
            EmpresasSemilla = new List<CompanyModel>();

            for (int i = 0; i < 4; ++i)
            {
                CompanyModel tempCompany = new CompanyModel
                {
                    Nombre = "Testing " + i,
                    TipoNegocio = "Negocio de testing " + i,
                    PaisBase = "Pais testing " + 1,
                    ValorEstimado = 1000.15m,
                    EsTransnacional = true
                };
                // ingresamos en la base y y actualizamos el modelo actual
                // la actualizacion es para recibir el numero de id que asigna la base
                tempCompany = _DbContext.Add(tempCompany).Entity;

                EmpresasSemilla.Add(tempCompany);
            }

            // ingresamos en la base los puestos semilla
            // _DbContext.AddRange(EmpresasSemilla.ToArray());
            // guardamos en la base los cambios realizados
            _DbContext.SaveChanges();


            // recargamos la pagina para que se muestren los nuevos puestos
            Driver.Navigate().Refresh();
            
            // action
            // instancia para obtener los nombres de la pantalla
            MainPage mainPage = new(Driver);

            // creamos lista con los nombres de las empresa que se muestran en pantalla
            List<IWebElement> nombresEmpresas = mainPage.NombresEmpresas;

            // assert 
            // creamos lista con los nombres de las empresa semilla
            List<string?> NombresEmpresasSemilla = EmpresasSemilla.Select(x => x.Nombre).ToList();

            // lista de strings con los nombres de las empresas en la vista
            List<string> NombresEmpresasVista = nombresEmpresas.Select(x => x.Text).ToList();

            foreach (string nombre in NombresEmpresasSemilla)
            {
                Assert.IsTrue(NombresEmpresasVista.Contains(nombre), $"No se mostro en la pantalla la empresa '{nombre}'");
            }
        }
    }
}