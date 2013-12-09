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
    public class AdminController : Controller
    {

        Config m_config;

        public AdminController()
        {
            m_config = Config.Instance;
        }

        public ActionResult Index()
        {
            return View("Index");
        }

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
