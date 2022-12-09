using app_source.Data;
using app_source.Models;
using functional_tests.Pages;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using System.Security.Policy;

namespace functional_tests
{
    /// <summary>
    /// Clase que testea el funcionamiento de las operaciones CRUD de la pagina
    /// </summary>
    public class CompaniesTest
    {
        /// <summary>
        /// driver que maneja la conexion con chrome y selenium
        /// </summary>
        private WebDriver Driver;

        /// <summary>
        /// instancia para manejar la base de datos
        /// </summary>
        private CompanyContext _DbContext;

        /// <summary>
        /// empresas que se ingresan en la base para teastear
        /// </summary>
        private List<CompanyModel>? EmpresasSemilla;

        /// <summary>
        /// instancia que meneja datos y botones de la pagina principal
        /// </summary>
        /// <remark>
        /// todos los metodos usan esta instancia porque se accede desde el index al resto de las paginas
        /// </remark>
        MainPage? MainPage;

        /// <summary>
        /// en los test se crean empresas y para que tenga nombres unicos se les agrega este string
        /// asi los datos de testeo no chocan con otros datos que puedan haber en la base
        /// </summary>
        private const string UNIQUEID = "FXc21c02748";

        /// <summary>
        /// en el setup inicializamos todos los atributos
        /// ademas navegamos a la pagina con el driver de selenium
        /// </summary>
        [SetUp]
        public void Setup()
        {
            // inicializacion de selenium
            Driver = new ChromeDriver();
            Driver.Url = Helper.URL;
            Driver.Manage().Window.Maximize();
            Driver.Navigate().GoToUrl(Helper.URL);

            // inicializacion de la conexion con la base de datos
            _DbContext = Helper.DbContext;

            // creamos lista para manejar datos semilla de test
            EmpresasSemilla = new List<CompanyModel>();

            // inicializamos manejador de elementos del index
            MainPage = new(Driver);
        }

        /// <summary>
        /// en teardown eliminamos los puestos semilla de prueba de la base para dejar la base como estaba antes del test
        /// ademas se cierra el driver de selenium
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            // eliminacion de datos semilla
            foreach (var item in EmpresasSemilla)
            {
                _DbContext.Remove(item); 
            }
            _DbContext.SaveChanges();
            EmpresasSemilla.Clear();
            EmpresasSemilla = null;

            // reseteo de instancia que maneja elementos del index
            MainPage = null;

            // cerramos la ventana que abre selenium
            Driver.Close();
        }

