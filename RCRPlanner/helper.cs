using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Serialization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Controls;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Data;
using Microsoft.Win32;


namespace RCRPlanner
{
    public class ValueToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string input;
            string color = "#00000000";
            try
            {
                System.Windows.Controls.DataGridCell dgc = (System.Windows.Controls.DataGridCell)value;
                System.Data.DataRowView rowView = (System.Data.DataRowView)dgc.DataContext;
                input = (string)rowView.Row.ItemArray[dgc.Column.DisplayIndex].ToString();
            }
            catch (InvalidCastException e)
            {
                return DependencyProperty.UnsetValue;
            }
            try
            {

                int min = statistics.partMin;
                int max = statistics.partMax;
                int tenpC = (max - min) / 10;
                int cellval = 0;
                if (!String.IsNullOrWhiteSpace(input))
                {
                    cellval = System.Convert.ToInt32(input);
                }
                if (cellval < 8) { color = "#770000"; }
                if (cellval >= 8) { color = "#002800"; }
                if (cellval >= tenpC && tenpC > 8) { color = "#00330e"; }
                if (cellval >= tenpC * 2 && tenpC * 2 > 8) { color = "#00431d"; }
                if (cellval >= tenpC * 3 && tenpC * 3 > 8) { color = "#00552c"; }
                if (cellval >= tenpC * 4 && tenpC * 4 > 8) { color = "#00673c"; }
                if (cellval >= tenpC * 5 && tenpC * 5 > 8) { color = "#007a4d"; }
                if (cellval >= tenpC * 6 && tenpC * 6 > 8) { color = "#008d5e"; }
                if (cellval >= tenpC * 7 && tenpC * 7 > 8) { color = "#00a06f"; }
                if (cellval >= tenpC * 8 && tenpC * 8 > 8) { color = "#00b481"; }
                if (cellval >= tenpC * 9 && tenpC * 9 > 8) { color = "#00c994"; }

            }
            catch { return color; }
            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    public class fileInfo
    {
        public string FileName { get; set; }
        public string FileHash { get; set; }
    }
    public class Alarms
    {
        public int SerieId { get; set; }
        public DateTime AlarmTime { get; set; }
    }
    class helper
    {
        static Assembly _assembly = Assembly.GetExecutingAssembly();
        static string resourceName = "RCRPlanner.salt.txt";
        static Stream stream1 = _assembly.GetManifestResourceStream(resourceName);
        static StreamReader reader = new StreamReader(stream1);
        static string ent = reader.ReadToEnd();

        static byte[] entropy = Encoding.Unicode.GetBytes(ent);
        public static byte[] CombineByte(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }
        // Password handling 
        public static string EncryptString(SecureString input)
        {
            byte[] encryptedData = ProtectedData.Protect(Encoding.Unicode.GetBytes(ToInsecureString(input)), entropy, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptedData);
        }
        public static void createHashes()
        {
            List<string> solutionFiles = new List<string>{
                "Microsoft.Bcl.AsyncInterfaces.dll",
                "Microsoft.Bcl.AsyncInterfaces.xml",
                "RCRPlanner.exe",
                "RCRPlanner.exe.config",
                "RCRPlanner.pdb",
                "System.Buffers.dll",
                "System.Buffers.xml",
                "System.Memory.dll",
                "System.Memory.xml",
                "System.Net.Http.Json.dll",
                "System.Net.Http.Json.xml",
                "System.Numerics.Vectors.dll",
                "System.Numerics.Vectors.xml",
                "System.Runtime.CompilerServices.Unsafe.dll",
                "System.Runtime.CompilerServices.Unsafe.xml",
                "System.Text.Encodings.Web.dll",
                "System.Text.Encodings.Web.xml",
                "System.Text.Json.dll",
                "System.Text.Json.xml",
                "System.Threading.Tasks.Extensions.dll",
                "System.Threading.Tasks.Extensions.xml",
                "System.ValueTuple.dll",
                "System.ValueTuple.xml",
                "RCRUpdater.exe",
                "XamlRadialProgressBar.DotNet.dll",
            };
            List<fileInfo> fileInfos = new List<fileInfo>();
            foreach(var file in solutionFiles)
            {
                try
                {
                    fileInfo fileinfo = new fileInfo { FileHash = CalculateMD5(file), FileName = file };
                    fileInfos.Add(fileinfo);
                }
                catch { }
            }
            SerializeObject<List<fileInfo>>(fileInfos , System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + "\\hashes.xml");
            if(File.Exists(System.Windows.Forms.Application.StartupPath + "\\new_RCRUpdater.exe"))
            {
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\RCRUpdater.exe");
                File.Move(System.Windows.Forms.Application.StartupPath + "\\new_RCRUpdater.exe", System.Windows.Forms.Application.StartupPath + "\\RCRUpdater.exe");
            }
        }
        static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
        public static SecureString DecryptString(string encryptedData)
        {
            try
            {
                byte[] decryptedData = ProtectedData.Unprotect(Convert.FromBase64String(encryptedData), entropy, DataProtectionScope.CurrentUser);
                return ToSecureString(Encoding.Unicode.GetString(decryptedData));
            }
            catch
            {
                return new SecureString();
            }
        }

