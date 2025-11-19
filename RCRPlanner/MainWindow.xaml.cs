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
using System.Security;
using System.Windows.Media.Animation;
using System.Diagnostics;
using System.ComponentModel;
using System.Globalization;
using Microsoft.Win32;
using System.Timers;
using System.Windows.Controls.Primitives;
using System.Text.RegularExpressions;
using System.Media;


namespace RCRPlanner
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>       


    public partial class MainWindow : Window
    {
        internal static MainWindow main;
        internal string Status
        {
            get { return lblLoadingText.Content.ToString(); }
            set { Dispatcher.Invoke(new Action(() => { lblLoadingText.Content = value; })); }
        }

        readonly double moveAnimationDuration = 0.3;
        private readonly FetchData fData = new FetchData();
        private readonly filehandler fh = new filehandler();
        private readonly statistics statistics = new statistics();
        ProcessWatcher processWatcher = new ProcessWatcher("iRacingSim64DX11");
        //private static readonly HttpClient client = new HttpClient();
        public memberInfo.Root User = new memberInfo.Root();
        string username;
        SecureString password;
        readonly string userfile = @"user.xml";
        readonly bool isMetric = new RegionInfo(CultureInfo.CurrentCulture.Name).IsMetric;
        readonly string favsymbolSelected = "★";
        readonly string favsymbolUnselected = "✰";
        readonly string checksymbol = "🗸";
        readonly string unchecksymbol = "×";
        readonly string alarmClockSymbol = "⏰";
        readonly string clockSymbol = "🕙";
        readonly string magnifier = "🔍";
        readonly string warning = "⚠";
        readonly string thumbup = "👍";
        readonly string thumbdown = "👎";
        readonly string neutral = "◯";
        readonly string play = "►";
        readonly string pause = "▐▐";
        readonly string save = "💾";
        string activeGrid = "";
        bool reloadData = false;
        int lastLoginResult = -1;
        bool savelogin = false;
        int iRatingStatUserIrating = -1;
        public int partStatMinStarters = 8;
        readonly System.Timers.Timer alarmTimer = new System.Timers.Timer();

        readonly static Version version = Assembly.GetExecutingAssembly().GetName().Version;
        readonly static DateTime buildDate = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
        readonly string displayableVersion = $"{version} ({buildDate.ToShortDateString()})";

        readonly string favSeriesfile = @"\favoriteSeries.xml";
        readonly string favCarsfile = @"\favoriteCars.xml";
        readonly string favTracksfile = @"\favoriteTracks.xml";
        readonly string sympathyCombifile = @"\sympathyCombi.xml";
        List<memberInfo.FavoriteCars> favoriteCars = new List<memberInfo.FavoriteCars>();
        List<memberInfo.FavoriteSeries> favoriteSeries = new List<memberInfo.FavoriteSeries>();
        List<memberInfo.FavoriteTracks> favoriteTracks = new List<memberInfo.FavoriteTracks>();
        List<memberInfo.SympathyCombi> sympathyCombis = new List<memberInfo.SympathyCombi>();
        List<searchSerieResults.Root> seasonRaces = new List<searchSerieResults.Root>(); // races done in this season.
        List<participationCredits.Root> participationCredits = new List<participationCredits.Root>();

        readonly string autostartfile = @"\autostart.xml";
        public static autoStart.Root autoStartApps = new autoStart.Root();
        public bool autostartsuppress;
        public static readonly List<int> pIDs = new List<int>();
        private readonly List<dgObjects.autoStartDataGrid> dgAutoStartList = new List<dgObjects.autoStartDataGrid>();

        private readonly BackgroundWorker bwPresetLoader = new BackgroundWorker();
        private static readonly ManualResetEvent mre = new ManualResetEvent(false);

        List<series.Root> seriesList = new List<series.Root>();
        List<seriesAssets> seriesAssetsList = new List<seriesAssets>();
        List<seriesSeason.Root> seriesSeasonList = new List<seriesSeason.Root>();
        List<actualWeek> actualWeeks = new List<actualWeek>();
        List<dgObjects.seriesDataGrid> dgSeriesList = new List<dgObjects.seriesDataGrid>();
        private readonly string seriesFile = @"\static\series.xml";
        private readonly string seriesSeasonFile = @"\static\seriesSeason.xml";
        private readonly string seriesAssetsFile = @"\static\seriesAssets.xml";
        private readonly string seriesLogos = @"\static\series\";

        public List<cars.Root> carsList = new List<cars.Root>();
        private List<carAssets> carsAssetsList = new List<carAssets>();
        private List<carClass.Root> carClassList = new List<carClass.Root>();
        private List<carClass.CarInClassId> carClassesList = new List<carClass.CarInClassId>();
        private List<cars.CarsInSeries> carsInSeries = new List<cars.CarsInSeries>();
        readonly List<dgObjects.carsDataGrid> dgCarsList = new List<dgObjects.carsDataGrid>();
        private readonly string carsFile = @"\static\cars.xml";
        private readonly string carsAssetsFile = @"\static\carsAssets.xml";
        private readonly string carClassFile = @"\static\carClass.xml";
        private readonly string carLogos = @"\static\cars\";

        private List<tracks.Root> tracksList = new List<tracks.Root>();
        private List<trackAssets.Root> tracksAssetsList = new List<trackAssets.Root>();
        private readonly List<dgObjects.tracksDataGrid> dgTracksList = new List<dgObjects.tracksDataGrid>();
        private readonly List<dgObjects.tracksLayoutsDataGrid> dgTrackLayoutList = new List<dgObjects.tracksLayoutsDataGrid>();
        private List<tracks.TracksInSeries> tracksInSeries = new List<tracks.TracksInSeries>();
        private readonly string tracksFile = @"\static\tracks.xml";
        private readonly string tracksAssetsFile = @"\static\tracksAssets.xml";
        private readonly string tracksLogo = @"\static\tracks\";
        int selectedTrack = -1;

        private seriesPastSeasons.Root pastSeasons = new seriesPastSeasons.Root();
        private readonly List<dgObjects.tracksDataGrid> dgPurchaseGuideList = new List<dgObjects.tracksDataGrid>();
        private readonly List<dgObjects.RaceOverviewDataGrid> dgRaceOverviewList = new List<dgObjects.RaceOverviewDataGrid>();
        private List<comboBox> cbSeries = new List<comboBox>();
        private List<dgObjects.iRaitingDataGrid> dgiRaitingDataGridList = new List<dgObjects.iRaitingDataGrid>();
        public List<Alarms> RaceAlarms = new List<Alarms>();
        private List<comboBox> cbAlarms = new List<comboBox>(){
            new comboBox() { Name = "No Offset", Value = "i0" },
            new comboBox() { Name = "1 Minute", Value = "i1" },
            new comboBox() { Name = "2 Minutes", Value = "i2" },
            new comboBox() { Name = "3 Minutes", Value = "i3" },
            new comboBox() { Name = "5 Minutes", Value = "i5" },
            new comboBox() { Name = "10 Minutes", Value = "i10" },
            new comboBox() { Name = "15 Minutes", Value = "i15" },
            new comboBox() { Name = "30 Minutes", Value = "i30" }
        };

        private static readonly string exePath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        private List<partner> partners = new List<partner>();
        private bool partnerSliderHide = false;

        private readonly string defaultfilter = "cbFilterInOfficial;cbFilterOfficial;cbFilterOpenSetup;cbFilterFixedSetup;cbFilterFormula;cbFilterSports;cbFilterOval;cbFilterDirt;cbFilterDirtOval;cbFilterR;cbFilterD;cbFilterC;cbFilterB;cbFilterA;cbFilterP";

        private readonly SoundPlayer soundPlayer = new SoundPlayer(exePath + "\\alarm.wav");
        public MainWindow()
        {
            try
            {
                main = this;
                this.Resources["CanvasRightCalc"] = (double)50;
                this.Resources["CanvasLeftCalc"] = (double)0;
                int duration = (int)Math.Round(Application.Current.MainWindow.ActualWidth * 0.024);
                this.Resources["AnimationTime"] = (Duration)new TimeSpan(0, 0, duration);
                this.InitializeComponent();
                this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - 2;
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
                dpMenu3date.SelectedDate = DateTime.Now;
                gridLoading.Visibility = Visibility.Visible;
                gridLoadingBG.Visibility = Visibility.Visible;
                bwPresetLoader.DoWork += worker_DoWork;
                bwPresetLoader.WorkerReportsProgress = false;
                bwPresetLoader.RunWorkerCompleted += worker_RunWorkerCompleted;
                bwPresetLoader.RunWorkerAsync();
                btnLoadRaces_Click(null, null);

                alarmTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                alarmTimer.Interval = 10000;
                alarmTimer.Enabled = true;
                lblVersion.Content = displayableVersion;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Main: " + ex.InnerException.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                        soundPlayer.Play();
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

        private void yourFilter(object sender, FilterEventArgs e)
        {
            if (e.Item is series.Root obj)
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
            bool error = false;
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
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(async () => {
                fData.EnsureVersionCompatibility();
                lblLoadingText.Content = "Loading user information.";
                if (Properties.Settings.Default.username != string.Empty)
                {
                    lblLoadingText.Content = "Login information present.";
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
                try
                {
                    generatePartnerSlider();
                }
                catch { }
                if (File.Exists(userfile) && !reloadData)
                {
                    lblLoadingText.Content = "Loading user information from file.";
                    User = helper.DeSerializeObject<memberInfo.Root>(userfile);
                    Style_ProfileIcon(User);

                }
                else
                {
                    lblLoadingText.Content = "User information not present in file.";
                    filemissing = true;
                }
                if(File.Exists(exePath + autostartfile))
                {
                    autoStartApps = helper.DeSerializeObject<autoStart.Root>(exePath + autostartfile);
                    if(autoStartApps.Programs != null && autoStartApps.Active && !autostartsuppress)
                    {
                        startPrograms();

                        processWatcher.StartWatching();
                    }
                }
                else
                {
                    autoStartApps.Active = false;
                    autoStartApps.Programs = null;
                }
                if (File.Exists(exePath + favSeriesfile))
                {
                    favoriteSeries = helper.DeSerializeObject<List<memberInfo.FavoriteSeries>>(exePath + favSeriesfile);
                    favoriteSeries = favoriteSeries.GroupBy(x => x.series_id).Select(x => x.First()).ToList();

                }
                if (File.Exists(exePath + favTracksfile))
                {
                    favoriteTracks = helper.DeSerializeObject<List<memberInfo.FavoriteTracks>>(exePath + favTracksfile);
                    favoriteTracks = favoriteTracks.GroupBy(x => x.track_id).Select(x => x.First()).ToList();
                }
                if (File.Exists(exePath + favCarsfile))
                {
                    favoriteCars = helper.DeSerializeObject<List<memberInfo.FavoriteCars>>(exePath + favCarsfile);
                    favoriteCars = favoriteCars.GroupBy(x => x.car_id).Select(x => x.First()).ToList();
                }
                if (File.Exists(exePath + sympathyCombifile))
                {
                    sympathyCombis = helper.DeSerializeObject<List<memberInfo.SympathyCombi>>(exePath + sympathyCombifile);
                }
            }));
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                lblLoadingText.Content = "Checking present data files.";
            }));
            foreach (var filename in ((System.Reflection.TypeInfo)typeof(MainWindow)).DeclaredFields)
            {
                if (filename.Name.EndsWith("File"))
                {
                    var file = exePath + filename.GetValue(this);
                    if (File.Exists(file))
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            lblLoadingText.Content = "Loading information from " + filename.GetValue(this);
                        }));
                    }
                    else
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            lblLoadingText.Content = "Missing file: " + filename.GetValue(this);
                        }));
                        filemissing = true;
                    }
                }
            }
            if (filemissing || reloadData)
            {

                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    lblLoadingText.Content = "Fetching data from iRacion API due to missing files.";
                }));
                if (savelogin && password.Length != 0)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        lblLoadingText.Content = "Credentials present. Connecting to API...";
                    }));
                    var status = fData.Login_API(Encoding.UTF8.GetBytes((username).ToLower()), Encoding.UTF8.GetBytes(helper.ToInsecureString(password)),false);
                    try
                    {
                        status.Wait();
                        lastLoginResult = status.Result;

                        if (status.Result == 401)
                        {
                            move_grid(gridLogin, "bottom", -250, moveAnimationDuration);
                            mre.WaitOne();
                        }
                        if (status.Result == 503)
                        {
                            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                lblLoadingText.Content = "iRacing is in maintenance mode!";
                            }));
                        }
                        if (status.Result >= 200 && 210 >= status.Result)
                        {
                            try
                            {
                                Task<memberInfo.Root> getUser = fData.getMemberInfo();
                                getUser.Wait();
                                User = getUser.Result;
                                helper.SerializeObject<memberInfo.Root>(User, userfile);
                            }
                            catch (Exception ex)
                            {
                                if(ex.InnerException != null)
                                {
                                    MessageBox.Show("Download user data: " + ex.InnerException.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                                }
                                error = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if(ex.InnerException != null)
                        {
                            MessageBox.Show("Reload data: " + ex.InnerException.Message,"Something went wrong." , MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        error = true;
                    }
                }
                else
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        lblLoadingText.Content = "Credentials NOT present. Please enter credentials.";
                    }));
                    move_grid(gridLogin, "bottom", -250, moveAnimationDuration);
                    mre.WaitOne();
                }
            }
            if (error == false)
            {
                if (!(lastLoginResult >= 200 && 210 >= lastLoginResult) && lastLoginResult != -1)
                {
                    try
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            lblLoadingText.Content = "iRacing is down!";
                        }));
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            lblLoadingText.Content = "Loading series information.";
                        }));
                        seriesList = await fh.getSeriesList(seriesFile, reloadData);
                        seriesAssetsList = await fh.getSeriesAssets(seriesAssetsFile, seriesLogos, reloadData);
                        seriesSeasonList = await fh.getSeriesSeason(seriesSeasonFile, reloadData);
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null)
                        {
                            MessageBox.Show("Error loading series: " + ex.InnerException.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            MessageBox.Show("Error loading series: " + ex.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }


                    try
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            lblLoadingText.Content = "Loading car information.";
                        }));

                        carsList = await fh.getCarList(carsFile, reloadData);
                        carsAssetsList = await fh.getCarAssetsList(carsAssetsFile, carLogos, reloadData);
                        carClassList = await fh.getCarClassList(carClassFile, reloadData);
                        carClassesList = fh.getCarClassesList(carsList, carClassList);
                        carsInSeries = fh.getCarsInSeries(carClassesList, seriesSeasonList);
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null)
                        {
                            MessageBox.Show("Error loading cars: " + ex.InnerException.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            MessageBox.Show("Error loading cars: " + ex.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    try
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            lblLoadingText.Content = "Loading track information.";
                        }));

                        tracksList = await fh.getTracksList(tracksFile, reloadData);
                        tracksAssetsList = await fh.getTracksAssets(tracksAssetsFile, reloadData);
                        tracksInSeries = fh.getTracksInSeries(tracksList, seriesSeasonList);
                        await fh.getTrackSVGAsync(tracksAssetsList, exePath + tracksLogo);
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null)
                        {
                            MessageBox.Show("Error loading tracks: " + ex.InnerException.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            MessageBox.Show("Error loading tracks: " + ex.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }

                    createDgData();

                }
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
                try
                {
                    Style_ProfileIcon(User);
                    checkNewRelease();
                    spPartner_SizeChanged(null, null);
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        MessageBox.Show("Style user profile: " + ex.InnerException.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MessageBox.Show("Error creating series: " + ex.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }));
        }
        private void startPrograms()
        {
            try
            {
                if (autoStartApps.Programs.Count() > 0)
                {
                    foreach (var prog in autoStartApps.Programs)
                    {
                        if (!prog.Paused && !prog.withiRacing)
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
                }
                tbstartPrograms.Foreground = Application.Current.Resources["BrushDarkGray"] as SolidColorBrush;
                tbstopPrograms.Foreground = Application.Current.Resources["BrushYellow"] as SolidColorBrush;
                btnstart_Programs.Visibility = Visibility.Hidden;
                btnstop_Programs.Visibility = Visibility.Visible;
            }
            catch { }
        }
        private void stopPrograms()
        {
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
            tbstartPrograms.Foreground = Application.Current.Resources["BrushYellow"] as SolidColorBrush;
            tbstopPrograms.Foreground = Application.Current.Resources["BrushDarkGray"] as SolidColorBrush;
            btnstart_Programs.Visibility = Visibility.Visible;
            btnstop_Programs.Visibility = Visibility.Hidden;
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
                stopPrograms();
                processWatcher.StopWatching();
                helper.SerializeObject<List<memberInfo.FavoriteTracks>>(favoriteTracks, exePath + favTracksfile);
                helper.SerializeObject<List<memberInfo.FavoriteSeries>>(favoriteSeries, exePath + favSeriesfile);
                helper.SerializeObject<List<memberInfo.FavoriteCars>>(favoriteCars, exePath + favCarsfile);
                helper.SerializeObject<List<memberInfo.SympathyCombi>>(sympathyCombis, exePath + sympathyCombifile);
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
            MenuNotification.Visibility = Visibility.Hidden;
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
                try
                {
                    string formulaClass = User.licenses.formula_car.group_name.ToString().Replace("Class ", "").Replace("Rookie", "R").Replace("Pro", "P");
                    string sportClass = User.licenses.sports_car.group_name.ToString().Replace("Class ", "").Replace("Rookie", "R").Replace("Pro", "P");
                    string ovalClass = User.licenses.oval.group_name.ToString().Replace("Class ", "").Replace("Rookie", "R").Replace("Pro", "P");
                    string dirtovalClass = User.licenses.dirt_oval.group_name.ToString().Replace("Class ", "").Replace("Rookie", "R").Replace("Pro", "P");
                    string dirtroadClass = User.licenses.dirt_road.group_name.ToString().Replace("Class ", "").Replace("Rookie", "R").Replace("Pro", "P");
                    
                    progressOval.Value = Convert.ToDouble(helper.Mapdec(Convert.ToDecimal(User.licenses.oval.safety_rating / (4.99 / 100)), 0, 100, 0, 75));
                    progressOval.Foreground = (Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#" + User.licenses.oval.color);
                    textProgressOval.Text = ovalClass + User.licenses.oval.safety_rating.ToString();
                    lbliRatingOval.Content = User.licenses.oval.irating.ToString() + " iR";
                    lblCpiOval.Content = Math.Round(User.licenses.oval.cpi, 2).ToString() + " CPI";

                    progressSport.Value = Convert.ToDouble(helper.Mapdec(Convert.ToDecimal(User.licenses.sports_car.safety_rating / (4.99 / 100)), 0, 100, 0, 75));
                    progressSport.Foreground = (Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#" + User.licenses.sports_car.color);
                    textProgressSport.Text = formulaClass + User.licenses.sports_car.safety_rating.ToString();
                    lbliRatingSport.Content = User.licenses.sports_car.irating.ToString() + " iR";
                    lblCpiSport.Content = Math.Round(User.licenses.sports_car.cpi, 2).ToString() + " CPI";

                    progressFormula.Value = Convert.ToDouble(helper.Mapdec(Convert.ToDecimal(User.licenses.formula_car.safety_rating / (4.99 / 100)), 0, 100, 0, 75));
                    progressFormula.Foreground = (Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#" + User.licenses.formula_car.color);
                    textProgressFormula.Text = formulaClass + User.licenses.formula_car.safety_rating.ToString();
                    lbliRatingFormula.Content = User.licenses.formula_car.irating.ToString() + " iR";
                    lblCpiFormula.Content = Math.Round(User.licenses.formula_car.cpi, 2).ToString() + " CPI";

                    progressDirtOval.Value = Convert.ToDouble(helper.Mapdec(Convert.ToDecimal(User.licenses.dirt_oval.safety_rating / (4.99 / 100)), 0, 100, 0, 75));
                    progressDirtOval.Foreground = (Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#" + User.licenses.dirt_oval.color);
                    textProgressDirtOval.Text = dirtovalClass + User.licenses.dirt_oval.safety_rating.ToString();
                    lbliRatingDirtOval.Content = User.licenses.dirt_oval.irating.ToString() + " iR";
                    lblCpiDirtOval.Content = Math.Round(User.licenses.dirt_oval.cpi, 2).ToString() + " CPI";

                    progressDirtRoad.Value = Convert.ToDouble(helper.Mapdec(Convert.ToDecimal(User.licenses.dirt_road.safety_rating / (4.99 / 100)), 0, 100, 0, 75));
                    progressDirtRoad.Foreground = (Brush)new System.Windows.Media.BrushConverter().ConvertFromString("#" + User.licenses.dirt_road.color);
                    textProgressDirtRoad.Text = dirtroadClass + User.licenses.dirt_road.safety_rating.ToString();
                    lbliRatingDirtRoad.Content = User.licenses.dirt_road.irating.ToString() + " iR";
                    lblCpiDirtRoad.Content = Math.Round(User.licenses.dirt_road.cpi, 2).ToString() + " CPI";
                    List<int> items = getPurchasedItems();
                    lblTrackItems.Content = "Purchased Tracks: " + items[0] +"("+items[4] +  ")*" + " / " + items[2];
                    lblCarItems.Content = "Purchased Cars: " + items[1] + "(" + items[5] + ")*" + " / " + items[3];
                    int itemcount = items[0] + items[1];
                    ProgItems.Value = itemcount;
                    tbItems.Width = 40;
                    int pos = Convert.ToInt32((ProgItems.RenderSize.Width / 40) * (itemcount > 40 ? 40 : itemcount) );
                    var gradient = (((System.Windows.Media.GradientBrush)ProgItems.Foreground).GradientStops);
                    gradient[1].Offset = itemcount<40 && itemcount > 0? 1/((float)itemcount/40): 1;
                    if (pos < 40) {
                        tbItems.Width = 40 + pos;
                        tbItems.Padding= new Thickness(pos, 0, 0, 0);
                        tbItems.Foreground = Application.Current.Resources["BrushTextWhite"] as SolidColorBrush;
                    }
                    if (pos >= 40)
                    {
                        tbItems.Padding = new Thickness(0,0,8,0);
                        tbItems.Width = pos;
                        tbItems.Foreground = Application.Current.Resources["BrushTextBlack"] as SolidColorBrush;
                    }
                    tbItems.Text = itemcount + " / 40**";
                    lblCustId.Content = "iRacing ID: " + User.cust_id.ToString();
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        MessageBox.Show("Profile: " + ex.InnerException.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MessageBox.Show("Profile: " + ex.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                position = 300;
                
            }
            else
            {
                position = 0;
            }

            resize_Grid(gridProfile, "height", position, moveAnimationDuration);
        }
        private void btnLogin_Click (object sender, RoutedEventArgs e)
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
        private async void btnLoginLogin_Click(object sender, RoutedEventArgs e)
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
            try
            {
                var status = await fData.Login_API(Encoding.UTF8.GetBytes((username).ToLower()), Encoding.UTF8.GetBytes(helper.ToInsecureString(password)), true);
                if (status >= 200 && 210 >= status)
                {
                    User = await fData.getMemberInfo();
                    helper.SerializeObject<memberInfo.Root>(User, userfile);
                    Style_ProfileIcon(User);
                    btnLogin_Click(null, null);
                    lblLogin.Content = "";
                    mre.Set();
                }
                else
                {
                    lblLogin.Content = "Something went wrong. Please retry";
                }
            }
            catch (Exception ex)
            {
                if(ex.InnerException != null)
                {
                    MessageBox.Show("Login: " + ex.InnerException.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Login: " + ex.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void Style_ProfileIcon(memberInfo.Root info)
        {
            try
            {
                if (info.display_name != null)
                {
                    var displayName = info.display_name.Split(' ');
                    string initials = "";
                    if (displayName.Length > 2)
                    {
                        initials = displayName[0].Substring(0, 1) + displayName[1].Substring(0, 1) + displayName[displayName.Length - 1].Substring(0, 1);
                    }
                    else
                    {
                        initials = displayName[0].Substring(0, 1) + displayName[1].Substring(0, 1);
                    }
                    tbHeaderMenuProfileName.Content = initials;
                    this.lblUsername.Content = User.display_name;
                }
                if (info.helmet != null)
                {
                    string color1 = "#" + info.helmet.color1;
                    string color2 = "#" + info.helmet.color2;
                    string color3 = "#" + info.helmet.color3;

                    elHeaderMenuProfileColor1.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color1));
                    elHeaderMenuProfileColor2.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color2));
                    elHeaderMenuProfileColor3.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color3));
                    this.tbHeaderMenuProfileName.Foreground = new SolidColorBrush(helper.getNegative(color3));
                }

            }
            catch (Exception ex)
            {
                if(ex.InnerException != null)
                {
                    MessageBox.Show("Profile styling: " + ex.InnerException.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Profile styling: " + ex.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }

        private void switchMainGridVisibility(List<System.Windows.Controls.DataGrid> gridToShow, bool Details)
        {
            List<System.Windows.Controls.DataGrid> allGrids = new List<System.Windows.Controls.DataGrid> { gridSeries, gridCars, gridTracksLayout, gridPurchaseGuide, gridRaces, gridCarDetail, gridTrackDetail, gridAutoStart, gridPartStat, gridiRatingStat, gridPartner };

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
            try
            {
                Status = "Loading series list.";
            }
            catch { }
            generateSeriesView();
            try
            {
                Status = "Loading car list.";
            }
            catch { }
            generateCarView();
            try
            {
                Status = "Loading tracks list.";
            }
            catch { }
            generateTrackView();
            try
            {
                Status = "Loading races list.";
            }
            catch { }
            generateRaceView();
        }
        private void generateSeriesView()
        {
            try
            {
                //Generate series information
                dgSeriesList.Clear();
                cbSeries.Clear();
                dgSeriesList = (from series in seriesList
                                join asset in seriesAssetsList on series.series_id equals asset.series_id
                                join season in seriesSeasonList on series.series_id equals season.series_id into _ssL
                                from alls in _ssL.DefaultIfEmpty()
                                join fav in favoriteSeries on series.series_id equals fav.series_id into ser
                                from allseries in ser.DefaultIfEmpty()
                                select new dgObjects.seriesDataGrid()
                                {
                                    SerieId = series.series_id,
                                    Seriesimage = asset.logo != null ? new Uri("file:///" + exePath + seriesLogos + asset.logo.ToString()) : new Uri("about:blank"),
                                    SeriesName = series.series_name,
                                    Category = series.category,
                                    Class = series.allowed_licenses[0].min_license_level >= series.allowed_licenses[0].max_license_level ? (series.allowed_licenses[1].group_name).Replace("Class ", "").Replace("ookie", "") : (series.allowed_licenses[0].group_name).Replace("Class ", "").Replace("ookie", ""),
                                    License = (series.allowed_licenses[0].group_name).Replace("Class ", "").Replace("ookie", "") + " " + (series.allowed_licenses[0].min_license_level - (series.allowed_licenses[0].license_group - 1) * 4).ToString("0.00"),
                                    Eligible = series.eligible == true ? checksymbol : unchecksymbol,
                                    Favorite = allseries?.series_id != null ? favsymbolSelected : favsymbolUnselected,
                                    Official = alls?.official == true ? checksymbol : unchecksymbol,
                                    Fixed = alls?.fixed_setup == true ? checksymbol : unchecksymbol,
                                    Season = alls != null ? alls : new seriesSeason.Root(),
                                    ForumLink = series.forum_url,
                                    RaceLength = alls == null ? "no Race" : alls?.schedules[0].race_lap_limit != null
                                 ? alls.schedules.Aggregate(0, (sum, schedule) => sum + (schedule.race_lap_limit == null ? 0 : schedule.race_lap_limit.Value))/alls.schedules.Count + " Laps"
                                 : (alls.schedules[0].race_time_limit >= 60 ? alls.schedules[0].race_time_limit / 60 + " h " + (alls.schedules[0].race_time_limit - ((alls.schedules[0].race_time_limit / 60) * 60)) + " min" : alls.schedules[0].race_time_limit + " Min" ),
                                }).ToList<dgObjects.seriesDataGrid>();
                foreach (var serie in dgSeriesList)
                {
                    cbSeries.Add(new comboBox() { Name = serie.SeriesName, Value = "s" + serie.SerieId.ToString() });
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
                        _trackobj.Created = tr.created.ToString(Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortDatePattern, Thread.CurrentThread.CurrentUICulture);
                        _trackobj.Length = isMetric ? Math.Round(tr.track_config_length * 1.60934, 3) : tr.track_config_length;
                        _trackobj.Owned = User.track_packages.Any(p => p.package_id == tr.package_id) ? checksymbol : "";
                        _trackobj.Pitlimit = isMetric ? Convert.ToInt32(tr.pit_road_speed_limit * 1.60934) : tr.pit_road_speed_limit;
                        _trackobj.Price = "$" + tr.price.ToString();
                        _trackobj.PackageID = tr.package_id;
                        _trackobj.TrackID = tr.track_id;
                        _trackobj.Category = tr.category;
                        _trackobj.TrackImage = new Uri("file:///" + exePath + tracksLogo + tr.track_id + ".png");
                        _trackobj.Week = track.SeasonSchedule.race_week_num + 1;
                        _trackobj.Weekdate = DateTime.Parse(track.SeasonSchedule.start_date, Thread.CurrentThread.CurrentUICulture).ToString(Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortDatePattern, Thread.CurrentThread.CurrentUICulture);
                        _trackobj.Racelength = track.SeasonSchedule.race_lap_limit != null
                            ? track.SeasonSchedule.race_lap_limit.ToString() + " Laps"
                            : (track.SeasonSchedule.race_time_limit >= 60 ? track.SeasonSchedule.race_time_limit / 60 + " h " + (track.SeasonSchedule.race_time_limit - ((track.SeasonSchedule.race_time_limit / 60) * 60)) + " min" : track.SeasonSchedule.race_time_limit + " Min");
                        tracks.Add(_trackobj);
                    }
                    tracks.Sort((x, y) => x.Week.CompareTo(y.Week));
                    int repeattimes = -1;
                    string evenOdd = "even ";
                    if (serie.Season.schedules != null) {
                        repeattimes  = ((serie.Season.schedules[0]).race_time_descriptors[0]).repeat_minutes;
                        if (((serie.Season.schedules[0]).race_time_descriptors[0]).first_session_time != null)
                        {
                            evenOdd = (DateTime.SpecifyKind(DateTime.Parse(((serie.Season.schedules[0]).race_time_descriptors[0]).first_session_time), DateTimeKind.Utc).ToLocalTime()).Hour % 2 == 0 ? "even " : "odd ";
                        }
                    }
                    if (repeattimes > 0)
                    {
                        if (repeattimes >= 60)
                        {
                            serie.RaceTimes = repeattimes >= 60 ? "Every " + evenOdd + repeattimes / 60 + " hours" : "Every " + evenOdd + repeattimes / 60 + " hour";
                        }
                        else
                        {
                            serie.RaceTimes = "Every " + repeattimes + " minutes";
                        }
                    }
                    else
                    {
                        if (repeattimes == -1)
                        {
                            serie.RaceTimes = "No schedule";
                        }
                        else
                        {
                            serie.RaceTimes = "Set times";
                        }
                    }
                    serie.Weeks = tracks.Where(t => t.Owned == checksymbol).Count() + "/" + tracks.Count();
                    serie.Tracks = tracks;
                }
                dgSeriesList.Sort((x, y) => x.SeriesName.CompareTo(y.SeriesName));
                cbSeries.Sort((x, y) => x.Name.CompareTo(y.Name));
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    MessageBox.Show("Error creating series: " + ex.InnerException.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Error creating series: " + ex.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void generateCarView()
        {
            try
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
                                                             Favorite = serie.Favorite,
                                                         }).FirstOrDefault();
                            seriesDataGridsList.Add(seriesDataGridsObject);
                        }

                    }
                    dgObjects.carsDataGrid carsDataGridObject = new dgObjects.carsDataGrid
                    {
                        Favorite = favoriteCars.Any(x => x.car_id == car.car_id) ? favsymbolSelected : favsymbolUnselected,
                        CarId = car.car_id,
                        CarImage = new Uri("file:///" + exePath + carLogos + car.car_id + ".png"),
                        CarLogo = new Uri("file:///" + exePath + carLogos + car.car_id + ".png"),
                        CarName = car.car_name,
                        Category = string.Join(",", car.categories),
                        Horsepower = isMetric ? Convert.ToInt32(car.hp * 1.01387) : car.hp,
                        Weight = isMetric ? Convert.ToInt32(car.car_weight * 0.453592) : car.car_weight,
                        Price = "$" + car.price.ToString(),
                        Owned = User.car_packages.Any(p => p.package_id == car.package_id) ? checksymbol : "",
                        Created = car.created.ToString(Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortDatePattern, Thread.CurrentThread.CurrentUICulture),
                        Series = seriesDataGridsList,
                        Series_Participations = seriesDataGridsList.Count,
                        ForumLink = car.forum_url
                    };
                    dgCarsList.Add(carsDataGridObject);
                }
                dgCarsList.Sort((x, y) => x.CarName.CompareTo(y.CarName));
                foreach (var serie in dgSeriesList)
                {
                    var cars = dgCarsList.FindAll(c => c.Series.Any(s => s != null && s.SerieId == serie.SerieId));
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
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    MessageBox.Show("Error creating cars: " + ex.InnerException.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Error creating cars: " + ex.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void generateTrackView()
        {
            try
            {
                //Generate tracks information
                dgTracksList.Clear();
                foreach (var track in tracksList)
                {
                    List<dgObjects.seriesDataGrid> seriesDataGridsList = new List<dgObjects.seriesDataGrid>();
                    foreach (var tracksinseries in tracksInSeries)
                    {
                        if (tracksinseries.track_id == track.track_id && !seriesDataGridsList.Any(s => s != null && s.SerieId == tracksinseries.series_id))
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
                                                             Favorite = serie.Favorite,
                                                         }).FirstOrDefault();
                            seriesDataGridsList.Add(seriesDataGridsObject);
                        }
                        else if (tracksinseries.track_id == track.track_id && seriesDataGridsList.Any(s => s != null && s.SerieId == tracksinseries.series_id))
                        {
                            var _trackObj = seriesDataGridsList.FirstOrDefault(t => t != null && t.SerieId == tracksinseries.series_id);
                            if (_trackObj != null)
                            {
                                _trackObj.Weeks += ", " + tracksinseries.week;
                            }
                        }
                    }

                    dgObjects.tracksDataGrid newLayout = new dgObjects.tracksDataGrid
                    {
                        Name = track.track_name,
                        Layoutname = track.config_name,
                        TrackImage = new Uri("file:///" + exePath + tracksLogo + track.track_id + ".png"),
                        Corners = track.corners_per_lap,
                        Created = track.created.ToString(Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortDatePattern, Thread.CurrentThread.CurrentUICulture),
                        Length = isMetric ? Math.Round(track.track_config_length * 1.60934, 3) : track.track_config_length,
                        Owned = User.track_packages.Any(p => p.package_id == track.package_id) ? checksymbol : "",
                        Pitlimit = isMetric ? Convert.ToInt32(track.pit_road_speed_limit * 1.60934) : track.pit_road_speed_limit,
                        Price = "$" + track.price.ToString(),
                        PackageID = track.package_id,
                        TrackID = track.track_id,
                        Category = track.category,
                        Series = seriesDataGridsList,
                        Participations = seriesDataGridsList.Count
                    };
                    dgTracksList.Add(newLayout);
                }
                dgTrackLayoutList.Clear();
                foreach (dgObjects.tracksDataGrid track in dgTracksList)
                {
                    if (!dgTrackLayoutList.Any(l => l.PackageID == track.PackageID))
                    {
                        List<dgObjects.tracksDataGrid> layout = new List<dgObjects.tracksDataGrid>() { track };
                        dgObjects.tracksLayoutsDataGrid newTrack = new dgObjects.tracksLayoutsDataGrid
                        {
                            Layouts = layout,
                            Created = track.Created,
                            TrackImage = new Uri("file:///" + exePath + tracksLogo + track.TrackID + ".png"),
                            Name = track.Name,
                            Owned = track.Owned,
                            PackageID = track.PackageID,
                            Price = track.Price,
                            Layouts_count = 1,
                            Participations = track.Participations,
                            Favorite = favoriteTracks.Any(x => x.track_id == track.PackageID) ? favsymbolSelected : favsymbolUnselected,
                            TrackID = track.TrackID
                        };
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
                            _trackObj.Created = (DateTime.Parse(track.Created, Thread.CurrentThread.CurrentUICulture) < DateTime.Parse(_trackObj.Created, Thread.CurrentThread.CurrentUICulture)) ? track.Created : _trackObj.Created;
                        }
                        if (_trackObj.Name.Contains("[Retired]"))
                        {
                            _trackObj.Name = _trackObj.Layouts.FirstOrDefault(l => !l.Name.Contains("[Retired]")) != null ? _trackObj.Layouts.FirstOrDefault(l => !l.Name.Contains("[Retired]")).Name : _trackObj.Name;
                        }
                    }
                }
                dgTrackLayoutList.Sort((x, y) => x.Name.CompareTo(y.Name));
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    MessageBox.Show("Error creating tracks: " + ex.InnerException.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Error creating tracks: " + ex.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void generatePurchaseGuideView()
        {
            try
            {
                //Generate Purchase information
                dgPurchaseGuideList.Clear();

                foreach (var item in tracksInSeries)
                {
                    var track = tracksList.First(t => t.track_id == item.track_id);
                    if (favoriteSeries.Any(f => f.series_id == item.series_id) && !User.track_packages.Any(p => p.package_id == track.package_id))
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
                                                         Favorite = serie.Favorite,
                                                         OwnTracks = serie.Tracks.Count(t => t.Owned == checksymbol),
                                                     }).FirstOrDefault();
                        seriesDataGridsList.Add(seriesDataGridsObject);
                        if (seriesDataGridsObject.OwnTracks < 8 || cbMenu2.IsChecked == false)
                        {
                            if (!dgPurchaseGuideList.Any(t => t.PackageID == track.package_id))
                            {
                                dgObjects.tracksDataGrid tracksDataGridObject = new dgObjects.tracksDataGrid
                                {
                                    Name = track.track_name,
                                    TrackImage = new Uri("file:///" + exePath + tracksLogo + track.track_id + ".png"),
                                    Created = track.created.ToString(Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortDatePattern, Thread.CurrentThread.CurrentUICulture),
                                    Price = "$" + track.price.ToString(),
                                    PackageID = track.package_id,
                                    TrackID = track.track_id,
                                    Category = track.category,
                                    Series = seriesDataGridsList,
                                    Length = isMetric ? Math.Round(track.track_config_length * 1.60934, 3) : track.track_config_length,
                                    Participations = 1,
                                    TrackLink = new Uri("https://members.iracing.com/membersite/member/TrackDetail.do?trkid=" + track.track_id)
                                };

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
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    MessageBox.Show("Error creating purchases: " + ex.InnerException.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Error creating purchases: " + ex.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void generateRaceView()
        {
            try
            {
                dgRaceOverviewList.Clear();
                DateTime actualtime = DateTime.Now.ToUniversalTime(); ;
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    if (((DateTime)dpMenu3date.SelectedDate).Date != DateTime.Now.Date)
                    {
                        actualtime = ((DateTime)dpMenu3date.SelectedDate).AddHours(12).ToUniversalTime();
                    }
                }));
                foreach (var serie in dgSeriesList)
                {
                    List<tracks.TracksInSeries> tracksinserie = new List<tracks.TracksInSeries>();
                    tracksinserie = tracksInSeries.Where(t => t.series_id == serie.SerieId).ToList<tracks.TracksInSeries>();
                    //tracksinserie = (tracksInSeries.FindAll(t => t.series_id == serie.SerieId)).ToList<tracks.TracksInSeries>();
                    DateTime firstracetime;
                    int daysoffset = 0;
                    int repeattimes;
                    bool over = false;
                    tracks.TracksInSeries actualweekofserie = new tracks.TracksInSeries();
                    tracksinserie.Sort((x, y) => x.SeasonSchedule.start_date.CompareTo(y.SeasonSchedule.start_date));
                    tracks.TracksInSeries lastseason = new tracks.TracksInSeries();
                    tracks.TracksInSeries nextseason = new tracks.TracksInSeries();
                    string actualsympathy = neutral;
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
                    if (tracksinserie.Count() > 0 && tracksinserie[0].SeasonSchedule.race_time_descriptors[0].repeating)
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
                        if (lastseason.SeasonSchedule != null && (Convert.ToDateTime(lastseason.SeasonSchedule.start_date) <= actualtime.Date || lastseason.SeasonSchedule.race_week_num == 0) && Convert.ToDateTime(lastseason.SeasonSchedule.race_time_descriptors[0].session_times[lastseason.SeasonSchedule.race_time_descriptors[0].session_times.Count - 1]) >= actualtime)
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
                        if (actualweekofserie.SeasonSchedule != null)
                        {
                            var nextrace = actualweekofserie.SeasonSchedule.race_time_descriptors[0].session_times.FirstOrDefault(s => s >= actualtime);
                            firstracetime = nextrace;
                            repeattimes = actualweekofserie.SeasonSchedule.race_time_descriptors[0].repeat_minutes;
                        }
                        else
                        {
                            firstracetime = new DateTime();
                            repeattimes = 0;
                        }
                    }

                    actualWeeks.Add(new actualWeek { seriesId = serie.SerieId, week = actualweekofserie.week });
                    var racetime = helper.getNextRace(DateTime.SpecifyKind(firstracetime, DateTimeKind.Utc), repeattimes, actualtime).ToLocalTime();
                    dgObjects.RaceOverviewDataGrid _raceobj = new dgObjects.RaceOverviewDataGrid();
                    var tr = tracksList.FirstOrDefault(t => t.track_id == actualweekofserie.track_id);

                    foreach (var track in serie.Tracks)
                    {
                        track.Sympathy = neutral;
                        track.Series = new List<dgObjects.seriesDataGrid> { serie };
                        if (track.Week == actualweekofserie.week && !over)
                        {
                            track.WeekActive = true;
                        }
                        else
                        {
                            track.WeekActive = false;
                        }
                        foreach (var combi in sympathyCombis)
                        {
                            if (serie.SerieId == combi.series_id && combi.track_id == track.TrackID)
                            {
                                track.Sympathy = combi.status;
                                if (track.WeekActive)
                                {
                                    actualsympathy = combi.status;
                                }
                            }
                        }
                    }
                    if (tr != null)
                    {
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
                        _raceobj.SerieRaceLength = actualweekofserie.SeasonSchedule.race_lap_limit != null 
                            ? actualweekofserie.SeasonSchedule.race_lap_limit.ToString() + " Laps" 
                            : (actualweekofserie.SeasonSchedule.race_time_limit >= 60 ? actualweekofserie.SeasonSchedule.race_time_limit / 60 + " h " + (actualweekofserie.SeasonSchedule.race_time_limit - ((actualweekofserie.SeasonSchedule.race_time_limit / 60 ) * 60)) + " min" : actualweekofserie.SeasonSchedule.race_time_limit + " Min");
                        _raceobj.SeriesName = serie.SeriesName;
                        _raceobj.TrackName = tr.track_name;
                        _raceobj.Serie = serie;
                        _raceobj.TracksOwned = serie.Tracks.Count(t => t.Owned == checksymbol).ToString() + "/" + serie.Tracks.Count();
                        _raceobj.NextRace = racetime;
                        _raceobj.TrackTrackID = tr.track_id;
                        _raceobj.Timer = RaceAlarms.Any(a => a.SerieId == serie.SerieId) ? alarmClockSymbol : clockSymbol;
                        _raceobj.Sympathy = actualsympathy;
                        if (racetime.Date == DateTime.Now.Date && racetime > DateTime.Now && !over)
                        {
                            _raceobj.NextRaceTime = racetime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern);

                        }
                        else if (racetime.Year == DateTime.Parse("0001-01-01").Year || over)
                        {
                            _raceobj.NextRaceTime = "Season is over.";
                        }
                        else
                        {
                            _raceobj.NextRaceTime = racetime.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + ", " + CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern);
                        }
                        if (actualweekofserie.SeasonSchedule.race_time_descriptors[0].session_times != null)
                        {
                            List<DateTime> sessionTimes = new List<DateTime>();
                            foreach (DateTime x in actualweekofserie.SeasonSchedule.race_time_descriptors[0].session_times)
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
                        if (repeattimes > 0)
                        {
                            if (repeattimes >= 60)
                            {
                                _raceobj.Repeating = repeattimes >= 60 ? "Every " + repeattimes / 60 + " hours" : "Every " + repeattimes / 60 + " hour";
                            }
                            else
                            {
                                _raceobj.Repeating = "Every " + repeattimes + " minutes";
                            }
                        }
                        else
                        {
                            _raceobj.Repeating = "Set times";
                        }
                        _raceobj.TrackOwned = User.track_packages.Any(t => t.package_id == tr.package_id);
                        dgRaceOverviewList.Add(_raceobj);
                    }
                }
                dgRaceOverviewList.Sort((x, y) => x.NextRace.CompareTo(y.NextRace));
                filterRaces();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    MessageBox.Show("Error creating races: " + ex.InnerException.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Error creating races: " + ex.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void filterRaces()
        {
            try
            {
                List<dgObjects.RaceOverviewDataGrid> dgFilteredRaces = new List<dgObjects.RaceOverviewDataGrid>();
                dgFilteredRaces.Clear();
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
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
                            foreach (var id in content.content_ids)
                            {
                                usercars.Add(id);
                            }
                        }
                        if (cbFilterSeasonOver.IsChecked == true && race.NextRaceTime == "Season is over." ||
                         race.NextRaceTime != "Season is over.")
                        {
                            over = true;
                        }
                        if ((cbFilterDirtOval.IsChecked == true && race.Serie.Category == "dirt_oval") ||
                            (cbFilterOval.IsChecked == true && race.Serie.Category == "oval") ||
                            (cbFilterFormula.IsChecked == true && race.Serie.Category == "formula_car") ||
                            (cbFilterSports.IsChecked == true && race.Serie.Category == "sports_car") ||
                            (cbFilterDirt.IsChecked == true && race.Serie.Category == "dirt_road"))
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

                        if ((cbFilterR.IsChecked == true && race.Serie.Class == "R") ||
                            (cbFilterD.IsChecked == true && race.Serie.Class == "D") ||
                            (cbFilterC.IsChecked == true && race.Serie.Class == "C") ||
                            (cbFilterB.IsChecked == true && race.Serie.Class == "B") ||
                            (cbFilterA.IsChecked == true && race.Serie.Class == "A") ||
                            (cbFilterP.IsChecked == true && race.Serie.Class == "Pro"))
                        {
                            serieclass = true;
                        }
                        if (
                            ((cbFilterFavSeries.IsChecked == true && race.Serie.Favorite == favsymbolSelected || cbFilterFavSeries.IsChecked == false) &&
                            (cbFilterFavTracks.IsChecked == true && race.Track.Favorite == favsymbolSelected || cbFilterFavTracks.IsChecked == false) &&
                            (cbFilterFavCars.IsChecked == true && race.Cars.Any(c => favoriteCars.Any(u => u.car_id == c.CarId)) || cbFilterFavCars.IsChecked == false) ||
                            (cbFilterFavSeries.IsChecked == false && cbFilterFavTracks.IsChecked == false && cbFilterFavCars.IsChecked == false))
                          )
                        {
                            favs = true;
                        }

                        if (cbFilterOwnBoth.IsChecked == true)
                        {
                            if (race.TrackOwned == true && race.Cars.Any(c => usercars.Any(u => u == c.CarId)))
                            {
                                own = true;
                            }
                        }
                        else
                        {
                            if ((cbFilterOwnTracks.IsChecked == true && race.TrackOwned == true) ||
                            (cbFilterOwnCars.IsChecked == true && race.Cars.Any(c => usercars.Any(u => u == c.CarId))) ||
                            (cbFilterOwnTracks.IsChecked == false && cbFilterOwnCars.IsChecked == false))
                            {
                                own = true;
                            }
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
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    MessageBox.Show("Error filter races: " + ex.InnerException.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show("Error filter races: " + ex.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void filterSeries()
        {
            if (tbMenu2tb.Text != "" || cbMenu4.IsChecked == false || cbMenu5.IsChecked == false)
            {
                List<dgObjects.seriesDataGrid> dgFilteredSeries = new List<dgObjects.seriesDataGrid>();
                dgFilteredSeries.Clear();
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    foreach (var ser in dgSeriesList)
                    {
                        bool toadd = false;
                        if (cbMenu4.IsChecked == true && (ser.Eligible == checksymbol))
                        {
                            toadd = true;
                        }
                        if (cbMenu5.IsChecked == true && (ser.Eligible != checksymbol))
                        {
                            toadd = true;
                        }
                        if (toadd)
                        {
                            foreach (string sername in tbMenu2tb.Text.Split(','))
                            {
                                if (ser.SeriesName.ToLower().Contains(sername.Trim().ToLower()))
                                {
                                    dgFilteredSeries.Add(ser);
                                }
                            }
                        }
                    }
                    gridSeries.ItemsSource = null;
                    gridSeries.ItemsSource = dgFilteredSeries;
                    gridSeries.UpdateLayout();
                }));
            }
            else
            {
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    gridSeries.ItemsSource = null;
                    gridSeries.ItemsSource = dgSeriesList;
                    gridSeries.UpdateLayout();
                }));
            }
        }
        private void filterCars()
        {
            if (tbMenu2tb.Text != "" || cbMenu4.IsChecked == false || cbMenu5.IsChecked == false)
            {
                List<dgObjects.carsDataGrid> dgFilteredCars = new List<dgObjects.carsDataGrid>();
                dgFilteredCars.Clear();
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    foreach (var car in dgCarsList)
                    {
                        bool toadd = false;
                        if (cbMenu4.IsChecked == true && (car.Owned == checksymbol))
                        {
                            toadd = true;
                        }
                        if (cbMenu5.IsChecked == true && (car.Owned != checksymbol))
                        {
                            toadd = true;
                        }
                        if (toadd)
                        {
                            foreach (string carname in tbMenu2tb.Text.Split(','))
                            {
                                if (car.CarName.ToLower().Contains(carname.Trim().ToLower()))
                                {
                                    dgFilteredCars.Add(car);
                                }
                            }
                        }
                    }
                    gridCars.ItemsSource = null;
                    gridCars.ItemsSource = dgFilteredCars;
                    gridCars.UpdateLayout();
                }));
            }
            else
            {
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    gridCars.ItemsSource = null;
                    gridCars.ItemsSource = dgCarsList;
                    gridCars.UpdateLayout();
                }));
            }
        }
        private void filterTracks() {
            if (tbMenu2tb.Text != "" || cbMenu4.IsChecked == false || cbMenu5.IsChecked == false)
            {
                List<dgObjects.tracksLayoutsDataGrid> dgFilteredTracks = new List<dgObjects.tracksLayoutsDataGrid>();
                dgFilteredTracks.Clear();
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    foreach (var track in dgTrackLayoutList)
                    {
                        bool toadd = false;
                        if (cbMenu4.IsChecked == true && (track.Owned == checksymbol))
                        {
                            toadd = true;
                        }
                        if (cbMenu5.IsChecked == true && (track.Owned != checksymbol))
                        {
                            toadd = true;
                        }
                        if (toadd)
                        {
                            foreach (string trname in tbMenu2tb.Text.Split(','))
                            {
                                if (track.Name.ToLower().Contains(trname.Trim().ToLower()))
                                {
                                    dgFilteredTracks.Add(track);
                                }
                            }
                        }
                    }
                    gridTracksLayout.ItemsSource = null;
                    gridTracksLayout.ItemsSource = dgFilteredTracks;
                    gridTracksLayout.UpdateLayout();
                }));
            }
            else
            {
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    gridTracksLayout.ItemsSource = null;
                    gridTracksLayout.ItemsSource = dgTrackLayoutList;
                    gridTracksLayout.UpdateLayout();
                }));
            }
        }
        private void generateAutoStartView()
        {

            dgAutoStartList.Clear();
            if (autoStartApps.Programs != null)
            {
                foreach (var prog in autoStartApps.Programs)
                {
                    dgObjects.autoStartDataGrid autoStartData = new dgObjects.autoStartDataGrid
                    {
                        ID = prog.ID,
                        Path = prog.Path
                    };

                    PixelFormat pf = PixelFormats.Bgr32;
                    int width = 16;
                    int height = 16;
                    int rawStride = (width * pf.BitsPerPixel + 7) / 8;
                    byte[] rawImage = new byte[rawStride * height];

                    // Initialize the image with data.
                    Random value = new Random();
                    value.NextBytes(rawImage);

                    var bmpSrc = BitmapSource.Create(width, height,
                                    16, 16, pf, null,
                                    rawImage, rawStride);
                    autoStartData.Name = prog.Path;
                    if (File.Exists(prog.Path))
                    {
                        var sysicon = System.Drawing.Icon.ExtractAssociatedIcon(prog.Path);
                        bmpSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(sysicon.Handle, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                        sysicon.Dispose();
                        autoStartData.Name = FileVersionInfo.GetVersionInfo(prog.Path).ProductName != null ? FileVersionInfo.GetVersionInfo(prog.Path).ProductName : FileVersionInfo.GetVersionInfo(prog.Path.ToString()).FileName;
                    }
                    autoStartData.Icon = bmpSrc;
                    autoStartData.Pause = prog.Paused ? pause : play;
                    autoStartData.withiRacing = prog.withiRacing ? checksymbol : unchecksymbol;
                    dgAutoStartList.Add(autoStartData);
                }
            }
            gridAutoStart.ItemsSource = null;
            gridAutoStart.ItemsSource = dgAutoStartList;
        }
        private async Task<DataTable> generateSeasonOverview(bool reload)
        {
            var dgSeriesL = dgSeriesList.Where(x => x.Favorite.Contains(favsymbolSelected)).ToList();
            List<dgObjects.seasonOverviewDataGrid> dgSeasonOverview = new List<dgObjects.seasonOverviewDataGrid>();
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Day");
            List<string> links = new List<string>();
            var YearQuater = new List<(int year, int quarter)>();
            var YearQuaterSeries = new List<(int year, int quarter, int serie)>();
            foreach (var ser in dgSeriesL)
            {
                dataTable.Columns.Add(ser.SeriesName);
                foreach (var tr in ser.Tracks)
                {
                    dgSeasonOverview.Add(new dgObjects.seasonOverviewDataGrid { SerieId = ser.SerieId, Seriesimage = ser.Seriesimage, SeriesName = ser.SeriesName, StartTime = Convert.ToDateTime(tr.Weekdate), TrackId = tr.TrackID, Track = tr.Name, TrackOwned = tr.Owned, Week = tr.Week, WeekActive = tr.WeekActive });
                }
                if (!YearQuaterSeries.Contains((ser.Season.season_year, ser.Season.season_quarter, ser.SerieId)))
                {
                    if (ser.Season.season_year != 0 && ser.Season.season_quarter != 0)
                    {
                        YearQuater.Add((ser.Season.season_year, ser.Season.season_quarter));
                        YearQuaterSeries.Add((ser.Season.season_year, ser.Season.season_quarter, ser.SerieId));
                    }
                }
            }
            if (seasonRaces.Count == 0 || reload)
            {
                foreach (var yq in YearQuater.Distinct())
                {
                    string url = "https://members-ng.iracing.com/data/results/search_series?season_year=" + yq.year + "&season_quarter=" + yq.quarter + "&cust_id=" + User.cust_id + "&official_only=true&event_types=5";
                    links.AddRange(await fData.getSerieSearchLinks(url));


                }
                foreach (var link in links)
                {
                    seasonRaces.AddRange(await fData.getSeriesSearchResults(link));
                }
            }
            if (participationCredits.Count() == 0)
            {
                participationCredits = await fData.getparticipationCredits();
            }
            dataTable.Columns.Add("WeekActive");
            DataRow row = dataTable.NewRow();
            row[0] = "";
            dataTable.Rows.Add(row);
            row = dataTable.NewRow();
            row[0] = "Series:";
            dataTable.Rows.Add(row);
            row = dataTable.NewRow();
            row[0] = "Credit Tracker:";
            dataTable.Rows.Add(row);
            foreach (var ser in dgSeriesL)
            {
                var partCr = participationCredits.FirstOrDefault(s => s.series_id == ser.SerieId);
                string pC = "";
                if (partCr != null)
                {
                    pC = partCr.weeks + @"/" + partCr.min_weeks + " for $" + partCr.participation_credits;
                }
                else
                {
                    pC = @"0/" + (ser.Season.max_weeks - ser.Season.drops);
                }
                dataTable.Rows[0][ser.SeriesName] = new BitmapImage(new Uri(ser.Seriesimage.AbsoluteUri));
                dataTable.Rows[1][ser.SeriesName] = ser.SeriesName;
                dataTable.Rows[2][ser.SeriesName] = pC;
            }
            if (!cbMenu2.IsChecked.Value)
            {
                List<int> Weektimes = dgSeasonOverview.Select(d => d.Week).Distinct().ToList();
                Weektimes.Sort((x, y) => x.CompareTo(y));
                var activeweeks = new List<(int series, int week)>();
                foreach (var ser in dgSeasonOverview.Where(a => a.WeekActive == true).ToList())
                {
                    if (!activeweeks.Any(s => s.series == ser.SerieId))
                    {
                        activeweeks.Add((ser.SerieId, ser.Week));
                    }
                }
                foreach (var week in (Weektimes))
                {

                    row = dataTable.NewRow();
                    row[0] = "W" + week.ToString();
                    foreach (var seasonweek in dgSeasonOverview.Where(w => w.Week == week).ToList())
                    {
                        var yearquar = YearQuaterSeries.FirstOrDefault(s => s.serie == seasonweek.SerieId);
                        string pref = neutral;
                        string active = seasonweek.WeekActive ? checksymbol : unchecksymbol;
                        if (seasonRaces.Where(r => r.track.track_id == seasonweek.TrackId && r.series_id == seasonweek.SerieId && r.season_year == yearquar.year && r.season_quarter == yearquar.quarter).Count() > 0)
                        {
                            pref = checksymbol;
                        }
                        try
                        {
                            if (seasonweek.Week < activeweeks.FirstOrDefault(s => s.series == seasonweek.SerieId).week && pref != checksymbol)
                            {
                                pref = unchecksymbol;
                            }
                        }
                        catch (Exception ex) { }

                    row[seasonweek.SeriesName] = pref + active + "00: " + seasonweek.Track;
                        row["WeekActive"] = seasonweek.WeekActive;
                    }

                    dataTable.Rows.Add(row);
                }
                return dataTable;
            }
            else
            {
                // ######################## List with Dates #############################

                List<DateTime> Weektimes = dgSeasonOverview.Select(d => d.StartTime).Distinct().ToList();
                Weektimes.Sort((x, y) => x.CompareTo(y));
                var activeweeks = new List<(int series, int week)>();
                foreach (var ser in dgSeasonOverview.Where(a => a.WeekActive == true).ToList())
                {
                    if (!activeweeks.Any(s => s.series == ser.SerieId))
                    {
                        activeweeks.Add((ser.SerieId, ser.Week));
                    }
                }
                foreach (var week in (Weektimes))
                {

                    row = dataTable.NewRow();
                    row[0] = week.Date.ToShortDateString();
                    foreach (dgObjects.seasonOverviewDataGrid seasonweek in dgSeasonOverview.Where(w => w.StartTime == week).ToList())
                    {
                        var yearquar = YearQuaterSeries.FirstOrDefault(s => s.serie == seasonweek.SerieId);
                        string pref = neutral;
                        string active = seasonweek.WeekActive ? checksymbol : unchecksymbol;
                        string test = seasonweek.WeekActive.ToString();
                        string test2 = seasonweek.Week.ToString() + seasonweek.SeriesName + seasonweek.SerieId;
                        if (seasonRaces.Where(r => r.track.track_id == seasonweek.TrackId && r.series_id == seasonweek.SerieId && r.season_year == yearquar.year && r.season_quarter == yearquar.quarter).Count() > 0)
                        {
                            pref = checksymbol;
                        }
                        try
                        {
                            if (seasonweek.Week < activeweeks.FirstOrDefault(s => s.series == seasonweek.SerieId).week && pref != checksymbol)
                            {
                                pref = unchecksymbol;
                            }
                        }
                        catch (Exception ex) { }
                        row[seasonweek.SeriesName] = pref + seasonweek.Week.ToString().PadLeft(2, '0') + ": " + seasonweek.Track;
                        row["WeekActive"] = seasonweek.WeekActive;
                    }

                    dataTable.Rows.Add(row);
                }
                return dataTable;
            }
        }
        private async void generateSeasonOverviewGrid(bool reload)
        {
            try
            {
                if (await fData.Login_API(Encoding.UTF8.GetBytes((username).ToLower()), Encoding.UTF8.GetBytes(helper.ToInsecureString(password)), false) == 200)
                {
                    DataTable view = await generateSeasonOverview(reload);
                    gridSeasonOverview.Children.Clear();
                    Grid grid = new Grid();
                    grid.ShowGridLines = false;
                    Border border = new Border();
                    ColumnDefinition column1 = new ColumnDefinition();
                    RowDefinition row1 = new RowDefinition();
                    TextBlock cell1 = new TextBlock();
                    int cols = 0;
                    var tracklist = dgTracksList.Where(t => t.Owned == checksymbol).ToList();

                    foreach (var line in view.Rows)
                    {
                        if (((System.Data.DataRow)line).ItemArray.Length > cols) { cols = ((System.Data.DataRow)line).ItemArray.Length; }
                    }
                    for (int c = 0; c < cols; c++)
                    {
                        column1 = new ColumnDefinition();
                        if (c == cols - 1)
                        {
                            column1.Width = new GridLength(80, GridUnitType.Star);
                        }
                        grid.ColumnDefinitions.Add(column1);

                    }
                    int rowcount = 0;
                    foreach (var line in view.Rows)
                    {
                        row1 = new RowDefinition();
                        grid.RowDefinitions.Add(row1);
                        int colcount = 0;
                        foreach (var cell in ((System.Data.DataRow)line).ItemArray)
                        {

                            if (cell.ToString().StartsWith("file:"))
                            {
                                Image logo = new Image();
                                logo.Source = new BitmapImage(new Uri(cell.ToString()));
                                logo.Width = 100;
                                RenderOptions.SetBitmapScalingMode(logo, BitmapScalingMode.HighQuality);
                                Grid.SetColumn(logo, colcount);
                                Grid.SetRow(logo, rowcount);
                                grid.Children.Add(logo);
                            }
                            else
                            {
                                border = new Border();
                                border.BorderBrush = Brushes.Black;
                                border.MinWidth = 80;
                                cell1 = new TextBlock();
                                if (cell.ToString().StartsWith(checksymbol))
                                {
                                    cell1.TextDecorations = TextDecorations.Strikethrough;
                                }
                                if (colcount < cols - 1)
                                {
                                    cell1.Text = cell.ToString().Replace(neutral, "").Replace(checksymbol, "").Replace(unchecksymbol, "").Replace("00: ", "");
                                }
                                cell1.TextTrimming = TextTrimming.WordEllipsis;
                                cell1.Margin = new Thickness(3, 0, 5, 0);
                                if (Regex.Match(cell.ToString(), @"^[" + checksymbol + unchecksymbol + neutral + @"]*\d*?[:]\W").Success)
                                {
                                    cell1.TextAlignment = TextAlignment.Left;
                                }
                                else
                                {
                                    cell1.TextAlignment = TextAlignment.Center;
                                }
                                cell1.VerticalAlignment = VerticalAlignment.Center;
                                if (colcount != cols - 1)
                                {
                                    border.MaxWidth = 180;
                                }

                                if (rowcount < 2)
                                {
                                    border.BorderThickness = new Thickness(0, 0, 0, 0);
                                }
                                else
                                {
                                    border.BorderThickness = new Thickness(0, 0, 0, 1);
                                }
                                border.Height = 30;
                                if (tracklist.Any(t => t.Name.Equals(Regex.Replace(cell.ToString(), @"^[" + checksymbol + unchecksymbol + neutral + @"]*\d*?[:]\W", ""))))
                                {
                                    cell1.Foreground = Application.Current.Resources["BrushMiddleGreen"] as SolidColorBrush;
                                    if (cell.ToString().StartsWith(unchecksymbol))
                                    {
                                        cell1.Foreground = Application.Current.Resources["BrushDarkerGreen"] as SolidColorBrush;
                                    }
                                }
                                else
                                {
                                    cell1.Foreground = Application.Current.Resources["BrushTextWhite"] as SolidColorBrush;
                                    if (cell.ToString().StartsWith(unchecksymbol))
                                    {
                                        cell1.Foreground = Application.Current.Resources["BrushGray"] as SolidColorBrush;
                                    }
                                }
                                if (rowcount % 2 == 0 && rowcount > 2)
                                {
                                    border.Background = Application.Current.Resources["BrushOddBackground"] as SolidColorBrush;
                                }
                                if (cbMenu2.IsChecked == true)
                                {
                                    if (((System.Data.DataRow)line).ItemArray[cols - 1] != null && ((System.Data.DataRow)line).ItemArray[cols - 1] != DBNull.Value && Convert.ToBoolean(((System.Data.DataRow)line).ItemArray[cols - 1]))
                                    {
                                        border.Background = Application.Current.Resources["BrushGridHighlightWhite"] as SolidColorBrush;
                                        cell1.FontWeight = FontWeights.ExtraBold;

                                    }
                                }
                                else
                                {
                                    if (cell.ToString().Length > 2 && cell.ToString().Substring(1).StartsWith(checksymbol))
                                    {
                                        border.Background = Application.Current.Resources["BrushGridHighlightWhite"] as SolidColorBrush;
                                        cell1.FontWeight = FontWeights.ExtraBold;
                                    }
                                }
                                if (cell1.TextDecorations == TextDecorations.Strikethrough)
                                {
                                    border.Background = Application.Current.Resources["BrushGridHighlightBrightYellow"] as SolidColorBrush;
                                    cell1.Foreground = Application.Current.Resources["BrushDarkGray"] as SolidColorBrush;
                                }

                                border.Child = cell1;
                                Grid.SetColumn(border, colcount);
                                Grid.SetRow(border, rowcount);
                                grid.Children.Add(border);
                            }

                            colcount++;
                        }
                        rowcount++;
                    }
                    row1 = new RowDefinition();
                    row1.Height = new GridLength(30, GridUnitType.Star);
                    grid.RowDefinitions.Add(row1);
                    gridSeasonOverview.Children.Add(grid);
                }
            }
            catch (Exception ex) { }
        }
        private async void generatePartnerSlider()
        {
            partners = await fData.getPartner();
            Style s = (Style)this.FindResource("DefaultTextBlockStyle");
            spPartners.Children.Clear();
            foreach (partner part in partners)
            {
                if (part.date > DateTime.Now)
                {
                    if (!String.IsNullOrEmpty(part.picture.ToString()))
                    {
                        Image partnerImage = new Image();
                        partnerImage.Source = new BitmapImage(new Uri(part.picture.ToString()));
                        partnerImage.Height = 40;
                        partnerImage.MaxWidth = 100;
                        spPartners.Children.Add(partnerImage);
                    }
                    if (!String.IsNullOrEmpty(part.name.ToString()) && !String.IsNullOrEmpty(part.url.ToString()))
                    {
                        TextBlock partnerText = new TextBlock();
                        partnerText.Style = s;
                        Hyperlink partnerLink = new Hyperlink();
                        partnerLink.NavigateUri = new Uri(part.url);
                        partnerLink.Inlines.Add(part.name + "        ");
                        partnerLink.RequestNavigate += link_Click;
                        partnerLink.Style = (Style)this.FindResource("DefaultLinkStyle");
                        partnerText.Inlines.Add(partnerLink);
                        spPartners.Children.Add(partnerText);
                    }
                }
            }
            int sliderTimer = 30 + (partners.Count() * 3);
            this.Resources["AnimationTime"] = (Duration)new TimeSpan(0, 0, sliderTimer);
            if (Properties.Settings.Default.PartnerSlider)
            {
                this.Resources["RepeatBehavior"] = RepeatBehavior.Forever;
                partnerSliderHide = false;
                spPartners.Visibility = Visibility.Visible;

            }
            else
            {
                this.Resources["RepeatBehavior"] = (RepeatBehavior)new RepeatBehavior(2);
            }
        }
        
        private List<int> getPurchasedItems()
        {
            var tracks = User.track_packages.Select(t => t.package_id);
            var cars = User.car_packages.Select(c => c.package_id);
            int lowpriceTracks = 0;
            int purchasedTracks = 0;
            int lowpriceCars = 0;
            int purchasedCars = 0;
            var trackPrices = (tracksList.Where(p => p.price > 0).Select(p=> p.price)).Distinct();
            var carPrices = (carsList.Where(p => p.price > 0).Select(p => p.price)).Distinct();
            foreach(var item in tracks)
            {
                var track = tracksList.FirstOrDefault(t => t.package_id == item);

                if(track.price > 5)
                {
                    purchasedTracks++;
                }
                else if(track.price > 0)
                {
                    lowpriceTracks++;
                }
            }
            foreach(var item in cars)
            {
                var car = carsList.FirstOrDefault(c => c.package_id == item);
                if(car != null && car.price > 3)
                {
                    purchasedCars++;
                }
                else if(car != null && car.price > 0)
                {
                    lowpriceCars++;
                }
            }
            int purchaseableTracks = tracksList.Where(t => t.price > 0 && t.purchasable).GroupBy(t => t.package_id).Count();
            int purchaseableCars = carsList.Where(c => c.price > 0).GroupBy(c => c.package_id).Count();
            return new List<int>() { purchasedTracks, purchasedCars, purchaseableTracks, purchaseableCars, lowpriceTracks, lowpriceCars};
        }

        private void clearDetails()
        {
            lblDetails1.Content = "";
            lblDetails2.Content = "";
            lblDetails3.Content = "";
            lblDetails4.Content = "";
            lblDetails5.Content = "";
            lblDetails6.Content = "";
            lblDetails7.Content = "";
            lblDetails8.Content = "";
            tbDetail1.Text = "";
            tbDetail2.Text = "";
            tbDetail3.Text = "";
            tbDetail4.Text = "";
            tbDetail5.Text = "";
            tbDetail6.Text = "";
            tbDetail7.Visibility = Visibility.Hidden;
            tbDetail8.Visibility = Visibility.Hidden;
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
        private void dataGrid_ExtendCollapseDetails(object sender, MouseButtonEventArgs e)
        {
            if (!((System.Windows.Controls.DataGrid)sender).IsMouseCaptureWithin)
            {
                if (((System.Windows.Controls.DataGrid)sender).RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.Collapsed)
                {
                    ((System.Windows.Controls.DataGrid)sender).RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
                }
                else
                {
                    ((System.Windows.Controls.DataGrid)sender).RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
                }
            }
        }
        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((DataGrid)sender).SelectedItem != null || ((DataGrid)sender).SelectedIndex != -1)
            {
                if (((System.Windows.Controls.DataGrid)sender).RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.Collapsed)
                {
                    ((System.Windows.Controls.DataGrid)sender).RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
                }
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
                        lblDetails8.Content = "Forum Link:";
                        tbDetail8.Visibility = Visibility.Visible;
                        lblDetails7.Content = "🛒 Link:";
                        tbDetail7.Visibility = Visibility.Visible;
                        if (((RCRPlanner.dgObjects.carsDataGrid)((DataGrid)sender).SelectedItem).CarId != null)
                        {
                            string url = "https://members-ng.iracing.com/shop/cars?carId=" + ((RCRPlanner.dgObjects.carsDataGrid)((DataGrid)sender).SelectedItem).CarId;
                            tbDetail7Link.NavigateUri = new Uri(url);
                            tbDetail7Link.ToolTip = url;
                        }
                        else
                        {
                            tbDetail7.Visibility = Visibility.Hidden;
                        }
                        if (((RCRPlanner.dgObjects.carsDataGrid)((DataGrid)sender).SelectedItem).ForumLink != null){
                            tbDetail8Link.NavigateUri = new Uri(((RCRPlanner.dgObjects.carsDataGrid)((DataGrid)sender).SelectedItem).ForumLink);
                            tbDetail8Link.ToolTip = ((RCRPlanner.dgObjects.carsDataGrid)((DataGrid)sender).SelectedItem).ForumLink;
                        }
                        else
                        {
                            tbDetail8.Visibility = Visibility.Hidden;
                        }
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
                            lblDetails7.Content = "🛒 Link:";
                            tbDetail7.Visibility = Visibility.Visible;
                            if (((RCRPlanner.dgObjects.tracksLayoutsDataGrid)((DataGrid)sender).SelectedItem).TrackID != null)
                            {
                                string url = "https://members.iracing.com/membersite/member/TrackDetail.do?trkid=" + ((RCRPlanner.dgObjects.tracksLayoutsDataGrid)((DataGrid)sender).SelectedItem).TrackID;
                                tbDetail7Link.NavigateUri = new Uri(url);
                                tbDetail7Link.ToolTip = url;
                            }
                            else
                            {
                                tbDetail7.Visibility = Visibility.Hidden;
                            }
                        }
                        break;
                    case "gridSeries":
                        scrollDataGridIntoView(sender);
                        break;
                    case "gridRaces":
                        scrollDataGridIntoView(sender);
                        break;
                    case "gridTracks":
                        tbDetail3.Text = ((dgObjects.tracksDataGrid)((DataGrid)sender).SelectedItem).Corners.ToString();
                        lblDetails3.Content = "Corners:";
                        tbDetail2.Text = ((dgObjects.tracksDataGrid)((DataGrid)sender).SelectedItem).Pitlimit.ToString() + (isMetric ? "km/h" : "mph");
                        lblDetails2.Content = "Pit limit:";
                        tbDetail1.Text = ((dgObjects.tracksDataGrid)((DataGrid)sender).SelectedItem).Length.ToString() + (isMetric ? "km" : "m");
                        lblDetails1.Content = "Length:";
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
            if (!(sender is Control control))
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
        private void gridPartStat_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            e.Column.Header = ((sender as DataGrid).ItemsSource as DataView).Table.Columns[e.PropertyName].Caption;
        }

        private void btnLoadCars_Click(object sender, RoutedEventArgs e)
        {
            activeGrid = "gridCars";
            gridSeasonOverview.Visibility = Visibility.Hidden;
            scrollSeasonOverview.Visibility = Visibility.Hidden;
            btnMenu1.Visibility = Visibility.Visible;
            cbMenu2.Visibility = Visibility.Hidden;
            tbMenu2.Visibility = Visibility.Visible;
            dpMenu2.Visibility = Visibility.Hidden;
            lbMenu2.Visibility = Visibility.Hidden;
            cbMenu3.Visibility = Visibility.Hidden;
            dpMenu3.Visibility = Visibility.Hidden;
            dpMenu3Date.Visibility = Visibility.Hidden;
            cbMenu4.Visibility = Visibility.Visible;
            dpMenu4.Visibility = Visibility.Hidden;
            cbMenu5.Visibility = Visibility.Visible;
            dpMenu5.Visibility = Visibility.Hidden;
            cbMenu6.Visibility = Visibility.Hidden;
            tbMenu6.Visibility = Visibility.Hidden;
            dpMenu6.Visibility = Visibility.Hidden;
            btnMenu1.Content = magnifier;
            btnMenu1.Width = 40;
            btnMenu1.HorizontalAlignment = HorizontalAlignment.Right;
            tbMenu2lb.Content = "Car name:";
            tbMenu2tb.Text = "";
            cbMenu4.Content = "Owned cars";
            cbMenu4.IsChecked = true;
            cbMenu5.Content = "Not owned cars";
            cbMenu5.IsChecked = true;
            cbMenu5.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("White"));
            clearDetails();
            stackPanelMenuClose_MouseDown(null, null);
            switchMainGridVisibility(new List<System.Windows.Controls.DataGrid>{ gridCars, gridCarDetail } , true);
            filterCars();

        }

        private void btnLoadTracks_Click(object sender, RoutedEventArgs e)
        {
            activeGrid = "gridTracksLayout";
            gridSeasonOverview.Visibility = Visibility.Hidden;
            scrollSeasonOverview.Visibility = Visibility.Hidden;
            btnMenu1.Visibility = Visibility.Visible;
            cbMenu2.Visibility = Visibility.Hidden;
            tbMenu2.Visibility = Visibility.Visible;
            dpMenu2.Visibility = Visibility.Hidden;
            lbMenu2.Visibility = Visibility.Hidden;
            cbMenu3.Visibility = Visibility.Hidden;
            dpMenu3.Visibility = Visibility.Hidden;
            dpMenu3Date.Visibility = Visibility.Hidden;
            cbMenu4.Visibility = Visibility.Visible;
            dpMenu4.Visibility = Visibility.Hidden;
            cbMenu5.Visibility = Visibility.Visible;
            dpMenu5.Visibility = Visibility.Hidden;
            cbMenu6.Visibility = Visibility.Hidden;
            tbMenu6.Visibility = Visibility.Hidden;
            dpMenu6.Visibility = Visibility.Hidden;
            btnMenu1.Content = magnifier;
            btnMenu1.Width = 40;
            btnMenu1.HorizontalAlignment = HorizontalAlignment.Right;
            tbMenu2lb.Content = "Track name:";
            tbMenu2tb.Text = "";
            cbMenu4.Content = "Owned tracks";
            cbMenu4.IsChecked = true;
            cbMenu5.Content = "Not owned tracks";
            cbMenu5.IsChecked = true;
            cbMenu5.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("White"));
            clearDetails();
            stackPanelMenuClose_MouseDown(null, null);
            switchMainGridVisibility(new List<System.Windows.Controls.DataGrid> { gridTracksLayout, gridTrackDetail }, true);
            filterTracks();
        }

        private void btnLoadPurchase_Click(object sender, RoutedEventArgs e)
        {
            activeGrid = "gridPurchaseGuide";
            gridSeasonOverview.Visibility = Visibility.Hidden;
            scrollSeasonOverview.Visibility = Visibility.Hidden;
            btnMenu1.Visibility = Visibility.Visible;
            cbMenu2.Visibility = Visibility.Visible;
            tbMenu2.Visibility = Visibility.Hidden;
            dpMenu2.Visibility = Visibility.Hidden;
            lbMenu2.Visibility = Visibility.Hidden;
            cbMenu3.Visibility = Visibility.Hidden;
            dpMenu3.Visibility = Visibility.Hidden;
            dpMenu3Date.Visibility = Visibility.Hidden;
            cbMenu4.Visibility = Visibility.Hidden;
            dpMenu4.Visibility = Visibility.Hidden;
            cbMenu5.Visibility = Visibility.Hidden;
            dpMenu5.Visibility = Visibility.Hidden;
            cbMenu6.Visibility = Visibility.Hidden;
            tbMenu6.Visibility = Visibility.Hidden;
            dpMenu6.Visibility = Visibility.Hidden;
            btnMenu1.Content = "Reload";
            btnMenu1.Width = 80;
            btnMenu1.HorizontalAlignment = HorizontalAlignment.Center;
            cbMenu2.Content = "only if owned track in a series <8";
            clearDetails();
            generatePurchaseGuideView();
            stackPanelMenuClose_MouseDown(null, null);
            switchMainGridVisibility(new List<System.Windows.Controls.DataGrid> { gridPurchaseGuide }, false);
        }

        private void btnLoadSeries_Click(object sender, RoutedEventArgs e)
        {
            activeGrid = "gridSeries";
            gridSeasonOverview.Visibility = Visibility.Hidden;
            scrollSeasonOverview.Visibility = Visibility.Hidden;
            btnMenu1.Visibility = Visibility.Visible;
            cbMenu2.Visibility = Visibility.Hidden;
            tbMenu2.Visibility = Visibility.Visible;
            dpMenu2.Visibility = Visibility.Hidden;
            lbMenu2.Visibility = Visibility.Hidden;
            cbMenu3.Visibility = Visibility.Hidden;
            dpMenu3.Visibility = Visibility.Hidden;
            dpMenu3Date.Visibility = Visibility.Hidden;
            cbMenu4.Visibility = Visibility.Visible;
            dpMenu4.Visibility = Visibility.Hidden;
            cbMenu5.Visibility = Visibility.Visible;
            dpMenu5.Visibility = Visibility.Hidden;
            cbMenu6.Visibility = Visibility.Hidden;
            tbMenu6.Visibility = Visibility.Hidden;
            dpMenu6.Visibility = Visibility.Hidden;
            btnMenu1.Content = magnifier;
            btnMenu1.Width = 40;
            btnMenu1.HorizontalAlignment = HorizontalAlignment.Right;
            tbMenu2lb.Content = "Series name:";
            tbMenu2tb.Text = "";
            cbMenu4.Content = "Eligible series";
            cbMenu4.IsChecked = true;
            cbMenu5.Content = "Not eligible series";
            cbMenu5.IsChecked = true;
            cbMenu5.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("White"));
            stackPanelMenuClose_MouseDown(null, null);
            switchMainGridVisibility(new List<System.Windows.Controls.DataGrid> { gridSeries }, false);
            filterSeries();
        }

        private void btnLoadRaces_Click(object sender, RoutedEventArgs e)
        {
            generateRaceView();
            activeGrid = "gridRaces";
            gridSeasonOverview.Visibility = Visibility.Hidden;
            scrollSeasonOverview.Visibility = Visibility.Hidden;
            btnMenu1.Visibility = Visibility.Visible;
            cbMenu2.Visibility = Visibility.Hidden;
            tbMenu2.Visibility = Visibility.Hidden;
            dpMenu2.Visibility = Visibility.Visible;
            lbMenu2.Visibility = Visibility.Visible;
            cbMenu3.Visibility = Visibility.Hidden;
            dpMenu3.Visibility = Visibility.Hidden;
            dpMenu3Date.Visibility = Visibility.Visible;
            ddMenu3.Visibility = Visibility.Hidden;
            cbMenu4.Visibility = Visibility.Hidden;
            cbMenu5.Visibility = Visibility.Hidden;
            dpMenu4.Visibility = Visibility.Hidden;
            dpMenu5.Visibility = Visibility.Hidden;
            cbMenu6.Visibility = Visibility.Hidden;
            tbMenu6.Visibility = Visibility.Visible;
            dpMenu6.Visibility = Visibility.Hidden;
            btnMenu1.Content = "Show filter";
            btnMenu1.Width = 100;
            btnMenu1.HorizontalAlignment = HorizontalAlignment.Center;
            ddMenu2.ItemsSource = cbAlarms;
            ddMenu2.SelectedIndex = Properties.Settings.Default.defaultTimer;
            lbMenu2.Content = "Alarm offset:";
            lbMenu3Date.Content = "Date:";

            stackPanelMenuClose_MouseDown(null, null);

            switchMainGridVisibility(new List<System.Windows.Controls.DataGrid> { gridRaces }, false);
        }
        private void btnPartStats_Click(object sender, RoutedEventArgs e)
        {
            activeGrid = "gridPartStat";
            gridSeasonOverview.Visibility = Visibility.Hidden;
            scrollSeasonOverview.Visibility = Visibility.Hidden;
            tbMenu2.Visibility = Visibility.Hidden;
            dpMenu2.Visibility = Visibility.Visible;
            lbMenu2.Visibility = Visibility.Visible;
            dpMenu3.Visibility = Visibility.Visible;
            dpMenu3Date.Visibility = Visibility.Hidden;
            ddMenu3.Visibility = Visibility.Visible;
            dpMenu4.Visibility = Visibility.Visible;
            dpMenu5.Visibility = Visibility.Visible;
            btnMenu1.Visibility = Visibility.Visible;
            cbMenu2.Visibility = Visibility.Hidden;
            cbMenu3.Visibility = Visibility.Hidden;
            cbMenu4.Visibility = Visibility.Hidden;
            cbMenu5.Visibility = Visibility.Hidden;
            cbMenu6.Visibility = Visibility.Hidden;
            tbMenu6.Visibility = Visibility.Hidden;
            dpMenu6.Visibility = Visibility.Hidden;
            btnMenu1.Content = "Loading...";
            btnMenu1.Width = 100;
            btnMenu1.HorizontalAlignment = HorizontalAlignment.Center;
            btnMenu1.IsEnabled = false;
            ddMenu2.SelectedIndex = -1;
            ddMenu3.Items.Clear();
            ddMenu4.Items.Clear();
            lbMenu2.Content = "Series:";
            lbMenu3.Content = "Year:";
            lbMenu4.Content = "Season:";
            lbMenu5.Content = "Week:";
            ddMenu5.Items.Clear();
            ddMenu2.ItemsSource = cbSeries;
            stackPanelMenuClose_MouseDown(null, null);
            switchMainGridVisibility(new List<System.Windows.Controls.DataGrid> { gridPartStat }, false);
            btnMenu1.Content = "Get stat";
            btnMenu1.IsEnabled = true;
        }
        private void btniRatingStats_Click(object sender, RoutedEventArgs e)
        {
            activeGrid = "gridiRatingStat";
            gridSeasonOverview.Visibility = Visibility.Hidden;
            scrollSeasonOverview.Visibility = Visibility.Hidden;
            tbMenu2.Visibility = Visibility.Hidden;
            dpMenu2.Visibility = Visibility.Visible;
            lbMenu2.Visibility = Visibility.Visible;
            dpMenu3.Visibility = Visibility.Visible;
            dpMenu3Date.Visibility = Visibility.Hidden;
            ddMenu3.Visibility = Visibility.Visible;
            dpMenu4.Visibility = Visibility.Visible;
            dpMenu5.Visibility = Visibility.Hidden;
            btnMenu1.Visibility = Visibility.Visible;
            cbMenu2.Visibility = Visibility.Hidden;
            cbMenu3.Visibility = Visibility.Hidden;
            cbMenu4.Visibility = Visibility.Hidden;
            cbMenu5.Visibility = Visibility.Hidden;
            cbMenu6.Visibility = Visibility.Hidden;
            tbMenu6.Visibility = Visibility.Hidden;
            dpMenu6.Visibility = Visibility.Hidden;
            btnMenu1.Content = "Loading...";
            btnMenu1.Width = 80;
            btnMenu1.HorizontalAlignment = HorizontalAlignment.Center;
            btnMenu1.IsEnabled = false;
            ddMenu2.SelectedIndex = -1;
            ddMenu3.Items.Clear();
            ddMenu4.Items.Clear();
            lbMenu2.Content = "Series:";
            lbMenu3.Content = "Season:";
            lbMenu4.Content = "Car class:";
            ddMenu2.ItemsSource = cbSeries;
            stackPanelMenuClose_MouseDown(null, null);
            switchMainGridVisibility(new List<System.Windows.Controls.DataGrid> { gridiRatingStat }, false);
            btnMenu1.Content = "Get stat";
            btnMenu1.IsEnabled = true;
        }
        private void btnLoadAutoStart_Click(object sender, RoutedEventArgs e)
        {

            activeGrid = "gridAutoStart";
            gridSeasonOverview.Visibility = Visibility.Hidden;
            scrollSeasonOverview.Visibility = Visibility.Hidden;
            btnMenu1.Visibility = Visibility.Visible;
            cbMenu2.Visibility = Visibility.Visible;
            tbMenu2.Visibility = Visibility.Hidden;
            dpMenu2.Visibility = Visibility.Hidden;
            lbMenu2.Visibility = Visibility.Hidden;
            cbMenu3.Visibility = Visibility.Visible;
            dpMenu3.Visibility = Visibility.Hidden;
            dpMenu3Date.Visibility = Visibility.Hidden;
            cbMenu4.Visibility = Visibility.Visible;
            dpMenu4.Visibility = Visibility.Hidden;
            cbMenu5.Visibility = Visibility.Visible;
            dpMenu5.Visibility = Visibility.Hidden;
            cbMenu6.Visibility = Visibility.Hidden;
            tbMenu6.Visibility = Visibility.Hidden;
            dpMenu6.Visibility = Visibility.Visible;
            cbMenu2.IsChecked = autoStartApps.Active;
            btnMenu1.Content = "Add program";
            btnMenu1.Width = 100;
            btnMenu1.HorizontalAlignment = HorizontalAlignment.Center;
            cbMenu2.Content = "Auto start programs on launch?";
            cbMenu3.Content = "Start programs minimized?";
            cbMenu3.IsChecked = autoStartApps.Minimized;
            cbMenu4.Content = "Kill programs on close?";
            cbMenu4.IsChecked = autoStartApps.Kill;
            cbMenu5.Content = "Kill programs by Name?";
            cbMenu5.ToolTip = "Can lead to data loss, since all programs with the specified program name will be closed!";
            cbMenu5.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("Yellow"));
            cbMenu5.IsChecked = autoStartApps.KillByName;
            tbMenu6tb.Text = Properties.Settings.Default.delayTime.ToString();
            btnMenu6tb.Text = save;
            tbMenu6lb.Content = "Time to wait for iRacing:";
            tbMenu6lb.ToolTip = "Time in seconds.";
            clearDetails();
            stackPanelMenuClose_MouseDown(null, null);
            generateAutoStartView();
            switchMainGridVisibility(new List<System.Windows.Controls.DataGrid> { gridAutoStart }, true);
        }
        private void btnLoadSeasonOverview_Click(object sender, RoutedEventArgs e)
        {
            activeGrid = "gridSeasonOverview";
            btnMenu1.Visibility = Visibility.Hidden;
            cbMenu2.Visibility = Visibility.Visible;
            tbMenu2.Visibility = Visibility.Hidden;
            dpMenu2.Visibility = Visibility.Hidden;
            lbMenu2.Visibility = Visibility.Hidden;
            cbMenu3.Visibility = Visibility.Hidden;
            dpMenu3.Visibility = Visibility.Hidden;
            dpMenu3Date.Visibility = Visibility.Hidden;
            cbMenu4.Visibility = Visibility.Hidden;
            cbMenu5.Visibility = Visibility.Hidden;
            dpMenu4.Visibility = Visibility.Hidden;
            dpMenu5.Visibility = Visibility.Hidden;
            cbMenu6.Visibility = Visibility.Hidden;
            dpMenu6.Visibility = Visibility.Hidden;
            tbMenu6.Visibility = Visibility.Visible;
            cbMenu2.Content = "Sort by date?";
            cbMenu2.IsChecked = Properties.Settings.Default.SortDate;
            generateSeasonOverviewGrid(false);
            stackPanelMenuClose_MouseDown(null, null);
            //generateSeasonOverview(false);
            switchMainGridVisibility(new List<System.Windows.Controls.DataGrid> { null }, false);
            gridSeasonOverview.Visibility = Visibility.Visible;
            scrollSeasonOverview.Visibility = Visibility.Visible;
        }
        private void btnParnter_Click(object sender, RoutedEventArgs e)
        {
            activeGrid = "gridPartner";
            btnMenu1.Visibility = Visibility.Hidden;
            cbMenu2.Visibility = Visibility.Visible;
            tbMenu2.Visibility = Visibility.Hidden;
            dpMenu2.Visibility = Visibility.Hidden;
            lbMenu2.Visibility = Visibility.Hidden;
            cbMenu3.Visibility = Visibility.Hidden;
            dpMenu3.Visibility = Visibility.Hidden;
            dpMenu3Date.Visibility = Visibility.Hidden;
            cbMenu4.Visibility = Visibility.Hidden;
            cbMenu5.Visibility = Visibility.Hidden;
            dpMenu4.Visibility = Visibility.Hidden;
            dpMenu5.Visibility = Visibility.Hidden;
            cbMenu6.Visibility = Visibility.Hidden;
            dpMenu6.Visibility = Visibility.Hidden;
            tbMenu6.Visibility = Visibility.Hidden;
            cbMenu2.Content = "Run partner slider (App restart required)";
            cbMenu2.IsChecked = Properties.Settings.Default.PartnerSlider;
            stackPanelMenuClose_MouseDown(null, null);
            switchMainGridVisibility(new List<System.Windows.Controls.DataGrid> { gridPartner }, false);
            
        }
        private void btnstartPrograms_Click(object sender, RoutedEventArgs e)
        {
            startPrograms();
            processWatcher.StartWatching();
        }

        private void btnstopPrograms_Click(object sender, RoutedEventArgs e)
        {
            stopPrograms();
            processWatcher.StopWatching();
        }
        private void gridSeriesFavorite_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int ID = ((RCRPlanner.dgObjects.seriesDataGrid)((System.Windows.FrameworkElement)sender).DataContext).SerieId;
            if (((System.Windows.Controls.TextBlock)sender).Text != favsymbolSelected)
            {
                dgSeriesList.First(r => r.SerieId == ID).Favorite = favsymbolSelected;
                favoriteSeries.Add(new memberInfo.FavoriteSeries { series_id = ID });
            }
            else
            {
                dgSeriesList.First(r => r.SerieId == ID).Favorite = favsymbolUnselected;
                var fav = favoriteSeries.FirstOrDefault(i => i.series_id == ID);
                favoriteSeries.Remove(fav);
            }
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => {
                if (gridSeries.ItemsSource != null)
                {
                    CollectionViewSource.GetDefaultView(gridSeries.ItemsSource).Refresh();
                }
            }));
        }
        private void gridCarsFavorite_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int ID = ((RCRPlanner.dgObjects.carsDataGrid)((System.Windows.FrameworkElement)sender).DataContext).CarId;
            if (((System.Windows.Controls.TextBlock)sender).Text != favsymbolSelected)
            {
                dgCarsList.First(r => r.CarId == ID).Favorite = favsymbolSelected;
                favoriteCars.Add(new memberInfo.FavoriteCars { car_id = ID });
            }
            else
            {
                dgCarsList.First(r => r.CarId == ID).Favorite = favsymbolUnselected;
                var fav = favoriteCars.FirstOrDefault(i => i.car_id == ID);
                favoriteCars.Remove(fav);
            }
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => {
                CollectionViewSource.GetDefaultView(gridCars.ItemsSource).Refresh();
            }));
        }

        private void gridTracksFavorite_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int ID = ((RCRPlanner.dgObjects.tracksLayoutsDataGrid)((System.Windows.FrameworkElement)sender).DataContext).PackageID;
            if (((System.Windows.Controls.TextBlock)sender).Text != favsymbolSelected)
            {
                dgTrackLayoutList.First(r => r.PackageID == ID).Favorite = favsymbolSelected;
                favoriteTracks.Add(new memberInfo.FavoriteTracks { track_id = ID });
            }
            else
            {
                dgTrackLayoutList.First(r => r.PackageID == ID).Favorite = favsymbolUnselected;
                var fav = favoriteTracks.FirstOrDefault(i => i.track_id == ID);
                favoriteTracks.Remove(fav);
            }
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => {
                CollectionViewSource.GetDefaultView(gridTracksLayout.ItemsSource).Refresh();
            }));

        }

        private void gridRacesAlarm_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int offset = Convert.ToInt32(ddMenu2.SelectedValue.ToString().Replace("i", ""));
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
        private void gridRacesHated2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            int serid = -1;
            int traid = -1;
            string sympathy = "";
            bool save = false;
            if(((System.Windows.Controls.TextBlock)sender).Text == neutral)
            {
                sympathy = thumbdown;
            }
            if (((System.Windows.Controls.TextBlock)sender).Text == thumbdown)
            {
                sympathy = thumbup;
            }
            if (((System.Windows.Controls.TextBlock)sender).Text == thumbup)
            {
                sympathy = neutral;
            }
            var _type = ((System.Windows.FrameworkElement)sender).DataContext.GetType();
            if (_type.Name == "RaceOverviewDataGrid")
            {
                serid = ((RCRPlanner.dgObjects.RaceOverviewDataGrid)((System.Windows.FrameworkElement)sender).DataContext).SerieId;
                traid = ((RCRPlanner.dgObjects.RaceOverviewDataGrid)((System.Windows.FrameworkElement)sender).DataContext).TrackTrackID;
                dgRaceOverviewList.First(r => r.SerieId == serid).Sympathy = ((System.Windows.Controls.TextBlock)sender).Text = sympathy ;
                dgRaceOverviewList.First(r => r.SerieId == serid).Tracks.First(t => t.TrackID == traid).Sympathy = ((System.Windows.Controls.TextBlock)sender).Text = sympathy;
                save = true;
            }
            else if(_type.Name == "tracksDataGrid")
            {
                serid = ((RCRPlanner.dgObjects.tracksDataGrid)((System.Windows.FrameworkElement)sender).DataContext).Series[0].SerieId;
                traid = ((RCRPlanner.dgObjects.tracksDataGrid)((System.Windows.FrameworkElement)sender).DataContext).TrackID;
                dgRaceOverviewList.First(r => r.SerieId == serid).Tracks.First(t => t.TrackID == traid).Sympathy = ((System.Windows.Controls.TextBlock)sender).Text = sympathy;
                if (((RCRPlanner.dgObjects.tracksDataGrid)((System.Windows.FrameworkElement)sender).DataContext).WeekActive)
                {
                    dgRaceOverviewList.First(r => r.SerieId == serid).Sympathy = ((System.Windows.Controls.TextBlock)sender).Text = sympathy;
                }
                save = true;
            }
            if (save)
            {
                var combi = new memberInfo.SympathyCombi { series_id = serid, track_id = traid, status = sympathy };

                if (((System.Windows.Controls.TextBlock)sender).Text != neutral)
                {

                    sympathyCombis.Add(combi);

                }
                else
                {
                    var del = sympathyCombis.Find(c => c.series_id == serid && c.track_id == traid);
                    sympathyCombis.Remove(del);
                }
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    CollectionViewSource.GetDefaultView(gridRaces.ItemsSource).Refresh();
                }));
            }
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
            Properties.Settings.Default.Save();
        }
        private void btnFilterReset_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.filter = @defaultfilter;
            Properties.Settings.Default.Save();
            Properties.Settings.Default.Reload();
            List<string> filter = new List<string>();
            filter = defaultfilter.Split(';').ToList();
            try
            {
                var cbFilter = FindVisualChildren<CheckBox>(gridFilter);
                foreach (CheckBox cb in cbFilter)
                {
                    if (filter.Contains(cb.Name))
                    {
                        cb.IsChecked = true;
                    }
                    else
                    {
                        cb.IsChecked = false;
                    }
                }
            }
            catch { }
        }
        private async void btnProfileUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await fData.getGithubLastRelease(Properties.Settings.Default.updateURL.ToString(), version.ToString());
            }
            catch (Exception ex)
            {
                if(ex.InnerException != null)
                {
                    MessageBox.Show("App Update:" + ex.InnerException.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private async void checkNewRelease()
        {
            try
            {
                var gitrelease = await fData.getGithubActualReleaseinfo(Properties.Settings.Default.updateURL.ToString());
                Version releaseVersion = new Version(gitrelease.tag_name);
                DateTime releaseDate = new DateTime(2000, 1, 1).AddDays(releaseVersion.Build).AddSeconds(releaseVersion.Revision * 2);
                if (releaseDate > buildDate)
                {
                    btnProfileUpdate.IsEnabled = true;
                    btnProfileUpdate.Style = (Style)FindResource("ButtonHighlight");
                    btnProfileUpdate.ToolTip = gitrelease.body;
                    MenuNotification.Visibility = Visibility.Visible;
                }
                else
                {
                    btnProfileUpdate.IsEnabled = false;
                    MenuNotification.Visibility = Visibility.Hidden;
                }
            }
            catch
            {
                btnProfileUpdate.IsEnabled = false;
                MenuNotification.Visibility = Visibility.Hidden;
            }

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
        private void gridAutoStartPause_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (((System.Windows.FrameworkElement)sender).DataContext.GetType().Name == "autoStartDataGrid")
            {
                string status = "";
                int ID = ((RCRPlanner.dgObjects.autoStartDataGrid)((System.Windows.FrameworkElement)sender).DataContext).ID;
                if (((System.Windows.Controls.TextBlock)sender).Text == play)
                {
                    var prog = autoStartApps.Programs.FirstOrDefault(i => i.ID == ID);
                    prog.Paused = true;
                    status = pause;
                }
                if (((System.Windows.Controls.TextBlock)sender).Text == pause)
                {
                    var prog = autoStartApps.Programs.FirstOrDefault(i => i.ID == ID );
                    prog.Paused = false;
                    status = play;
                }
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    generateAutoStartView();
                    CollectionViewSource.GetDefaultView(gridAutoStart.ItemsSource).Refresh();
                }));
                helper.SerializeObject<autoStart.Root>(autoStartApps, exePath + autostartfile);
            }
        }
        private void gridAutoStartwithiRacing_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (((System.Windows.FrameworkElement)sender).DataContext.GetType().Name == "autoStartDataGrid")
            {
                string status = "";
                int ID = ((RCRPlanner.dgObjects.autoStartDataGrid)((System.Windows.FrameworkElement)sender).DataContext).ID;
                if (((System.Windows.Controls.TextBlock)sender).Text == checksymbol)
                {
                    var prog = autoStartApps.Programs.FirstOrDefault(i => i.ID == ID);
                    prog.withiRacing = false;
                    status = unchecksymbol;
                }
                if (((System.Windows.Controls.TextBlock)sender).Text == unchecksymbol)
                {
                    var prog = autoStartApps.Programs.FirstOrDefault(i => i.ID == ID);
                    prog.withiRacing = true;
                    status = checksymbol;
                }
                System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    generateAutoStartView();
                    CollectionViewSource.GetDefaultView(gridAutoStart.ItemsSource).Refresh();
                }));
                helper.SerializeObject<autoStart.Root>(autoStartApps, exePath + autostartfile);
            }
        }
        private async void btnMenu1_Click(object sender, RoutedEventArgs e)
        {
            switch (activeGrid)
            {
                case "gridAutoStart":
                    OpenFileDialog openFileDialog = new OpenFileDialog
                    {
                        Filter = "Programs (*.exe)| *.exe"
                    };
                    string path = null;
                    if (openFileDialog.ShowDialog() == true)
                    {
                        path = openFileDialog.FileName;
                    }
                    if (path != null)
                    {
                        var prog = new autoStart.Programs { ID = autoStartApps.Programs != null ? autoStartApps.Programs.Count + 1 : 1, Path = path };
                        List<autoStart.Programs> progs = new List<autoStart.Programs>
                        {
                            prog
                        };
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
                    resize_Grid(gridFilter, "height", 300, moveAnimationDuration);
                    break;
                case "gridSeries":
                    filterSeries();
                    break;
                case "gridCars":
                    filterCars();
                    break;
                case "gridTracksLayout":
                    filterTracks();
                    break;
                case "gridPurchaseGuide":
                    generatePurchaseGuideView();
                    break;
                case "gridPartStat":
                    try
                    {
                        if (await fData.Login_API(Encoding.UTF8.GetBytes((username).ToLower()), Encoding.UTF8.GetBytes(helper.ToInsecureString(password)), false) == 200)
                        {
                            if (ddMenu2.SelectedIndex != -1 && ddMenu3.SelectedIndex != -1 && ddMenu4.SelectedIndex != -1)
                            {
                                int _week = -1;
                                if (ddMenu5.SelectedIndex != 0)
                                {
                                    _week = Convert.ToInt32(((System.Windows.FrameworkElement)ddMenu5.SelectedValue).Name.Replace("w", ""));
                                }
                                try
                                {
                                    int _serId = Convert.ToInt32(ddMenu2.SelectedValue.ToString().Replace("s", ""));
                                    int _year = Convert.ToInt32(((System.Windows.FrameworkElement)ddMenu3.SelectedValue).Name.Replace("y", ""));
                                    int _season = Convert.ToInt32(((System.Windows.FrameworkElement)ddMenu4.SelectedValue).Name.Replace("s", ""));
                                    DataTable dataTable = new DataTable();
                                    btnMenu1.Content = "Loading...";
                                    btnMenu1.IsEnabled = false;
                                    partStatMinStarters = (seriesList.Find(x => x.series_id == _serId)).min_starters;
                                    dataTable = await statistics.ParticipationStats(_serId,_year,_season,_week);
                                    gridPartStat.ItemsSource = null;
                                    gridPartStat.ItemsSource = dataTable.DefaultView;
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
                            btnMenu1.Content = "Please re-login!";
                        }
                    }
                    catch (Exception ex)
                    {
                        if(ex.InnerException != null)
                        {
                            MessageBox.Show("Participation stats: " + ex.InnerException.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        btnMenu1.Content = "Get stat";
                        btnMenu1.IsEnabled = true;
                    }
                    break;
                case "gridiRatingStat":
                    try
                    {
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
                            btnMenu1.Content = "Please re-login!";
                        }
                    }
                    catch (Exception ex)
                    {
                        if(ex.InnerException != null)
                        {
                            MessageBox.Show("iRating stats: " + ex.InnerException.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        btnMenu1.Content = "Get stat";
                        btnMenu1.IsEnabled = true;
                    }
                    break;
                case "gridSeasonOverview":

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
                    helper.SerializeObject<autoStart.Root>(autoStartApps, exePath + autostartfile);
                    break;
                case "gridSeasonOverview":
                    Properties.Settings.Default.SortDate = cbMenu2.IsChecked.Value;
                    Properties.Settings.Default.Save();
                    generateSeasonOverviewGrid(false);
                    break;
                case "gridPartner":
                    Properties.Settings.Default.PartnerSlider = cbMenu2.IsChecked.Value;
                    Properties.Settings.Default.Save();
                    generatePartnerSlider();
                    try
                    {
                        BeginStoryboard storyboardbegin = (BeginStoryboard)(this).sbslidebeg;
                        storyboardbegin.Storyboard.RepeatBehavior = (RepeatBehavior)this.Resources["RepeatBehavior"];
                        storyboardbegin.Storyboard.Begin();
                    }
                    catch { }
                    break;
            }
            
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
        private void cbMenu6_Click(object sender, RoutedEventArgs e)
        {

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
                        try
                        {
                            if (await fData.Login_API(Encoding.UTF8.GetBytes((username).ToLower()), Encoding.UTF8.GetBytes(helper.ToInsecureString(password)), false) == 200)
                            {
                                ddMenu3.Items.Clear();
                                ddMenu4.Items.Clear();
                                ddMenu5.Items.Clear();
                                var selectedValue = Convert.ToInt32(ddMenu2.SelectedValue.ToString().Replace("s", ""));
                                pastSeasons = await fData.getSeriesPastSeasons(selectedValue.ToString());
                                List<int> years = new List<int>();

                                foreach (var season in pastSeasons.series.seasons)
                                {
                                    years.Add(season.season_year);
                                }
                                foreach (int _year in years.Distinct())
                                {
                                    ddMenu3.Items.Add(new ComboBoxItem() { Content = _year, Name = "y" + _year });
                                }
                                ddMenu3.SelectedIndex = 0;
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex.InnerException != null)
                            {
                                MessageBox.Show("Loading iRating stats: " + ex.InnerException.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }


                    }
                    break;
                case "gridiRatingStat":
                    if (ddMenu2.SelectedIndex != -1)
                    {
                        try
                        {
                            if (await fData.Login_API(Encoding.UTF8.GetBytes((username).ToLower()), Encoding.UTF8.GetBytes(helper.ToInsecureString(password)), false) == 200)
                            {
                                ddMenu3.Items.Clear();
                                ddMenu4.Items.Clear();
                                var selectedValue = Convert.ToInt32((ddMenu2.SelectedValue).ToString().Replace("s", ""));
                                pastSeasons = await fData.getSeriesPastSeasons(selectedValue.ToString());
                                switch (pastSeasons.series.category)
                                {
                                    case "formula_car":
                                        iRatingStatUserIrating = User.licenses.formula_car.irating;
                                        break;
                                    case "sports_car":
                                        iRatingStatUserIrating = User.licenses.sports_car.irating;
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
                                foreach (var season in pastSeasons.series.seasons)
                                {
                                    ddMenu3.Items.Add(new ComboBoxItem() { Content = season.season_short_name, Name = "s" + season.season_id.ToString() });
                                }
                                ddMenu3.SelectedIndex = 0;
                            }
                        }
                        catch (Exception ex)
                        {
                            if(ex.InnerException != null)
                            {
                                MessageBox.Show("Loading iRating stats: " + ex.InnerException.Message, "Something went wrong.", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    if (ddMenu3.SelectedIndex != -1)
                    {
                        ddMenu4.Items.Clear();
                        ddMenu5.Items.Clear();
                        if (ddMenu2.SelectedIndex != -1)
                        {
                            foreach (var season in pastSeasons.series.seasons.Where(x => x.season_year == Convert.ToInt32(((System.Windows.FrameworkElement)ddMenu3.SelectedValue).Name.Replace("y", ""))))
                            {
                                ddMenu4.Items.Add(new ComboBoxItem() { Content = "S" + season.season_quarter, Name = "s" + season.season_quarter.ToString() });
                            }
                        }
                        ddMenu4.SelectedIndex = 0;
                    }
                    break;
                case "gridiRatingStat":
                    if (ddMenu3.SelectedIndex != -1)
                    {
                        ddMenu4.Items.Clear();
                        ddMenu5.Items.Clear();
                        if (ddMenu2.SelectedIndex != -1)
                        {
                            List<seriesPastSeasons.CarClass> carclasses = new List<seriesPastSeasons.CarClass>();
                            foreach (var season in pastSeasons.series.seasons.Where(x => x.season_short_name == ((ContentControl)ddMenu3.SelectedValue).Content.ToString()))
                            {
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
                        ddMenu4.SelectedIndex = 0;
                    }
                    break;
            }
        }

        private void ddMenu4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (activeGrid)
            {
                case "gridPartStat":
                    if (ddMenu4.SelectedIndex != -1)
                    {
                        if (ddMenu3.SelectedIndex != -1)
                        {
                            if (ddMenu2.SelectedIndex != -1)
                            {
                                ddMenu5.Items.Clear();
                                ddMenu5.Items.Add(new ComboBoxItem() { Content = "All", Name = "" });
                                ddMenu5.SelectedIndex = 0;
                                try
                                {
                                    int weeks = 0;
                                    foreach (var season in pastSeasons.series.seasons)
                                    {
                                        if (season.season_year == Convert.ToInt32(((System.Windows.FrameworkElement)ddMenu3.SelectedValue).Name.Replace("y", "")) && season.season_quarter == Convert.ToInt32(((System.Windows.FrameworkElement)ddMenu4.SelectedValue).Name.Replace("s", "")))
                                        {


                                            if (season.active)
                                            {
                                                weeks = actualWeeks.First(x => x.seriesId == season.series_id).week;
                                            }
                                            else
                                            {
                                                weeks = season.race_weeks.Count();
                                            }
                                        }
                                    }
                                    for (int i = 0; i < weeks; i++)
                                    {
                                        ddMenu5.Items.Add(new ComboBoxItem() { Content = i+1, Name = "w" + i });
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                    break;
                case "gridiRatingStat":
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
                    dpMenu3date.SelectedDate = DateTime.Now;
                    generateRaceView();
                    break;
                case "gridSeasonOverview":
                    generateSeasonOverviewGrid(true);
                    break;
            }
        }
        private void btnMenu6_Click(object sender, RoutedEventArgs e)
        {
            switch (activeGrid)
            {
                case "gridAutoStart":
                    Properties.Settings.Default.delayTime = Convert.ToInt32(tbMenu6tb.Text);
                    Properties.Settings.Default.Save();
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
            bwPresetLoader.RunWorkerCompleted += worker_RunWorkerCompleted;
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

        private void link_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (((Hyperlink)sender).NavigateUri != null)
                {
                    Process.Start(new ProcessStartInfo(((Hyperlink)sender).NavigateUri.ToString()));
                    e.Handled = true;
                }
            }
            catch { }

        }

        private void link_Copy(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (((Hyperlink)sender).NavigateUri != null)
                {
                    Clipboard.SetText(((Hyperlink)sender).NavigateUri.ToString());
                    e.Handled = true;
                }
            }
            catch { }
        }

        private void cbFilterOwnBoth_Changed(object sender, RoutedEventArgs e)
        {
            if(((CheckBox)sender).IsChecked == true)
            {
                cbFilterOwnCars.IsEnabled = false;
                cbFilterOwnTracks.IsEnabled = false;
            }
            if (((CheckBox)sender).IsChecked == false)
            {
                cbFilterOwnCars.IsEnabled = true;
                cbFilterOwnTracks.IsEnabled = true;
            }
        }

        private void LoginClose_MouseDown(object sender, MouseButtonEventArgs e)
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

        private void ScrollWeekIntoView(object sender, RoutedEventArgs e)
        {
            DataGrid dataGrid = new DataGrid();
            dataGrid = sender as DataGrid;
            foreach(var row in dataGrid.Items)
             {
                if (((RCRPlanner.dgObjects.tracksDataGrid)row).WeekActive == true)
                {
                    dataGrid.ScrollIntoView(dataGrid.Items[dataGrid.Items.Count - 1]);
                    dataGrid.UpdateLayout();
                    dataGrid.ScrollIntoView(row);
                    dataGrid.UpdateLayout();
                    break;
                }
            }
        }
        private void spPartner_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Resources["CanvasRightCalc"] = (double)helper.MeasureString(spPartners).Width + Application.Current.MainWindow.ActualWidth;
            int duration = (int)Math.Round(Application.Current.MainWindow.ActualWidth * 0.024);
            this.Resources["AnimationTime"] = (Duration)new TimeSpan(0, 0, duration);
            Canvas.SetLeft(spPartners, 0 - (double)helper.MeasureString(spPartners).Width);
            try
            {
                BeginStoryboard storyboardbegin = (BeginStoryboard)(this).sbslidebeg;
                
                storyboardbegin.Storyboard.Begin();
            }
            catch { }
        }

        private void spPartner_MouseEnter(object sender, MouseEventArgs e)
        {
            BeginStoryboard storyboardbegin = (BeginStoryboard)(this).sbslidebeg;
            storyboardbegin.Storyboard.Pause();
        }

        private void spPartner_MouseLeave(object sender, MouseEventArgs e)
        {
            BeginStoryboard storyboardbegin = (BeginStoryboard)(this).sbslidebeg;
            storyboardbegin.Storyboard.Resume();
        }

        private void btnDateLower_Click(object sender, RoutedEventArgs e)
        {
            dpMenu3date.SelectedDate = ((DateTime)(dpMenu3date.SelectedDate)).AddDays(-1);
        }

        private void btnDateHigher_Click(object sender, RoutedEventArgs e)
        {
            dpMenu3date.SelectedDate = ((DateTime)(dpMenu3date.SelectedDate)).AddDays(1);
        }

        private void dpMenu3date_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            generateRaceView();
        }

        private void daSlider_Completed(object sender, EventArgs e)
        {
            if (this.Resources["RepeatBehavior"].ToString() != "Forever")
            {
                spPartners.Visibility = Visibility.Hidden;
                partnerSliderHide = true;
            }
        }
    }
}
