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

            //FIX ME(VASEK): Load from config file instead of fixed values
            SemesterStart = new DateTime(2013, 9, 23);
            SemesterEnd = new DateTime(2014, 2, 14);
            Created = new DateTime(2012,10,16);
            LinkToAdditionalInformation = "http://www.km.fjfi.cvut.cz/rozvrh/info.pd";

	    }
	
	   // accessor for instance
	   public static Config Instance
	   {
		   get
		   {
			   return _instance.Value;
		   }
	   }

       /// <summary> 
       /// Hyperlink to webpage with additional information about timetable
       /// </summary>
       /// <remarks>Used in SVG export.</remarks>
       [XmlElement("LinkToInfo")]
       public string LinkToAdditionalInformation
       {
           get; set;
       }

       /// <summary> 
       /// Date the timetable was created
       /// </summary>
       /// <remarks>Used in SVG export.</remarks>
       [XmlElement("Created")]
       public DateTime Created
       {
           get; set;
       }

       /// <summary> 
       /// When does the semester starts.
       /// </summary>
       [XmlElement("SemesterStart")]
       public DateTime SemesterStart
       {
           get; set;
       }

       /// <summary> 
       /// When does the semester start.
       /// </summary>
       [XmlElement("SemesterEnd")]
       public DateTime SemesterEnd
       {
           get; set;
       }

       /// <summary> 
       /// File path to XML with timetables. 
       /// </summary>
        [XmlElement("XMLTimeTablePath")]
        public String XMLTimetableFilePath{
            get; set;
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