using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Serialization;


namespace RCRUpdater
{
    public class fileInfo
    {
        public string FileName { get; set; }
        public string FileHash { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            string updatePath, updateFileName, processID, updateURL;
            updatePath = args[0].ToString();
            //updatePath = @"C:\Users\oxtrave\Work Folders\Documents\Visual Studio Projects\iRacingPlanner\iRacingPlanner\bin\Debug";
            updateFileName = args[1].ToString();
            //updateFileName = "RCRPlanner.exe";
            processID = args[2].ToString();

            updateURL = args[3].ToString();
            //updateURL = @"https://ravencreecracing.de/rcrplanner/";
            Process.GetProcessById(Convert.ToInt32(processID)).Kill();


            List<fileInfo> localFiles = DeSerializeObject<List<fileInfo>>(updatePath + "\\hashes.xml");
            List<fileInfo> onlineHashes = new List<fileInfo>();
            using (WebClient wc = new WebClient())
            {
                wc.DownloadFile(updateURL + "hashes.xml", "onlineHashes.xml");
                onlineHashes = DeSerializeObject<List<fileInfo>>(updatePath + "\\onlineHashes.xml");
                wc.Dispose();
            }
            foreach(var file in onlineHashes)
            {
                bool download = false;
                if(localFiles.FirstOrDefault(l => l.FileName == file.FileName) != null)
                {
                    if((localFiles.FirstOrDefault(l => l.FileName == file.FileName)).FileHash != file.FileHash)
                    {
                        download = true;
                    }
                }
                else
                {
                    download = true;
                }

                if (download)
                {
                    using (WebClient wc = new WebClient())
                    {
                        if (file.FileName == "RCRUpdater.exe")
                        {
                            Console.WriteLine("Downloading " + file.FileName);
                            wc.DownloadFile(updateURL + file.FileName, "new_RCRUpdater.exe");
                            wc.Dispose();
                        }
                        else
                        {
                            wc.DownloadFile(updateURL + file.FileName, file.FileName);
                            wc.Dispose();
                        }
                    }
                }
            }
            File.Delete(updatePath + "\\onlineHashes.xml");
            Process.Start(updatePath + "\\" + updateFileName, "/noAutostart");
        }
        public static T DeSerializeObject<T>(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) { return default(T); }

            T objectOut = default(T);

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(fileName);
                string xmlString = xmlDocument.OuterXml;

                using (StringReader read = new StringReader(xmlString))
                {
                    Type outType = typeof(T);

                    XmlSerializer serializer = new XmlSerializer(outType);
                    using (XmlReader reader = new XmlTextReader(read))
                    {
                        objectOut = (T)serializer.Deserialize(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                //Log exception here
            }

            return objectOut;
        }
    }
}
