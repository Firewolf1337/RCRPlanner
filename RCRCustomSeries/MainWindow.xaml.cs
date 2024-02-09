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
using static RCRPlanner.memberInfo;
using RCRPlanner;

namespace RCRCustomSeries
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal static MainWindow main;
        internal string Status
        {
            get { return ""; }
            set { Dispatcher.Invoke(new Action(() => { lblLoadingText.Content = value; })); }
        }
        memberInfo.Root User = new memberInfo.Root();
        readonly string userfile = @"user.xml";
        readonly double moveAnimationDuration = 0.3;
        private readonly filehandler fh = new filehandler();
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

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text

        private readonly BackgroundWorker bwPresetLoader = new BackgroundWorker();
        private static readonly ManualResetEvent mre = new ManualResetEvent(false);

        List<series.Root> seriesList = new List<series.Root>();
        List<seriesAssets> seriesAssetsList = new List<seriesAssets>();
        List<seriesSeason.Root> seriesSeasonList = new List<seriesSeason.Root>();
        List<actualWeek> actualWeeks = new List<actualWeek>();
        List<dgObjects.seriesDataGrid> dgSeriesList = new List<dgObjects.seriesDataGrid>();
        List<int> customSeriesIDs = new List<int>();
        private readonly string customseriesFile = @"customseries_";
        private readonly string customseriesSeasonFile = @"\customseriesSeason_";
        private readonly string customseriesAssetsFile = @"customseriesAssets_";
        private readonly string seriesFile = @"\static\series.xml";
        private readonly string seriesSeasonFile = @"\static\seriesSeason.xml";
        private readonly string seriesAssetsFile = @"\static\seriesAssets.xml";
        private readonly string seriesLogos = @"\static\series\";
        private int nextfreeID;

        private List<cars.Root> carsList = new List<cars.Root>();
        private List<carAssets> carsAssetsList = new List<carAssets>();
        private List<carClass.Root> carClassList = new List<carClass.Root>();
        private List<carClass.CarInClassId> carClassesList = new List<carClass.CarInClassId>();
        List<carClass.Root> singlecarclass = new List<carClass.Root>();
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
        private List<comboBox> cbSeries = new List<comboBox>();

        private readonly string exePath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        public class CbfromCodeBehind
        {
            public string Name { get; set; }
            public Uri Picture { get; set; }
            public int ID { get; set; }
            public int ClassID { get; set; }
        }
        List<CbfromCodeBehind> cbCarsItems = new List<CbfromCodeBehind>();
        List<CbfromCodeBehind> dgSeriesCarClasses = new List<CbfromCodeBehind>();
        public MainWindow()
        {
            main = this;
            this.InitializeComponent();
            bwPresetLoader.DoWork += worker_DoWork;
            bwPresetLoader.WorkerReportsProgress = false;
            bwPresetLoader.RunWorkerCompleted += worker_RunWorkerCompleted;
            bwPresetLoader.RunWorkerAsync();
            btnLoadSeries_Click(null, null);
        }
        private async void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            bool error = false;
            bool filemissing = false;
            if (!Directory.Exists(exePath + tracksLogo))
            {
                Directory.CreateDirectory(exePath + tracksLogo);
            }
            if (!Directory.Exists(exePath + carLogos))
            {
                Directory.CreateDirectory(exePath + carLogos);
            }
            if (!Directory.Exists(exePath + seriesLogos))
            {
                Directory.CreateDirectory(exePath + seriesLogos);
            }
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => {
                lblLoadingText.Content = "Loading user information.";

                if (File.Exists(userfile) && !reloadData)
                {
                    lblLoadingText.Content = "Loading user information from file.";
                    User = helper.DeSerializeObject<memberInfo.Root>(userfile);

                }
                else
                {
                    lblLoadingText.Content = "User information not present in file.";
                    filemissing = true;
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
            reloadData = false;
            try
            {
                seriesList = await fh.getSeriesList(seriesFile, reloadData);
                seriesAssetsList = await fh.getSeriesAssets(seriesAssetsFile, seriesLogos, reloadData);
                seriesSeasonList = await fh.getSeriesSeason(seriesSeasonFile, reloadData);
                foreach(string custSeries in await fh.getCustomSeriesFiles(customseriesFile))
                {
                    List<series.Root> custserie = await fh.getSeriesList(custSeries, reloadData);
                    custserie.ForEach(x => customSeriesIDs.Add(x.series_id));
                    seriesList.AddRange(custserie);
                }
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
            carsList = await fh.getCarList(carsFile, reloadData);
            carClassList = await fh.getCarClassList(carClassFile, reloadData);
            foreach (var carclass in carClassList)
            {
                if (carclass.cars_in_class.Count() == 1)
                {
                    singlecarclass.Add(carclass);
                }
            }
            carClassesList = fh.getCarClassesList(carsList, carClassList);
            carsInSeries = fh.getCarsInSeries(carClassesList, seriesSeasonList);
            tracksList = await fh.getTracksList(tracksFile, reloadData);
            tracksAssetsList = await fh.getTracksAssets(tracksAssetsFile, reloadData);
            tracksInSeries = fh.getTracksInSeries(tracksList, seriesSeasonList);
            fh.getTrackSVG(tracksAssetsList, exePath + tracksLogo);

            generateCarView();
            generateTrackView();
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                cbCars.ItemsSource = cbCarsItems;
            }));
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

                }
                catch (Exception ex)
                {

                }
            }));
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


        private void stackPanelMenu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            resize_Grid(gridMenu, "width", 250, moveAnimationDuration);
        }
        private void stackPanelMenuClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            resize_Grid(gridMenu, "width", 0, moveAnimationDuration);
        }

        private void btnMenu1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cbMenu2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ddMenu2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cbMenu3_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cbMenu4_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cbMenu5_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cbMenu6_Click(object sender, RoutedEventArgs e)
        {

        }

        private void tbMenu6_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void link_Copy(object sender, MouseButtonEventArgs e)
        {

        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {

        }

        private void ddMenu3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ddMenu4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ddMenu5_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnMenu6_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dataGrid_ExtendCollapseDetails(object sender, MouseButtonEventArgs e)
        {

        }

        private void link_Click(object sender, RoutedEventArgs e)
        {

        }



        private void btnLoadSeries_Click(object sender, RoutedEventArgs e)
        {
            activeGrid = "gridSeries";
            gridAddSeries.Visibility = Visibility.Hidden;
            gridAddSeriesDetails.Visibility = Visibility.Hidden;
            btnMenu1.Visibility = Visibility.Visible;
            cbMenu2.Visibility = Visibility.Hidden;
            tbMenu2.Visibility = Visibility.Visible;
            dpMenu2.Visibility = Visibility.Hidden;
            lbMenu2.Visibility = Visibility.Hidden;
            cbMenu3.Visibility = Visibility.Hidden;
            dpMenu3.Visibility = Visibility.Hidden;
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
            switchMainGridVisibility(new List<System.Windows.Controls.DataGrid> { gridSeries });
            filterSeries();
        }

        private void btnAddSeries_Click(object sender, RoutedEventArgs e)
        {
            activeGrid = "gridAddSeries";
            nextfreeID = helper.getNextFreeCustomSeriesID(9000, seriesList);
            tbSeriesID.Text = nextfreeID.ToString();
            gridAddSeries.Visibility = Visibility.Visible;
            gridAddSeriesDetails.Visibility = Visibility.Hidden;
            btnMenu1.Visibility = Visibility.Hidden;
            cbMenu2.Visibility = Visibility.Hidden;
            tbMenu2.Visibility = Visibility.Hidden;
            dpMenu2.Visibility = Visibility.Hidden;
            lbMenu2.Visibility = Visibility.Hidden;
            cbMenu3.Visibility = Visibility.Hidden;
            dpMenu3.Visibility = Visibility.Hidden;
            cbMenu4.Visibility = Visibility.Hidden;
            dpMenu4.Visibility = Visibility.Hidden;
            cbMenu5.Visibility = Visibility.Hidden;
            dpMenu5.Visibility = Visibility.Hidden;
            cbMenu6.Visibility = Visibility.Hidden;
            tbMenu6.Visibility = Visibility.Hidden;
            dpMenu6.Visibility = Visibility.Hidden;
            stackPanelMenuClose_MouseDown(null, null);
            switchMainGridVisibility(new List<System.Windows.Controls.DataGrid> { null });
            filterSeries();
        }

        private void btnCarClasses_Click(object sender, RoutedEventArgs e)
        {
            activeGrid = "gridCarClasses";
            gridAddSeries.Visibility = Visibility.Hidden;
            gridAddSeriesDetails.Visibility = Visibility.Hidden;
            btnMenu1.Visibility = Visibility.Hidden;
            cbMenu2.Visibility = Visibility.Hidden;
            tbMenu2.Visibility = Visibility.Hidden;
            dpMenu2.Visibility = Visibility.Hidden;
            lbMenu2.Visibility = Visibility.Hidden;
            cbMenu3.Visibility = Visibility.Hidden;
            dpMenu3.Visibility = Visibility.Hidden;
            cbMenu4.Visibility = Visibility.Hidden;
            dpMenu4.Visibility = Visibility.Hidden;
            cbMenu5.Visibility = Visibility.Hidden;
            dpMenu5.Visibility = Visibility.Hidden;
            cbMenu6.Visibility = Visibility.Hidden;
            tbMenu6.Visibility = Visibility.Hidden;
            dpMenu6.Visibility = Visibility.Hidden;
            stackPanelMenuClose_MouseDown(null, null);
            switchMainGridVisibility(new List<System.Windows.Controls.DataGrid> { gridCarClasses });
            
        }
        private void btnAddDetails_Click(object sender, RoutedEventArgs e)
        {
            activeGrid = "gridAddSeriesDetails";
            gridAddSeries.Visibility = Visibility.Hidden;
            gridAddSeriesDetails.Visibility = Visibility.Visible;
            btnMenu1.Visibility = Visibility.Hidden;
            cbMenu2.Visibility = Visibility.Hidden;
            tbMenu2.Visibility = Visibility.Hidden;
            dpMenu2.Visibility = Visibility.Hidden;
            lbMenu2.Visibility = Visibility.Hidden;
            cbMenu3.Visibility = Visibility.Hidden;
            dpMenu3.Visibility = Visibility.Hidden;
            cbMenu4.Visibility = Visibility.Hidden;
            dpMenu4.Visibility = Visibility.Hidden;
            cbMenu5.Visibility = Visibility.Hidden;
            dpMenu5.Visibility = Visibility.Hidden;
            cbMenu6.Visibility = Visibility.Hidden;
            tbMenu6.Visibility = Visibility.Hidden;
            dpMenu6.Visibility = Visibility.Hidden;
            cbCustomSeriesIDs.ItemsSource = seriesList.Where(x => customSeriesIDs.Contains(x.series_id)).ToList();
            stackPanelMenuClose_MouseDown(null, null);
            switchMainGridVisibility(new List<System.Windows.Controls.DataGrid> { null });
        }
        private void switchMainGridVisibility(List<System.Windows.Controls.DataGrid> gridToShow)
        {
            List<System.Windows.Controls.DataGrid> allGrids = new List<System.Windows.Controls.DataGrid> { gridSeries, gridCarClasses };

            foreach (System.Windows.Controls.DataGrid grid in allGrids)
            {
                if (gridToShow.Contains(grid))
                {
                    grid.Visibility = Visibility.Visible;
                }
                else
                {
                    grid.Visibility = Visibility.Hidden;
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
        private void generateCarView()
        {
            try
            {
                // Generate cars informationF
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
                        Favorite = favsymbolUnselected,
                        CarId = car.car_id,
                        CarImage = new Uri("file:///" + exePath + carLogos + car.car_id + ".png"),
                        CarLogo = new Uri("file:///" + exePath + carLogos + car.car_id + "_logo.png"),
                        CarName = car.car_name,
                        Category = string.Join(",", car.categories),
                        Horsepower = isMetric ? Convert.ToInt32(car.hp * 1.01387) : car.hp,
                        Weight = isMetric ? Convert.ToInt32(car.car_weight * 0.453592) : car.car_weight,
                        Price = "$" + car.price.ToString(),
                        Owned = "",
                        Created = car.created.ToString(Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortDatePattern, Thread.CurrentThread.CurrentUICulture),
                        Series = seriesDataGridsList,
                        Series_Participations = seriesDataGridsList.Count,
                        ForumLink = car.forum_url
                    };
                    dgCarsList.Add(carsDataGridObject);

                    cbCarsItems.Add(new CbfromCodeBehind { ID = car.car_id, Name = car.car_name, Picture = new Uri("file:///" + exePath + carLogos + car.car_id + ".png"), ClassID = singlecarclass.FirstOrDefault(x => x.cars_in_class[0].car_id == car.car_id).car_class_id });
                }
                dgCarsList.Sort((x, y) => x.CarName.CompareTo(y.CarName));
                cbCarsItems.Sort((x, y) => x.Name.CompareTo(y.Name));
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
                        Owned = "",
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
                            Favorite = favsymbolUnselected,
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
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
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

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void btnaddcarstoseries_Click(object sender, RoutedEventArgs e)
        {
            if (cbCars.SelectedItem != null)
            {
                if (!dgSeriesCarClasses.Contains(cbCars.SelectedItem))
                {
                    dgSeriesCarClasses.Add((CbfromCodeBehind)cbCars.SelectedItem);
                    gridSeriesCarClasses.ItemsSource = null;
                    gridSeriesCarClasses.ItemsSource = dgSeriesCarClasses;
                    gridSeriesCarClasses.UpdateLayout();
                }
            }
        }
        private void gridSeriesCarClassesRemove_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (((System.Windows.Controls.TextBlock)sender).Text == unchecksymbol)
            {
                var rev = dgSeriesCarClasses.FirstOrDefault(i => i.ID == ((CbfromCodeBehind)((System.Windows.FrameworkElement)sender).DataContext).ID);
                dgSeriesCarClasses.Remove(rev);
            }
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() => {
                CollectionViewSource.GetDefaultView(gridSeriesCarClasses.ItemsSource).Refresh();
            }));
        }

        private void cbSeriesIknow_Click(object sender, RoutedEventArgs e)
        {
            if (cbSeriesIknow.IsChecked == true)
            {
                tbSeriesID.IsEnabled = true;
            }
            else
            {
                tbSeriesID.IsEnabled = false;
            }
        }

        private void btnSeriesAddSerie_Click(object sender, RoutedEventArgs e)
        {
            List<series.Root> newSeriesList = new List<series.Root>();
            string cat = cbSeriesCategory.SelectionBoxItem.ToString().ToLower().Replace(' ', '_');
            int catid = Convert.ToInt32(((System.Windows.UIElement)cbSeriesCategory.SelectedValue).Uid);
            series.Root newSeries = new series.Root()
            {
                category = cat,
                category_id = catid ,
                series_id = Convert.ToInt32(tbSeriesID.Text),
                series_name = tbSeriesName.Text,
                series_short_name = tbSeriesName.Text,
                search_filters = "",
                max_starters = Convert.ToInt32(tbSeriesMaxStart.Text),
                min_starters = Convert.ToInt32(tbSeriesMinStart.Text),
                eligible = true,
                forum_url = tbSeriesLink.Text,
                oval_caution_type = 0,
                road_caution_type = 0,
                allowed_licenses = helper.getAllowedLicenses(((System.Windows.Controls.TextBlock)((System.Windows.Controls.ContentControl)cbSeriesClass.SelectedItem).Content).Text.ToString(), 
                Convert.ToInt32(cbSeriesSR.SelectionBoxItem.ToString()))
            };
            newSeriesList.Add(newSeries);
            helper.SerializeObject<List<series.Root>>(newSeriesList, exePath + @"\static\" + customseriesFile + tbSeriesID.Text +".xml");
            seriesList.Add(newSeries);
            customSeriesIDs.Add(Convert.ToInt32(tbSeriesID.Text.ToString()));

        }
    }
}
