using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using System.IO;
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
        /// <param name="XMLTimetableFilePathField">Path to the data file.</param>
        /// <param name="SemesterStartField">Starting date of the study period.</param>
        /// <param name="SemesterEndField">Ending date of the study period.</param>
        /// <param name="CreatedField">The date when the timetable data generated.</param>
        /// <param name="LinkToInfoField">Link to the additional info in timetable footer.</param>
        /// <param name="WelcomeMessage">Message to be shown on the empty filtering result list.</param>
        /// <param name="PrefixPoolLinkField">Prefix of the link to the course popularity pool.</param>
        /// <param name="SufixPoolLinkField">Sufix of the link to the course popularity link.
        /// <returns>Partial view of the actualized Config.</returns>
        [HttpPost]
        public PartialViewResult ConfigButtonAction(string submitButton, string XMLTimetableFilePathField, DateTime SemesterStartField, DateTime SemesterEndField, 
                                                    DateTime CreatedField, string LinkToInfoField, string WelcomeMessage, string PrefixPoolLinkField, string SufixPoolLinkField)
        {
            log.Debug("Method entry.");
            m_config.ErrorMessage = "";
            switch (submitButton)
            {
                case "Reload":
                    m_config.Set(XMLTimetableFilePathField, SemesterStartField, SemesterEndField, CreatedField, LinkToInfoField, WelcomeMessage, PrefixPoolLinkField, SufixPoolLinkField);
                    if (!m_config.SaveToFile())
                        m_config.ErrorMessage = "Nepovedlo se uložit nastavení do souboru, více v logovacím souboru./n";
                    ModelState.Clear();
                    if (!XMLTimetable.Instance.Refresh(m_config.XMLTimetableFilePath))
                        m_config.ErrorMessage += "Nepovedlo se načíst nebo naparsovat soubor. Více informací v log souboru./n";
                    log.Debug("Reload button clicked.");
                    return PartialView("Config", m_config.GetIConfig());

                case "Uložit":
                    m_config.Set(XMLTimetableFilePathField, SemesterStartField, SemesterEndField, CreatedField, LinkToInfoField, WelcomeMessage, PrefixPoolLinkField, SufixPoolLinkField);
                    if (!m_config.SaveToFile())
                        m_config.ErrorMessage = "Nepovedlo se uložit nastavení do souboru, více v logovacím souboru.";
                    ModelState.Clear();
                    log.Debug("Uložit button clicked.");
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
            byte[] honzik =     Convert.FromBase64String("4zJDk3AHl5HuiaKAdeH3bcweE1Y=");
            byte[] stika =      Convert.FromBase64String("VenMx97CkpVO0NmJb+cywDpEZ18=");
            byte[] krbalek =    Convert.FromBase64String("");

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
            if (sha1PasswordBytes.SequenceEqual(honzik))
            {
                log.Info("Honzík successfully loged on.");
                return true;
            }
            if (sha1PasswordBytes.SequenceEqual(stika))
            {
                log.Info("Štika successfully loged on.");
                return true;
            }

            //LOG(neúspěšné přihlášení)
            log.Info("Uuccessfull logging on attempt.");
            return false;
        }

    }
}
