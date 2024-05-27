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
using System.Reflection;
using System.Windows.Data;
using System.Windows.Controls;

namespace RCRPlanner
{
    public class ValueToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string input;
            string color = "#00000000";
            bool sof = false;
            System.Data.DataRowView rowView;
            int rowid;
            int colid;
            try
            {
                System.Windows.Controls.DataGridCell dgc = (System.Windows.Controls.DataGridCell)value;
                System.Windows.Controls.DataGridRow r2 = System.Windows.Controls.DataGridRow.GetRowContainingElement(dgc);
                rowid = r2.GetIndex();
                rowView = (System.Data.DataRowView)dgc.DataContext;
                input = (string)rowView.Row.ItemArray[dgc.Column.DisplayIndex].ToString();
                sof = dgc.Column.DisplayIndex > statistics.sofcol ? true : false;
                colid = dgc.Column.DisplayIndex;
            }
            catch (Exception ex)
            {
                return DependencyProperty.UnsetValue;
            }
            try
            {
                if (colid > 0)
                {
                    if (!sof)
                    {

                        int minStarter = MainWindow.main.partStatMinStarters;
                        int min = statistics.partMin;
                        int max = statistics.partMax;
                        int tenpC = (max - min) / 10;
                        int cellval = 0;
                        if (!String.IsNullOrWhiteSpace(input))
                        {
                            cellval = System.Convert.ToInt32(input);
                        }
                        if (rowid > 0)
                        {
                            if (cellval < minStarter) { color = "#770000"; }
                            if (cellval >= minStarter) { color = "#002800"; }
                            if (cellval >= tenpC && tenpC > minStarter) { color = "#00330e"; }
                            if (cellval >= tenpC * 2 && tenpC * 2 > minStarter) { color = "#00431d"; }
                            if (cellval >= tenpC * 3 && tenpC * 3 > minStarter) { color = "#00552c"; }
                            if (cellval >= tenpC * 4 && tenpC * 4 > minStarter) { color = "#00673c"; }
                            if (cellval >= tenpC * 5 && tenpC * 5 > minStarter) { color = "#007a4d"; }
                            if (cellval >= tenpC * 6 && tenpC * 6 > minStarter) { color = "#008d5e"; }
                            if (cellval >= tenpC * 7 && tenpC * 7 > minStarter) { color = "#00a06f"; }
                            if (cellval >= tenpC * 8 && tenpC * 8 > minStarter) { color = "#00b481"; }
                            if (cellval >= tenpC * 9 && tenpC * 9 > minStarter) { color = "#00c994"; }
                        }
                        else
                        {
                            color = "#001700";
                        }
                    }
                    else
                    {
                        int min = statistics.sofMin;
                        int max = statistics.sofMax;
                        int tenpC = (max - min) / 10;
                        int cellval = 0;
                        if (!String.IsNullOrWhiteSpace(input))
                        {
                            cellval = System.Convert.ToInt32(input);
                        }
                        color = "#494C13";
                        if (cellval >= min) { color = "#5B5E18"; }
                        if (cellval >= tenpC + min) { color = "#6C6F1C"; }
                        if (cellval >= tenpC * 2 + min) { color = "#7D8120"; }
                        if (cellval >= tenpC * 3 + min) { color = "#8F9325"; }
                        if (cellval >= tenpC * 4 + min) { color = "#A0A529"; }
                        if (cellval >= tenpC * 5 + min) { color = "#B1B72E"; }
                        if (cellval >= tenpC * 6 + min) { color = "#C3C932"; }
                        if (cellval >= tenpC * 7 + min) { color = "#D4DB37"; }
                        if (cellval >= tenpC * 8 + min) { color = "#E5ED3B"; }
                        if (cellval >= tenpC * 9 + min) { color = "#f7ff40"; }
                    }

                }
            }
            catch { return color; }
            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    public class ValueToBrushTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string color = Application.Current.Resources["BrushTextWhite"].ToString();
            System.Drawing.Color light;
            bool sof = false;
            try
            {
                System.Windows.Controls.DataGridCell dgc = (System.Windows.Controls.DataGridCell)value;
                var backcolor = ((System.Windows.Media.SolidColorBrush)dgc.Background).Color;
                light = System.Drawing.Color.FromArgb(backcolor.A,backcolor.R,backcolor.G,backcolor.B);
                sof = dgc.Column.DisplayIndex > statistics.sofcol ? true : false;
            }

            catch
            {
                return DependencyProperty.UnsetValue;
            }
            try
            {
                if (sof)
                {
                    color = Application.Current.Resources["BrushTextBlack"].ToString();
                }

            }
            catch { return color; }
            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    public class actualWeek
    {
        public int seriesId { get; set; }
        public int week { get; set; }
    }
    public class comboBox
    {
        public string Name { get; set; }
        public string Value { get; set; }
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
    public class helper
    {
        //static Assembly _assembly = Assembly.GetExecutingAssembly();
        //static string resourceName = "RCRPlanner.salt.txt";
        //static Stream stream1 = _assembly.GetManifestResourceStream(resourceName);
        //static StreamReader reader = new StreamReader(stream1);
        //static string ent = reader.ReadToEnd();

        static readonly byte[] entropy = null;
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
            catch
            {
            }
        }


        public static T DeSerializeObject<T>(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) { return default; }

            T objectOut = default;

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
            catch
            {

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
        public static int getNextFreeCustomSeriesID(int startid, List<series.Root> existingSeries)
        {
            List<int> ids = existingSeries.Select(i => i.series_id).ToList();
            int firstAvailable = Enumerable.Range(startid, 20000).Except(ids).First();
            return firstAvailable;
        }
        public static List<series.AllowedLicense> getAllowedLicenses(string SeriesClass, int SR)
        {
            List<series.AllowedLicense> allowedLicense = new List<series.AllowedLicense>();
            var ClassIDs = new (int, string, string)[]
              {
                  (1, "R", "Rookie"),
                  (2, "D", "Class D"),
                  (3, "C", "Class C"),
                  (4, "B", "Class B"),
                  (5, "A", "Class A"),
                  (6, "Pro", "Pro"),
                  (7, "Pro/WC", "Pro/WC")
              };
            int startClass = ClassIDs.First(x => x.Item2 == SeriesClass).Item1;
            for(int i = startClass; i<= 7; i++)
            {
                int MinSR = i == startClass ? SR + (i - 1) * 4: (i - 1)*4 + 1 ;
                allowedLicense.Add(new series.AllowedLicense() { group_name = ClassIDs[i-1].Item3, license_group = i, min_license_level = MinSR , max_license_level = (i *4) });  
            }
            return allowedLicense;
        }

        public static Size MeasureString(StackPanel stackPanel)
        {
            var size = new Size(0, 0);
            foreach (var child in stackPanel.Children)
            {
                if (child is TextBlock)
                {
                    size.Width += ((TextBlock)child).ActualWidth;
                    size.Height += ((TextBlock)child).ActualHeight;
                }
                if (child is Image)
                {
                    size.Width += ((Image)child).ActualWidth;
                    size.Height += ((Image)child).ActualHeight;
                }
            }


            return size;
        }
    }
}
