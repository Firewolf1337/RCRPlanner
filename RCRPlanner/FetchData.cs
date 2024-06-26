﻿using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using System.Diagnostics;
using System.Windows;

using System.Drawing;



namespace RCRPlanner
{
    class FetchData
    {
        readonly string iracingAuthUrl = "https://members-ng.iracing.com/auth";
        readonly string iracingDataDoc = "https://members-ng.iracing.com/data/doc";

        readonly string iracingCarsGet = "https://members-ng.iracing.com/data/car/get";
        readonly string iracingCarAssets = "https://members-ng.iracing.com/data/car/assets";
        readonly string iracingCarClassGet = "https://members-ng.iracing.com/data/carclass/get";
        //string iracingCarPics = "https://ir-core-sites.iracing.com/members/member_images/cars/carid_401/profile.jpg";

        readonly string iracingMemberInfo = "https://members-ng.iracing.com/data/member/info";
        readonly string iracingSeriesGet = "https://members-ng.iracing.com/data/series/get";
        readonly string iracingSeriesAssets = "https://members-ng.iracing.com/data/series/assets";
        readonly string iracingSeriesSeason = "https://members-ng.iracing.com/data/series/seasons";
        readonly string iracingSeriesPastSeasons = "https://members-ng.iracing.com/data/series/past_seasons?series_id=";

        readonly string iracingParticipationCredits = "https://members-ng.iracing.com/data/member/participation_credits";

        readonly string iracingTracksGet = "https://members-ng.iracing.com/data/track/get";
        readonly string iracingTacksAssets = "https://members-ng.iracing.com/data/track/assets";

        //string iracingSeriesImages = "https://images-static.iracing.com/img/logos/series/";

        readonly string partnerLink = "https://api.npoint.io/6cef9bfd30441a01c8c9";

