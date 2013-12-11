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
        /// Property holding the path to the config file.
        /// </summary>
        private string configFilePath = "C:\\config.xml";

		/// <summary>
        /// Constructor. Private to prevent direct instantiation. Primary loading from config file, but set some default setting from code when the file is inaccessible.
		/// </summary>
        private Config()
		{
            if (!LoadFromFile())
            {
                SetDefault();
                //POPUP
            }
	    }

        /// <summary>
        /// Return data part of the config.
        /// </summary>
        /// <returns>IConfig class, only data.</returns>
        public IConfig GetIConfig()
        {
            IConfig ic = new IConfig();
            ic.LinkToAdditionalInformation = LinkToAdditionalInformation;
            ic.SemesterStart = SemesterStart;
            ic.SemesterEnd = SemesterEnd;
            ic.Created = Created;
            ic.SufixPoolLink = SufixPoolLink;
            ic.PrefixPoolLink = PrefixPoolLink;
            ic.XMLTimetableFilePath = XMLTimetableFilePath;

            return ic;
        }

        /// <summary>
        /// Set the properties values.
        /// </summary>
        /// <param name="src">Another Config or IConfig instance.</param>
        public void Set(IConfig src)
        {
            LinkToAdditionalInformation = src.LinkToAdditionalInformation;
            SemesterStart = src.SemesterStart;
            SemesterEnd = src.SemesterEnd;
            Created = src.Created;
            SufixPoolLink = src.SufixPoolLink;
            PrefixPoolLink = src.PrefixPoolLink;
            XMLTimetableFilePath = src.XMLTimetableFilePath;
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
            this.XMLTimetableFilePath = XMLTimetableFilePath;
            this.SemesterStart = SemesterStart;
            this.SemesterEnd = SemesterEnd;
            this.Created = Created;
            this.LinkToAdditionalInformation = LinkToInfo;
            this.PrefixPoolLink = PrefixPoolLink;
            this.SufixPoolLink = SufixPoolLink;
        }

        /// <summary>
        /// Sets config to some values when the config file inaccessible.
        /// </summary>
        public void SetDefault()
        {
            XMLTimetableFilePath = "C:\\Aktualni_databaze.xml";
            SemesterStart = new DateTime(2013, 9, 23);
            SemesterEnd = new DateTime(2014, 2, 14);
            Created = new DateTime(2013,11,13);
            LinkToAdditionalInformation = "http://www.km.fjfi.cvut.cz/rozvrh/info.pdf";
            PrefixPoolLink = "http://geraldine.fjfi.cvut.cz/WORK/Anketa/ZS2012/67_pub/courses/";
            SufixPoolLink = "/index.html";
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
        public void SaveToFile()
        {
            var serializer = new XmlSerializer(typeof(Rozvrh.Models.IConfig));
            using (var writer = new StreamWriter(configFilePath))
            {
                serializer.Serialize(writer, this.GetIConfig());
            }

        }

        /// <summary>
        /// Loads setting from config file.
        /// </summary>
        /// <returns></returns>
        public bool LoadFromFile()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(IConfig));
                TextReader reader = new StreamReader(configFilePath);
                if (reader == null)
                {
                    //LOG unable to open the config file 
                }
                IConfig readConfig = (IConfig)serializer.Deserialize(reader);
                reader.Close();

                Set(readConfig);
            }
            catch
            {
                return false;
            }
            return true;
            

        }
	}
}