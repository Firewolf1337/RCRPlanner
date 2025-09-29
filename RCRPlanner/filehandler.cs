using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static RCRPlanner.memberInfo;

namespace RCRPlanner
{
    public class filehandler
    {
        //MainWindow mw = new MainWindow();
        readonly string iracingSeriesImages = "https://images-static.iracing.com/img/logos/series/";
        readonly string[] iracingCarImages = { "https://ir-core-sites.iracing.com/members/member_images/cars/carid_", "/profile.jpg", "https://images-static.iracing.com" };
        readonly string localIracingWebserver = "http://localhost:32034/pk_car.png";
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
                try
                {
                    if (MainWindow.main != null)
                    {
                        MainWindow.main.Status = "Loading series logo " + counter + " / " + seriesAssetsList.Count();
                    }
                }
                catch { }
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
        public async Task<List<string>> getCustomSeriesFiles(string filepattern)
        {
            List<string> Files = new List<string>();
            DirectoryInfo searchBase = new DirectoryInfo(exePath + @"\static\");
            FileInfo[] filesInDir = searchBase.GetFiles(filepattern + "*.xml");

            foreach (FileInfo foundFile in filesInDir)
            {
                Files.Add(@"\static\" + foundFile.Name);
            }
            return Files;
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
            bool islokalinstalled = false;
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

            if ((await fData.checkServer(localIracingWebserver)).IsSuccessStatusCode)
            {
                islokalinstalled=true;
            }
            foreach (var logo in carsAssetsList)
            {
                counter++;
                if (MainWindow.main != null)
                {
                    MainWindow.main.Status = "Loading car pictures " + counter + " / " + carsAssetsList.Count();
                }
                /*file = exePath + imagefolder + logo.car_id + "_logo.png";
                if (!File.Exists(file))
                {
                    try
                    {
                        await fData.getImage(iracingCarImages[2] + logo.folder + "/" + logo.small_image, file, true, 50);
                    }
                    catch (Exception ex) { }
                }*/
                file = exePath + imagefolder + logo.car_id + ".png";
                if (!File.Exists(file))
                {
                    try
                    {
                        if (islokalinstalled)
                        {
                            var _car = MainWindow.main.carsList.First(c => c.car_id == logo.car_id);
                            string _color =  MainWindow.main.User.helmet.color1 + "," + MainWindow.main.User.helmet.color2 + "," + MainWindow.main.User.helmet.color3;
                            string _url = localIracingWebserver + "?size=2&numShow=0&carPat=3&carPath=" + _car.car_dirpath + "&carCol=" + _color;
                            await fData.getImage(_url, file, false);
                        }
                        else
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
                    }
                    catch { }
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
        public async Task getTrackSVGAsync(List<trackAssets.Root> tracks, string targetfolder)
        {
            int counter = 0;
            int totalTracks = tracks.Count;

            var styleRegex = new Regex(@"<style.*?>[\s\S]*?</style>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var commentRegex = new Regex(@"<!--.*?-->", RegexOptions.Compiled);

            var httpClient = new HttpClient();
            var svgCache = new ConcurrentDictionary<string, Task<string>>();
            var throttler = new SemaphoreSlim(Environment.ProcessorCount * 2);

            long lastUpdateTicks = DateTime.UtcNow.Ticks;
            long updateIntervalTicks = TimeSpan.FromMilliseconds(500).Ticks;

            async Task<string> LoadAndCleanAsync(string url)
            {
                try
                {
                    var data = await httpClient.GetStringAsync(url).ConfigureAwait(false);
                    data = commentRegex.Replace(data, "");
                    data = styleRegex.Replace(data, "");
                    return data;
                }
                catch
                {
                    return string.Empty;
                }
            }

            async Task WriteTextToFileAsync(string path, string content)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path) ?? ".");
                using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    await sw.WriteAsync(content).ConfigureAwait(false);
                    await sw.FlushAsync().ConfigureAwait(false);
                }
            }

            var tasks = tracks.Select(async track =>
            {
                await throttler.WaitAsync().ConfigureAwait(false);
                try
                {
                    int current = Interlocked.Increment(ref counter);

                    long prev = Interlocked.Read(ref lastUpdateTicks);
                    long now = DateTime.UtcNow.Ticks;
                    if (Interlocked.CompareExchange(ref lastUpdateTicks, now, prev) == prev)
                    {
                        var _ = MainWindow.main.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            MainWindow.main.Status = $"Loading track pictures {current} / {totalTracks}";
                        }));
                    }

                    string trackpic = Path.Combine(targetfolder, $"{track.track_id}.png");
                    string trackcss = File.ReadAllText("track.css");
                    if (!File.Exists(trackpic))
                    {
                        var sb = new StringBuilder();
                        sb.Append("<!DOCTYPE html><html><head>");
                        sb.Append("<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\" />");
                        sb.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
                        sb.Append("<style>");
                        sb.Append(trackcss);
                        sb.Append("</style></head>");
                        sb.Append("<body><div id=\"svgMap\">");

                        string svgpath = track.track_map;
                        var properties = track.track_map_layers.GetType().GetProperties();

                        var layerTasks = properties.Select(async prop =>
                        {
                            var value = prop.GetValue(track.track_map_layers);
                            if (value == null) return null;

                            string name = prop.Name;
                            string url = svgpath + value;

                            var svgTask = svgCache.GetOrAdd(url, key => LoadAndCleanAsync(key));
                            var svg = await svgTask.ConfigureAwait(false);

                            if (string.IsNullOrEmpty(svg)) return null;
                            return $"<div id=\"track-{name}\">{svg}</div>";
                        });

                        var results = await Task.WhenAll(layerTasks).ConfigureAwait(false);
                        foreach (var r in results.Where(x => x != null))
                            sb.Append(r);

                        sb.Append("</div></body></html>");
                        await Task.Run(() => ConvertHtmlToPngFromString(sb.ToString(), trackpic)).ConfigureAwait(false);
                    }


                }
                finally
                {
                    throttler.Release();
                }
            });

