using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Media.Imaging;
using System.Diagnostics;


namespace RCRPlanner
{
    class FetchData
    {
        string iracingAuthUrl = "https://members-ng.iracing.com/auth";
        string iracingDataDoc = "https://members-ng.iracing.com/data/doc";

        string iracingCarsGet = "https://members-ng.iracing.com/data/car/get";
        string iracingCarAssets = "https://members-ng.iracing.com/data/car/assets";
        string iracingCarClassGet = "https://members-ng.iracing.com/data/carclass/get";
        string iracingCarPics = "https://ir-core-sites.iracing.com/members/member_images/cars/carid_401/profile.jpg";

        string iracingMemberInfo = "https://members-ng.iracing.com/data/member/info";
        string iracingSeriesGet = "https://members-ng.iracing.com/data/series/get";
        string iracingSeriesAssets = "https://members-ng.iracing.com/data/series/assets";
        string iracingSeriesSeason = "https://members-ng.iracing.com/data/series/seasons";
        string iracingSeriesPastSeasons = "https://members-ng.iracing.com/data/series/past_seasons?series_id=";

        string iracingTracksGet = "https://members-ng.iracing.com/data/track/get";
        string iracingTacksAssets = "https://members-ng.iracing.com/data/track/assets";

        string iracingSeriesImages = "https://images-static.iracing.com/img/logos/series/";

        private CookieContainer cookie = new CookieContainer();
        public HttpClientHandler handler = new HttpClientHandler();
        public static HttpClient client = new HttpClient();
        public bool loggedIn = false;

        public class iRacingLinks
        {
            public string link { get; set; }
            public DateTime expires { get; set; }
        }
        public async Task<int> Login_API(byte[] Email, byte[] Password, bool forcelogin)
        {
            HttpResponseMessage response;
            foreach (Cookie cookie in cookie.GetCookies(new Uri("https://iracing.com")))
            {
                if (cookie.Name == "authtoken_members" && cookie.Expires > DateTime.Now.AddMinutes(10) && forcelogin == false)
                {
                    return 200;
                }
            }
            if (handler.CookieContainer.Count == 0 || forcelogin)
            {
                handler.CookieContainer = cookie;
                client = new HttpClient(handler);
                string loginHash = EncryptPW(Email, Password);
                string postBody = "{\"email\": \"" + Encoding.Default.GetString(Email) + "\",\"password\": \"" + loginHash + "\"}";
                var content = new StringContent(postBody, Encoding.UTF8, "application/json");

                response = await client.PostAsync(iracingAuthUrl, content);
            }


            response = await client.GetAsync(iracingDataDoc);

            return Convert.ToInt32(response.StatusCode);
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
        public async Task getImage(string url, string outputPath)
        {
            byte[] fileBytes = await client.GetByteArrayAsync(url);
            File.WriteAllBytes(outputPath, fileBytes);

        }
        public async Task<string> getLinklist()
        {
            var response = await client.GetAsync(iracingDataDoc);
            response.EnsureSuccessStatusCode();
            var contents = await response.Content.ReadAsStringAsync();
            var jsonfile = JsonSerializer.Deserialize<List<DataDoc.Root>>(contents);
            foreach (DataDoc.Root item in jsonfile)
            {
                string link = item.car.get.link;
            }
            return contents;
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
                responseObject = JsonSerializer.Deserialize<memberInfo.Root>(contents);
            }
            catch
            {

            }
            return responseObject;
        }

        public async Task<List<cars.Root>> getCars()
        {
            var link = await getLink(iracingCarsGet);
            var response = await client.GetAsync(link);
            response.EnsureSuccessStatusCode();
            var contents = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<List<cars.Root>>(contents);

            return responseObject;
        }
        public async Task<List<carClass.Root>> getCarClass()
        {
            var link = await getLink(iracingCarClassGet);
            var response = await client.GetAsync(link);
            response.EnsureSuccessStatusCode();
            var contents = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<List<carClass.Root>>(contents);

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
                responseObject = (JsonSerializer.Deserialize<Dictionary<string, carAssets>>(contents)).Values.ToList();
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
            var responseObject = JsonSerializer.Deserialize<List<series.Root>>(contents);

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
                responseObject = (JsonSerializer.Deserialize<Dictionary<string, seriesAssets>>(contents)).Values.ToList();
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
            var responseObject = JsonSerializer.Deserialize<List<seriesSeason.Root>>(contents);

            return responseObject;
        }
        public async Task<seriesPastSeasons.Root> getSeriesPastSeasons(string seriesID)
        {
            var link = await getLink(iracingSeriesPastSeasons + seriesID);
            var response = await client.GetAsync(link);
            response.EnsureSuccessStatusCode();
            var contents = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<seriesPastSeasons.Root>(contents);

            return responseObject;
        }

        public async Task<List<tracks.Root>> getTracks()
        {
            var link = await getLink(iracingTracksGet);
            var response = await client.GetAsync(link);
            response.EnsureSuccessStatusCode();
            var contents = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<List<tracks.Root>>(contents);

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
                responseObject = (JsonSerializer.Deserialize<Dictionary<string, trackAssets.Root>>(contents)).Values.ToList();
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
            var responseObject = JsonSerializer.Deserialize<List<searchSerieResults.Root>>(contents);

            return responseObject;
        }
        public async Task<List<seasonDriverStandings.Root>> getseasonDriverStandingsResults(string link)
        {
            var response = await client.GetAsync(link);
            response.EnsureSuccessStatusCode();
            var contents = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<List<seasonDriverStandings.Root>>(contents);

            return responseObject;
        }
        public async Task<string> getTrackSVG(string url)
        {
            string download = "";
            using (WebClient downloader = new WebClient())
            {
                download = downloader.DownloadString(url);
            }
            return download;
        }

        public async Task<githubLatestRelease.Root> getGithubLastRelease(string url, string version)
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("RCRPlanner");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github.v3+json");
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var contents = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<githubLatestRelease.Root>(contents);
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
    }
}
