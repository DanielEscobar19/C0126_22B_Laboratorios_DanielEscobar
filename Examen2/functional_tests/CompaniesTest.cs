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
        MainPage MainPage;


    [SetUp]
        public void Setup()
        {
            Driver = new ChromeDriver();
            Driver.Url = Helper.URL;
            Driver.Manage().Window.Maximize();
            Driver.Navigate().GoToUrl(Helper.URL);
            _DbContext = Helper.DbContext;
            EmpresasSemilla = new List<CompanyModel>();
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var item in EmpresasSemilla)
            {
                _DbContext.Remove(item); 
            }
            EmpresasSemilla.Clear();
            EmpresasSemilla = null;
            _DbContext.SaveChanges();
            Driver.Close();
        }

        [Test]
        public void ReadOperation_DisplaysAllCompanies()
        {
            // arrange 
            // creamos lista con empresas de prueba
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
            MainPage = new(Driver);

            // creamos lista con los nombres de las empresa que se muestran en pantalla
            List<IWebElement> nombresEmpresas = MainPage.NombresEmpresas;

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

        [Test]
        public void CreateNotRepeatedNameCompany_ReturnsToIndexAndDisplaysNewCompany()
        {
            // arrange 
            MainPage = new(Driver);
            MainPage.BotonCrear.Click();

            CreatePage createPage = new(Driver);

            CompanyModel nuevaEmpresa = new CompanyModel{
                Nombre = "Nombre no repetido 456789",
                PaisBase = "Pais no repetido 456789",
                TipoNegocio = "Tipo negocio no repetido 456789",
            };

            // action
            createPage.InputNombre.SendKeys(nuevaEmpresa.Nombre);
            createPage.InputPais.SendKeys(nuevaEmpresa.PaisBase);
            createPage.InputTipo.SendKeys(nuevaEmpresa.TipoNegocio);

            createPage.BotonCrear.Click();

            // assert
            // creamos lista con los nombres de las empresa que se muestran en pantalla
            List<string> nombreEmpresaEnPantalla = MainPage.NombresEmpresas.Select(x => x.Text).ToList();
            List<string> tipoEmpresasEnPantalla = MainPage.TipoEmpresas.Select(x => x.Text).ToList();
            List<string> paisBaseEmpresasEnPantalla = MainPage.PaisEmpresas.Select(x => x.Text).ToList();

            int indexNuevaEmpresa = nombreEmpresaEnPantalla.FindIndex(x => x == nuevaEmpresa.Nombre);

            Assert.That(indexNuevaEmpresa, Is.GreaterThan(-1), "No se encontró la empresa creada");
            
            // para que el teardown funcione bien 
            // lo hacemos luego de revisar que si se haya creado
            // los puestos semilal son eliminado de la base siempre
            // por eso lo agregamos aqui para que se elimine en caso de que falle el test
            List<CompanyModel> empresasEnBase = _DbContext.CompanyModel.ToList();

            CompanyModel? temp = empresasEnBase.Find(x => x.Nombre == nuevaEmpresa.Nombre);
            if (temp is not null)
            {
                EmpresasSemilla.Add(temp);
            }
            //

            Assert.That(nuevaEmpresa.Nombre, Is.EqualTo(nombreEmpresaEnPantalla[indexNuevaEmpresa]), "El nombre mostrado en pantalla no es el mismo que el ingresado");
            Assert.That(nuevaEmpresa.PaisBase, Is.EqualTo(paisBaseEmpresasEnPantalla[indexNuevaEmpresa]), "El país base mostrado en pantalla no es el mismo que el ingresado");
            Assert.That(nuevaEmpresa.TipoNegocio, Is.EqualTo(tipoEmpresasEnPantalla[indexNuevaEmpresa]), "El tipo de negocio mostrado en pantalla no es el mismo que el ingresado");
        }
    }
}