using System;
using System.Linq;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using Rozvrh.Models;

namespace Rozvrh.Controllers
{
    /// <summary>
    /// Controller for the Admin site.
    /// </summary>
    public class AdminController : Controller
    {
        /// <summary>
        /// Property holding the configuration instatnce.
        /// </summary>
        Config m_config;

        /// <summary>
        /// Controller constructor.
        /// </summary>
        public AdminController()
        {
            m_config = Config.Instance;
        }

        /// <summary>
        /// Logger instance.
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Method giving the Index when ~/Admin site is called.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View("Index");
        }

        /// <summary>
        /// Method processing the input of the password field.
        /// </summary>
        /// <param name="passwordBox">Given password.</param>
        /// <returns>The Index view for invalid password, Config view otherwise.</returns>
        public ActionResult ValidatePassword(string passwordBox)
        {
            log.Debug("Method entry.");
            if (passwordPassed(passwordBox))
            {
                log.Debug("Method exit.");
                return View("Config", m_config.GetIConfig());
            }
            else
            {
                log.Debug("Method exit.");
                return View("Index", (object)"Neplatné heslo! Zkus to znovu.");
            }
            
        }

        /// <summary>
        /// Method processing input from the configuration fields of the Config view.
        /// </summary>
        /// <param name="submitButton">Which type of action should be done.</param>
        /// <param name="ArchivePath">Path to the folder with timetable files (.xml and .info)</param>
        /// <param name="WelcomeMessageFilePath">Path to the file with message to be shown on the empty filtering result list.</param>
        /// <returns>Partial view of the actualized Config.</returns>
        [HttpPost]
        public PartialViewResult ConfigButtonAction(string submitButton, string ArchivePath, string WelcomeMessageFilePath)
        {
            log.Debug("Method entry.");
            ViewBag.ErrorMessage = "";
            switch (submitButton)
            {

                case "Uložit a Restartovat":
                    m_config.Set(ArchivePath, WelcomeMessageFilePath);
                    if (!m_config.SaveToFile())
                        ViewBag.ErrorMessage = "Nepovedlo se uložit nastavení do souboru, více v logovacím souboru./n";
                    ModelState.Clear();
                    if (!XMLTimetable.Instance.LoadDatabase())
                        ViewBag.ErrorMessage += "Nepovedlo se načíst nebo naparsovat soubor. Více informací v log souboru./n";
                    log.Debug("Save and Reload button clicked.");
                    return PartialView("Config", m_config.GetIConfig());

                case "Zrušit":
                    ModelState.Clear();
                    log.Debug("Zrušit button clicked.");
                    return PartialView("Config", m_config.GetIConfig());
                
                default:
                     ModelState.Clear();
                    log.Debug("Default action.");
                    return PartialView("Config", m_config.GetIConfig());    
            }
        }
        
        /// <summary>
        /// Validates given password.
        /// </summary>
        /// <param name="password">Given password string.</param>
        /// <returns>True for registered password, false otherwise.</returns>
        private bool passwordPassed(string password)
        {
            log.Debug("Trying to validate password.");

            byte[] passwordBytes = Encoding.ASCII.GetBytes(password);
            var sha1 = new SHA1CryptoServiceProvider();
            byte[] sha1PasswordBytes = sha1.ComputeHash(passwordBytes);
            sha1PasswordBytes = sha1.ComputeHash(Encoding.ASCII.GetBytes(Convert.ToBase64String(sha1PasswordBytes)));

            byte[] blatsky =    Convert.FromBase64String("E3oVOby8gYF+HUdfP3JV5tK+3a4=");
            byte[] krbalek =    Convert.FromBase64String("ULhngopNWAsQhSq4cWuMj+yC3YY=");

            if (sha1PasswordBytes.SequenceEqual(krbalek))
            {
                log.Info("Krbálek successfully loged on.");
                return true;
            }
            if (sha1PasswordBytes.SequenceEqual(blatsky))
            {
                log.Info("Blatský successfully loged on.");
                return true;
            }

            log.Info("Uuccessfull logging on attempt.");
            return false;
        }

    }
}
