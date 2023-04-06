using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace RCRPlanner
{
    class filehandler
    {
        //MainWindow mw = new MainWindow();
        readonly string iracingSeriesImages = "https://images-static.iracing.com/img/logos/series/";
        readonly string[] iracingCarImages = { "https://ir-core-sites.iracing.com/members/member_images/cars/carid_", "/profile.jpg", "https://images-static.iracing.com" };
        readonly RCRPlanner.FetchData fData = new RCRPlanner.FetchData();
        readonly string exePath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        public async Task<List<seriesAssets>> getSeriesAssets(string filestring, string logofolder, bool reload)
        {
            List<seriesAssets> seriesAssetsList = new List<seriesAssets>();
            //Dictionary<string, string> datafiles = new Dictionary<string, string>();

            //Load base series information


            //Load asset series information and fetching logos
            string file = exePath + filestring;
            int counter = 0;
            if (File.Exists(file) && !reload)
            {
                seriesAssetsList = helper.DeSerializeObject<List<seriesAssets>>(file);
            }
            else
            {
                try
                {
                    seriesAssetsList = await fData.getSeriesAssets();
                    helper.SerializeObject<List<seriesAssets>>(seriesAssetsList, file);
                }
                catch { }
            }
            foreach (var logo in seriesAssetsList)
            {
                counter++;
                MainWindow.main.Status = "Loading serie logo " + counter + " / " + seriesAssetsList.Count();
                file = exePath + logofolder + logo.logo;
                if (!File.Exists(file))
                {
                    try
                    {
                        await fData.getImage(iracingSeriesImages + logo.logo, file, true, 75);
                    }
                    catch { }
                }
            }
            return seriesAssetsList;
        }
        public async Task<List<seriesSeason.Root>> getSeriesSeason(string filestring, bool reload)
        {
            List<seriesSeason.Root> seriesSeasonList = new List<seriesSeason.Root>();
            string file = exePath + filestring;
            if (File.Exists(file) && !reload)
            {
                seriesSeasonList = helper.DeSerializeObject<List<seriesSeason.Root>>(file);
            }
            else
            {
                try
                {
                    seriesSeasonList = await fData.getSeriesSeason();
                    helper.SerializeObject<List<seriesSeason.Root>>(seriesSeasonList, file);
                }
                catch { }
            }
            return seriesSeasonList;
        }
        public async Task<List<series.Root>> getSeriesList(string filestring, bool reload)
        {
            List<series.Root> seriesList = new List<series.Root>();
            //Load base series information
            string file = exePath + filestring;

            if (File.Exists(file) && !reload)
            {
                seriesList = helper.DeSerializeObject<List<series.Root>>(file);
            }
            else
            {
                try
                {
                    seriesList = await fData.getSeries();
                    helper.SerializeObject<List<series.Root>>(seriesList, file);
                }
                catch { }
            }
            return seriesList;
        }
        public List<cars.CarsInSeries> getCarsInSeries(List<carClass.CarInClassId> cars, List<seriesSeason.Root> series )
        {
            List<cars.CarsInSeries> carsInSeries = new List<cars.CarsInSeries>();
            foreach(var car in cars)
            {
                foreach (var carclass in car.car_class_id)
                {
                    foreach (var serie in series)
                    {
                        foreach (var carclassid in serie.car_class_ids)
                        {
                            if(carclassid == carclass)
                            {
                                carsInSeries.Add(new cars.CarsInSeries { car_id = car.car_id, series_id = serie.series_id });
                            }
                        }
                    }
                }
            }
            return carsInSeries;

        }

        public async Task<List<cars.Root>> getCarList(string filestring, bool reload) 
        {

            List<cars.Root> carsList = new List<cars.Root>();

            //Load base series information
            string file = exePath + filestring;
            if (File.Exists(file) && !reload)
            {
                carsList = helper.DeSerializeObject<List<cars.Root>>(file);
            }
            else
            {
                try
                {
                    carsList = await fData.getCars();
                    helper.SerializeObject<List<cars.Root>>(carsList, file);
                }
                catch{ }
            }
            return carsList;
        }
        public async Task<List<carAssets>> getCarAssetsList(string filestring, string imagefolder, bool reload) {
            List<carAssets> carsAssetsList = new List<carAssets>();
            List<oddCarImages> oddCarImgs = new List<oddCarImages>();
            oddCarImgs = helper.DeSerializeObject<List<oddCarImages>>(@"oddCarImages.xml");
            //Load asset series information and fetching logos
            string file = exePath + filestring;
            int counter = 0;
            if (File.Exists(file) && !reload)
            {
                carsAssetsList = helper.DeSerializeObject<List<carAssets>>(file);
            }
            else
            {
                try
                {
                    carsAssetsList = await fData.getCarsAssets();
                    helper.SerializeObject<List<carAssets>>(carsAssetsList, file);
                }
                catch{ }
            }
            foreach (var logo in carsAssetsList)
            {
                counter++;
                MainWindow.main.Status = "Loading car pictures " + counter + " / " + carsAssetsList.Count();
                file = exePath + imagefolder + logo.car_id + ".png";
                if (!File.Exists(file))
                {
                    try
                    {
                        if (oddCarImgs.FirstOrDefault(c => c.carid == logo.car_id) != null)
                        {
                            await fData.getImage(oddCarImgs.FirstOrDefault(c => c.carid == logo.car_id).imageUrl, file, false);
                        }
                        else
                        {
                            await fData.getImage(iracingCarImages[0] + logo.car_id + iracingCarImages[1], file, false);
                        }
                    }
                    catch { }
                }
                file = exePath + imagefolder + logo.car_id + "_logo.png";
                if (!File.Exists(file))
                {
                    try
                    {
                        await fData.getImage(iracingCarImages[2] + logo.folder + "/" + logo.small_image, file, true, 50);
                    }
                    catch (Exception ex) { }
                }
            }
            return carsAssetsList;
        }
        public async Task<List<carClass.Root>> getCarClassList(string filestring, bool reload)
        {
            List<carClass.Root> carClassList = new List<carClass.Root>();
            string file = exePath + filestring;
            if (File.Exists(file) && !reload)
            {
                carClassList = helper.DeSerializeObject<List<carClass.Root>>(file);
            }
            else
            {
                try
                {
                    carClassList = await fData.getCarClass();
                    helper.SerializeObject<List<carClass.Root>>(carClassList, file);
                }
                catch { }
            }
            return carClassList;
        }
        public List<carClass.CarInClassId> getCarClassesList(List<cars.Root> carsList, List<carClass.Root> carClassList)
        {
            List<carClass.CarInClassId> carClassesList = new List<carClass.CarInClassId>();
            foreach (var car in carsList)
            {
                List<int> ClassIDs = new List<int>();
                foreach (var clss in carClassList)
                {
                    foreach (var cars in clss.cars_in_class)
                    {
                        if (cars.car_id == car.car_id)
                        {
                            ClassIDs.Add(clss.car_class_id);
                        }
                    }
                }
                carClassesList.Add(new carClass.CarInClassId
                {
                    car_id = car.car_id,
                    car_class_id = ClassIDs,
                });
            }
            return carClassesList;
        }


        public async Task<List<tracks.Root>> getTracksList(string filestring, bool reload)
        {

            List<tracks.Root> tracksList = new List<tracks.Root>();
            string file = exePath + filestring;
            if (File.Exists(file) && !reload)
            {
                tracksList = helper.DeSerializeObject<List<tracks.Root>>(file);
            }
            else
            {
                try
                {
                    tracksList = await fData.getTracks();
                    helper.SerializeObject<List<tracks.Root>>(tracksList, file);
                }
                catch { }
            }
            return tracksList;
        }
        public async Task<List<trackAssets.Root>> getTracksAssets(string filestring, bool reload)
        {
            List<trackAssets.Root> trackAssetsList = new List<trackAssets.Root>();
            string file = exePath + filestring;
            if (File.Exists(file) && !reload)
            {
                trackAssetsList = helper.DeSerializeObject<List<trackAssets.Root>>(file);
            }
            else
            {
                try
                {
                    trackAssetsList = await fData.getTracksAssets();
                    helper.SerializeObject<List<trackAssets.Root>>(trackAssetsList, file);
                }
                catch { }
            }

            return trackAssetsList;
        }
        public void getTrackSVG(List<trackAssets.Root> tracks, string targetfolder)
        {
            int counter = 0;
            if(!File.Exists(targetfolder + "track.css"))
            {
                File.Copy(exePath + "\\track.css", targetfolder + "track.css");
            }
            foreach (var track in tracks)
            {
                counter++;
                string trackfile = track.track_id + ".html";
                string trackpic = track.track_id + ".png";
                MainWindow.main.Status = "Loading track pictures " + counter + " / " + tracks.Count();
                if (!File.Exists(targetfolder+trackfile))
                {
                    string htmlcontent = "<!DOCTYPE html><html><head><meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\" /><meta http-equiv=\"Content-Type\" content = \"text/html; charset=utf-8\" /><link type=\"text/css\" rel=\"stylesheet\" href=\"track.css\" /></head><body><div id=\"svgMap\">";
                    string svgpath = track.track_map;
                    FieldInfo[] fields = typeof(trackAssets.TrackMapLayers).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (var item in track.track_map_layers.GetType().GetProperties())
                    {

                        string name = item.Name;
                        if (item.GetValue(track.track_map_layers) != null)
                        {
                            string url = svgpath + item.GetValue(track.track_map_layers);
                            string content = Regex.Replace((fData.getTrackSVG(url)), @"<style.*?>[\s\S]*?.*?[\s\S]*?<\/style>", "");
                            content = Regex.Replace(content, @"<!--.*?-->", "");
                            string createDiv = "<div id=\"track-" + name + "\">";
                            htmlcontent += createDiv + content + "</div>";
                        }
                    }
                    htmlcontent += "</div></body></html>";
                    File.WriteAllText(targetfolder + trackfile, htmlcontent);
                }
                if(!File.Exists(targetfolder + trackpic))
                {
                    convertHTMLtoPNG(targetfolder + trackfile, targetfolder + trackpic);
                }
            }
        }

        private static void convertHTMLtoPNG(string source, string target)
        {
            var th = new Thread(() =>
            {
                var webBrowser = new WebBrowser
                {
                    ScrollBarsEnabled = false,
                    IsWebBrowserContextMenuEnabled = true,
                    AllowNavigation = true
                };
                webBrowser.DocumentCompleted += webBrowser_DocumentCompleted;
                webBrowser.Url = new Uri(String.Format(source));
                webBrowser.AccessibleDescription = target;
                webBrowser.Width = 300;
                webBrowser.Height = 150;

                Application.Run();

            });
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
            th.Join();
        }
        static void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var webBrowser = (WebBrowser)sender;
            var content = webBrowser.Document.Body.InnerHtml;
            content = Regex.Replace(content, @"<div id=""track-StartFinish"">[\s\S]*?.*?[\s\S]*?<\/div>", "");
            content = Regex.Replace(content, @"<div id=""track-background"">[\s\S]*?.*?[\s\S]*?<\/div>", "");
            content = Regex.Replace(content, @"<div id=""track-inactive"">[\s\S]*?.*?[\s\S]*?<\/div>", "");
            content = Regex.Replace(content, @"<div id=""track-turns"">[\s\S]*?.*?[\s\S]*?<\/div>", "");
            webBrowser.Document.Body.InnerHtml = content;
            using (Bitmap bitmap = new Bitmap(webBrowser.Width, webBrowser.Height))
            {
                webBrowser.DrawToBitmap(bitmap,  new Rectangle(0, 0, bitmap.Width, bitmap.Height));
                bitmap.Save(((System.Windows.Forms.WebBrowser)sender).AccessibleDescription, System.Drawing.Imaging.ImageFormat.Png);
            }
            webBrowser.Dispose();
            Application.Exit();
        }

        public List<tracks.TracksInSeries> getTracksInSeries(List<tracks.Root> tracks, List<seriesSeason.Root> series)
        {
            List<tracks.TracksInSeries> tracksInSeries = new List<tracks.TracksInSeries>();
            foreach (var track in tracks)
            {
                foreach (var serie in series)
                {
                    foreach (var schedule in serie.schedules)
                    {
                        if (track.track_id == schedule.track.track_id)
                        {
                            tracksInSeries.Add(new tracks.TracksInSeries { track_id = track.track_id, series_id = schedule.series_id, week = schedule.race_week_num+1, SeasonSchedule = schedule, Serie = serie });
                        }
                    }
                }
            }
            return tracksInSeries;
        }
    }
}