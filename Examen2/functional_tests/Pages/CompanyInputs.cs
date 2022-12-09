using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace functional_tests.Pages
{
    /// <summary>
    /// Clase que maneja los inpust de edicion y creacion de una empresa
    /// Vistas: Create.cshtml y Edit.cshtml
    /// </summary>
    public class CompanyInputs
    {
        private IWebDriver Driver;
        public CompanyInputs(IWebDriver driver)
        {
            Driver = driver;
        }

        /// <summary>
        /// boton que clickea el usuario para confirmar la edicion o creacion de la empresa.
        /// </summary>
        public IWebElement BotonAceptar
        {
            get {
                return Helper.FindWaitElement(By.Name("botonAceptar"), Driver);
            }
        }

        /// <summary>
        /// boton que clickea el usuario para cancelar la edicion o creacion de la empresa.
        /// </summary>
        public IWebElement BotonCancelar
        {
            get
            {
                return Helper.FindWaitElement(By.Name("botonCancelar"), Driver);
            }
        }

        /// <summary>
        /// input donde se ingresa el nombre de la empresa
        /// </summary>
        public IWebElement InputNombre
        {
            get
            {
                return Helper.FindWaitElement(By.Id("inputNombre"), Driver);
            }
        }

        /// <summary>
        /// input donde se ingresa el tipo de negocio de la empresa
        /// </summary>
        public IWebElement InputTipo
        {
            get
            {
                return Helper.FindWaitElement(By.Id("inputTipo"), Driver);
            }
        }

        /// <summary>
        /// input donde se ingresa el el pais base de la empresa
        /// </summary>
        public IWebElement InputPais
        {
            get
            {
                return Helper.FindWaitElement(By.Id("inputPais"), Driver);
            }
        }

        /// <summary>
        /// input donde se ingresa el valor estimado de la empresa
        /// </summary>
        public IWebElement InputValor
        {
            get
            {
                return Helper.FindWaitElement(By.Id("inputValor"), Driver);
            }
        }

        /// <summary>
        /// input donde se booleano que indica si es transacional o no la empresa
        /// </summary>
        public IWebElement InputEsTransnacional
        {
            get
            {
                return Helper.FindWaitElement(By.Id("inputEsTransnacional"), Driver);
            }
        }
    }
}
