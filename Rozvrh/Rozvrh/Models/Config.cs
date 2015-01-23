using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Rozvrh.Models
{ 
    /// <summary>
    /// Config class derived from IConfig (data part) adding method functionalities.
    /// </summary>
	public class Config : IConfig
	{
		/// <summary>
        /// Static holder for instance, need to use lambda to construct since constructor private.
		/// </summary>
        private static readonly Lazy<Config> _instance = new Lazy<Config>(() => new Config());
        
        /// <summary>
        /// Logger instance.
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Property holding the path to the config file.
        /// </summary>
        private string configFilePath = "~/Editable/config.xml";

		/// <summary>
        /// Constructor. Private to prevent direct instantiation. Primary loading from config file, but set some default setting from code when the file is inaccessible.
		/// </summary>
        private Config()
		{
            log.Info("----------------------Application is starting ---------------------");
            log.Debug("Method entry.");
            if (!LoadConfigFromFile())
            {
                Init();
                log.Info("Default hard-coded configuration loaded.");
                //POPUP
            }
            log.Debug("Method exit.");
	    }

        /// <summary>
        /// Return data part of the config.
        /// </summary>
        /// <returns>IConfig class, only data.</returns>
        public IConfig GetIConfig()
        {
            log.Debug("Method entry.");

            IConfig ic = new IConfig();
            ic.MostRecentTimetableXMLFilePath = MostRecentTimetableXMLFilePath;
            ic.WelcomeMessageFilePath = WelcomeMessageFilePath;
            ic.ArchivePath = ArchivePath;
            log.Debug("Method exit.");

            return ic;
        }

        /// <summary>
        /// Set the properties values.
        /// </summary>
        /// <param name="src">Another Config or IConfig instance.</param>
        public void Set(IConfig src)
        {
            log.Debug("Method entry.");
            
            MostRecentTimetableXMLFilePath = src.MostRecentTimetableXMLFilePath;
            WelcomeMessageFilePath = src.WelcomeMessageFilePath;
            ArchivePath = src.ArchivePath;
            
            log.Debug("Method exit.");
        }

        /// <summary>
        /// Set the properties values.
        /// </summary>
        /// <param name="MostRecentTimetableXMLFilePath">File path to the most recent XML with timetables.</param>
        /// <param name="ArchivePath">Path to the folder with timetable files (.xml and .info)</param>
        /// <param name="WelcomeMessageFilePath">Message to be shown on the empty filtering result list.</param>
        public void Set(string mMostRecentTimetableXMLFilePath, string mArchivePath, string mWelcomeMessage)
        {
            log.Debug("Method entry.");
            
            MostRecentTimetableXMLFilePath = mMostRecentTimetableXMLFilePath;
            WelcomeMessageFilePath = mWelcomeMessage;
            ArchivePath = mArchivePath;

            log.Debug("Method exit.");
        }

        /// <summary>
        /// Sets config to some values when the config file inaccessible.
        /// </summary>
        public void Init()
        {
            log.Debug("Method entry.");

            MostRecentTimetableXMLFilePath = "~/Editable/13-14-zimni.xml";
            WelcomeMessageFilePath = "Vítej v aplikaci MůjRozvrh FIFJ! Začni v sekci filtrování vpravo.";
            ArchivePath = "~/Editable/Archive/";

            log.Debug("Method exit.");
        }

        /// <summary>
        /// Accessir for the instance.
        /// </summary>
	    public static Config Instance
	   {
		   get
		   {
			   return _instance.Value;
		   }
	   }

        /// <summary>
        /// Saves setting to config file.
        /// </summary>
        /// <returns>Bool result.</returns>
        public bool SaveToFile()
        {
            log.Debug("Method entry.");

            try
            {
                log.Debug("Trying to save config to file: '" + configFilePath + "'");
                var serializer = new XmlSerializer(typeof(Rozvrh.Models.IConfig));
                using (var writer = new StreamWriter(System.Web.HttpContext.Current.Server.MapPath(configFilePath)))
                {
                    serializer.Serialize(writer, this.GetIConfig());
                }
                log.Info("Configuration successfully saved to file: '" + configFilePath + "'");
                log.Debug("Method exit.");
                return true;
            }
            catch
            {
                log.Error("Unable to save configuration to file: '" + configFilePath + "'");
                log.Debug("Method exit.");
                return false;
            }
        }

        /// <summary>
        /// Loads setting from config file.
        /// </summary>
        /// <returns></returns>
        /// <returns>Bool result.</returns>
        public bool LoadConfigFromFile()
        {
            log.Debug("Method entry.");
            try
            {
                string fileContent = Utils.Files.LoadFileContentFromPath(configFilePath);
                TextReader reader = new StringReader(fileContent);
                if (reader == null)
                    log.Error("Unable to load config file (wrong filepath).");

                XmlSerializer serializer = new XmlSerializer(typeof(IConfig));
                IConfig readConfig = (IConfig)serializer.Deserialize(reader);
                reader.Close();

                Set(readConfig);
                log.Info("Configuration successfully loaded from file: '" + configFilePath + "'");
                this.WelcomeMessage = LoadWelcomeMessageFromFile(this.WelcomeMessageFilePath);
                return true;
            }
            catch
            {
                log.Error("Configuration not loaded from file: '" + configFilePath + "'");
                return false;
            }
            finally
            {
                log.Debug("Method exit");
            }
        }

        private static string LoadWelcomeMessageFromFile(string welcomeMessageFilePath)
        {
            log.Debug("Method entry.");
            string wm = Utils.Files.LoadFileContentFromPath(welcomeMessageFilePath);
            if (string.IsNullOrEmpty(wm))
            {
                log.Error("Unable to load a welcome message from " + welcomeMessageFilePath);
            }
            else
            {
                log.Info("Welcome message successfully loaded from: " + welcomeMessageFilePath);
            }
            log.Debug("Method exit");
            return wm;
        }
	}
}