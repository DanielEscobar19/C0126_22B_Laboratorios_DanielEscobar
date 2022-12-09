using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace functional_tests.Pages
{
    public class DeletePage
    {
        private IWebDriver Driver;
        public DeletePage(IWebDriver driver)
        {
            Driver = driver;
        }

        public IWebElement BotonEliminar
        {
            get
            {
                return Helper.FindWaitElement(By.Id("botonEliminar"), Driver);
            }
        }
    }
}