        public static SecureString ToSecureString(string input)
        {
            SecureString secure = new SecureString();
            foreach (char c in input)
            {
                secure.AppendChar(c);
            }
            secure.MakeReadOnly();
            return secure;
        }

        public static DateTime getNextRace(DateTime starttime, int repeationminutes, DateTime time)
        {
            DateTime date = starttime;
            if (repeationminutes > 0)
            {
                while (date < time)
                {
                    date = date.AddMinutes(repeationminutes);
                }
            }
            return date;
        }
        public static string ToInsecureString(SecureString input)
        {
            string returnValue = string.Empty;
            IntPtr ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(input);
            try
            {
                returnValue = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);
            }
            return returnValue;
        }
        public static decimal Mapdec(decimal value, decimal fromSource, decimal toSource, decimal fromTarget, decimal toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }
        public static void update()
        {
            string path = System.Windows.Forms.Application.StartupPath;
            string fileName = Path.GetFileName(System.Windows.Forms.Application.ExecutablePath);
            string pid = Process.GetCurrentProcess().Id.ToString();
            string URL = Properties.Settings.Default.updateURL;
            Process.Start("RCRUpdater.exe", "\"" + path + "\" \"" + fileName  + "\" \"" + pid + "\" \"" + URL + "\"");
        } 
        public static System.Windows.Media.Color getNegative(string color)
        {
            System.Windows.Media.Color negative = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(color);
            negative = System.Windows.Media.Color.FromArgb(Convert.ToByte(negative.A), Convert.ToByte(255 - negative.R), Convert.ToByte(255 - negative.G), Convert.ToByte(255 - negative.B));
            return negative;

        }
        public static void SerializeObject<T>(T serializableObject, string fileName)
        {
            if (serializableObject == null) { return; }

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, serializableObject);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                    xmlDocument.Save(fileName);
                }
            }
            catch (Exception ex)
            {
                //Log exception here
            }
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
        internal static class SystemHelper
        {
            public static int GetCurrentDPI()
            {
                return (int)typeof(SystemParameters).GetProperty("Dpi", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, null);
            }

            public static double GetCurrentDPIScaleFactor()
            {
                return (double)SystemHelper.GetCurrentDPI() / 96;
            }

            public static System.Drawing.Point GetMousePositionWindowsForms()
            {
                System.Drawing.Point point = System.Windows.Forms.Control.MousePosition;
                return new System.Drawing.Point(point.X, point.Y);
            }
        }
        public static Thickness GetDefaultMarginForDpi()
        {
            int currentDPI = SystemHelper.GetCurrentDPI();
            Thickness thickness = new Thickness(7, 7, 7, 7);
            if (currentDPI == 120)
            {
                thickness = new Thickness(7, 7, 4, 5);
            }
            else if (currentDPI == 144)
            {
                thickness = new Thickness(7, 7, 3, 1);
            }
            else if (currentDPI == 168)
            {
                thickness = new Thickness(6, 6, 2, 0);
            }
            else if (currentDPI == 192)
            {
                thickness = new Thickness(6, 6, 0, 0);
            }
            else if (currentDPI == 240)
            {
                thickness = new Thickness(6, 6, 0, 0);
            }
            return thickness;
        }
        public static Thickness GetFromMinimizedMarginForDpi()
        {
            int currentDPI = SystemHelper.GetCurrentDPI();
            Thickness thickness = new Thickness(7, 7, 5, 7);
            if (currentDPI == 120)
            {
                thickness = new Thickness(6, 6, 4, 6);
            }
            else if (currentDPI == 144)
            {
                thickness = new Thickness(7, 7, 4, 4);
            }
            else if (currentDPI == 168)
            {
                thickness = new Thickness(6, 6, 2, 2);
            }
            else if (currentDPI == 192)
            {
                thickness = new Thickness(6, 6, 2, 2);
            }
            else if (currentDPI == 240)
            {
                thickness = new Thickness(6, 6, 0, 0);
            }
            return thickness;
        }

    }
}
