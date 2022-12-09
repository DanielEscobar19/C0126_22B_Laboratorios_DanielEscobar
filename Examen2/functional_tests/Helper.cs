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
    public class Helper
    {
        public static string URL = "https://localhost:7013/";

        private static string Connectionstring = "Data Source=172.16.202.209;Initial Catalog=C02748;TrustServerCertificate=True;Persist Security Info=True;User ID=C02748;Password=c02748";

        public static CompanyContext DbContext
        {
            get
            {
                var optionsBuilder = new DbContextOptionsBuilder<CompanyContext>();
                optionsBuilder.UseSqlServer(Connectionstring);

                return new CompanyContext(optionsBuilder.Options);
            }

        }

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
