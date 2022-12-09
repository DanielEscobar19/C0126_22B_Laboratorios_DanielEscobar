using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace functional_tests.Pages
{
    public class CompanyInputs
    {
        private IWebDriver Driver;
        public CompanyInputs(IWebDriver driver)
        {
            Driver = driver;
        }

        public IWebElement BotonAceptar
        {
            get {
                return Helper.FindWaitElement(By.Name("botonAceptar"), Driver);
            }
        }

        public IWebElement BotonCancelar
        {
            get
            {
                return Helper.FindWaitElement(By.Name("botonCancelar"), Driver);
            }
        }

        public IWebElement InputNombre
        {
            get
            {
                return Helper.FindWaitElement(By.Id("inputNombre"), Driver);
            }
        }
        public IWebElement InputTipo
        {
            get
            {
                return Helper.FindWaitElement(By.Id("inputTipo"), Driver);
            }
        }

        public IWebElement InputPais
        {
            get
            {
                return Helper.FindWaitElement(By.Id("inputPais"), Driver);
            }
        }

        public IWebElement InputValor
        {
            get
            {
                return Helper.FindWaitElement(By.Id("inputValor"), Driver);
            }
        }

        public IWebElement InputEsTransnacional
        {
            get
            {
                return Helper.FindWaitElement(By.Id("inputEsTransnacional"), Driver);
            }
        }
    }
}
