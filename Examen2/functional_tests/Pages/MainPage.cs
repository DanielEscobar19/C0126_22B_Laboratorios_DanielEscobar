using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace functional_tests.Pages
{
    /// <summary>
    /// Clase que maneja botones y datos de la pagina principal
    /// Vistas: Index.cshtml
    /// </summary>
    public class MainPage
    {
        private IWebDriver Driver;

        public MainPage (IWebDriver driver)
        {
            this.Driver = driver;
        }

        /// <summary>
        /// instancia para clickear el boton de agregar una empresa
        /// </summary>
        public IWebElement BotonCrear
        {
            get {
                return Helper.FindWaitElement(By.Name("botonAgregarEmpresa"), this.Driver);
            }
        }

        /// <summary>
        /// atributo que retorna los botones de editar de las empresas
        /// </summary>
        public List<IWebElement> BotonesEditar
        {
            get
            {
                return Driver.FindElements(By.Name("botonEditarEmpresa")).ToList();
            }
        }

        /// <summary>
        /// atributo que retorna los botones de editar de las empresas
        /// </summary>
        public List<IWebElement> BotonesEliminar
        {
            get
            {
                return Driver.FindElements(By.Name("botonEliminarEmpresa")).ToList();
            }
        }

        /// <summary>
        /// atributo que retorna los nombres de las empresas
        /// </summary>
        public List<IWebElement> NombresEmpresas
        {
            get
            {
                return Driver.FindElements(By.Id("nombreEmpresa")).ToList();
            }
        }

        /// <summary>
        /// atributo que retorna los tipos de negocio de las empresas
        /// </summary>
        public List<IWebElement> TipoEmpresas
        {
            get
            {
                return Driver.FindElements(By.Id("tipoNegocio")).ToList();
            }
        }

        /// <summary>
        /// atributo que retorna los paises base de las empresas
        /// </summary>
        public List<IWebElement> PaisEmpresas
        {
            get
            {
                return Driver.FindElements(By.Id("paisBase")).ToList();
            }
        }

        /// <summary>
        /// atributo que retorna los valores estimadosde las empresas
        /// </summary>
        public List<IWebElement> ValorEmpresas
        {
            get
            {
                return Driver.FindElements(By.Id("valorEstimado")).ToList();
            }
        }

        /// <summary>
        /// atributo que retorna los booleanos de las empresas indicando si son transnacionales
        /// </summary>
        /// <remark>
        /// los booleanos se muestran en la como texto: Si y No
        /// por lo que el IWebElement contiene tal texto
        /// </remark>
        public List<IWebElement> EsTransnacionalEmpresas
        {
            get
            {
                return Driver.FindElements(By.Id("esTransnacional")).ToList();
            }
        }
    }
}