        private CookieContainer cookie = new CookieContainer();
        public HttpClientHandler handler = new HttpClientHandler();
        public static HttpClient client = new HttpClient();
        public bool loggedIn = false;
        Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings
        {
            Error = (se, ev) =>
            {
                MessageBox.Show("Deserialization: " + ev.ErrorContext.Error.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                ev.ErrorContext.Handled = true;
            }
        };

        public class iRacingLinks
        {
            public string link { get; set; }
            public DateTime expires { get; set; }
        }
        public async Task<int> Login_API(byte[] Email, byte[] Password, bool forcelogin)
        {
            HttpResponseMessage response;

            List<Cookie> cookies = new List<Cookie>();
            foreach (Cookie cookie in cookie.GetCookies(new Uri("https://iracing.com")))
            {
                cookies.Add(cookie);
            }
            int theCookie = cookies.FindIndex(c => c.Name == "authtoken_members");
            if (theCookie != -1 && forcelogin == false)
            {
                if (cookies[theCookie].Expires > (DateTime.Now.AddMinutes(10)))
                {
                    try
                    {
                        response = await client.GetAsync(iracingDataDoc);
                        return Convert.ToInt32(response.StatusCode);

                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null)
                        {
                            MessageBox.Show("Existing connection: " + ex.InnerException.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            else
            {
                if (handler.CookieContainer.Count == 0 || forcelogin)
                {
                    try
                    {
                        handler = new HttpClientHandler
                        {
                            CookieContainer = cookie
                        };
                        client = new HttpClient(handler);
                        string loginHash = EncryptPW(Email, Password);
                        string postBody = "{\"email\": \"" + Encoding.Default.GetString(Email) + "\",\"password\": \"" + loginHash + "\"}";
                        var content = new StringContent(postBody, Encoding.UTF8, "application/json");

                        response = await client.PostAsync(iracingAuthUrl, content);
                        response = await client.GetAsync(iracingDataDoc);
                        return Convert.ToInt32(response.StatusCode);
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null)
                        {
                            MessageBox.Show("Connection:" + ex.InnerException.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            return 0;
        }

        public async Task<string> getLink(string url)
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var contents = await response.Content.ReadAsStringAsync();
            iRacingLinks linkJson = JsonSerializer.Deserialize<iRacingLinks>(contents);
            string link = linkJson.link;

            return link;
        }
        public async Task getImage(string url, string outputPath, bool resize, int sizePerC = 100)
        {
            byte[] fileBytes = await client.GetByteArrayAsync(url);
            if (resize)
            {
                System.IO.MemoryStream stream = new System.IO.MemoryStream(fileBytes, false);
                Bitmap source = new Bitmap(stream);
                int newWidth = (source.Width / 100) * sizePerC;
                int newHight = (source.Height / 100) * sizePerC;
                string fileextension = Path.GetExtension(outputPath);
                if (fileextension == ".jpg" || fileextension == ".jpeg")
                {
                    new jpegResizing(fileBytes)
                        .Resize(newWidth, newHight)
                        .Quality(100)
                        .Save(outputPath);
                }
                else if (fileextension == ".png")
                {
                    new pngResizing(fileBytes)
                        .Resize(newWidth, newHight)
                        .Save(outputPath);
                }
                else
                {
                    new jpegResizing(fileBytes)
                        .Resize(newWidth, newHight)
                        .Quality(100)
                        .Save(outputPath);
                }
                source.Dispose();
            }
            else
            {
                File.WriteAllBytes(outputPath, fileBytes);
            }
        }

        public string EncryptPW(byte[] Email, byte[] Password)
        {
            byte[] logindata = helper.CombineByte(Password, Email);
            SHA256Managed SHA256 = new SHA256Managed();
            byte[] hash = SHA256.ComputeHash(logindata);
            string Base64 = Convert.ToBase64String(hash);
            return Base64;
        }

        public async Task<memberInfo.Root> getMemberInfo()
        {
            var link = await getLink(iracingMemberInfo);
            var response = await client.GetAsync(link);

            response.EnsureSuccessStatusCode();
            var contents = await response.Content.ReadAsStringAsync();
            var responseObject = new memberInfo.Root();
            try
            {
                responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<memberInfo.Root>(contents, settings);
                //responseObject = JsonSerializer.Deserialize<memberInfo.Root>(contents);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    MessageBox.Show("User data download:" + ex.InnerException.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("User data download:" + ex.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return responseObject;
        }

        public async Task<List<cars.Root>> getCars()
        {
            var link = await getLink(iracingCarsGet);
            var response = await client.GetAsync(link);
            response.EnsureSuccessStatusCode();
            var contents = await response.Content.ReadAsStringAsync();
            //var responseObject = JsonSerializer.Deserialize<List<cars.Root>>(contents);
            var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<List<cars.Root>>(contents, settings);

            return responseObject;
        }
        public async Task<List<carClass.Root>> getCarClass()
        {
            var link = await getLink(iracingCarClassGet);
            var response = await client.GetAsync(link);
            response.EnsureSuccessStatusCode();
            var contents = await response.Content.ReadAsStringAsync();
            //var responseObject = JsonSerializer.Deserialize<List<carClass.Root>>(contents);
            var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<List<carClass.Root>>(contents, settings);

            return responseObject;
        }
        public async Task<List<carAssets>> getCarsAssets()
        {
            var link = await getLink(iracingCarAssets);
            var response = await client.GetAsync(link);
            response.EnsureSuccessStatusCode();
            var contents = await response.Content.ReadAsStringAsync();
            var responseObject = new List<carAssets>();
            try
            {
                //responseObject = (JsonSerializer.Deserialize<Dictionary<string, carAssets>>(contents)).Values.ToList();
                responseObject = (Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, carAssets>>(contents, settings)).Values.ToList();
            }
            catch
            {

            }
            return responseObject;
        }


        public async Task<List<series.Root>> getSeries()
        {
            var link = await getLink(iracingSeriesGet);
            var response = await client.GetAsync(link);
            response.EnsureSuccessStatusCode();
            var contents = await response.Content.ReadAsStringAsync();
            //var responseObject = JsonSerializer.Deserialize<List<series.Root>>(contents);
            var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<List<series.Root>>(contents, settings);

            return responseObject;
        }
        public async Task<List<seriesAssets>> getSeriesAssets()
        {
            var link = await getLink(iracingSeriesAssets);
            var response = await client.GetAsync(link);
            response.EnsureSuccessStatusCode();
            var contents = await response.Content.ReadAsStringAsync();
            var responseObject = new List<seriesAssets>();
            try
            {
                //responseObject = (JsonSerializer.Deserialize<Dictionary<string, seriesAssets>>(contents)).Values.ToList();
                responseObject = (Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, seriesAssets>>(contents, settings)).Values.ToList();
            }
            catch
            {

            }
            return responseObject;
        }
        public async Task<List<seriesSeason.Root>> getSeriesSeason()
        {
            var link = await getLink(iracingSeriesSeason);
            var response = await client.GetAsync(link);
            response.EnsureSuccessStatusCode();
            var contents = await response.Content.ReadAsStringAsync();
            //var responseObject = JsonSerializer.Deserialize<List<seriesSeason.Root>>(contents);
            var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<List<seriesSeason.Root>>(contents, settings);

            return responseObject;
        }
        public async Task<seriesPastSeasons.Root> getSeriesPastSeasons(string seriesID)
        {
            var link = await getLink(iracingSeriesPastSeasons + seriesID);
            var response = await client.GetAsync(link);
            response.EnsureSuccessStatusCode();
            var contents = await response.Content.ReadAsStringAsync();
            //var responseObject = JsonSerializer.Deserialize<seriesPastSeasons.Root>(contents);
            var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<seriesPastSeasons.Root>(contents, settings);

            return responseObject;
        }

        public async Task<List<tracks.Root>> getTracks()
        {
            var link = await getLink(iracingTracksGet);
            var response = await client.GetAsync(link);
            response.EnsureSuccessStatusCode();
            var contents = await response.Content.ReadAsStringAsync();
            //var responseObject = JsonSerializer.Deserialize<List<tracks.Root>>(contents);
            var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<List<tracks.Root>>(contents, settings);

            return responseObject;
        }
        public async Task<List<trackAssets.Root>> getTracksAssets()
        {
            var link = await getLink(iracingTacksAssets);
            var response = await client.GetAsync(link);
            response.EnsureSuccessStatusCode();
            var contents = await response.Content.ReadAsStringAsync();
            var responseObject = new List<trackAssets.Root>();
            try
            {
                //responseObject = (JsonSerializer.Deserialize<Dictionary<string, trackAssets.Root>>(contents)).Values.ToList();
                responseObject = (Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, trackAssets.Root>>(contents, settings)).Values.ToList();

            }
            catch
            {

            }
            return responseObject;
        }

        public async Task<List<string>> getSerieSearchLinks(string url)
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var contents = await response.Content.ReadAsStringAsync();
            List<string> link = new List<string>();
            searchSeries.Root linkJson = JsonSerializer.Deserialize<searchSeries.Root>(contents);
            foreach (var chunk in linkJson.data.chunk_info.chunk_file_names)
            {
                link.Add(linkJson.data.chunk_info.base_download_url + chunk);
            }
            return link;
        }
        public async Task<List<string>> getSesasonDriverStandingsLinks(string url)
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var contents = await response.Content.ReadAsStringAsync();
            List<string> link = new List<string>();
            seasonDriverStandingsLinks.Root linkJson = JsonSerializer.Deserialize<seasonDriverStandingsLinks.Root>(contents);
            foreach (var chunk in linkJson.chunk_info.chunk_file_names)
            {
                link.Add(linkJson.chunk_info.base_download_url + chunk);
            }
            return link;
        }

        public async Task<List<searchSerieResults.Root>> getSeriesSearchResults(string link)
        {
            var response = await client.GetAsync(link);
            response.EnsureSuccessStatusCode();
            var contents = await response.Content.ReadAsStringAsync();
            //var responseObject = JsonSerializer.Deserialize<List<searchSerieResults.Root>>(contents);
            var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<List<searchSerieResults.Root>>(contents, settings);

            return responseObject;
        }
        public async Task<List<seasonDriverStandings.Root>> getseasonDriverStandingsResults(string link)
        {
            var response = await client.GetAsync(link);
            response.EnsureSuccessStatusCode();
            var contents = await response.Content.ReadAsStringAsync();
            //var responseObject = JsonSerializer.Deserialize<List<seasonDriverStandings.Root>>(contents);
            var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<List<seasonDriverStandings.Root>>(contents, settings);

            return responseObject;
        }
        public string getTrackSVG(string url)
        {
            string download = "";
            using (WebClient downloader = new WebClient())
            {
                download = downloader.DownloadString(url);
            }
            return download;
        }
        public async Task<List<participationCredits.Root>> getparticipationCredits()
        {
            var link = await getLink(iracingParticipationCredits);
            var response = await client.GetAsync(link);
            response.EnsureSuccessStatusCode();
            var contents = await response.Content.ReadAsStringAsync();
            //var responseObject = JsonSerializer.Deserialize<List<series.Root>>(contents);
            var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<List<participationCredits.Root>>(contents, settings);

            return responseObject;
        }

        public async Task<githubLatestRelease.Root> getGithubLastRelease(string url, string version)
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("RCRPlanner");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github.v3+json");
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var contents = await response.Content.ReadAsStringAsync();
            //var responseObject = JsonSerializer.Deserialize<githubLatestRelease.Root>(contents);
            var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<githubLatestRelease.Root>(contents, settings);
            if (version != responseObject.tag_name)
            {
                using (HttpResponseMessage zipResponse = await client.GetAsync(responseObject.assets[0].browser_download_url))
                {
                    if (zipResponse.IsSuccessStatusCode)
                    {
                        string path = System.Windows.Forms.Application.StartupPath;
                        string tempDir = Path.Combine(path, Guid.NewGuid().ToString());
                        string fileName = Path.GetFileName(System.Windows.Forms.Application.ExecutablePath);
                        string pid = Process.GetCurrentProcess().Id.ToString();
                        Directory.CreateDirectory(tempDir);

                        Stream zipStream = await zipResponse.Content.ReadAsStreamAsync();
                        ZipArchive zipfile = new ZipArchive(zipStream, ZipArchiveMode.Read);
                        try
                        {
                            zipfile.ExtractToDirectory(tempDir);
                        }
                        catch { }
                        if (File.Exists(tempDir + "\\RCRUpdater.exe"))
                        {
                            File.Delete(System.Windows.Forms.Application.StartupPath + "\\RCRUpdater.exe");
                            File.Move(tempDir + "\\RCRUpdater.exe", System.Windows.Forms.Application.StartupPath + "\\RCRUpdater.exe");
                        }

                        Process.Start("RCRUpdater.exe", "\"" + path + "\" \"" + fileName + "\" \"" + pid + "\" \"" + tempDir + "\"");
                    }
                }
            }

            return responseObject;
        }
        public bool EnsureVersionCompatibility()
        {
            string[] favfiles =
            {
                "favouriteCars.xml",
                "favouriteSeries.xml",
                "favouriteTracks.xml"
            };
            try
            {
                foreach (string file in favfiles)
                    {
                    string path = System.Windows.Forms.Application.StartupPath + "\\" + file;
                    if (File.Exists(path)) {
                        string text = File.ReadAllText(path);
                        text = text.Replace("avourite", "avorite");
                        text = text.Replace("avoutire", "avorite");
                        File.WriteAllText(path, text);
                        try
                        {
                            System.IO.File.Move(path, System.Windows.Forms.Application.StartupPath + "\\" + file.Replace("avourite", "avorite"));

                        }
                        catch { }
                    }
                }
                return true;
            }
            catch {
                return false; 
            }
        }
        public async Task<githubLatestRelease.Root> getGithubActualReleaseinfo(string url)
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("RCRPlanner");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github.v3+json");
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var contents = await response.Content.ReadAsStringAsync();
            var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<githubLatestRelease.Root>(contents, settings);
            return responseObject;
        }

        public async Task<List<partner>> getPartner()
        {
            var response = await client.GetAsync(partnerLink);

            response.EnsureSuccessStatusCode();
            var contents = await response.Content.ReadAsStringAsync();
            var responseObject = new List<partner>();
            try
            {
                responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<List<partner>>(contents, settings);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    MessageBox.Show("Partner download:" + ex.InnerException.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Partner download:" + ex.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return responseObject;
        }
    }
}