        /// <summary>
        /// test de la operacion read
        /// </summary>
        /// <objetivo>
        /// la prueba ingresa empresas semilla en la base y comprueba que se vea en la vista index de la aplicacion
        /// </objetivo>
        /// <resultado>
        /// se leen las empresas que hay en la tabla de index y se evalua que se muestren los mismos datos que se ingresaron con las empresas semilla
        /// </resultado>
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
                    TipoNegocio = $"Negocio de testing {i} {UNIQUEID}",
                    PaisBase = "Pais testing " + 1,
                    ValorEstimado = 1000.15m,
                    EsTransnacional = true
                };
                // ingresamos en la base y y actualizamos el modelo actual
                // la actualizacion es para recibir el numero de id que asigna la base
                tempCompany = _DbContext.Add(tempCompany).Entity;

                EmpresasSemilla.Add(tempCompany);
            }

            // guardamos en la base los cambios realizados
            _DbContext.SaveChanges();


            // recargamos la pagina para que se muestren los nuevos puestos
            Driver.Navigate().Refresh();
            

            // creamos lista con los nombres de las empresa que se muestran en pantalla
            List<IWebElement> nombresEmpresas = MainPage.NombresEmpresas;

            // assert 

            // creamos lista con los nombres de las empresa que se muestran en pantalla
            List<string> nombreEmpresaEnPantalla = MainPage.NombresEmpresas.Select(x => x.Text).ToList();

            // creamos lista con los tipos de negocio de las empresa que se muestran en pantalla
            List<string> tipoEmpresasEnPantalla = MainPage.TipoEmpresas.Select(x => x.Text).ToList();

            // creamos lista con los paises base de las empresa que se muestran en pantalla
            List<string> paisBaseEmpresasEnPantalla = MainPage.PaisEmpresas.Select(x => x.Text).ToList();

            // creamos lista con los valores estimados de las empresa que se muestran en pantalla
            List<string> valoresEmpresasEnPantalla = MainPage.ValorEmpresas.Select(x => x.Text).ToList();

            // creamos lista con los booleanos que indican si la empresa es transnacional que se muestran en pantalla
            List<string> esTransnacionalEmpresasEnPantalla = MainPage.EsTransnacionalEmpresas.Select(x => x.Text).ToList();

            // con el ciclo verificamos que los valores en pantalla sean los mismos que los de cada empresa igresada
            int indexEmpresa = 0;
            foreach (var empresaActual in EmpresasSemilla)
            {
                // obtenemos el indice de la empresa en la lista
                indexEmpresa = nombreEmpresaEnPantalla.FindIndex(x => x == empresaActual.Nombre);

                Assert.That(empresaActual.Nombre, Is.EqualTo(nombreEmpresaEnPantalla[indexEmpresa]), $"No se mostró en la pantalla la empresa '{empresaActual.Nombre}'");
                Assert.That(empresaActual.TipoNegocio, Is.EqualTo(tipoEmpresasEnPantalla[indexEmpresa]), $"No se mostró en la pantalla el tipo de negocio '{empresaActual.TipoNegocio}'");
                Assert.That(empresaActual.PaisBase, Is.EqualTo(paisBaseEmpresasEnPantalla[indexEmpresa]), $"No se mostró en la pantalla el país base '{empresaActual.PaisBase}'");
                Assert.That(empresaActual.ValorEstimado.ToString(), Is.EqualTo(valoresEmpresasEnPantalla[indexEmpresa]), $"No se mostró en la pantalla el valor estimado '{empresaActual.ValorEstimado}'");
                Assert.That(esTransnacionalEmpresasEnPantalla[indexEmpresa], Is.EqualTo("Sí"), $"No se mostró en la pantalla el booleano esperado 'Sí'");
            }
        }

        /// <summary>
        /// test de la operacin create
        /// </summary>
        /// <objetivo>
        /// al crear una empresa que no tiene un nombre repetido, se muestra en el index tal empresa
        /// </objetivo>
        /// <resultado>
        /// en el index se muestran los datos de la empresa creada
        /// </resultado>
        /// <remark>
        /// si el nombre esta repetido no se permite la creacion y se manda un mensaje en la vista
        /// en otro test se puede probar que esto ocurra correctamente
        /// </remark>
        [Test]
        public void CreateNotRepeatedNameCompany_ReturnsToIndexAndDisplaysNewCompany()
        {
            // arrange 
            MainPage.BotonCrear.Click();

            // page que maneja inputs de vista de crear
            CompanyInputs createPage = new(Driver);

            // empresa que se va a crear
            CompanyModel nuevaEmpresa = new CompanyModel{
                Nombre = $"Nombre no repetido {UNIQUEID}",
                PaisBase = $"Pais no repetido {UNIQUEID}",
                TipoNegocio = $"Tipo negocio no repetido {UNIQUEID}",
            };

            // action
            // ingresamos los datos para crear la empresa
            createPage.InputNombre.SendKeys(nuevaEmpresa.Nombre);
            createPage.InputPais.SendKeys(nuevaEmpresa.PaisBase);
            createPage.InputTipo.SendKeys(nuevaEmpresa.TipoNegocio);

            // confirmamos la creacion
            createPage.BotonAceptar.Click();

            // assert
            // creamos lista con los nombres de las empresa que se muestran en pantalla
            List<string> nombreEmpresaEnPantalla = MainPage.NombresEmpresas.Select(x => x.Text).ToList();
            
            // creamos lista con los tipos de negocio de las empresa que se muestran en pantalla
            List<string> tipoEmpresasEnPantalla = MainPage.TipoEmpresas.Select(x => x.Text).ToList();

            // creamos lista con los paises base de negocio de las empresa que se muestran en pantalla
            List<string> paisBaseEmpresasEnPantalla = MainPage.PaisEmpresas.Select(x => x.Text).ToList();

            // buscamos la posicion de la empresa creada en la tabla del index
            int indexNuevaEmpresa = nombreEmpresaEnPantalla.FindIndex(x => x == nuevaEmpresa.Nombre);

            // si no se encuentra la empresa creada
            Assert.That(indexNuevaEmpresa, Is.GreaterThan(-1), "No se encontró la empresa creada");
            
            // custom teardown
            // para que el teardown funcione bien 
            // lo hacemos luego de revisar que si se haya creado
            // los puestos semilla son eliminado de la base siempre
            // por eso lo agregamos aqui para que se elimine en caso de que falle el test
            List<CompanyModel> empresasEnBase = _DbContext.CompanyModel.ToList();
            // custom teardown

            CompanyModel? temp = empresasEnBase.Find(x => x.Nombre == nuevaEmpresa.Nombre);
            if (temp is not null)
            {
                EmpresasSemilla.Add(temp);
            }
            // fin custom teardown

            Assert.That(nuevaEmpresa.Nombre, Is.EqualTo(nombreEmpresaEnPantalla[indexNuevaEmpresa]), "El nombre mostrado en pantalla no es el mismo que el ingresado");
            Assert.That(nuevaEmpresa.PaisBase, Is.EqualTo(paisBaseEmpresasEnPantalla[indexNuevaEmpresa]), "El país base mostrado en pantalla no es el mismo que el ingresado");
            Assert.That(nuevaEmpresa.TipoNegocio, Is.EqualTo(tipoEmpresasEnPantalla[indexNuevaEmpresa]), "El tipo de negocio mostrado en pantalla no es el mismo que el ingresado");
        }

        /// <summary>
        /// test de la operacin delete
        /// </summary>
        /// <objetivo>
        /// ingresamos en la base una empresa la cual deseamos eliminar. En la tabla del index no se debe mostrar la empresa borrada
        /// </objetivo>
        /// <resultado>
        /// verificamos que en la tabla del index ninguna empresa tenga el nombre de la empresa que se elimino
        /// </resultado>
        /// <remark>
        /// solo el nombre de la empresa es required al crear una empresa. Para el test solo ingresamos este valor
        /// ya que no son necesarios los otros valores para el borrado
        /// </remark>
        [Test]
        public void DeletedCompany_DoesNotAppearInIndex()
        {
            // arrange 
            // creamos una empresa a borrar
            // solo es necesario ponerle un nombre unico para borrarla
            CompanyModel borrarCompany = new(){
                Nombre = $"Empresa a borrar {UNIQUEID}"
            };

            // actualiza el modelo segun la base para obtener el id de la base
            borrarCompany = _DbContext.Add(borrarCompany).Entity;
            _DbContext.SaveChanges();

            // refrescamos la pagina para que se muestre la nueva empresa
            Driver.Navigate().Refresh();
            
            // buscamos el boton de eliminar de la empresa que deseamos borrar
            List<string> nombresEmpresasEnPantalla = MainPage.NombresEmpresas.Select(x => x.Text).ToList();
            List<IWebElement> botonesELiminarEmpresasEnPantalla = MainPage.BotonesEliminar;
            int indexABorrar = nombresEmpresasEnPantalla.FindIndex(x => x == borrarCompany.Nombre);

            // action
            // clickeamos el boton de borrar
            botonesELiminarEmpresasEnPantalla[indexABorrar].Click();
            
            // en la pagina de borrado clickeamos el boton de eliminar
            DeletePage deletePage = new(Driver);
            deletePage.BotonEliminar.Click();

            // assert
            // lista con todos los nombres de las empresas que se muestran en el index
            nombresEmpresasEnPantalla = MainPage.NombresEmpresas.Select(x => x.Text).ToList();

            // custom teardown
            // si no se borro correctamente lo agregamos a la lista de empresas semilla para que el teardown lo elimine
            string? noSeBorro = nombresEmpresasEnPantalla.Find(x => x == borrarCompany.Nombre);
            if (noSeBorro is not null)
            {
                EmpresasSemilla.Add(borrarCompany);
            }
            // fin custom teardown

            // si no esta el nombre de la empresa que borramos se cumplio con exito la prueba
            Assert.IsFalse(nombresEmpresasEnPantalla.Contains(borrarCompany.Nombre), "La empresa no se eliminó correctamente y se mostró en pantalla");
        }

        /// <summary>
        /// test de la operacin udpate
        /// </summary>
        /// <objetivo>
        /// ingresamos en la base una empresa que deseamos actualizar. luego actualizamos sus datos y vemos que en el index se muestren los nuevos datos.
        /// </objetivo>
        /// <resultado>
        /// se verifica que en la tabla del index se muestren los nuevos datos de la empresa y que no aparezcan los datos viejos
        /// </resultado>
        /// <remark>
        /// si se actualiza con un nombre ya existente no se permite la accion y se muestra un mensaje de error en la pantalla
        /// esta funcionalidad se debe probar en otro metodo de test
        /// </remark>
        [Test]
        public void UpdatedCompany_AppearsInIndex()
        {
            // arrange 

            string nombrePreActualizacion = $"Nombre pre actualizacion {UNIQUEID}";

            // empresa base que deseamos actualizar
            CompanyModel empresaActualizar = new CompanyModel
            {
                Nombre = nombrePreActualizacion,
                ValorEstimado = 1909.56m,
                PaisBase = $"Pais pre actualizacion {UNIQUEID}",
                TipoNegocio = $"Tipo negocio pre actualizacion {UNIQUEID}",
                EsTransnacional = true
            };

            // actualiza el modelo segun la base para obtener el id de la base
            empresaActualizar = _DbContext.Add(empresaActualizar).Entity;
            _DbContext.SaveChanges();

            // ingresamos modelo en las plantillas para que sea eliminado en el teardown
            EmpresasSemilla.Add(empresaActualizar);

            // refrescamos la pagina para que se muestre la nueva empresa
            Driver.Navigate().Refresh();

            // buscamos el botond de editar de la empresa
            List<string> nombresEmpresasEnPantalla = MainPage.NombresEmpresas.Select(x => x.Text).ToList();
            List<IWebElement> botonesEditar = MainPage.BotonesEditar;
            int indexAEditar = nombresEmpresasEnPantalla.FindIndex(x => x == empresaActualizar.Nombre);

            // action
            // clickeamos el boton de editar de la empresa
            botonesEditar[indexAEditar].Click();
        
            // acualizamos datos del modelo
            empresaActualizar.Nombre = $"Nombre post actualizacion {UNIQUEID}";
            empresaActualizar.ValorEstimado = 4587.56m;
            empresaActualizar.PaisBase = $"Pais post actualizacion {UNIQUEID}";
            empresaActualizar.TipoNegocio = $"Tipo negocio post actualizacion {UNIQUEID}";
            empresaActualizar.EsTransnacional = false;
            
            // page que manejar los inputs del modelo en la vista
            CompanyInputs editPage = new(Driver);

            // ingresamos los datos nuevos en los inputs
            // los nuevos datos esta en el modelo empresaActualizar
            editPage.InputNombre.Clear();
            editPage.InputNombre.SendKeys(empresaActualizar.Nombre);

            editPage.InputValor.Clear();
            editPage.InputValor.SendKeys(empresaActualizar.ValorEstimado.ToString());

            editPage.InputPais.Clear();
            editPage.InputPais.SendKeys(empresaActualizar.PaisBase);

            editPage.InputTipo.Clear();
            editPage.InputTipo.SendKeys(empresaActualizar.TipoNegocio);
            
            editPage.InputEsTransnacional.Click();

            // hacemos click del confirmar
            editPage.BotonAceptar.Click();

            // assert
            // lista con los nombres de las empresas en pantalla
            nombresEmpresasEnPantalla = MainPage.NombresEmpresas.Select(x => x.Text).ToList();

            // lista con los valores estimados de las empresas en pantalla
            List<string> valoresEnPantalla = MainPage.ValorEmpresas.Select(x => x.Text).ToList();

            // lista con los paises base de las empresas en pantalla
            List<string> paisesEnPantalla = MainPage.PaisEmpresas.Select(x => x.Text).ToList();

            // lista con los tipos de negocio de las empresas en pantalla
            List<string> tiposEnPantalla = MainPage.TipoEmpresas.Select(x => x.Text).ToList();

            // creamos lista con los booleanos que indican si la empresa es transnacional que se muestran en pantalla
            List<string> esTransnacionalEmpresasEnPantalla = MainPage.EsTransnacionalEmpresas.Select(x => x.Text).ToList();

            // indice de la empresa en cuestion
            int indexAVerificar = nombresEmpresasEnPantalla.FindIndex(x => x == empresaActualizar.Nombre);

            Assert.That(indexAVerificar, Is.GreaterThan(-1), "No se encontró el negocio actualizado");
            Assert.That(empresaActualizar.Nombre, Is.EqualTo(nombresEmpresasEnPantalla[indexAVerificar]), "El nombre no se actualizó correctamente");
            Assert.That(empresaActualizar.ValorEstimado.ToString(), Is.EqualTo(valoresEnPantalla[indexAVerificar]), "El valor estimado no se actualizó correctamente");
            Assert.That(empresaActualizar.PaisBase, Is.EqualTo(paisesEnPantalla[indexAVerificar]), "El país base no se actualizó correctamente");
            Assert.That(empresaActualizar.TipoNegocio, Is.EqualTo(tiposEnPantalla[indexAVerificar]), "El tipo de negocio no se actualizó correctamente");
            Assert.That(esTransnacionalEmpresasEnPantalla[indexAVerificar], Is.EqualTo("No"), "El booleano es transnacional no se actualizó correctamente");
            Assert.IsFalse(nombresEmpresasEnPantalla.Contains(nombrePreActualizacion), "El nombre pre actualización de la empresa sigue en la lista");
        }
    }
}