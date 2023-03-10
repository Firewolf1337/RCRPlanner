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

            if (!updatePath.EndsWith(@"\"))
            {
                updatePath += @"\";
            }
            if (!tempDir.EndsWith(@"\"))
            {
                tempDir += @"\";
            }
            try
            {
                Process pro = Process.GetProcessById(Convert.ToInt32(processID));
                pro.Kill();
                pro.WaitForExit(5000);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Step killing Planner files error:");
                Console.WriteLine(ex.Message);
            }




            foreach (var file in Directory.GetFiles(tempDir))
            {
                string filename = Path.GetFileName(file);
                try
                {
                    File.Delete(updatePath + filename);
                    File.Move(file, updatePath + filename);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Step moving files error:");
                    Console.WriteLine(ex.Message);
                }
            }
            try
            {
                Directory.Delete(tempDir);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Step deleting temp folder error:");
                Console.WriteLine(ex.Message);
            }


            Process.Start(updatePath + "\\" + updateFileName, "/noAutostart");
            Console.WriteLine("Press any key to continue.");
            Console.ReadLine();
        }
    }
}
