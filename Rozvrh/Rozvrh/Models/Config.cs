using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Rozvrh.Models
{
    [Serializable]
	public class Config
	{
        // static holder for instance, need to use lambda to construct since constructor private
		private static readonly Lazy<Config> _instance
			= new Lazy<Config>(() => new Config());
	 
		// private to prevent direct instantiation.
		private Config()
		{
            XMLTimetableFilePath = "C:\\Aktualni_databaze.xml";
	    }
	
	   // accessor for instance
	   public static Config Instance
	   {
		   get
		   {
			   return _instance.Value;
		   }
	   }

        [XmlElement("XMLTimeTablePath")]
        public String XMLTimetableFilePath{
            get; private set;
        }

        public void UpdateXmlTimetablePath(String path){
            XMLTimetableFilePath = path;
            var serializer = new XmlSerializer(typeof(Config));
            using(var writer = new StreamWriter("C:\\config.xml"))
            {
                serializer.Serialize(writer, Config.Instance);
            }
        }

	}
}