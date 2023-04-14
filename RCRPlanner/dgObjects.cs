using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RCRPlanner
{
    public class dgObjects
    {
        public class seriesDataGrid
        {
            public int SerieId { get; set; }
            public Uri Seriesimage { get; set; }
            public string SeriesName { get; set; }
            public string Category { get; set; }
            public string Class { get; set; }
            public string License { get; set; }
            public string Eligible { get; set; }
            public string RaceLength { get; set; }
            public string RaceTimes { get; set; }
            public string Weeks { get; set; }
            public string Favourite { get; set; }
            public string Fixed { get; set; }
            public string Official { get; set; }
            public List<tracksDataGrid> Tracks { get; set; }
            public seriesSeason.Root Season { get; set; }
            public List<carsDataGrid> Cars { get; set; }
            public string OwnCars { get; set; }
            public int OwnTracks { get; set; }
            public string ForumLink { get; set; }
        }

        public class carsDataGrid
        {
            public int CarId { get; set; }
            public Uri CarImage { get; set; }
            public Uri CarLogo { get; set; }
            public string CarName { get; set; }
            public string Category { get; set; }
            public int Weight { get; set; }
            public int Horsepower { get; set; }
            public string Price { get; set; }
            public string Owned { get; set; }
            public string Created { get; set; }
            public List<seriesDataGrid> Series { get; set; }
            public int PackageID { get; set; }
            public int Series_Participations { get; set; }
            public string Favourite { get; set; }
            public string ForumLink { get; set; }
        }

        public class tracksDataGrid
        {
            public string Name { get; set; }
            public string Layoutname { get; set; }
            public string Category { get; set; }
            public string Price { get; set; }
            public string Owned { get; set; }
            public string Created { get; set; }
            public double Length { get; set; }
            public int Corners { get; set; }
            public int Pitlimit { get; set; }
            public int PackageID { get; set; }
            public int Participations { get; set; }
            public int TrackID { get; set; }
            public List<seriesDataGrid> Series { get; set; }
            public int Week { get; set; }
            public string Weekdate { get; set; }
            public Uri TrackImage { get; set; }
            public string Racelenght { get; set; }
            public bool WeekActive { get; set; }
            public Uri TrackLink {get; set;}

        }
        public class tracksLayoutsDataGrid
        {
            public List<tracksDataGrid> Layouts { get; set; }
            public string Name { get; set; }
            public string Price { get; set; }
            public string Owned { get; set; }
            public int PackageID { get; set; }
            public int Layouts_count { get; set; }
            public int Participations { get; set; }
            public Uri TrackImage { get; set; }
            public string Created { get; set; }
            public string Favourite { get; set; }
            public int TrackID { get; set; }
        }

        public class RaceOverviewDataGrid
        {
            public int SerieId { get; set; }
            public Uri Seriesimage { get; set; }
            public string SeriesName { get; set; }
            public string minSR { get; set; }
            public string Eligible { get; set; }
            public string SerieRaceLength { get; set; }
            public string RaceTimes { get; set; }
            public string SerieWeek { get; set; }
            public string Favourite { get; set; }
            public string SerieFixed { get; set; }
            public string SerieOfficial { get; set; }
            public DateTime NextRace { get; set; }
            public string NextRaceTime { get; set; }
            public tracksDataGrid Tracklayout { get; set; }
            public string TrackName { get; set; }
            public string Layoutname { get; set; }
            public string TrackCategory { get; set; }
            public string TrackPrice { get; set; }
            public bool TrackOwned { get; set; }
            public string TrackCreated { get; set; }
            public double TrackLength { get; set; }
            public int TrackCorners { get; set; }
            public int TrackPitlimit { get; set; }
            public int TrackPackageID { get; set; }
            public int TrackParticipations { get; set; }
            public int TrackTrackID { get; set; }
            public Uri TrackImage { get; set; }
            public int Season_id { get; set; }
            public string season_name { get; set; }
            public string schedule_name { get; set; }
            public string start_date { get; set; }
            public string start_type { get; set; }
            public string Timer { get; set; }
            public List<int> day_offset { get; set; }
            public string FirstSessionTime { get; set; }
            public string Repeating{ get; set; }
            public string SessionTimes { get; set; }
            public seriesDataGrid Serie { get; set; }
            public tracksLayoutsDataGrid Track { get; set; }
            public List<carsDataGrid> Cars { get; set; }
            public List<tracksDataGrid> Tracks { get; set; }
            public string TracksOwned { get; set; }

        }
        public class autoStartDataGrid
        {
            public int ID { get; set; }
            public string Path { get; set; }
            public BitmapSource Icon { get; set; }
            public string Name { get; set; }

        }
        public class participationDataGrid
        {
            public int SessionId { get; set; }
            public DateTime StartTime { get; set; }
            public string Time { get; set; }
            public string DayOfWeek { get; set; }
            public int NumberDriver { get; set; }
            public string Track { get; set; }

        }
        public class iRaitingDataGrid
        {
            public string iRatingGroup { get; set; }
            public int iRatingInt { get; set; }
            public int DriverCount { get; set; }
            public string Diagram { get; set; }

        }
        public class seasonOverviewDataGrid
        {
            public int SerieId { get; set; }
            public Uri Seriesimage { get; set; }
            public string SeriesName { get; set; }
            public DateTime StartTime { get; set; }
            public int Week { get; set; }
            public string Track { get; set; }
            public string TrackOwned { get; set; }
            public bool WeekActive { get; set; }

        }
    }
}
