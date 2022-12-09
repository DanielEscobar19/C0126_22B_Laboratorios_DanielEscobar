using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace functional_tests.Pages
{
    /// <summary>
    /// Clase que maneja los botones de la vista de eliminar
    /// Vistas: Delet.cshtml
    /// </summary>
    public class DeletePage
    {
        private IWebDriver Driver;
        public DeletePage(IWebDriver driver)
        {
            Driver = driver;
        }

        /// <summary>
        /// boton con el cual se confirma el borrado de la empresa.
        /// </summary>
        public IWebElement BotonEliminar
        {
            get
            {
                return Helper.FindWaitElement(By.Id("botonEliminar"), Driver);
            }
        }
    }
}
