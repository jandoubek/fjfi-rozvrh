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
            if (passwordPassed(passwordBox))
            {
                return View("Config", m_config.GetIConfig());
            }
            else
            {
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
        /// <param name="PrefixPoolLinkField">Prefix of the link to the course popularity pool.</param>
        /// <param name="SufixPoolLinkField">Sufix of the link to the course popularity link.
        /// <returns>Partial view of the actualized Config.</returns>
        [HttpPost]
        public PartialViewResult ConfigButtonAction(string submitButton, string XMLTimetableFilePathField, DateTime SemesterStartField, DateTime SemesterEndField, 
                                                    DateTime CreatedField, string LinkToInfoField, string PrefixPoolLinkField, string SufixPoolLinkField)
        {
           
            switch (submitButton)
            {
                case "Reload":
                    m_config.Set(XMLTimetableFilePathField, SemesterStartField, SemesterEndField, CreatedField, LinkToInfoField, PrefixPoolLinkField, SufixPoolLinkField);
                    m_config.SaveToFile();
                    ModelState.Clear();
                    XMLTimetable.Instance.Refresh(m_config.XMLTimetableFilePath);
                    return PartialView("Config", m_config.GetIConfig());

                case "Uložit":
                    m_config.Set(XMLTimetableFilePathField, SemesterStartField, SemesterEndField, CreatedField, LinkToInfoField, PrefixPoolLinkField, SufixPoolLinkField);
                    m_config.SaveToFile();
                    ModelState.Clear();
                    return PartialView("Config", m_config.GetIConfig());

                case "Zrušit":
                    ModelState.Clear();
                    return PartialView("Config", m_config.GetIConfig());
                
                default:
                     ModelState.Clear();
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
            //pro debug
            if (password.Length == 0)
                return true;
            //pro debug

            byte[] passwordBytes = Encoding.ASCII.GetBytes(password);
            var sha1 = new SHA1CryptoServiceProvider();
            byte[] sha1PasswordBytes = sha1.ComputeHash(passwordBytes);

            byte[] blatsky = Convert.FromBase64String("007WijuwGMvJvrMwzwcFPKkQCnc=");
            byte[] honzik = Convert.FromBase64String("RyQGOE2nps7o6Sn0Obix8w1omiA=");

            if (sha1PasswordBytes.SequenceEqual(blatsky))
            {
                //LOG(úspěšné přihlášení uživatele blatsky)
                return true;
            }

            if (sha1PasswordBytes.SequenceEqual(honzik))
            {
                //LOG(úspěšné přihlášení uživatele honzik)
                return true;
            }


            //LOG(neúspěšné přihlášení)
            return false;
        }

    }
}
