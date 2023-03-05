using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security;
using System.Security.Cryptography;
using System.Net.Http;
using System.Windows.Media.Animation;
using System.Diagnostics;
using System.ComponentModel;
using System.Globalization;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Timers;
using System.Media;
using System.Windows.Controls.Primitives;
using System.Runtime.CompilerServices;
using System.Reflection;


namespace RCRPlanner
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>       


    public partial class MainWindow : Window
    {
        double moveAnimationDuration = 0.3;
        RCRPlanner.FetchData fData = new RCRPlanner.FetchData();
        RCRPlanner.filehandler fh = new RCRPlanner.filehandler();
        RCRPlanner.statistics statistics = new RCRPlanner.statistics();
        private static readonly HttpClient client = new HttpClient();
        memberInfo.Root User = new memberInfo.Root();
        string username;
        SecureString password;
        string userfile = @"user.xml";
        bool isMetric = new RegionInfo(CultureInfo.CurrentCulture.Name).IsMetric;
        string favsymbolSelected = "★";
        string favsymbolUnselected = "✰";
        string checksymbol = "🗸";
        string unchecksymbol = "×";
        string alarmClockSymbol = "⏰";
        string clockSymbol = "🕙";
        string activeGrid = "";
        bool reloadData = false;
        int lastLoginResult = -1;
        bool savelogin = false;
        int iRatingStatUserIrating = -1;
        System.Timers.Timer alarmTimer = new System.Timers.Timer();

        static Version version = Assembly.GetExecutingAssembly().GetName().Version;
        static DateTime buildDate = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
        string displayableVersion = $"{version} ({buildDate})";

        string favSeriesfile = @"\favouriteSeries.xml";
        string favCarsfile = @"\favouriteCars.xml";
        string favTracksfile = @"\favouriteTracks.xml";
        List<memberInfo.FavoutireCars> favoutireCars = new List<memberInfo.FavoutireCars>();
        List<memberInfo.FavoutireSeries> favoutireSeries = new List<memberInfo.FavoutireSeries>();
        List<memberInfo.FavoutireTracks> favoutireTracks = new List<memberInfo.FavoutireTracks>();

        string autostartfile = @"\autostart.xml";
        autoStart.Root autoStartApps = new autoStart.Root();
        public bool autostartsuppress;
        List<int> pIDs = new List<int>();
        List<dgObjects.autoStartDataGrid> dgAutoStartList = new List<dgObjects.autoStartDataGrid>();

        private readonly BackgroundWorker bwPresetLoader = new BackgroundWorker();
        private static ManualResetEvent mre = new ManualResetEvent(false);

        List<series.Root> seriesList = new List<series.Root>();
        List<seriesAssets> seriesAssetsList = new List<seriesAssets>();
        List<seriesSeason.Root> seriesSeasonList = new List<seriesSeason.Root>();
        Dictionary<string, string> datafiles = new Dictionary<string, string>();
        List<dgObjects.seriesDataGrid> dgSeriesList = new List<dgObjects.seriesDataGrid>();
        string seriesFile = @"\static\series.xml";
        string seriesSeasonFile = @"\static\seriesSeason.xml";
        string seriesAssetsFile = @"\static\seriesAssets.xml";
        string seriesLogos = @"\static\series\";
        string iracingSeriesImages = "https://images-static.iracing.com/img/logos/series/";

        List<cars.Root> carsList = new List<cars.Root>();
        List<carAssets> carsAssetsList = new List<carAssets>();
        List<carClass.Root> carClassList = new List<carClass.Root>();
        List<carClass.CarInClassId> carClassesList = new List<carClass.CarInClassId>();
        List<cars.CarsInSeries> carsInSeries = new List<cars.CarsInSeries>();
        List<dgObjects.carsDataGrid> dgCarsList = new List<dgObjects.carsDataGrid>();
        string carsFile = @"\static\cars.xml";
        string carsAssetsFile = @"\static\carsAssets.xml";
        string carClassFile = @"\static\carClass.xml";
        string carLogos = @"\static\cars\";
        string[] iracingCarImages = { "https://ir-core-sites.iracing.com/members/member_images/cars/carid_", "/profile.jpg", "https://images-static.iracing.com" };

        List<tracks.Root> tracksList = new List<tracks.Root>();
        List<trackAssets.Root> tracksAssetsList = new List<trackAssets.Root>();
        List<dgObjects.tracksDataGrid> dgTracksList = new List<dgObjects.tracksDataGrid>();
        List<dgObjects.tracksLayoutsDataGrid> dgTrackLayoutList = new List<dgObjects.tracksLayoutsDataGrid>();
        List<tracks.TracksInSeries> tracksInSeries = new List<tracks.TracksInSeries>();
        string tracksFile = @"\static\tracks.xml";
        string tracksAssetsFile = @"\static\tracksAssets.xml";
        string tracksLogo = @"\static\tracks\";
        int selectedTrack = -1;

        List<dgObjects.tracksDataGrid> dgPurchaseGuideList = new List<dgObjects.tracksDataGrid>();
        List<dgObjects.RaceOverviewDataGrid> dgRaceOverviewList = new List<dgObjects.RaceOverviewDataGrid>();
        List<dgObjects.iRaitingDataGrid> dgiRaitingDataGridList = new List<dgObjects.iRaitingDataGrid>();
        public List<Alarms> RaceAlarms = new List<Alarms>();
        MediaPlayer mediaPlayer = new MediaPlayer();

        string exePath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        string defaultfilter = "cbFilterInOfficial;cbFilterOfficial;cbFilterOpenSetup;cbFilterFixedSetup;cbFilterRoad;cbFilterOval;cbFilterDirt;cbFilterDirtOval;cbFilterR;cbFilterD;cbFilterC;cbFilterB;cbFilterA;cbFilterP";

        public MainWindow()
        {
            this.InitializeComponent();
            if (Properties.Settings.Default.UpdateSettings)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpdateSettings = false;
                Properties.Settings.Default.Save();
            }
            if (Properties.Settings.Default.minimized == "true")
            {
                this.WindowState = WindowState.Minimized;
                this.cbStartMinimized.IsChecked = true;
            }
            gridLoading.Visibility = Visibility.Visible;
            gridLoadingBG.Visibility = Visibility.Visible;
            bwPresetLoader.DoWork += worker_DoWork;
            bwPresetLoader.WorkerReportsProgress = true;
            bwPresetLoader.RunWorkerCompleted += worker_RunWorkerCompleted;
            bwPresetLoader.ProgressChanged += worker_ProgressChanged;
            bwPresetLoader.RunWorkerAsync();
            btnLoadRaces_Click(null, null);

            alarmTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            alarmTimer.Interval = 10000;
            alarmTimer.Enabled = true;
            lblVersion.Content = displayableVersion;

        }
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                List<Alarms> toRemove = new List<Alarms>();
                foreach (var alarm in RaceAlarms)
                {
                    if (DateTime.Now >= alarm.AlarmTime)
                    {

                        mediaPlayer.Open(new Uri(exePath + "\\alarm.wav"));
                        mediaPlayer.Play();
                        toRemove.Add(alarm);
                        

                    }
                }
                RaceAlarms = RaceAlarms.Except(toRemove).ToList<Alarms>();
                if(toRemove.Count > 0)
                {
                    generateRaceView();
                }
            }));
        }
        private class bwProgress
        {
            public string StatusText { get; set; }
        }
        private void yourFilter(object sender, FilterEventArgs e)
        {
            series.Root obj = e.Item as series.Root;
            if (obj != null)
            {
                if (obj.category.Contains("road"))
                    e.Accepted = true;
                else
                    e.Accepted = false;
            }
        }
        public void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.BorderThickness = helper.GetDefaultMarginForDpi();
            }
            else
            {
                this.BorderThickness = new System.Windows.Thickness(0);
            }
        }
        private async void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            bool filemissing = false;
            if (!Directory.Exists(exePath+tracksLogo))
            {
                Directory.CreateDirectory(exePath+tracksLogo);
            }
            if (!Directory.Exists(exePath+carLogos))
            {
                Directory.CreateDirectory(exePath+carLogos);
            }
            if (!Directory.Exists(exePath+seriesLogos))
            {
                Directory.CreateDirectory(exePath+seriesLogos);
            }
            bwProgress _bwProgress = new bwProgress() { StatusText = "Init"};
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => {

                _bwProgress.StatusText = "Loading user information.";
                bwPresetLoader.ReportProgress(0, _bwProgress);
                if (Properties.Settings.Default.username != string.Empty)
                {
                    _bwProgress.StatusText = "Login information present.";
                    bwPresetLoader.ReportProgress(3, _bwProgress);
                    username = Properties.Settings.Default.username;
                    cbSaveLogin.IsChecked = true;
                    savelogin = true;
                    password = helper.DecryptString(Properties.Settings.Default.password);
                    tbLoginName.Text = username;

                }
                List<string> filter = new List<string>();
                if (Properties.Settings.Default.filter != string.Empty)
                {
                    try
                    {
                        filter = Properties.Settings.Default.filter.Split(';').ToList();

                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        filter = defaultfilter.Split(';').ToList();

                    }
                    catch { }
                }
                try
                {
                    var cbFilter = FindVisualChildren<CheckBox>(gridFilter);
                    foreach (CheckBox cb in cbFilter)
                    {
                        if (filter.Contains(cb.Name))
                        {
                            cb.IsChecked = true;
                        }
                    }
                }
                catch { }
                if (File.Exists(userfile) && !reloadData)
                {
                    _bwProgress.StatusText = "Loading user information from file.";
                    bwPresetLoader.ReportProgress(5, _bwProgress);
                    User = helper.DeSerializeObject<memberInfo.Root>(userfile);
                    Style_ProfileIcon(User);

                }
                else
                {
                    _bwProgress.StatusText = "User information not present in file.";
                    bwPresetLoader.ReportProgress(5, _bwProgress);
                    filemissing = true;
                }
                if(File.Exists(exePath + autostartfile))
                {
                    autoStartApps = helper.DeSerializeObject<autoStart.Root>(exePath + autostartfile);
                    if(autoStartApps.Programs != null && autoStartApps.Active && !autostartsuppress)
                    {
                        startPrograms();
                    }
                }
                else
                {
                    autoStartApps.Active = false;
                    autoStartApps.Programs = null;
                }
                if (File.Exists(exePath + favSeriesfile))
                {
                    favoutireSeries = helper.DeSerializeObject<List<memberInfo.FavoutireSeries>>(exePath + favSeriesfile);
                    favoutireSeries = favoutireSeries.GroupBy(x => x.series_id).Select(x => x.First()).ToList();

                }
                if (File.Exists(exePath + favTracksfile))
                {
                    favoutireTracks = helper.DeSerializeObject<List<memberInfo.FavoutireTracks>>(exePath + favTracksfile);
                    favoutireTracks = favoutireTracks.GroupBy(x => x.track_id).Select(x => x.First()).ToList();
                }
                if (File.Exists(exePath + favCarsfile))
                {
                    favoutireCars = helper.DeSerializeObject<List<memberInfo.FavoutireCars>>(exePath + favCarsfile);
                    favoutireCars = favoutireCars.GroupBy(x => x.car_id).Select(x => x.First()).ToList();
                }
            }));
            _bwProgress.StatusText = "Checking present data files.";
            bwPresetLoader.ReportProgress(10, _bwProgress);
            foreach (var filename in ((System.Reflection.TypeInfo)typeof(MainWindow)).DeclaredFields)
            {
                if (filename.Name.EndsWith("File"))
                {
                    var file = exePath + filename.GetValue(this);
                    if (File.Exists(file))
                    {
                        _bwProgress.StatusText = "Loading information from " + filename.GetValue(this);
                        bwPresetLoader.ReportProgress(12, _bwProgress);
                    }
                    else
                    {
                        _bwProgress.StatusText = "Missing file: " + filename.GetValue(this);
                        bwPresetLoader.ReportProgress(12, _bwProgress);
                        filemissing = true;
                    }
                }
            }
            if (filemissing || reloadData)
            {
                _bwProgress.StatusText = "Fetching data from iRacion API due to missing files.";
                bwPresetLoader.ReportProgress(20, _bwProgress);
                if (savelogin && password.Length != 0)
                {
                    _bwProgress.StatusText = "Credentials present. Conneting to API...";
                    bwPresetLoader.ReportProgress(25, _bwProgress);
                    var status = fData.Login_API(Encoding.UTF8.GetBytes((username).ToLower()), Encoding.UTF8.GetBytes(helper.ToInsecureString(password)),false);
                    status.Wait();
                    if (status.Result == 401)
                    {
                        move_grid(gridLogin, "bottom", -250, moveAnimationDuration);
                        mre.WaitOne();
                    }
                    if (status.Result >= 200 && 210 >= status.Result)
                    {
                        Task<memberInfo.Root> getUser = fData.getMemberInfo();
                        getUser.Wait();
                        User = getUser.Result;
                        helper.SerializeObject<memberInfo.Root>(User, userfile);
                    }
                    if(lastLoginResult == 503)
                    {
                        try
                        {
                            _bwProgress.StatusText = "iRacing is down!";
                        }
                        catch { }
                    }
                }
                else
                {
                    _bwProgress.StatusText = "Credentials NOT present. Please enter credentials.";
                    bwPresetLoader.ReportProgress(25, _bwProgress);
                    move_grid(gridLogin, "bottom", -250, moveAnimationDuration);
                    mre.WaitOne();
                }
            }


            if (!(lastLoginResult >= 200 && 210 >= lastLoginResult) && lastLoginResult != -1)
            {
                try
                {
                    _bwProgress.StatusText = "iRacing is down!";
                    bwPresetLoader.ReportProgress(99, _bwProgress);
                }
                catch { }
            }
            else
            {
                try
                {
                    _bwProgress.StatusText = "Loading series information.";
                    bwPresetLoader.ReportProgress(30, _bwProgress);
                }
                catch { }
                seriesList = await fh.getSeriesList(seriesFile, reloadData);
                seriesAssetsList = await fh.getSeriesAssets(seriesAssetsFile, seriesLogos, reloadData);
                seriesSeasonList = await fh.getSeriesSeason(seriesSeasonFile, reloadData);
                
                try
                {
                    _bwProgress.StatusText = "Loading car information.";
                    bwPresetLoader.ReportProgress(40, _bwProgress);
                }
                catch { }
                carsList = await fh.getCarList(carsFile, reloadData);
                carsAssetsList = await fh.getCarAssetsList(carsAssetsFile, carLogos, reloadData);
                carClassList = await fh.getCarClassList(carClassFile, reloadData);
                carClassesList = await fh.getCarClassesList(carsList, carClassList);
                carsInSeries = await fh.getCarsInSeries(carClassesList, seriesSeasonList);

                try
                {
                    _bwProgress.StatusText = "Loading track information.";
                    bwPresetLoader.ReportProgress(50, _bwProgress);
                }
                catch { }
                tracksList = await fh.getTracksList(tracksFile, reloadData);
                tracksAssetsList = await fh.getTracksAssets(tracksAssetsFile, reloadData);
                tracksInSeries = await fh.getTracksInSeries(tracksList, seriesSeasonList);
                fh.getTrackSVG(tracksAssetsList, exePath + tracksLogo);
                try
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        _bwProgress.StatusText = "Loading views.";
                        bwPresetLoader.ReportProgress(70, _bwProgress);
                    }));
                    
                }
                catch { }
                createDgData();
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    gridSeries.ItemsSource = dgSeriesList;
                    gridCars.ItemsSource = dgCarsList;
                    gridTracksLayout.ItemsSource = dgTrackLayoutList;
                }));
                try
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        _bwProgress.StatusText = "Done";
                        bwPresetLoader.ReportProgress(100, _bwProgress);
                    }));
                    
                }
                catch{ }
            }
            reloadData = false;
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                gridLoading.Visibility = Visibility.Hidden;
                gridLoadingBG.Visibility = Visibility.Hidden;
                gridMainwindow.Effect = null;
            }));
        }
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => {
                Style_ProfileIcon(User);
            }));
        }
        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => {
                lblLoadingText.Content = ((bwProgress)e.UserState).StatusText;
            }));
            if(e.ProgressPercentage == 100)
            {

            }
        }
        private void startPrograms()
        {
            foreach (var prog in autoStartApps.Programs)
            {
                Process pr = new Process();
                pr.StartInfo.FileName = prog.Path;
                if (autoStartApps.Minimized == true)
                {
                    pr.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                }
                pr.Start();
                if (autoStartApps.Kill)
                {
                    pIDs.Add(pr.Id);
                }
            }
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        private void Close_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                alarmTimer.Stop();
                if (autoStartApps.Kill)
                {
                    if (autoStartApps.KillByName)
                    {
                        foreach (var pname in autoStartApps.Programs)
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
                    else
                    {
                        foreach (var pid in pIDs)
                        {
                            try
                            {
                                Process.GetProcessById(pid).Kill();
                            }
                            catch { }
                        }
                    }
                }
                this.Close();
            }
        }
        private void MaxMin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (this.WindowState != WindowState.Maximized)
                    this.WindowState = WindowState.Maximized;
                else
                    this.WindowState = WindowState.Normal;
            }
        }

        private void Min_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void stackPanelMenu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            resize_Grid(gridMenu, "width", 250, moveAnimationDuration);
        }
        private void stackPanelMenuClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            resize_Grid(gridMenu, "width", 0, moveAnimationDuration);
        }
        private async void elHeaderMenuProfileColor1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int position;
            if (gridProfile.Height != 300)
            {
                if (password == null || password.Length == 0)
                {
                    if (savelogin && await fData.Login_API(Encoding.UTF8.GetBytes((username).ToLower()), Encoding.UTF8.GetBytes(helper.ToInsecureString(password)), false) == 200)
                    {
                        User = await fData.getMemberInfo();
                        helper.SerializeObject<memberInfo.Root>(User, userfile);
                    }
                }
                string roadClass = User.licenses.road.group_name.ToString().Replace("Class ", "").Replace("Rookie", "R").Replace("Pro","P");
                string ovalClass = User.licenses.oval.group_name.ToString().Replace("Class ", "").Replace("Rookie", "R").Replace("Pro", "P");
                string dirtovalClass = User.licenses.dirt_oval.group_name.ToString().Replace("Class ", "").Replace("Rookie", "R").Replace("Pro", "P");
                string dirtroadClass = User.licenses.dirt_road.group_name.ToString().Replace("Class ", "").Replace("Rookie", "R").Replace("Pro", "P");
                progressOval.Value = Convert.ToDouble(helper.Mapdec(Convert.ToDecimal(User.licenses.oval.safety_rating / (4.99 / 100)), 0, 100, 0, 75));
                progressOval.Foreground = (Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#" + User.licenses.oval.color);
                textProgressOval.Text = ovalClass + User.licenses.oval.safety_rating.ToString();
                lbliRatingOval.Content = User.licenses.oval.irating.ToString() + " iR";
                lblCpiOval.Content = Math.Round(User.licenses.oval.cpi,2).ToString() + " CPI";

                progressRoad.Value = Convert.ToDouble(helper.Mapdec(Convert.ToDecimal(User.licenses.road.safety_rating / (4.99 / 100)), 0, 100, 0, 75));
                progressRoad.Foreground = (Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#" + User.licenses.road.color);
                textProgressRoad.Text = roadClass + User.licenses.road.safety_rating.ToString();
                lbliRatingRoad.Content = User.licenses.road.irating.ToString() + " iR";
                lblCpiRoad.Content = Math.Round(User.licenses.road.cpi,2).ToString() + " CPI";

                progressDirtOval.Value = Convert.ToDouble(helper.Mapdec(Convert.ToDecimal(User.licenses.dirt_oval.safety_rating / (4.99 / 100)), 0, 100, 0, 75));
                progressDirtOval.Foreground = (Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#" + User.licenses.dirt_oval.color);
                textProgressDirtOval.Text = dirtovalClass + User.licenses.dirt_oval.safety_rating.ToString();
                lbliRatingDirtOval.Content = User.licenses.dirt_oval.irating.ToString() + " iR";
                lblCpiDirtOval.Content = Math.Round(User.licenses.dirt_oval.cpi,2).ToString() + " CPI";

                progressDirtRoad.Value = Convert.ToDouble(helper.Mapdec(Convert.ToDecimal(User.licenses.dirt_road.safety_rating / (4.99 / 100)), 0, 100, 0, 75));
                progressDirtRoad.Foreground = (Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#" + User.licenses.dirt_road.color);
                textProgressDirtRoad.Text = dirtroadClass + User.licenses.dirt_road.safety_rating.ToString();
                lbliRatingDirtRoad.Content = User.licenses.dirt_road.irating.ToString() + " iR";
                lblCpiDirtRoad.Content = Math.Round(User.licenses.dirt_road.cpi,2).ToString() + " CPI";


                lblCustId.Content = "iRacing ID: " + User.cust_id.ToString();

                position = 300;

            }
            else
            {
                position = 0;
            }

            resize_Grid(gridProfile, "height", position, moveAnimationDuration);
            //move_grid(gridProfile, "right", 0, moveAnimationDuration);
        }
        private void spHeaderMenuLogin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Thickness margin = gridLogin.Margin;
            int position;
            if (margin.Bottom != 50)
            {
                position = 50;
            }
            else
            {
                position = -250;
            }
            move_grid(gridLogin, "bottom", position, moveAnimationDuration);
        }
        private void resize_Grid(Grid grid, string direction, int size, double duration)
        {
            //grid.Dispatcher.Invoke(new Action(() =>{
                DoubleAnimation animation = new DoubleAnimation();
                switch (direction)
                {
                    case "width":
                        animation.From = grid.Width;
                        animation.To = size;
                        animation.Duration = new Duration(TimeSpan.FromSeconds(duration));
                        grid.BeginAnimation(WidthProperty, animation);
                        break;
                    case "height":
                        animation.From = grid.Height;
                        animation.To = size;
                        animation.Duration = new Duration(TimeSpan.FromSeconds(duration));
                        grid.BeginAnimation(HeightProperty, animation);
                        break;
                }
            //}));
        }
        private void move_grid(Grid grid, string direction, int position, double duration)
        {
            grid.Dispatcher.Invoke(new Action(() => {
            Thickness margin = grid.Margin;
               switch(direction)
            {
                case "left":
                    margin.Left = position;
                    break;
                case "right":
                    margin.Right = position;
                    break;
                case "top":
                    margin.Top = position;
                    break;
                case "bottom":
                    margin.Bottom = position;
                    break;

            }
            ThicknessAnimation PositionAnimation = new ThicknessAnimation(margin, new Duration(TimeSpan.FromSeconds(duration)));
            grid.BeginAnimation(MarginProperty, PositionAnimation);
            }));
        }
        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (tbLoginPasword.Password != "")
            {
                if (cbSaveLogin.IsChecked == true)
                {
                    Properties.Settings.Default.username = tbLoginName.Text;
                    Properties.Settings.Default.password = helper.EncryptString(helper.ToSecureString(tbLoginPasword.Password));
                    Properties.Settings.Default.Save();
                    username = Properties.Settings.Default.username;
                    password = helper.DecryptString(Properties.Settings.Default.password);
                }
                else if (cbSaveLogin.IsChecked == false)
                {
                    Properties.Settings.Default.username = "";
                    Properties.Settings.Default.password = "";
                    Properties.Settings.Default.Save();
                    username = tbLoginName.Text;
                    password = tbLoginPasword.SecurePassword;
                }
            }
            var status = await fData.Login_API(Encoding.UTF8.GetBytes((username).ToLower()), Encoding.UTF8.GetBytes(helper.ToInsecureString(password)),true);

            if (status >= 200 && 210 >= status)
            {
                User = await fData.getMemberInfo();
                helper.SerializeObject<memberInfo.Root>(User, userfile);
                Style_ProfileIcon(User);
                spHeaderMenuLogin_MouseDown(null, null);
                lblLogin.Content = "";
                mre.Set();
            }
            else
            {
                lblLogin.Content = "Something went wrong. Please retry";
            }
            
        }
        private void Style_ProfileIcon(memberInfo.Root info)
        {
            var displayName = info.display_name.Split(' ');
            string initials = "" ;
            if (displayName.Length > 2)
            {
                initials = displayName[0].Substring(0,1) + displayName[1].Substring(0, 1) + displayName[displayName.Length - 1].Substring(0, 1);
            }
            else
            {
                initials = displayName[0].Substring(0, 1) + displayName[1].Substring(0, 1);
            }

            string color1 = "#" + info.helmet.color1;
            string color2 = "#" + info.helmet.color2;
            string color3 = "#" + info.helmet.color3;
            elHeaderMenuProfileColor1.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color1));
            elHeaderMenuProfileColor2.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color2));
            elHeaderMenuProfileColor3.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color3));
            this.tbHeaderMenuProfileName.Foreground = new SolidColorBrush(helper.getNegative(color3));
            tbHeaderMenuProfileName.Content = initials;
            this.lblUsername.Content = User.display_name;
            
        }

        private void switchMainGridVisibility(List<System.Windows.Controls.DataGrid> gridToShow, bool Details)
        {
            List<System.Windows.Controls.DataGrid> allGrids = new List<System.Windows.Controls.DataGrid> { gridSeries, gridCars, gridTracksLayout, gridPurchaseGuide, gridRaces, gridCarDetail, gridTrackDetail, gridAutoStart, gridPartStat, gridiRatingStat };

            if (Details)
            {
                gridDetails.Visibility = Visibility.Visible;
            }
            else
            {
                gridDetails.Visibility = Visibility.Hidden;
            }
            foreach (System.Windows.Controls.DataGrid grid in allGrids)
            {
                if(gridToShow.Contains(grid))
                {
                    grid.Visibility = Visibility.Visible;
                }
                else
                {
                    grid.Visibility = Visibility.Hidden;
                }
            }
        }

        private void createDgData()
        {
            generateSeriesView();
            generateCarView();
            generateTrackView();
            generateRaceView();
        }
        private void generateSeriesView()
        {
            //Generate series information
            dgSeriesList.Clear();
            dgSeriesList = (from series in seriesList
                            join asset in seriesAssetsList on series.series_id equals asset.series_id
                            join season in seriesSeasonList on series.series_id equals season.series_id
                            join fav in favoutireSeries on series.series_id equals fav.series_id into ser
                            from allseries in ser.DefaultIfEmpty()
                            select new dgObjects.seriesDataGrid()
                            {
                                SerieId = series.series_id,
                                Seriesimage = asset.logo != null ? new Uri("file:///" + exePath + seriesLogos + asset.logo.ToString()) : new Uri("about:blank"),
                                SeriesName = series.series_name,
                                Category = series.category,
                                Class = new List<series.AllowedLicense>(series.allowed_licenses).Count > 1 ? ((new List<series.AllowedLicense>(series.allowed_licenses)[1]).group_name).Replace("Class ", "").Replace("ookie", "") : ((new List<series.AllowedLicense>(series.allowed_licenses)[0]).group_name).Replace("Class ", "").Replace("ookie", ""),
                                License = ((new List<series.AllowedLicense>(series.allowed_licenses)[0]).group_name).Replace("Class ", "").Replace("ookie", "") + " " + (new List<series.AllowedLicense>(series.allowed_licenses)[0].min_license_level - ((new List<series.AllowedLicense>(series.allowed_licenses)[0]).license_group - 1) * 4).ToString("0.00"),
                                Eligible = series.eligible == true ? checksymbol : "",
                                Favourite = allseries?.series_id != null ? favsymbolSelected : favsymbolUnselected,
                                Official = season.official == true ? checksymbol : unchecksymbol,
                                Fixed = season.fixed_setup == true ? checksymbol : unchecksymbol,
                                Season = season,
                            }).ToList<dgObjects.seriesDataGrid>();
           foreach(var serie in dgSeriesList)
            {
                List<tracks.TracksInSeries> tracksinserie = new List<tracks.TracksInSeries>();
                tracksinserie = (tracksInSeries.FindAll(t => t.series_id == serie.SerieId)).ToList<tracks.TracksInSeries>();
                List<dgObjects.tracksDataGrid> tracks = new List<dgObjects.tracksDataGrid>();
                foreach (var track in tracksinserie)
                {
                    dgObjects.tracksDataGrid _trackobj = new dgObjects.tracksDataGrid();
                    var tr = tracksList.FirstOrDefault(t => t.track_id == track.track_id);
                    _trackobj.Name = tr.track_name;
                    _trackobj.Layoutname = tr.config_name;
                    _trackobj.Corners = tr.corners_per_lap;
                    _trackobj.Created = tr.created.ToString(Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortDatePattern);
                    _trackobj.Length = isMetric ? Math.Round(tr.track_config_length * 1.60934, 3) : tr.track_config_length;
                    _trackobj.Owned = User.track_packages.Any(p => p.package_id == tr.package_id) ? checksymbol : "";
                    _trackobj.Pitlimit = isMetric ? Convert.ToInt32(tr.pit_road_speed_limit * 1.60934) : tr.pit_road_speed_limit;
                    _trackobj.Price = "$" + tr.price.ToString();
                    _trackobj.PackageID = tr.package_id;
                    _trackobj.TrackID = tr.track_id;
                    _trackobj.Category = tr.category;
                    _trackobj.TrackImage = new Uri("file:///" + exePath + tracksLogo + tr.track_id + ".png");
                    _trackobj.Week = track.SeasonSchedule.race_week_num+1;
                    _trackobj.Weekdate = DateTime.Parse(track.SeasonSchedule.start_date).ToString(Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortDatePattern);
                    _trackobj.Racelenght = track.SeasonSchedule.race_lap_limit != null ? track.SeasonSchedule.race_lap_limit.ToString() +" Laps": track.SeasonSchedule.race_time_limit.ToString() + " Min";
                    tracks.Add(_trackobj);
                }
                tracks.Sort((x, y) => x.Week.CompareTo(y.Week));
                serie.Weeks = tracks.Where(t => t.Owned == checksymbol).Count() + "/" + tracks.Count();
                serie.Tracks = tracks;
            }
            dgSeriesList.Sort((x, y) => x.SeriesName.CompareTo(y.SeriesName));
        }
        private void generateCarView()
        {
            // Generate cars information
            dgCarsList.Clear();
            foreach (var car in carsList)
            {
                List<dgObjects.seriesDataGrid> seriesDataGridsList = new List<dgObjects.seriesDataGrid>();
                foreach (var carsinseries in carsInSeries)
                {
                    if (carsinseries.car_id == car.car_id)
                    {
                        var seriesDataGridsObject = (from serie in dgSeriesList
                                                     where serie.SerieId == carsinseries.series_id
                                                     select new dgObjects.seriesDataGrid()
                                                     {
                                                         SerieId = serie.SerieId,
                                                         Seriesimage = serie.Seriesimage,
                                                         SeriesName = serie.SeriesName,
                                                         Category = serie.Category,
                                                         Class = serie.Class,
                                                         License = serie.License,
                                                         Eligible = serie.Eligible,
                                                         Favourite = serie.Favourite,
                                                     }).FirstOrDefault();
                        seriesDataGridsList.Add(seriesDataGridsObject);
                    }

                }
                dgObjects.carsDataGrid carsDataGridObject = new dgObjects.carsDataGrid();
                carsDataGridObject.Favourite = favoutireCars.Any(x => x.car_id == car.car_id) ? favsymbolSelected : favsymbolUnselected;
                carsDataGridObject.CarId = car.car_id;
                carsDataGridObject.CarImage = new Uri("file:///" + exePath + carLogos + car.car_id + ".png");
                carsDataGridObject.CarLogo = new Uri("file:///" + exePath + carLogos + car.car_id + "_logo.png");
                carsDataGridObject.CarName = car.car_name;
                carsDataGridObject.Category = string.Join(",", car.categories);
                carsDataGridObject.Horsepower = isMetric ? Convert.ToInt32(car.hp * 1.01387) : car.hp;
                carsDataGridObject.Weight = isMetric ? Convert.ToInt32(car.car_weight * 0.453592) : car.car_weight;
                carsDataGridObject.Price = "$" + car.price.ToString();
                carsDataGridObject.Owned = User.car_packages.Any(p => p.package_id == car.package_id) ? checksymbol : "";
                carsDataGridObject.Created = car.created.ToString(Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortDatePattern);
                carsDataGridObject.Series = seriesDataGridsList;
                carsDataGridObject.Series_Participations = seriesDataGridsList.Count;
                dgCarsList.Add(carsDataGridObject);
                dgCarsList.Sort((x, y) => x.CarName.CompareTo(y.CarName));
            }
            foreach(var serie in dgSeriesList)
            {
                var cars = dgCarsList.FindAll(c => c.Series.Any(s => s.SerieId == serie.SerieId));
                if (serie.Cars == null)
                {
                    serie.Cars = cars;
                }
                else
                {
                    serie.Cars.AddRange(cars);
                }
                serie.OwnCars = serie.Cars.Where(c => c.Owned == checksymbol).Count() + "/" + serie.Cars.Count();
            }
        }
        private void generateTrackView()
        {
            //Generate tracks information
            dgTracksList.Clear();
            foreach (var track in tracksList)
            {
                List<dgObjects.seriesDataGrid> seriesDataGridsList = new List<dgObjects.seriesDataGrid>();
                foreach (var tracksinseries in tracksInSeries)
                {
                    if (tracksinseries.track_id == track.track_id && !seriesDataGridsList.Any(s => s.SerieId == tracksinseries.series_id))
                    {

                        var seriesDataGridsObject = (from serie in dgSeriesList
                                                     where serie.SerieId == tracksinseries.series_id
                                                     select new dgObjects.seriesDataGrid()
                                                     {
                                                         SerieId = serie.SerieId,
                                                         Seriesimage = serie.Seriesimage,
                                                         SeriesName = serie.SeriesName,
                                                         Category = serie.Category,
                                                         Class = serie.Class,
                                                         License = serie.License,
                                                         Eligible = serie.Eligible,
                                                         Weeks = tracksinseries.week.ToString(),
                                                         Favourite = serie.Favourite,
                                                     }).FirstOrDefault();
                        seriesDataGridsList.Add(seriesDataGridsObject);
                    }
                    else if (tracksinseries.track_id == track.track_id && seriesDataGridsList.Any(s => s.SerieId == tracksinseries.series_id))
                    {
                        var _trackObj = seriesDataGridsList.FirstOrDefault(t => t.SerieId == tracksinseries.series_id);
                        if (_trackObj != null)
                        {
                            _trackObj.Weeks += ", " + tracksinseries.week;
                        }
                    }
                }

                dgObjects.tracksDataGrid newLayout = new dgObjects.tracksDataGrid();
                newLayout.Name = track.track_name;
                newLayout.Layoutname = track.config_name;
                newLayout.TrackImage = new Uri("file:///" + exePath + tracksLogo + track.track_id + ".png");
                newLayout.Corners = track.corners_per_lap;
                newLayout.Created = track.created.ToString(Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortDatePattern);
                newLayout.Length = isMetric ? Math.Round(track.track_config_length * 1.60934,3) : track.track_config_length; ;
                newLayout.Owned = User.track_packages.Any(p => p.package_id == track.package_id) ? checksymbol : "";
                newLayout.Pitlimit = isMetric ? Convert.ToInt32(track.pit_road_speed_limit * 1.60934) : track.pit_road_speed_limit;
                newLayout.Price = "$" + track.price.ToString();
                newLayout.PackageID = track.package_id;
                newLayout.TrackID = track.track_id;
                newLayout.Category = track.category;
                newLayout.Series = seriesDataGridsList;
                newLayout.Participations = seriesDataGridsList.Count;
                dgTracksList.Add(newLayout);
            }
            dgTrackLayoutList.Clear();
            foreach (dgObjects.tracksDataGrid track in dgTracksList)
            {
                if (!dgTrackLayoutList.Any(l => l.PackageID == track.PackageID))
                {
                    List<dgObjects.tracksDataGrid> layout = new List<dgObjects.tracksDataGrid>() { track };
                    dgObjects.tracksLayoutsDataGrid newTrack = new dgObjects.tracksLayoutsDataGrid();
                    newTrack.Layouts = layout;
                    newTrack.Created = track.Created;
                    newTrack.TrackImage = new Uri("file:///" + exePath + tracksLogo + track.TrackID + ".png");
                    newTrack.Name = track.Name;
                    newTrack.Owned = track.Owned;
                    newTrack.PackageID = track.PackageID;
                    newTrack.Price = track.Price;
                    newTrack.Layouts_count = 1;
                    newTrack.Participations = track.Participations;
                    newTrack.Favourite = favoutireTracks.Any(x => x.track_id == track.PackageID)? favsymbolSelected : favsymbolUnselected;
                    dgTrackLayoutList.Add(newTrack);
                }
                else if (dgTrackLayoutList.Any(l => l.PackageID == track.PackageID))
                {
                    var _trackObj = dgTrackLayoutList.FirstOrDefault(t => t.PackageID == track.PackageID);
                    if (_trackObj != null)
                    {
                        _trackObj.Layouts.Add(track);
                        _trackObj.Layouts_count++;
                        _trackObj.Participations += track.Participations;
                        _trackObj.Created = (DateTime.Parse(track.Created) < DateTime.Parse(_trackObj.Created)) ? track.Created : _trackObj.Created;
                    }
                    if (_trackObj.Name.Contains("[Retired]"))
                    {
                        _trackObj.Name = _trackObj.Layouts.FirstOrDefault(l => !l.Name.Contains("[Retired]")) != null? _trackObj.Layouts.FirstOrDefault(l => !l.Name.Contains("[Retired]")).Name: _trackObj.Name;
                    }
                }
            }
            dgTrackLayoutList.Sort((x, y) => x.Name.CompareTo(y.Name));
        }
        private void generatePurchaseGuideView()
        {
            //Generate Purchase information
            dgPurchaseGuideList.Clear();
            
            foreach(var item in tracksInSeries)
            {
                var track = tracksList.First(t => t.track_id == item.track_id);
                if (favoutireSeries.Any(f => f.series_id == item.series_id) && !User.track_packages.Any(p => p.package_id == track.package_id))
                {
                    List<dgObjects.seriesDataGrid> seriesDataGridsList = new List<dgObjects.seriesDataGrid>();
                    var seriesDataGridsObject = (from serie in dgSeriesList
                                                 where serie.SerieId == item.series_id
                                                 select new dgObjects.seriesDataGrid()
                                                 {
                                                     SerieId = serie.SerieId,
                                                     Seriesimage = serie.Seriesimage,
                                                     SeriesName = serie.SeriesName,
                                                     Category = serie.Category,
                                                     Class = serie.Class,
                                                     License = serie.License,
                                                     Eligible = serie.Eligible,
                                                     Weeks = item.week.ToString(),
                                                     Favourite = serie.Favourite,
                                                     OwnTracks = serie.Tracks.Count(t => t.Owned == checksymbol),
                                                 }).FirstOrDefault();
                    seriesDataGridsList.Add(seriesDataGridsObject);
                    //var serie = seriesList.First(s => s.series_id == item.series_id);
                    if (seriesDataGridsObject.OwnTracks < 8 || cbMenu2.IsChecked == false)
                    {
                        if (!dgPurchaseGuideList.Any(t => t.PackageID == track.package_id))
                        {
                            dgObjects.tracksDataGrid tracksDataGridObject = new dgObjects.tracksDataGrid();
                            tracksDataGridObject.Name = track.track_name;
                            tracksDataGridObject.TrackImage = new Uri("file:///" + exePath + tracksLogo + track.track_id + ".png");
                            tracksDataGridObject.Created = track.created.ToString(Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortDatePattern);
                            tracksDataGridObject.Price = "$" + track.price.ToString();
                            tracksDataGridObject.PackageID = track.package_id;
                            tracksDataGridObject.TrackID = track.track_id;
                            tracksDataGridObject.Category = track.category;
                            tracksDataGridObject.Series = seriesDataGridsList;
                            tracksDataGridObject.Length = isMetric ? Math.Round(track.track_config_length * 1.60934, 3) : track.track_config_length;
                            tracksDataGridObject.Participations = 1;
                            dgPurchaseGuideList.Add(tracksDataGridObject);
                        }
                        else
                        {
                            var obj = dgPurchaseGuideList.First(t => t.PackageID == track.package_id);
                            obj.Participations++;
                            obj.Series.Add(seriesDataGridsObject);
                        }
                    }
                }
            }
            dgPurchaseGuideList.Sort((x, y) => y.Participations.CompareTo(x.Participations));
            gridPurchaseGuide.ItemsSource = null;
            gridPurchaseGuide.ItemsSource = dgPurchaseGuideList;
            gridPurchaseGuide.UpdateLayout();
        }

        private void generateRaceView()
        {
            dgRaceOverviewList.Clear();
            foreach (var serie in dgSeriesList)
            {

                List<tracks.TracksInSeries> tracksinserie = new List<tracks.TracksInSeries>();
                tracksinserie = (tracksInSeries.FindAll(t => t.series_id == serie.SerieId)).ToList<tracks.TracksInSeries>();
                DateTime actualtime = DateTime.Now.ToUniversalTime();
                DateTime firstracetime;
                int daysoffset = 0;
                int repeattimes;
                bool over = false;
                tracks.TracksInSeries actualweekofserie = new tracks.TracksInSeries();
                tracksinserie.Sort((x, y) => x.SeasonSchedule.start_date.CompareTo(y.SeasonSchedule.start_date));
                tracks.TracksInSeries lastseason = new tracks.TracksInSeries();
                tracks.TracksInSeries nextseason = new tracks.TracksInSeries();

                foreach (var season in tracksinserie)
                {
                    if (lastseason.SeasonSchedule == null)
                    {
                        lastseason = season;
                    }
                    DateTime seasonStartDate = Convert.ToDateTime(season.SeasonSchedule.start_date);
                    if (seasonStartDate <= actualtime.Date && seasonStartDate > Convert.ToDateTime(lastseason.SeasonSchedule.start_date))
                    {
                        lastseason = season;
                    }
                    if (seasonStartDate > actualtime.Date && seasonStartDate > Convert.ToDateTime(lastseason.SeasonSchedule.start_date) && (nextseason.SeasonSchedule == null || seasonStartDate < Convert.ToDateTime(nextseason.SeasonSchedule.start_date)))
                    {
                        nextseason = season;
                    }
                }
                if (tracksinserie[0].SeasonSchedule.race_time_descriptors[0].repeating)
                {
                    int index = tracksinserie[lastseason.week - 1].SeasonSchedule.race_time_descriptors[0].day_offset.Count();
                    daysoffset = tracksinserie[lastseason.week - 1].SeasonSchedule.race_time_descriptors[0].day_offset[index - 1];
                    if (Convert.ToDateTime(lastseason.SeasonSchedule.start_date).AddDays(daysoffset) >= actualtime.Date)
                    {
                        actualweekofserie = lastseason;
                    }
                    else if (nextseason.SeasonSchedule != null)
                    {
                        actualweekofserie = nextseason;
                    }
                    else
                    {
                        actualweekofserie = lastseason;
                        over = true;
                    }
                    firstracetime = Convert.ToDateTime(actualweekofserie.SeasonSchedule.race_time_descriptors[0].first_session_time);
                    repeattimes = actualweekofserie.SeasonSchedule.race_time_descriptors[0].repeat_minutes;
                }
                else
                {
                    if (Convert.ToDateTime(lastseason.SeasonSchedule.start_date) <= actualtime.Date && Convert.ToDateTime(lastseason.SeasonSchedule.race_time_descriptors[0].session_times[lastseason.SeasonSchedule.race_time_descriptors[0].session_times.Count - 1]) >= actualtime.Date)
                    {
                        actualweekofserie = lastseason;
                    }
                    else if (nextseason.SeasonSchedule != null)
                    {
                        actualweekofserie = nextseason;
                    }
                    else
                    {
                        actualweekofserie = lastseason;
                        over = true;
                    }
                    var nextrace = actualweekofserie.SeasonSchedule.race_time_descriptors[0].session_times.FirstOrDefault(s => s >= actualtime);
                    firstracetime = nextrace;
                    repeattimes = actualweekofserie.SeasonSchedule.race_time_descriptors[0].repeat_minutes;
                }
                foreach (var track in serie.Tracks)
                {
                    if (track.Week == actualweekofserie.week && !over)
                    {
                        track.WeekActive = true;
                    }
                    else
                    {
                        track.WeekActive = false;
                    }
                }
                var racetime = helper.getNextRace(DateTime.SpecifyKind(firstracetime, DateTimeKind.Utc), repeattimes, actualtime).ToLocalTime();
                dgObjects.RaceOverviewDataGrid _raceobj = new dgObjects.RaceOverviewDataGrid();
                var tr = tracksList.FirstOrDefault(t => t.track_id == actualweekofserie.track_id);
                List<dgObjects.carsDataGrid> cars = new List<dgObjects.carsDataGrid>();
                foreach (var car in carsInSeries.Where(s => s.series_id == serie.SerieId))
                {
                    cars.Add(dgCarsList.First(c => c.CarId == car.car_id));
                }
                _raceobj.Cars = cars;
                _raceobj.Track = dgTrackLayoutList.FirstOrDefault(t => t.PackageID == tr.package_id);
                _raceobj.Tracks = serie.Tracks;
                _raceobj.SerieId = serie.SerieId;
                _raceobj.Seriesimage = serie.Seriesimage;
                _raceobj.SerieRaceLength = actualweekofserie.SeasonSchedule.race_lap_limit != null ? actualweekofserie.SeasonSchedule.race_lap_limit.ToString() + " Laps" : actualweekofserie.SeasonSchedule.race_time_limit.ToString() + " Min";
                _raceobj.SeriesName = serie.SeriesName;
                _raceobj.TrackName = tr.track_name;
                _raceobj.Serie = serie;
                _raceobj.TracksOwned = serie.Tracks.Count(t => t.Owned == checksymbol).ToString() + "/" + serie.Tracks.Count();
                _raceobj.NextRace = racetime;

                _raceobj.Timer = RaceAlarms.Any(a => a.SerieId == serie.SerieId) ? alarmClockSymbol : clockSymbol;
                if (racetime.Date == DateTime.Now.Date && racetime > DateTime.Now && !over)
                {
                    _raceobj.NextRaceTime = racetime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern);

                }
                else if (racetime.Year == DateTime.Parse("01.01.0001").Year || over)
                {
                    _raceobj.NextRaceTime = "Season is over.";
                }
                else
                {
                    _raceobj.NextRaceTime = racetime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + ", " + CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern);
                }
                if(actualweekofserie.SeasonSchedule.race_time_descriptors[0].session_times != null)
                {
                    List<DateTime> sessionTimes = new List<DateTime>();
                    foreach(DateTime x in actualweekofserie.SeasonSchedule.race_time_descriptors[0].session_times)
                    {
                        sessionTimes.Add(DateTime.SpecifyKind(x, DateTimeKind.Utc).ToLocalTime());
                    }
                    _raceobj.SessionTimes = String.Join(", ", sessionTimes);
                }
                else
                {
                    _raceobj.SessionTimes = null;
                }
                _raceobj.FirstSessionTime = firstracetime.ToLocalTime().ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern);
                if(repeattimes > 0)
                {
                    if(repeattimes >= 60)
                    {
                        _raceobj.Repeating = repeattimes >= 60 ? "Every " + repeattimes / 60 + " hours" : "Every " + repeattimes / 60 + " hour";
                    }
                    else
                    {
                        _raceobj.Repeating = "Every " + repeattimes + " minutes" ;
                    }
                }
                else
                {
                    _raceobj.Repeating = "Set times";
                }
                _raceobj.TrackOwned = User.track_packages.Any(t => t.package_id == tr.package_id) ? true : false;
                dgRaceOverviewList.Add(_raceobj);
            }
            dgRaceOverviewList.Sort((x, y) => x.NextRace.CompareTo(y.NextRace));
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {

                gridRaces.ItemsSource = dgRaceOverviewList;

            }));
            filterRaces();
        }
        private void filterRaces()
        {
            List<dgObjects.RaceOverviewDataGrid> dgFilteredRaces = new List<dgObjects.RaceOverviewDataGrid>();
            dgFilteredRaces.Clear();
            //cbFilterInOfficial; cbFilterOfficial; cbFilterOpenSetup; cbFilterFixedSetup; cbFilterRoad; cbFilterOval; cbFilterDirt; cbFilterDirtOval; cbFilterR; cbFilterD; cbFilterC; cbFilterB; cbFilterA; cbFilterP
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => {
                foreach (var race in dgRaceOverviewList)
                {
                    bool category = false;
                    bool official = false;
                    bool serieclass = false;
                    bool favs = false;
                    bool own = false;
                    bool over = false;
                    List<int> usercars = new List<int>();
                    foreach (var content in User.car_packages)
                    {
                        foreach(var id in content.content_ids)
                        {
                            usercars.Add(id);
                        }
                    }
                    if(cbFilterSeasonOver.IsChecked == true && race.NextRaceTime == "Season is over." ||
                     race.NextRaceTime != "Season is over.")
                    {
                        over = true;
                    }
                    if ((cbFilterDirtOval.IsChecked == true && race.Serie.Category == "dirtoval") ||
                        (cbFilterOval.IsChecked == true && race.Serie.Category == "oval") ||
                        (cbFilterRoad.IsChecked == true && race.Serie.Category == "road") ||
                        (cbFilterDirt.IsChecked == true && race.Serie.Category == "dirt"))
                    {
                        category = true;
                    }
                    if (((cbFilterOfficial.IsChecked == true && race.Serie.Season.official == true) ||
                        (cbFilterInOfficial.IsChecked == true && race.Serie.Season.official == false)) &&
                        ((cbFilterOpenSetup.IsChecked == true && race.Serie.Season.fixed_setup == false) ||
                        (cbFilterFixedSetup.IsChecked == true && race.Serie.Season.fixed_setup == true)))
                    {
                        official = true;
                    }

                    if((cbFilterR.IsChecked == true && race.Serie.Class == "R") ||
                        (cbFilterD.IsChecked == true && race.Serie.Class == "D") ||
                        (cbFilterC.IsChecked == true && race.Serie.Class == "C") ||
                        (cbFilterB.IsChecked == true && race.Serie.Class == "B") ||
                        (cbFilterA.IsChecked == true && race.Serie.Class == "A") ||
                        (cbFilterP.IsChecked == true && race.Serie.Class == "Pro"))
                    {
                        serieclass = true;
                    }
                    if(
                        ((cbFilterFavSeries.IsChecked == true && race.Serie.Favourite == favsymbolSelected || cbFilterFavSeries.IsChecked == false) &&
                        (cbFilterFavTracks.IsChecked == true && race.Track.Favourite == favsymbolSelected || cbFilterFavTracks.IsChecked == false) &&
                        (cbFilterFavCars.IsChecked == true && race.Cars.Any(c => favoutireCars.Any(u => u.car_id == c.CarId)) || cbFilterFavCars.IsChecked == false) ||
                        (cbFilterFavSeries.IsChecked == false && cbFilterFavTracks.IsChecked == false && cbFilterFavCars.IsChecked == false ))
                      )
                    {
                        favs = true;
                    }

                    if ((cbFilterOwnTracks.IsChecked == true && race.TrackOwned == true) || (cbFilterOwnCars.IsChecked == true && race.Cars.Any(c => usercars.Any(u => u == c.CarId))) || (cbFilterOwnTracks.IsChecked == false && cbFilterOwnCars.IsChecked == false))
                    {
                        own = true;
                    }
                        if (category && official && serieclass && favs && own && over)
                    {
                        dgFilteredRaces.Add(race);
                    }
                }
                gridRaces.ItemsSource = null;
                gridRaces.ItemsSource = dgFilteredRaces;
                gridRaces.UpdateLayout();
            }));
        }
        private void generateAutoStartView()
        {

            dgAutoStartList.Clear();
            if (autoStartApps.Programs != null)
            {
                foreach (var prog in autoStartApps.Programs)
                {
                    dgObjects.autoStartDataGrid autoStartData = new dgObjects.autoStartDataGrid();
                    autoStartData.ID = prog.ID;
                    autoStartData.Path = prog.Path;

                    var sysicon = System.Drawing.Icon.ExtractAssociatedIcon(prog.Path);
                    var bmpSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(sysicon.Handle, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                    sysicon.Dispose();
                    autoStartData.Icon = bmpSrc;
                    autoStartData.Name = FileVersionInfo.GetVersionInfo(prog.Path).ProductName != null ? FileVersionInfo.GetVersionInfo(prog.Path).ProductName : FileVersionInfo.GetVersionInfo(prog.Path.ToString()).FileName;
                    dgAutoStartList.Add(autoStartData);
                }
            }
            gridAutoStart.ItemsSource = null;
            gridAutoStart.ItemsSource = dgAutoStartList;
        }
        private void clearDetails()
        {
            lblDetails1.Content = "";
            lblDetails2.Content = "";
            lblDetails3.Content = "";
            lblDetails4.Content = "";
            lblDetails5.Content = "";
            lblDetails6.Content = "";
            tbDetail1.Text = "";
            tbDetail2.Text = "";
            tbDetail3.Text = "";
            tbDetail4.Text = "";
            tbDetail5.Text = "";
            tbDetail6.Text = "";
            imDetailImage.Source = null;
        }
        private void scrollDataGridIntoView(object sender)
        {
            if (((DataGrid)sender).SelectedItem != null)
            {
                ((DataGrid)sender).ScrollIntoView(((DataGrid)sender).Items[((DataGrid)sender).Items.Count - 1]);
                ((DataGrid)sender).UpdateLayout();
                ((DataGrid)sender).ScrollIntoView(((DataGrid)sender).SelectedItem);
            }
        }
        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((DataGrid)sender).SelectedItem != null || ((DataGrid)sender).SelectedIndex != -1)
            {
                switch (((DataGrid)sender).Name)
                {
                    case "gridCars":
                        gridCarDetail.ItemsSource = ((RCRPlanner.dgObjects.carsDataGrid)((DataGrid)sender).SelectedItem).Series;
                        tbDetail3.Text = ((RCRPlanner.dgObjects.carsDataGrid)((DataGrid)sender).SelectedItem).Created;
                        lblDetails3.Content = "Created:";
                        tbDetail2.Text = ((RCRPlanner.dgObjects.carsDataGrid)((DataGrid)sender).SelectedItem).Horsepower.ToString();
                        lblDetails2.Content = "Horsepower:";
                        tbDetail1.Text = ((RCRPlanner.dgObjects.carsDataGrid)((DataGrid)sender).SelectedItem).Weight.ToString();
                        lblDetails1.Content = "Weight:";
                        try
                        {
                            imDetailImage.Source = new BitmapImage(((RCRPlanner.dgObjects.carsDataGrid)((DataGrid)sender).SelectedItem).CarImage);
                        }
                        catch { imDetailImage.Source = null; }
                        break;
                    case "gridTracksLayout":
                        if (selectedTrack == -1 || selectedTrack != ((RCRPlanner.dgObjects.tracksLayoutsDataGrid)((DataGrid)sender).SelectedItem).PackageID)
                        {
                            clearDetails();
                            selectedTrack = ((RCRPlanner.dgObjects.tracksLayoutsDataGrid)((DataGrid)sender).SelectedItem).PackageID;
                            scrollDataGridIntoView(sender);
                            List<dgObjects.seriesDataGrid> seriesOnTrack = new List<dgObjects.seriesDataGrid>();
                            foreach (var layout in ((RCRPlanner.dgObjects.tracksLayoutsDataGrid)((DataGrid)sender).SelectedItem).Layouts)
                            {
                                foreach (var _serie in layout.Series)
                                {
                                    _serie.Category = layout.Layoutname;
                                    seriesOnTrack.Add(_serie);
                                }
                            }
                            gridTrackDetail.ItemsSource = seriesOnTrack;
                            //gridTrackDetail.ItemsSource = ((RCRPlanner.dgObjects.tracksLayoutsDataGrid)((DataGrid)sender).SelectedItem).Layouts;
                        }
                        break;
                    case "gridSeries":
                        scrollDataGridIntoView(sender);
                        break;
                    case "gridTracks":
                        tbDetail3.Text = ((dgObjects.tracksDataGrid)((DataGrid)sender).SelectedItem).Corners.ToString();
                        lblDetails3.Content = "Corners:";
                        tbDetail2.Text = ((dgObjects.tracksDataGrid)((DataGrid)sender).SelectedItem).Pitlimit.ToString() + (isMetric ? "km/h" : "mph");
                        lblDetails2.Content = "Pitlimit:";
                        tbDetail1.Text = ((dgObjects.tracksDataGrid)((DataGrid)sender).SelectedItem).Length.ToString() + (isMetric ? "km" : "m");
                        lblDetails1.Content = "Lenght:";
                        tbDetail4.Text = ((dgObjects.tracksDataGrid)((DataGrid)sender).SelectedItem).Created.ToString();
                        lblDetails4.Content = "Created:";
                        try
                        {
                            imDetailImage.Source = new BitmapImage(((dgObjects.tracksDataGrid)((DataGrid)sender).SelectedItem).TrackImage);
                        }
                        catch { imDetailImage.Source = null; }
                        break;
                    case "gridTracksinSerie":
                        if (true)
                        {
                            DataGridRow row = (DataGridRow)(gridRaces.ItemContainerGenerator.ContainerFromItem(gridRaces.SelectedItem));
                            DataGridDetailsPresenter presenter = FindVisualChild<DataGridDetailsPresenter>(row);
                            DataTemplate template = presenter.ContentTemplate;
                            Image Trackimage = (Image)template.FindName("imRaceTrack", presenter);
                            try
                            {
                                Trackimage.Source = new BitmapImage(((dgObjects.tracksDataGrid)((DataGrid)sender).SelectedItem).TrackImage);
                            }
                            catch { Trackimage.Source = null; }
                        }
                        break;
                    case "gridSeriesTrack":
                        if (true)
                        {
                            DataGridRow row = (DataGridRow)(gridSeries.ItemContainerGenerator.ContainerFromItem(gridSeries.SelectedItem));
                            DataGridDetailsPresenter presenter = FindVisualChild<DataGridDetailsPresenter>(row);
                            DataTemplate template = presenter.ContentTemplate;
                            Image Trackimage = (Image)template.FindName("imRaceTrack", presenter);
                            try
                            {
                                Trackimage.Source = new BitmapImage(((dgObjects.tracksDataGrid)((DataGrid)sender).SelectedItem).TrackImage);
                            }
                            catch { Trackimage.Source = null;}
                        }
                        break;

                }
            }
        }

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }
            Control control = sender as Control;
            if (control == null)
            {
                return;
            }
            e.Handled = true;
            var wheelArgs = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = MouseWheelEvent,
                Source = control
            };
            var parent = control.Parent as UIElement;
            parent?.RaiseEvent(wheelArgs);
        }

        private void btnLoadCars_Click(object sender, RoutedEventArgs e)
        {
            activeGrid = "gridCars";
            btnMenu1.Visibility = Visibility.Hidden;
            cbMenu2.Visibility = Visibility.Hidden;
            dpMenu2.Visibility = Visibility.Hidden;
            lbMenu2.Visibility = Visibility.Hidden;
            cbMenu3.Visibility = Visibility.Hidden;
            dpMenu3.Visibility = Visibility.Hidden;
            cbMenu4.Visibility = Visibility.Hidden;
            dpMenu4.Visibility = Visibility.Hidden;
            cbMenu5.Visibility = Visibility.Hidden;
            dpMenu5.Visibility = Visibility.Hidden;
            tbMenu6.Visibility = Visibility.Hidden;
            btnMenu6.Visibility = Visibility.Hidden;
            clearDetails();
            stackPanelMenuClose_MouseDown(null, null);
            switchMainGridVisibility(new List<System.Windows.Controls.DataGrid>{ gridCars, gridCarDetail } , true);

        }

        private void btnLoadTracks_Click(object sender, RoutedEventArgs e)
        {
            activeGrid = "gridTracksLayout";
            btnMenu1.Visibility = Visibility.Hidden;
            cbMenu2.Visibility = Visibility.Hidden;
            dpMenu2.Visibility = Visibility.Hidden;
            lbMenu2.Visibility = Visibility.Hidden;
            cbMenu3.Visibility = Visibility.Hidden;
            dpMenu3.Visibility = Visibility.Hidden;
            cbMenu4.Visibility = Visibility.Hidden;
            dpMenu4.Visibility = Visibility.Hidden;
            cbMenu5.Visibility = Visibility.Hidden;
            dpMenu5.Visibility = Visibility.Hidden;
            tbMenu6.Visibility = Visibility.Hidden;
            btnMenu6.Visibility = Visibility.Hidden;
            clearDetails();
            stackPanelMenuClose_MouseDown(null, null);
            switchMainGridVisibility(new List<System.Windows.Controls.DataGrid> { gridTracksLayout, gridTrackDetail }, true);
        }

        private void btnLoadPurchase_Click(object sender, RoutedEventArgs e)
        {
            activeGrid = "gridPurchaseGuide";
            btnMenu1.Visibility = Visibility.Visible;
            btnMenu1.Content = "Reload";
            cbMenu2.Visibility = Visibility.Visible;
            cbMenu2.Content = "only if owned track in serie <8";
            dpMenu2.Visibility = Visibility.Hidden;
            lbMenu2.Visibility = Visibility.Hidden;
            cbMenu3.Visibility = Visibility.Hidden;
            dpMenu3.Visibility = Visibility.Hidden;
            cbMenu4.Visibility = Visibility.Hidden;
            dpMenu4.Visibility = Visibility.Hidden;
            cbMenu5.Visibility = Visibility.Hidden;
            dpMenu5.Visibility = Visibility.Hidden;
            tbMenu6.Visibility = Visibility.Hidden;
            btnMenu6.Visibility = Visibility.Hidden;
            generatePurchaseGuideView();
            stackPanelMenuClose_MouseDown(null, null);
            switchMainGridVisibility(new List<System.Windows.Controls.DataGrid> { gridPurchaseGuide }, false);
        }

        private void btnLoadSeries_Click(object sender, RoutedEventArgs e)
        {
            activeGrid = "gridSeries";
            btnMenu1.Visibility = Visibility.Hidden;
            cbMenu2.Visibility = Visibility.Hidden;
            dpMenu2.Visibility = Visibility.Hidden;
            lbMenu2.Visibility = Visibility.Hidden;
            dpMenu3.Visibility = Visibility.Hidden;
            cbMenu3.Visibility = Visibility.Hidden;
            dpMenu4.Visibility = Visibility.Hidden;
            cbMenu4.Visibility = Visibility.Hidden;
            cbMenu5.Visibility = Visibility.Hidden;
            dpMenu5.Visibility = Visibility.Hidden;
            tbMenu6.Visibility = Visibility.Hidden;
            btnMenu6.Visibility = Visibility.Hidden;
            stackPanelMenuClose_MouseDown(null, null);
            switchMainGridVisibility(new List<System.Windows.Controls.DataGrid> { gridSeries }, false);
        }

        private void btnLoadRaces_Click(object sender, RoutedEventArgs e)
        {
            activeGrid = "gridRaces";
            btnMenu1.Visibility = Visibility.Visible;
            btnMenu1.Content = "Filter";
            

            cbMenu2.Visibility = Visibility.Hidden;
            dpMenu2.Visibility = Visibility.Visible;
            ddMenu2.Items.Clear();
            ddMenu2.Items.Add(new ComboBoxItem() { Content = "No Offset", Name = "i0" });
            ddMenu2.Items.Add(new ComboBoxItem() { Content = "1 Minute", Name = "i1" });
            ddMenu2.Items.Add(new ComboBoxItem() { Content = "2 Minutes", Name = "i2" });
            ddMenu2.Items.Add(new ComboBoxItem() { Content = "5 Minutes", Name = "i5" });
            ddMenu2.Items.Add(new ComboBoxItem() { Content = "10 Minutes", Name = "i10" });
            ddMenu2.Items.Add(new ComboBoxItem() { Content = "15 Minutes", Name = "i15" });

            ddMenu2.SelectedIndex = Properties.Settings.Default.defaultTimer;
            lbMenu2.Visibility = Visibility.Visible;
            lbMenu2.Content = "Alarm offset:";

            cbMenu3.Visibility = Visibility.Hidden;
            dpMenu3.Visibility = Visibility.Hidden;
            cbMenu4.Visibility = Visibility.Hidden;
            cbMenu5.Visibility = Visibility.Hidden;
            dpMenu4.Visibility = Visibility.Hidden;
            dpMenu5.Visibility = Visibility.Hidden;
            tbMenu6.Visibility = Visibility.Visible;
            btnMenu6.Visibility = Visibility.Hidden;

            stackPanelMenuClose_MouseDown(null, null);
            generateRaceView();
            switchMainGridVisibility(new List<System.Windows.Controls.DataGrid> { gridRaces }, false);
        }
        private void btnPartStats_Click(object sender, RoutedEventArgs e)
        {
            activeGrid = "gridPartStat";
            dpMenu2.Visibility = Visibility.Visible;
            lbMenu2.Visibility = Visibility.Visible;
            dpMenu3.Visibility = Visibility.Visible;
            dpMenu4.Visibility = Visibility.Visible;
            dpMenu5.Visibility = Visibility.Visible;
            btnMenu1.Visibility = Visibility.Visible;
            btnMenu1.Content = "Loading...";
            btnMenu1.IsEnabled = false;
            ddMenu2.Items.Clear();
            ddMenu3.Items.Clear();
            ddMenu4.Items.Clear();
            lbMenu2.Content = "Serie:";
            lbMenu3.Content = "Year:";
            lbMenu4.Content = "Season:";
            lbMenu5.Content = "Week:";
            ddMenu4.Items.Add(new ComboBoxItem() { Content = "S1", Name = "s1" });
            ddMenu4.Items.Add(new ComboBoxItem() { Content = "S2", Name = "s2" });
            ddMenu4.Items.Add(new ComboBoxItem() { Content = "S3", Name = "s3" });
            ddMenu4.Items.Add(new ComboBoxItem() { Content = "S4", Name = "s4" });
            ddMenu5.Items.Clear();
            foreach (var serie in dgSeriesList)
            {
                ddMenu2.Items.Add(new ComboBoxItem() { Content = serie.SeriesName, Name = "s" + serie.SerieId.ToString() });
            }
            for(int i=2007; i <= DateTime.Now.Year; i++)
            {
                ddMenu3.Items.Add(new ComboBoxItem() { Content = i, Name = "y" + i });
            }
            ddMenu3.SelectedIndex = ddMenu3.Items.IndexOf(ddMenu3.Items.OfType<ComboBoxItem>().FirstOrDefault(x => x.Content.ToString() == DateTime.Now.Year.ToString()));


            cbMenu2.Visibility = Visibility.Hidden;
            cbMenu3.Visibility = Visibility.Hidden;
            cbMenu4.Visibility = Visibility.Hidden;
            cbMenu5.Visibility = Visibility.Hidden;
            tbMenu6.Visibility = Visibility.Hidden;
            btnMenu6.Visibility = Visibility.Hidden;
            stackPanelMenuClose_MouseDown(null, null);
            switchMainGridVisibility(new List<System.Windows.Controls.DataGrid> { gridPartStat }, false);
            btnMenu1.Content = "Get stat";
            btnMenu1.IsEnabled = true;
        }
        private void btniRatingStats_Click(object sender, RoutedEventArgs e)
        {
            activeGrid = "gridiRatingStat";
            dpMenu2.Visibility = Visibility.Visible;
            lbMenu2.Visibility = Visibility.Visible;
            dpMenu3.Visibility = Visibility.Visible;
            dpMenu4.Visibility = Visibility.Visible;
            dpMenu5.Visibility = Visibility.Hidden;
            btnMenu1.Visibility = Visibility.Visible;
            btnMenu1.Content = "Loading...";
            btnMenu1.IsEnabled = false;
            ddMenu2.Items.Clear();
            ddMenu3.Items.Clear();
            ddMenu4.Items.Clear();
            lbMenu2.Content = "Serie:";
            lbMenu3.Content = "Season:";
            lbMenu4.Content = "Car class:";
            foreach (var serie in dgSeriesList)
            {
                ddMenu2.Items.Add(new ComboBoxItem() { Content = serie.SeriesName, Name = "s" + serie.SerieId.ToString() });
            }
           

            cbMenu2.Visibility = Visibility.Hidden;
            cbMenu3.Visibility = Visibility.Hidden;
            cbMenu4.Visibility = Visibility.Hidden;
            cbMenu5.Visibility = Visibility.Hidden;
            tbMenu6.Visibility = Visibility.Hidden;
            btnMenu6.Visibility = Visibility.Hidden;
            stackPanelMenuClose_MouseDown(null, null);
            switchMainGridVisibility(new List<System.Windows.Controls.DataGrid> { gridiRatingStat }, false);
            btnMenu1.Content = "Get stat";
            btnMenu1.IsEnabled = true;
        }
        private void btnLoadAutoStart_Click(object sender, RoutedEventArgs e)
        {
            activeGrid = "gridAutoStart";
            btnMenu1.Visibility = Visibility.Visible;
            btnMenu1.Content = "Add program";

            cbMenu2.Visibility = Visibility.Visible;
            cbMenu2.Content = "Auto start progams on launch?";
            cbMenu2.IsChecked = autoStartApps.Active;
            dpMenu2.Visibility = Visibility.Hidden;
            lbMenu2.Visibility = Visibility.Hidden;

            cbMenu3.Visibility = Visibility.Visible;
            cbMenu3.Content = "Start programs minimized?";
            cbMenu3.IsChecked = autoStartApps.Minimized;
            dpMenu3.Visibility = Visibility.Hidden;

            cbMenu4.Visibility = Visibility.Visible;
            cbMenu4.Content = "Kill programs on close?";
            cbMenu4.IsChecked = autoStartApps.Kill;
            dpMenu4.Visibility = Visibility.Hidden;

            cbMenu5.Visibility = Visibility.Visible;
            cbMenu5.Content = "Kill programs by Name?";
            cbMenu5.ToolTip = "Can lead to data loss, since all programs with the specified program name will be closed!";
            cbMenu5.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Yellow"));
            cbMenu5.IsChecked = autoStartApps.KillByName;
            dpMenu5.Visibility = Visibility.Hidden;

            tbMenu6.Visibility = Visibility.Hidden;
            btnMenu6.Visibility = Visibility.Hidden;
            clearDetails();
            stackPanelMenuClose_MouseDown(null, null);
            generateAutoStartView();
            switchMainGridVisibility(new List<System.Windows.Controls.DataGrid> { gridAutoStart }, true);
        }
        private void btnstartPrograms_Click(object sender, RoutedEventArgs e)
        {
            startPrograms();
        }
        private void gridSeriesFavoutire_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (((System.Windows.Controls.TextBlock)sender).Text != favsymbolSelected)
            {
                dgSeriesList.First(r => r.SerieId == ((RCRPlanner.dgObjects.seriesDataGrid)((System.Windows.FrameworkElement)sender).DataContext).SerieId).Favourite = favsymbolSelected;
                favoutireSeries.Add(new memberInfo.FavoutireSeries { series_id = ((RCRPlanner.dgObjects.seriesDataGrid)((System.Windows.FrameworkElement)sender).DataContext).SerieId });
            }
            else
            {
                dgSeriesList.First(r => r.SerieId == ((RCRPlanner.dgObjects.seriesDataGrid)((System.Windows.FrameworkElement)sender).DataContext).SerieId).Favourite = favsymbolUnselected;
                var fav = favoutireSeries.FirstOrDefault(i => i.series_id == ((RCRPlanner.dgObjects.seriesDataGrid)((System.Windows.FrameworkElement)sender).DataContext).SerieId);
                favoutireSeries.Remove(fav);
            }
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => {
                CollectionViewSource.GetDefaultView(gridSeries.ItemsSource).Refresh();
            }));
            helper.SerializeObject<List<memberInfo.FavoutireSeries>>(favoutireSeries, exePath + favSeriesfile);
        }
        private void gridCarsFavoutire_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (((System.Windows.Controls.TextBlock)sender).Text != favsymbolSelected)
            {
                dgCarsList.First(r => r.CarId == ((RCRPlanner.dgObjects.carsDataGrid)((System.Windows.FrameworkElement)sender).DataContext).CarId).Favourite = favsymbolSelected;
                favoutireCars.Add(new memberInfo.FavoutireCars { car_id = ((RCRPlanner.dgObjects.carsDataGrid)((System.Windows.FrameworkElement)sender).DataContext).CarId });
            }
            else
            {
                dgCarsList.First(r => r.CarId == ((RCRPlanner.dgObjects.carsDataGrid)((System.Windows.FrameworkElement)sender).DataContext).CarId).Favourite = favsymbolUnselected;
                var fav = favoutireCars.FirstOrDefault(i => i.car_id == ((RCRPlanner.dgObjects.carsDataGrid)((System.Windows.FrameworkElement)sender).DataContext).CarId);
                favoutireCars.Remove(fav);
            }
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => {
                CollectionViewSource.GetDefaultView(gridCars.ItemsSource).Refresh();
            }));
            helper.SerializeObject<List<memberInfo.FavoutireCars>>(favoutireCars, exePath + favCarsfile);
        }

        private void gridTracksFavoutire_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (((System.Windows.Controls.TextBlock)sender).Text != favsymbolSelected)
            {
                dgTrackLayoutList.First(r => r.PackageID == ((RCRPlanner.dgObjects.tracksLayoutsDataGrid)((System.Windows.FrameworkElement)sender).DataContext).PackageID).Favourite = favsymbolSelected;
                favoutireTracks.Add(new memberInfo.FavoutireTracks { track_id = ((RCRPlanner.dgObjects.tracksLayoutsDataGrid)((System.Windows.FrameworkElement)sender).DataContext).PackageID });
            }
            else
            {
                dgTrackLayoutList.First(r => r.PackageID == ((RCRPlanner.dgObjects.tracksLayoutsDataGrid)((System.Windows.FrameworkElement)sender).DataContext).PackageID).Favourite = favsymbolUnselected;
                var fav = favoutireTracks.FirstOrDefault(i => i.track_id == ((RCRPlanner.dgObjects.tracksLayoutsDataGrid)((System.Windows.FrameworkElement)sender).DataContext).PackageID);
                favoutireTracks.Remove(fav);
            }
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => {
                CollectionViewSource.GetDefaultView(gridTracksLayout.ItemsSource).Refresh();
            }));
            helper.SerializeObject<List<memberInfo.FavoutireTracks>>(favoutireTracks, exePath + favTracksfile);
        }

        private void gridRacesAlarm_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int offset = Convert.ToInt32(((System.Windows.FrameworkElement)ddMenu2.SelectedValue).Name.ToString().Replace("i", ""));
            if (((System.Windows.Controls.TextBlock)sender).Text != alarmClockSymbol)
            {
                dgRaceOverviewList.First(r => r.SerieId == ((RCRPlanner.dgObjects.RaceOverviewDataGrid)((System.Windows.FrameworkElement)sender).DataContext).SerieId).Timer = alarmClockSymbol;
                RaceAlarms.Add(new Alarms { SerieId = ((RCRPlanner.dgObjects.RaceOverviewDataGrid)((System.Windows.FrameworkElement)sender).DataContext).SerieId, AlarmTime = Convert.ToDateTime(((RCRPlanner.dgObjects.RaceOverviewDataGrid)((System.Windows.FrameworkElement)sender).DataContext).NextRaceTime).AddMinutes(-offset) });

            }
            else
            {
                dgRaceOverviewList.First(r => r.SerieId == ((RCRPlanner.dgObjects.RaceOverviewDataGrid)((System.Windows.FrameworkElement) sender).DataContext).SerieId).Timer = clockSymbol;
                RaceAlarms.Remove(RaceAlarms.FirstOrDefault(i => i.SerieId == ((RCRPlanner.dgObjects.RaceOverviewDataGrid)((System.Windows.FrameworkElement)sender).DataContext).SerieId));
            }
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => {
                CollectionViewSource.GetDefaultView(gridRaces.ItemsSource).Refresh();
            }));
        }

        private void stackPanelFilterClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            resize_Grid(gridFilter, "height", 0, moveAnimationDuration);
        }
        private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                {
                    return (childItem)child;
                }
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void btnFilterSave_Click(object sender, RoutedEventArgs e)
        {
            List<string> filter = new List<string>();
            var cbFilter = FindVisualChildren<CheckBox>(gridFilter);
            foreach (CheckBox cb in cbFilter)
            {
                if (cb.IsChecked == true)
                {
                    filter.Add(cb.Name);
                }
            }
            Properties.Settings.Default.filter = String.Join(";", filter);
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
            filterRaces();
        }

        private void btnProfileReset_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.username = "";
            Properties.Settings.Default.password = "";
            Properties.Settings.Default.filter = @defaultfilter;
            Properties.Settings.Default.Save();
        }
        private async void btnProfileUpdate_Click(object sender, RoutedEventArgs e)
        {
            await fData.getGithubLastRelease(Properties.Settings.Default.updateURL.ToString(), version.ToString());
        }
        private void gridAutoStartRemove_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (((System.Windows.Controls.TextBlock)sender).Text == unchecksymbol)
            {
                var rev = autoStartApps.Programs.FirstOrDefault(i => i.ID == ((RCRPlanner.dgObjects.autoStartDataGrid)((System.Windows.FrameworkElement)sender).DataContext).ID);
                autoStartApps.Programs.Remove(rev);
            }
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => {
                generateAutoStartView();
                CollectionViewSource.GetDefaultView(gridAutoStart.ItemsSource).Refresh();
            }));
            helper.SerializeObject<autoStart.Root>(autoStartApps, exePath + autostartfile);
        }
        private async void btnMenu1_Click(object sender, RoutedEventArgs e)
        {
            switch (activeGrid)
            {
                case "gridAutoStart":
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = "Programs (*.exe)| *.exe";
                    string path = null;
                    if (openFileDialog.ShowDialog() == true)
                    {
                        path = openFileDialog.FileName;
                    }
                    if (path != null)
                    {
                        var prog = new autoStart.Programs { ID = autoStartApps.Programs != null ? autoStartApps.Programs.Count + 1 : 1, Path = path };
                        List<autoStart.Programs> progs = new List<autoStart.Programs>();
                        progs.Add(prog);
                        if (autoStartApps.Programs == null)
                        {
                            autoStartApps.Programs = progs;
                        }
                        else
                        {
                            autoStartApps.Programs.Add(prog);
                        }
                        helper.SerializeObject<autoStart.Root>(autoStartApps, exePath + autostartfile);
                        generateAutoStartView();
                    }
                    break;
                case "gridRaces":
                    resize_Grid(gridFilter, "height", 285, moveAnimationDuration);
                    break;
                case "gridPurchaseGuide":
                    generatePurchaseGuideView();
                    break;
                case "gridPartStat":
                    if (await fData.Login_API(Encoding.UTF8.GetBytes((username).ToLower()), Encoding.UTF8.GetBytes(helper.ToInsecureString(password)), false) == 200)
                    {
                        if (ddMenu2.SelectedIndex != -1 && ddMenu3.SelectedIndex != -1 && ddMenu4.SelectedIndex != -1)
                        {
                            int week = -1;
                            if (ddMenu5.SelectedIndex != 0)
                            {
                                week = Convert.ToInt32(((System.Windows.FrameworkElement)ddMenu5.SelectedValue).Name.Replace("w", ""));
                            }
                            try
                            {
                                DataTable dataTable = new DataTable();
                                btnMenu1.Content = "Loading...";
                                btnMenu1.IsEnabled = false;
                                dataTable = await statistics.PaticipationStats(Convert.ToInt32(
                                    ((System.Windows.FrameworkElement)ddMenu2.SelectedValue).Name.Replace("s", "")),
                                    Convert.ToInt32(((System.Windows.FrameworkElement)ddMenu3.SelectedValue).Name.Replace("y", "")),
                                    Convert.ToInt32(((System.Windows.FrameworkElement)ddMenu4.SelectedValue).Name.Replace("s", "")),
                                    week);
                                gridPartStat.ItemsSource = null;
                                gridPartStat.ItemsSource = dataTable.DefaultView;
                                /*gridPartStat.DataContext = (await statistics.PaticipationStats(Convert.ToInt32(
                                    ((System.Windows.FrameworkElement)ddMenu2.SelectedValue).Name.Replace("s", "")),
                                    Convert.ToInt32(((System.Windows.FrameworkElement)ddMenu3.SelectedValue).Name.Replace("y", "")),
                                    Convert.ToInt32(((System.Windows.FrameworkElement)ddMenu4.SelectedValue).Name.Replace("s", "")),
                                    week)).DefaultView;*/
                                gridPartStat.UpdateLayout();
                                btnMenu1.Content = "Get stat";
                                btnMenu1.IsEnabled = true;

                            }
                            catch
                            {
                                btnMenu1.Content = "Get stat";
                                btnMenu1.IsEnabled = true;
                            }
                        }
                    }
                    else
                    {
                        btnMenu1.Content = "Please relogin!";
                    }
                    break;
                case "gridiRatingStat":

                    if (await fData.Login_API(Encoding.UTF8.GetBytes((username).ToLower()), Encoding.UTF8.GetBytes(helper.ToInsecureString(password)), false) == 200)
                    {
                        if (ddMenu2.SelectedIndex != -1 && ddMenu3.SelectedIndex != -1)
                        {
                            try
                            {
                                btnMenu1.Content = "Loading...";
                                btnMenu1.IsEnabled = false;
                                dgiRaitingDataGridList = await statistics.DriverIratingPerSeries(
                                    Convert.ToInt32(((System.Windows.FrameworkElement)ddMenu3.SelectedValue).Name.Replace("s", "")),
                                    Convert.ToInt32(((System.Windows.FrameworkElement)ddMenu4.SelectedValue).Name.Replace("c", ""))
                                    , iRatingStatUserIrating);
                                gridiRatingStat.ItemsSource = null;
                                gridiRatingStat.ItemsSource = dgiRaitingDataGridList;
                                btnMenu1.Content = "Get stat";
                                btnMenu1.IsEnabled = true;
                            }
                            catch
                            {
                                btnMenu1.Content = "Get stat";
                                btnMenu1.IsEnabled = true;
                            }
                        }
                    }
                    else
                    {
                        btnMenu1.Content = "Please relogin!";
                    }
                    break;
            }
        }
        private void cbMenu2_Click(object sender, RoutedEventArgs e)
        {
            switch (activeGrid)
            {
                case "gridAutoStart":
                    if (cbMenu2.IsChecked == true)
                    {
                        autoStartApps.Active = true;
                    }
                    else
                    {
                        autoStartApps.Active = false;
                    }
                    break;
            }
            helper.SerializeObject<autoStart.Root>(autoStartApps, exePath + autostartfile);
        }
        private void cbMenu3_Click(object sender, RoutedEventArgs e)
        {
            switch (activeGrid)
            {
                case "gridAutoStart":
                    if (cbMenu3.IsChecked == true)
                    {
                        autoStartApps.Minimized = true;
                    }
                    else
                    {
                        autoStartApps.Minimized = false;
                    }
                    break;
            }
            helper.SerializeObject<autoStart.Root>(autoStartApps, exePath + autostartfile);
        }
        private void cbMenu4_Click(object sender, RoutedEventArgs e)
        {
            switch (activeGrid)
            {
                case "gridAutoStart":
                    if (cbMenu4.IsChecked == true)
                    {
                        autoStartApps.Kill = true;
                    }
                    else
                    {
                        autoStartApps.Kill = false;
                    }
                    break;
            }
            helper.SerializeObject<autoStart.Root>(autoStartApps, exePath + autostartfile);
        }
        private void cbMenu5_Click(object sender, RoutedEventArgs e)
        {
            switch (activeGrid)
            {
                case "gridAutoStart":
                    if (cbMenu5.IsChecked == true)
                    {
                        autoStartApps.KillByName = true;
                    }
                    else
                    {
                        autoStartApps.KillByName = false;
                    }
                    break;
            }
            helper.SerializeObject<autoStart.Root>(autoStartApps, exePath + autostartfile);
        }
        private async void ddMenu2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (activeGrid)
            {
                case "gridRaces":
                    if (ddMenu2.SelectedIndex != -1)
                    {
                        Properties.Settings.Default.defaultTimer = ddMenu2.SelectedIndex;
                        Properties.Settings.Default.Save();
                    }
                    break;
                case "gridPartStat":
                    if (ddMenu2.SelectedIndex != -1)
                    {
                        var selectedValue = Convert.ToInt32(((System.Windows.FrameworkElement)ddMenu2.SelectedValue).Name.Replace("s",""));
                        int weeks = dgSeriesList.FirstOrDefault(s => s.SerieId == selectedValue).Tracks.Count();
                        ddMenu5.Items.Clear();
                        ddMenu5.Items.Add(new ComboBoxItem() { Content = "All", Name = "" });
                        ddMenu5.SelectedIndex = 0;
                        for(int i = 1; i <= weeks; i++)
                        {
                            ddMenu5.Items.Add(new ComboBoxItem() { Content = i, Name = "w" + i });
                        }
                    }
                    break;
                case "gridiRatingStat":
                    if (ddMenu2.SelectedIndex != -1)
                    {
                        if (await fData.Login_API(Encoding.UTF8.GetBytes((username).ToLower()), Encoding.UTF8.GetBytes(helper.ToInsecureString(password)), false) == 200)
                        {
                            ddMenu3.Items.Clear();
                            ddMenu4.Items.Clear();
                            var selectedValue = Convert.ToInt32(((System.Windows.FrameworkElement)ddMenu2.SelectedValue).Name.Replace("s", ""));
                            var seasons = await fData.getSeriesPastSeasons(selectedValue.ToString());
                            switch (seasons.series.category)
                            {
                                case "road":
                                    iRatingStatUserIrating = User.licenses.road.irating;
                                    break;
                                case "oval":
                                    iRatingStatUserIrating = User.licenses.oval.irating;
                                    break;
                                case "dirt_road":
                                    iRatingStatUserIrating = User.licenses.dirt_road.irating;
                                    break;
                                case "dirt_oval":
                                    iRatingStatUserIrating = User.licenses.dirt_oval.irating;
                                    break;
                            }
                            List<seriesPastSeasons.CarClass> carclasses = new List<seriesPastSeasons.CarClass>();
                            foreach (var season in seasons.series.seasons)
                            {
                                ddMenu3.Items.Add(new ComboBoxItem() { Content = season.season_short_name, Name = "s" + season.season_id.ToString() });
                                foreach (var carclass in season.car_classes)
                                {
                                    if (carclasses.FirstOrDefault(c => c.car_class_id == carclass.car_class_id) == null)
                                    {
                                        carclasses.Add(carclass);
                                    }
                                }
                            }
                            foreach (var carclass in carclasses)
                            {
                                ddMenu4.Items.Add(new ComboBoxItem() { Content = carclass.name, Name = "c" + carclass.car_class_id.ToString() });
                            }
                        }
                    }
                    break;
            }
        }
        private void ddMenu3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (activeGrid)
            {
                case "gridPartStat":
                    break;
            }
        }

        private void ddMenu4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (activeGrid)
            {
                case "gridPartStat":
                    break;
            }
        }

        private void ddMenu5_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (activeGrid)
            {
                case "gridPartStat":
                    break;
            }
        }


        private void tbMenu6_MouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (activeGrid)
            {
                case "gridRaces":
                    generateRaceView();
                    break;
            }
        }
        private void btnMenu6_Click(object sender, RoutedEventArgs e)
        {
            switch (activeGrid)
            {
                case "gridAutoStart":
                    
                    break;
            }
        }

        private void btnProfileReload_Click(object sender, RoutedEventArgs e)
        {
            reloadData = true;
            gridLoading.Visibility = Visibility.Visible;
            gridLoadingBG.Visibility = Visibility.Visible;
            bwPresetLoader.WorkerReportsProgress = true;
            bwPresetLoader.RunWorkerCompleted -= worker_RunWorkerCompleted;
            bwPresetLoader.ProgressChanged -= worker_ProgressChanged;
            bwPresetLoader.RunWorkerCompleted += worker_RunWorkerCompleted;
            bwPresetLoader.ProgressChanged += worker_ProgressChanged;
            bwPresetLoader.RunWorkerAsync();
            generateRaceView();
            switchMainGridVisibility(new List<System.Windows.Controls.DataGrid> { gridRaces }, false);
            activeGrid = "gridRaces";
        }

        private void cbStartMinimized_Click(object sender, RoutedEventArgs e)
        {
            if (cbStartMinimized.IsChecked == true)
            {
                Properties.Settings.Default.minimized = "true";
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.minimized = "false";
                Properties.Settings.Default.Save();
            }

        }


    }
}
