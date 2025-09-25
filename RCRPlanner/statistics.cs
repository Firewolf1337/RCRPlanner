using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Data;

namespace RCRPlanner
{
    public class statistics
    {
        public static int partMin;
        public static int partMax;
        public static int sofMin;
        public static int sofMax;
        public static int sofcol;
        private readonly FetchData fData = new FetchData();
        public async Task<DataTable> ParticipationStats(int seriesId, int year, int season, int week)
        {
            sofcol = 1;
            string url = "https://members-ng.iracing.com/data/results/search_series?season_year=" + year + "&season_quarter=" + season + "&series_id=" + seriesId + "&event_types=5";
            if (week != -1) {
                url = "https://members-ng.iracing.com/data/results/search_series?season_year="+ year + "&season_quarter="+ season +"&series_id="+ seriesId + "&race_week_num="+ week +"&event_types=5";
            }

            // https://members-ng.iracing.com/data/results/search_series?season_year=2022&season_quarter=4&series_id=112&race_week_num=10&event_types=5
            List<string> links = await fData.getSerieSearchLinks(url);
            List<searchSerieResults.Root> results = new List<searchSerieResults.Root>();
            List<dgObjects.participationDataGrid> dgParticipation = new List<dgObjects.participationDataGrid>();
            
            foreach(var link in links)
            {
               results.AddRange(await fData.getSeriesSearchResults(link));
            }

            foreach(var res in results.Select(x => x.session_id).Distinct())
            {
                dgParticipation.Add(new dgObjects.participationDataGrid { SessionId = res , NumberDriver = 0});
            }
            foreach(var id in dgParticipation)
            {
                List<searchSerieResults.Root> session = results.Where(r => r.session_id == id.SessionId).ToList();
                id.NumberDriver += session.Sum(x => x.num_drivers);
                id.StartTime = session[0].start_time;
                id.DayOfWeek = (session[0].start_time.DayOfWeek).ToString();
                id.Time = new DateTime(session[0].start_time.Ticks).ToString(Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortTimePattern.ToString());
                id.Track = session[0].track.track_name + " - " + session[0].track.config_name;
                id.SoF = session.Max(s => s.event_strength_of_field);
                id.Splits = session.Count();
            }
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Day");
            partMin = dgParticipation.Select(i => i.NumberDriver).Min();
            partMax = dgParticipation.Select(i => i.NumberDriver).Max();
            sofMin = dgParticipation.Select(i => i.SoF).Min();
            sofMax = dgParticipation.Select(i => i.SoF).Max();

            foreach (var time in dgParticipation.Select(t => t.StartTime.TimeOfDay).Distinct().OrderBy(t => t.Ticks)) {
                string thetime = new DateTime(time.Ticks).ToString(Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortTimePattern.ToString());
                dataTable.Columns.Add(thetime);
                sofcol++;
            }
            dataTable.Columns.Add("Track");
            foreach (var time in dgParticipation.Select(t => t.StartTime.TimeOfDay).Distinct().OrderBy(t => t.Ticks))
            {
                string thetime = new DateTime(time.Ticks).ToString(Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortTimePattern.ToString());
                DataColumn column = new DataColumn();
                column.ColumnName = "s" + thetime;
                column.Caption = thetime;
                dataTable.Columns.Add(column);
            }
            string lasttrack = "first";
            DataRow row;
            row = dataTable.NewRow();
            row[0] = " ";
            row["Track"] = "<- Participations | Top split SoF ->";
            dataTable.Rows.Add(row);
            foreach (var session in dgParticipation.Select(d => d.StartTime.DayOfYear).Distinct())
            {
                List<dgObjects.participationDataGrid> tempgrid = dgParticipation.Where(c => c.StartTime.DayOfYear == session).ToList();
                row = dataTable.NewRow();
                row[0] = tempgrid[0].DayOfWeek + " " + tempgrid[0].StartTime.Date.ToShortDateString();
                int rownumber = dataTable.Rows.Count-1;
                int itemnumber = rownumber >-1 ? dataTable.Rows[rownumber].ItemArray.Count()-1: 0;
                if (rownumber <= 0 || (rownumber > 0 && lasttrack.ToString() != (tempgrid[0].Track).ToString()))
                {
                    row["Track"] = tempgrid[0].Track;
                }
                else
                {
                    row["Track"] = "-";
                }
                lasttrack = tempgrid[0].Track;
                foreach (var entry in tempgrid)
                {
                    row[entry.Time] = entry.NumberDriver;
                    row["s" + entry.Time] =  entry.SoF;
                }
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }
        public async Task<List<dgObjects.iRaitingDataGrid>> DriverIratingPerSeries(int SeasonId, int CarId, int driveriRating)
        {
            string url = "https://members-ng.iracing.com/data/stats/season_driver_standings?season_id=" + SeasonId + "&car_class_id=" + CarId;
            List<string> links = await fData.getSesasonDriverStandingsLinks(await fData.getLink(url));
            List<seasonDriverStandings.Root> results = new List<seasonDriverStandings.Root>();
            List<dgObjects.iRaitingDataGrid> dgIratings = new List<dgObjects.iRaitingDataGrid>();
            for (int iratingrange = 500; iratingrange <= 11000; iratingrange += 500 )
            {
                if (iratingrange < 11000)
                {
                    dgIratings.Add(new dgObjects.iRaitingDataGrid { DriverCount = 0, iRatingGroup = (iratingrange - 499).ToString() +" - " + iratingrange.ToString(), iRatingInt = iratingrange });
                }
                else
                {
                    dgIratings.Add(new dgObjects.iRaitingDataGrid { DriverCount = 0, iRatingGroup = (iratingrange - 499).ToString() + " - " + iratingrange.ToString(), iRatingInt = iratingrange });
                    dgIratings.Add(new dgObjects.iRaitingDataGrid { DriverCount = 0, iRatingGroup = ">" + iratingrange.ToString(), iRatingInt = iratingrange+1 });
                }
            }
            foreach (var link in links)
            {
                results.AddRange(await fData.getseasonDriverStandingsResults(link));
            }
            int driverCount = 0;
            int iratingsum = 0;
            int highestiRating = 0;
            int driversWithLoweriRating = 0;
            foreach (var res in results)
            {
                int irange = 0;
                if(res.license.irating < driveriRating)
                {
                    driversWithLoweriRating++;
                }
                for(irange = 0; (dgIratings[irange].iRatingInt < res.license.irating && irange < dgIratings.Count()-1); irange++) { }
                var dgData = dgIratings.FirstOrDefault(r => r.iRatingInt == dgIratings[irange].iRatingInt);
                dgData.DriverCount++;
                driverCount++;
                iratingsum += res.license.irating;
                highestiRating = highestiRating < res.license.irating ? res.license.irating : highestiRating;
            }
            foreach(var data in dgIratings)
            {
                string diagram = "";
                double i = 0;
                double onepercent = (driverCount / 100);
                double percent = Math.Round(data.DriverCount / onepercent, 2);
                while(i < percent)
                {
                    diagram += "|";
                    i += 0.1;
                }
                data.Diagram = diagram;
            }
            dgIratings.Add(new dgObjects.iRaitingDataGrid { DriverCount = driverCount, iRatingGroup = "Total Drivers" });
            dgIratings.Add(new dgObjects.iRaitingDataGrid { DriverCount = iratingsum/driverCount, iRatingGroup = "iRating ∅" });
            dgIratings.Add(new dgObjects.iRaitingDataGrid { DriverCount = highestiRating, iRatingGroup = "Highest iRating" });
            dgIratings.Add(new dgObjects.iRaitingDataGrid { DriverCount = (100 - (driversWithLoweriRating/(driverCount/100))), iRatingGroup = "You are in Top %" });

            return dgIratings;
        }
    }
}
