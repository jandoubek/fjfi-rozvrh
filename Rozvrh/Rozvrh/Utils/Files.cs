using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Rozvrh.Utils
{
    public class Files
    {
        #region Log
        /// <summary>
        /// Logger instance.
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        public static string LoadFileContentFromPath(string filepath)
        {
            log.Debug("Method entry.");
            string result = "";
            try
            {
                log.Debug("Trying to load a content from the file: '" + filepath + "'");

                //if the path starts with ~ - use local server loading
                if (String.CompareOrdinal("~", 0, filepath, 0, 1) == 0)
                {
                    log.Debug("Loading XML file from the server map path: '" + filepath + "'.");
                    filepath = System.Web.HttpContext.Current.Server.MapPath(filepath);
                }
                else
                {
                    log.Debug("Loading XML file from the absolute path (like http): '" + filepath + "'.");
                }

                TextReader reader = new StreamReader(filepath);
                if (reader == null)
                {
                    log.Error("Unable to load a content of the file (wrong filepath).");
                }
                result = reader.ReadToEnd();
                reader.Close();
                log.Debug("Content successfully loaded from the file: '" + filepath + "'");
                log.Debug("Method exit.");
                return result;
            }
            catch
            {
                log.Error("Content does not loaded from the file: '" + filepath + "'");
                log.Error("Method exit");
                return result;
            }
        }
    }
}