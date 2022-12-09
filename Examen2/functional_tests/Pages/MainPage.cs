using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace functional_tests.Pages
{
    public class MainPage
    {
        private IWebDriver Driver;

        public MainPage (IWebDriver driver)
        {
            this.Driver = driver;
        }

        // instancia para clickear el boton de agregar una empresa
        public IWebElement BotonCrear
        {
            get {
                return Helper.FindWaitElement(By.Name("botonAgregarEmpresa"), this.Driver);
            }
        }

        // metodo que retorna los botones de editar de las empresas
        public List<IWebElement> BotonesEditar
        {
            get
            {
                return Driver.FindElements(By.Name("botonEditarEmpresa")).ToList();
            }
        }

        // metodo que retorna los botones de eliminar de las empresas
        public List<IWebElement> BotonesEliminar
        {
            get
            {
                return Driver.FindElements(By.Name("botonEliminarEmpresa")).ToList();
            }
        }

        // metodo que retorna los nombres de las empresas
        public List<IWebElement> NombresEmpresas
        {
            get
            {
                return Driver.FindElements(By.Id("nombreEmpresa")).ToList();
            }
        }

        public List<IWebElement> TipoEmpresas
        {
            get
            {
                return Driver.FindElements(By.Id("tipoNegocio")).ToList();
            }
        }

        public List<IWebElement> PaisEmpresas
        {
            get
            {
                return Driver.FindElements(By.Id("paisBase")).ToList();
            }
        }

        public List<IWebElement> ValorEmpresas
        {
            get
            {
                return Driver.FindElements(By.Id("valorEstimado")).ToList();
            }
        }

        public List<IWebElement> EsTransnacionalEmpresas
        {
            get
            {
                return Driver.FindElements(By.Id("esTransnacional")).ToList();
            }
        }
    }
}
