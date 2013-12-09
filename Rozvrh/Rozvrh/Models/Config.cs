using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Rozvrh.Models
{
    
	public class Config : IConfig
	{
        // static holder for instance, need to use lambda to construct since constructor private
		private static readonly Lazy<Config> _instance = new Lazy<Config>(() => new Config());
        private string configFilePath = "C:\\config.xml";

        // private to prevent direct instantiation.
		private Config()
		{
            if (!LoadFromFile())
            {
                SetDefault();
                //POPUP
            }
	    }

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

        public void Set(string XMLTimetableFilePathField, DateTime SemesterStart, DateTime SemesterEnd,
                        DateTime Created, string LinkToInfo, string PrefixPoolLinkField, string SufixPoolLinkField)
        {
            this.XMLTimetableFilePath = XMLTimetableFilePathField;
            this.SemesterStart = SemesterStart;
            this.SemesterEnd = SemesterEnd;
            this.Created = Created;
            this.LinkToAdditionalInformation = LinkToInfo;
            this.PrefixPoolLink = PrefixPoolLinkField;
            this.SufixPoolLink = SufixPoolLinkField;
        }

        public void SetDefault()
        {
            XMLTimetableFilePath = "C:\\Aktualni_databaze.xml";
            SemesterStart = new DateTime(2013, 9, 23);
            SemesterEnd = new DateTime(2014, 2, 14);
            Created = new DateTime(2012,10,16);
            LinkToAdditionalInformation = "http://www.km.fjfi.cvut.cz/rozvrh/info.pdf";
            PrefixPoolLink = "http://geraldine.fjfi.cvut.cz/WORK/Anketa/LS2013/67_pub/courses/";
            SufixPoolLink = "/index.html";
        }

	    // accessor for instance
	    public static Config Instance
	   {
		   get
		   {
			   return _instance.Value;
		   }
	   }

        public void SaveToFile()
        {
            var serializer = new XmlSerializer(typeof(Rozvrh.Models.IConfig));
            using (var writer = new StreamWriter(configFilePath))
            {
                serializer.Serialize(writer, this.GetIConfig());
            }

        }

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