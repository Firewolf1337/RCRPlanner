using System;
using System.Diagnostics;
using System.Threading;

namespace RCRPlanner
{
    public class ProcessWatcher
    {
        private string processName;
        private bool isRunning;
        private bool stopRequested;

        public ProcessWatcher(string processName)
        {
            this.processName = processName;
            isRunning = false;
            stopRequested = false;
        }

        public void StartWatching()
        {
            // Starte einen Hintergrundthread, um den Prozess kontinuierlich zu überwachen
            Thread monitoringThread = new Thread(MonitorProcess);
            monitoringThread.Start();
            stopRequested = false;
        }

        public void StopWatching()
        {
            stopRequested = true;
        }

        private void MonitorProcess()
        {
            while (!stopRequested)
            {
                Process[] processes = Process.GetProcessesByName(processName);

                if (processes.Length > 0 && !isRunning)
                {
                    // Die .exe wurde gestartet
                    isRunning = true;
                    OnProcessStarted();
                }
                else if (processes.Length == 0 && isRunning)
                {
                    // Die .exe wurde beendet
                    isRunning = false;
                    OnProcessStopped();
                }

                // Warte für eine bestimmte Zeit, bevor erneut überprüft wird
                Thread.Sleep(1000);
            }
        }

        // Aktion, die ausgeführt wird, wenn die .exe gestartet wird
        private void OnProcessStarted()
        {
            //Console.WriteLine("Die .exe wurde gestartet. Führe Aktion aus...");
            try
            {
                Thread.Sleep(Convert.ToInt32(Properties.Settings.Default.delayTime)*1000);
                if (MainWindow.autoStartApps.Programs.Count > 0)
                {
                    foreach (var prog in MainWindow.autoStartApps.Programs)
                    {
                        if (!prog.Paused && prog.withiRacing)
                        {
                            Process pr = new Process();
                            pr.StartInfo.FileName = prog.Path;
                            if (MainWindow.autoStartApps.Minimized == true)
                            {
                                pr.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                            }
                            pr.Start();
                            if (MainWindow.autoStartApps.Kill)
                            {
                                MainWindow.pIDs.Add(pr.Id);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        // Aktion, die ausgeführt wird, wenn die .exe beendet wird
        private void OnProcessStopped()
        {
            // Console.WriteLine("Die .exe wurde beendet. Führe andere Aktion aus...");
            // Hier kannst du den Code für die entsprechende Aktion einfügen
            if (MainWindow.autoStartApps.Kill)
            {
                foreach (var pname in MainWindow.autoStartApps.Programs)
                {
                    if (pname.withiRacing)
                    {
                        foreach (var pro in Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(pname.Path)))
                        {
                            try
                            {
                                Process.GetProcessById(pro.Id).Kill();
                            }
                            catch { }
                        }
                    }
                }
            }
        }
    }
}