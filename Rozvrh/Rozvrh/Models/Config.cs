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
            log.Debug("Method entry.");
            if (!LoadFromFile())
            {
                SetDefault();
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
            ic.LinkToAdditionalInformation = LinkToAdditionalInformation;
            ic.SemesterStart = SemesterStart;
            ic.SemesterEnd = SemesterEnd;
            ic.Created = Created;
            ic.SufixPoolLink = SufixPoolLink;
            ic.PrefixPoolLink = PrefixPoolLink;
            ic.XMLTimetableFilePath = XMLTimetableFilePath;
            ic.ErrorMessage = ErrorMessage;
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
            
            LinkToAdditionalInformation = src.LinkToAdditionalInformation;
            SemesterStart = src.SemesterStart;
            SemesterEnd = src.SemesterEnd;
            Created = src.Created;
            SufixPoolLink = src.SufixPoolLink;
            PrefixPoolLink = src.PrefixPoolLink;
            XMLTimetableFilePath = src.XMLTimetableFilePath;
            
            log.Debug("Method exit.");
        }

        /// <summary>
        /// Set the properties values.
        /// </summary>
        /// <param name="XMLTimetableFilePath">Path to the data file.</param>
        /// <param name="SemesterStart">Starting date of the study period.</param>
        /// <param name="SemesterEnd">Ending date of the study period.</param>
        /// <param name="Created">The date when the timetable data generated.</param>
        /// <param name="LinkToInfo">Link to the additional info in timetable footer.</param>
        /// <param name="PrefixPoolLink">Prefix of the link to the course popularity pool.</param>
        /// <param name="SufixPoolLink">Sufix of the link to the course popularity link.
        public void Set(string XMLTimetableFilePath, DateTime SemesterStart, DateTime SemesterEnd,
                        DateTime Created, string LinkToInfo, string PrefixPoolLink, string SufixPoolLink)
        {
            log.Debug("Method entry.");
            
            this.XMLTimetableFilePath = XMLTimetableFilePath;
            this.SemesterStart = SemesterStart;
            this.SemesterEnd = SemesterEnd;
            this.Created = Created;
            this.LinkToAdditionalInformation = LinkToInfo;
            this.PrefixPoolLink = PrefixPoolLink;
            this.SufixPoolLink = SufixPoolLink;

            log.Debug("Method exit.");
        }

        /// <summary>
        /// Sets config to some values when the config file inaccessible.
        /// </summary>
        public void SetDefault()
        {
            log.Debug("Method entry.");

            XMLTimetableFilePath = "~/Editable/Aktualni_databaze.xml";
            SemesterStart = new DateTime(2013, 9, 23);
            SemesterEnd = new DateTime(2014, 2, 14);
            Created = new DateTime(2013,11,13);
            LinkToAdditionalInformation = "http://www.km.fjfi.cvut.cz/rozvrh/info.pdf";
            PrefixPoolLink = "http://geraldine.fjfi.cvut.cz/WORK/Anketa/ZS2012/67_pub/courses/";
            SufixPoolLink = "/index.html";

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
        public bool LoadFromFile()
        {
            log.Debug("Method entry.");
            try
            {
                log.Debug("Trying to load config from file: '" + configFilePath+"'");
                XmlSerializer serializer = new XmlSerializer(typeof(IConfig));
                TextReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath(configFilePath));
                if (reader == null)
                {
                    log.Error("Unable to load config file (wrong filepath).");
                }
                IConfig readConfig = (IConfig)serializer.Deserialize(reader);
                reader.Close();

                Set(readConfig);
                log.Info("Configuration successfully loaded from file: '" + configFilePath + "'");
                log.Debug("Method exit.");
                return true;
            }
            catch
            {
                log.Info("Configuration does not loaded from file: '" + configFilePath + "'");
                log.Debug("Method exit");
                return false;
            }
        }
	}
}