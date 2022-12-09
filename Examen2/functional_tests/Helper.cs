using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using app_source.Data;

namespace functional_tests
{
    /// <summary>
    /// Clase con metodos que asisten en los test
    /// </summary>
    public class Helper
    {
        /// <summary>
        /// string con la url a la que acceder en los test
        /// </summary>
        public static string URL = "https://localhost:7013/";

        /// <summary>
        /// connection string con la base de datos que se desea usar
        /// </summary>
        private static string Connectionstring = "Data Source=172.16.202.209;Initial Catalog=C02748;TrustServerCertificate=True;Persist Security Info=True;User ID=C02748;Password=c02748";

        /// <summary>
        /// instancia que permite acceder y manejar la base de datos
        /// </summary>
        public static CompanyContext DbContext
        {
            get
            {
                var optionsBuilder = new DbContextOptionsBuilder<CompanyContext>();
                optionsBuilder.UseSqlServer(Connectionstring);

                return new CompanyContext(optionsBuilder.Options);
            }

        }

        /// <summary>
        /// metodo que busca un elemento y hace una espera de 20 segundos
        /// </summary>
        /// <remark>
        /// se usa para evitar errores si la pagina dura en cargar y el elemento no se muestra a tiempo
        /// </remark>
        static public IWebElement FindWaitElement(By identificador, IWebDriver driver)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            wait.Until(condition =>
            {
                try
                {
                    var elementToBeDisplayed = driver.FindElement(identificador);
                    return elementToBeDisplayed.Displayed;
                }
                catch (StaleElementReferenceException ex)
                {
                    var elementToBeDisplayed = driver.FindElement(identificador);
                    return elementToBeDisplayed.Displayed;
                }
                catch (TimeoutException e)
                {
                    var elementToBeDisplayed = driver.FindElement(identificador);
                    return elementToBeDisplayed.Displayed;
                }

            });
            Thread.Sleep(500);

            return driver.FindElement(identificador);

        }
    }
}
