using app_source.Data;
using app_source.Models;
using functional_tests.Pages;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Linq;
using System.Security.Policy;

namespace functional_tests
{
    public class Tests
    {
        private WebDriver Driver;
        private CompanyContext _DbContext;
        private List<CompanyModel>? EmpresasSemilla;
        MainPage? MainPage;


    [SetUp]
        public void Setup()
        {
            Driver = new ChromeDriver();
            Driver.Url = Helper.URL;
            Driver.Manage().Window.Maximize();
            Driver.Navigate().GoToUrl(Helper.URL);
            _DbContext = Helper.DbContext;
            EmpresasSemilla = new List<CompanyModel>();
            MainPage = new(Driver);
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var item in EmpresasSemilla)
            {
                _DbContext.Remove(item); 
            }
            _DbContext.SaveChanges();
            EmpresasSemilla.Clear();
            EmpresasSemilla = null;
            MainPage = null;
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
            MainPage.BotonCrear.Click();

            CompanyInputs createPage = new(Driver);

            CompanyModel nuevaEmpresa = new CompanyModel{
                Nombre = "Nombre no repetido 456789",
                PaisBase = "Pais no repetido 456789",
                TipoNegocio = "Tipo negocio no repetido 456789",
            };

            // action
            createPage.InputNombre.SendKeys(nuevaEmpresa.Nombre);
            createPage.InputPais.SendKeys(nuevaEmpresa.PaisBase);
            createPage.InputTipo.SendKeys(nuevaEmpresa.TipoNegocio);

            createPage.BotonAceptar.Click();

            // assert
            // creamos lista con los nombres de las empresa que se muestran en pantalla
            List<string> nombreEmpresaEnPantalla = MainPage.NombresEmpresas.Select(x => x.Text).ToList();
            List<string> tipoEmpresasEnPantalla = MainPage.TipoEmpresas.Select(x => x.Text).ToList();
            List<string> paisBaseEmpresasEnPantalla = MainPage.PaisEmpresas.Select(x => x.Text).ToList();

            int indexNuevaEmpresa = nombreEmpresaEnPantalla.FindIndex(x => x == nuevaEmpresa.Nombre);

            Assert.That(indexNuevaEmpresa, Is.GreaterThan(-1), "No se encontró la empresa creada");
            
            // para que el teardown funcione bien 
            // lo hacemos luego de revisar que si se haya creado
            // los puestos semilla son eliminado de la base siempre
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

        [Test]
        public void DeletedCompany_DoesNotAppearInIndex()
        {
            // arrange 
            // creamos una empresa a borrar
            // solo es necesario ponerle un nombre unico para borrarla
            CompanyModel borrarCompany = new(){
                Nombre = "Empresa a borrar 45678"
            };

            // actualiza el modelo segun la base para obtener el id de la base
            borrarCompany = _DbContext.Add(borrarCompany).Entity;
            _DbContext.SaveChanges();

            // refrescamos la pagina para que se muestre la nueva empresa
            Driver.Navigate().Refresh();
            
            List<string> nombresEmpresasEnPantalla = MainPage.NombresEmpresas.Select(x => x.Text).ToList();
            List<IWebElement> botonesELiminarEmpresasEnPantalla = MainPage.BotonesEliminar;
            int indexABorrar = nombresEmpresasEnPantalla.FindIndex(x => x == borrarCompany.Nombre);

            // action
            botonesELiminarEmpresasEnPantalla[indexABorrar].Click();
            DeletePage deletePage = new(Driver);
            deletePage.BotonEliminar.Click();

            // assert
            nombresEmpresasEnPantalla = MainPage.NombresEmpresas.Select(x => x.Text).ToList();

            Assert.IsFalse(nombresEmpresasEnPantalla.Contains(borrarCompany.Nombre), "La empresa no se eliminó correctamente y se mostró en pantalla");
        }

        [Test]
        public void UpdatedCompany_AppearsInIndex()
        {
            // arrange 
            CompanyModel empresaActualizar = new CompanyModel
            {
                Nombre = "Nombre pre actualizacion 456789",
                ValorEstimado = 1909.56m,
                PaisBase = "Pais pre actualizacion 456789",
                TipoNegocio = "Tipo negocio pre actualizacion 456789",
                EsTransnacional = true
            };

            // actualiza el modelo segun la base para obtener el id de la base
            empresaActualizar = _DbContext.Add(empresaActualizar).Entity;
            _DbContext.SaveChanges();

            // ingresamos modelo en las plantillas para que sea eliminado en el teardown
            EmpresasSemilla.Add(empresaActualizar);

            // refrescamos la pagina para que se muestre la nueva empresa
            Driver.Navigate().Refresh();


            List<string> nombresEmpresasEnPantalla = MainPage.NombresEmpresas.Select(x => x.Text).ToList();
            List<IWebElement> botonesEditar = MainPage.BotonesEditar;
            int indexAEditar = nombresEmpresasEnPantalla.FindIndex(x => x == empresaActualizar.Nombre);

            // acualizamos datos del modelo
            empresaActualizar.Nombre = "Nombre post actualizacion 456789";
            empresaActualizar.ValorEstimado = 4587.56m;
            empresaActualizar.PaisBase = "Pais post actualizacion 456789";
            empresaActualizar.TipoNegocio = "Tipo negocio post actualizacion 456789";
            empresaActualizar.EsTransnacional = false;


            // action
            botonesEditar[indexAEditar].Click();
            CompanyInputs editPage = new(Driver);

            // ingresamos los datos nuevos en los inputs
            editPage.InputNombre.Clear();
            editPage.InputNombre.SendKeys(empresaActualizar.Nombre);

            editPage.InputValor.Clear();
            editPage.InputValor.SendKeys(empresaActualizar.ValorEstimado.ToString());

            editPage.InputPais.Clear();
            editPage.InputPais.SendKeys(empresaActualizar.PaisBase);

            editPage.InputTipo.Clear();
            editPage.InputTipo.SendKeys(empresaActualizar.TipoNegocio);
            
            editPage.InputEsTransnacional.Click();

            editPage.BotonAceptar.Click();

            // assert
            nombresEmpresasEnPantalla = MainPage.NombresEmpresas.Select(x => x.Text).ToList();
            List<string> valoresEnPantalla = MainPage.ValorEmpresas.Select(x => x.Text).ToList();
            List<string> paisesEnPantalla = MainPage.PaisEmpresas.Select(x => x.Text).ToList();
            List<string> tiposEnPantalla = MainPage.TipoEmpresas.Select(x => x.Text).ToList();

            int indexAVerificar = nombresEmpresasEnPantalla.FindIndex(x => x == empresaActualizar.Nombre);

            Assert.That(indexAVerificar, Is.GreaterThan(-1), "No se encontró el negocio actualizado");
            Assert.That(empresaActualizar.Nombre, Is.EqualTo(nombresEmpresasEnPantalla[indexAVerificar]), "El nombre no se actualizó correctamente");
            Assert.That(empresaActualizar.ValorEstimado.ToString(), Is.EqualTo(valoresEnPantalla[indexAVerificar]), "El valor estimado no se actualizó correctamente");
            Assert.That(empresaActualizar.PaisBase, Is.EqualTo(paisesEnPantalla[indexAVerificar]), "El país base no se actualizó correctamente");
            Assert.That(empresaActualizar.TipoNegocio, Is.EqualTo(tiposEnPantalla[indexAVerificar]), "El tipo de negocio no se actualizó correctamente");
            Assert.That(empresaActualizar.EsTransnacional, Is.EqualTo(false), "El booleano es transnacional no se actualizó correctamente");
        }
    }
}