            await Task.WhenAll(tasks).ConfigureAwait(false);

            var __ = MainWindow.main.Dispatcher.BeginInvoke(new Action(() =>
            {
                MainWindow.main.Status = $"Track loading complete ({totalTracks})";
            }));
        }
        private static void ConvertHtmlToPngFromString(string htmlContent, string target)
        {
            var thread = new Thread(() =>
            {
                var browser = new WebBrowser
                {
                    ScrollBarsEnabled = false,
                    IsWebBrowserContextMenuEnabled = false,
                    Width = 300,
                    Height = 150
                };

                browser.DocumentCompleted += (s, e) =>
                {
                    try
                    {
                        var wb = (WebBrowser)s;

                        string content = wb.Document.Body.InnerHtml;
                        content = Regex.Replace(content, @"<div id=""track-StartFinish"">[\s\S]*?</div>", "");
                        content = Regex.Replace(content, @"<div id=""track-background"">[\s\S]*?</div>", "");
                        content = Regex.Replace(content, @"<div id=""track-inactive"">[\s\S]*?</div>", "");
                        content = Regex.Replace(content, @"<div id=""track-turns"">[\s\S]*?</div>", "");
                        wb.Document.Body.InnerHtml = content;

                        var bitmap = new Bitmap(wb.Width, wb.Height, PixelFormat.Format24bppRgb);
                        wb.DrawToBitmap(bitmap, new Rectangle(0, 0, bitmap.Width, bitmap.Height));
                        bitmap.Save(target, ImageFormat.Png);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error rendering HTML content: {ex.Message}");
                    }
                    finally
                    {
                        browser.Dispose();
                        Application.ExitThread();
                    }
                };

                browser.DocumentText = htmlContent;

                Application.Run();
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }

        public List<tracks.TracksInSeries> getTracksInSeries(List<tracks.Root> tracks, List<seriesSeason.Root> series)
        {
            List<tracks.TracksInSeries> tracksInSeries = new List<tracks.TracksInSeries>();
            foreach (var track in tracks)
            {
                foreach (var serie in series)
                {
                    if(serie.schedules == null)
                        continue;
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