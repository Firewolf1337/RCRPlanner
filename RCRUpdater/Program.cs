using System;
using System.Diagnostics;
using System.IO;



namespace RCRUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            string updatePath, updateFileName, processID, tempDir;
            updatePath = args[0].ToString();
            updateFileName = args[1].ToString();
            processID = args[2].ToString();

            tempDir = args[3].ToString();
            try
            {
                Process.GetProcessById(Convert.ToInt32(processID)).Kill();
            }
            catch { }


            foreach(var file in Directory.GetFiles(tempDir))
            {
                string filename = Path.GetFileName(file);
                File.Delete(updatePath + filename);
                File.Move(file, updatePath + filename);
            }
            try
            {
                Directory.Delete(tempDir);
            }
            catch { }
            Process.Start(updatePath + "\\" + updateFileName, "/noAutostart");
        }
    }
}
