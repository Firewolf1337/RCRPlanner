using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCRPlanner
{
    class DataDoc
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Assets
        {
            public string link { get; set; }
            public List<string> note { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class Awards
        {
            public string link { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class Car
        {
            public Assets assets { get; set; }
            public Get get { get; set; }
        }

        public class Carclass
        {
            public Get get { get; set; }
        }

        public class CarClassId
        {
            public string type { get; set; }
            public bool required { get; set; }
        }

        public class CarId
        {
            public string type { get; set; }
            public string note { get; set; }
            public bool required { get; set; }
        }

        public class Categories
        {
            public string link { get; set; }
            public string note { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class CategoryId
        {
            public string type { get; set; }
            public bool required { get; set; }
            public string note { get; set; }
        }

        public class CategoryIds
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class ChartData
        {
            public string link { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class ChartType
        {
            public string type { get; set; }
            public bool required { get; set; }
            public string note { get; set; }
        }

        public class ClubHistory
        {
            public string link { get; set; }
            public string note { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class ClubId
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class CombinedSessions
        {
            public string link { get; set; }
            public string note { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class Constants
        {
            public Categories categories { get; set; }
            public Divisions divisions { get; set; }
            public EventTypes event_types { get; set; }
        }

        public class Countries
        {
            public string link { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class CustId
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class CustIds
        {
            public string type { get; set; }
            public bool required { get; set; }
            public string note { get; set; }
        }

        public class CustLeagueSessions
        {
            public string link { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class Directory
        {
            public string link { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class Division
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class Divisions
        {
            public string link { get; set; }
            public string note { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class Drivers
        {
            public string link { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class EventLog
        {
            public string link { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class EventType
        {
            public string type { get; set; }
            public string note { get; set; }
            public bool required { get; set; }
        }

        public class EventTypes
        {
            public string link { get; set; }
            public string note { get; set; }
            public int expirationSeconds { get; set; }
            public string type { get; set; }
        }

        public class FinishRangeBegin
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class FinishRangeEnd
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class From
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class Get
        {
            public string link { get; set; }
            public int expirationSeconds { get; set; }
            public Parameters parameters { get; set; }
            public string note { get; set; }
        }

        public class GetPointsSystems
        {
            public string link { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class HostCustId
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class Hosted
        {
            public CombinedSessions combined_sessions { get; set; }
            public Sessions sessions { get; set; }
        }

        public class IncludeEndAfterFrom
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class IncludeLeague
        {
            public string type { get; set; }
        }

        public class IncludeLicenses
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class IncludeSeries
        {
            public string type { get; set; }
        }

        public class Info
        {
            public string link { get; set; }
            public string note { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class LapChartData
        {
            public string link { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class LapData
        {
            public string link { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class League
        {
            public CustLeagueSessions cust_league_sessions { get; set; }
            public Directory directory { get; set; }
            public Get get { get; set; }
            public GetPointsSystems get_points_systems { get; set; }
            public Membership membership { get; set; }
            public Seasons seasons { get; set; }
            public SeasonStandings season_standings { get; set; }
            public SeasonSessions season_sessions { get; set; }
        }

        public class LeagueId
        {
            public string type { get; set; }
            public bool required { get; set; }
            public string note { get; set; }
        }

        public class LeagueSeasonId
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class Licenses
        {
            public string link { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class List
        {
            public string link { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class Lookup
        {
            public ClubHistory club_history { get; set; }
            public Countries countries { get; set; }
            public Drivers drivers { get; set; }
            public Get get { get; set; }
            public Licenses licenses { get; set; }
        }

        public class Lowerbound
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class MaximumRosterCount
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class Member
        {
            public Awards awards { get; set; }
            public ChartData chart_data { get; set; }
            public Get get { get; set; }
            public Info info { get; set; }
            public Profile profile { get; set; }
        }

        public class MemberBests
        {
            public string link { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class MemberCareer
        {
            public string link { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class MemberDivision
        {
            public string link { get; set; }
            public string note { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class MemberRecentRaces
        {
            public string link { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class Membership
        {
            public string link { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class MemberSummary
        {
            public string link { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class MemberYearly
        {
            public string link { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class Mine
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class MinimumRosterCount
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class OfficialOnly
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class Order
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class PackageId
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class Parameters
        {
            public PackageId package_id { get; set; }
            public SeasonYear season_year { get; set; }
            public SeasonQuarter season_quarter { get; set; }
            public SearchTerm search_term { get; set; }
            public LeagueId league_id { get; set; }
            public CustId cust_id { get; set; }
            public CategoryId category_id { get; set; }
            public ChartType chart_type { get; set; }
            public CustIds cust_ids { get; set; }
            public IncludeLicenses include_licenses { get; set; }
            public SubsessionId subsession_id { get; set; }
            public SimsessionNumber simsession_number { get; set; }
            public TeamId team_id { get; set; }
            public Mine mine { get; set; }
            public StartRangeBegin start_range_begin { get; set; }
            public StartRangeEnd start_range_end { get; set; }
            public FinishRangeBegin finish_range_begin { get; set; }
            public FinishRangeEnd finish_range_end { get; set; }
            public HostCustId host_cust_id { get; set; }
            public SessionName session_name { get; set; }
            public LeagueSeasonId league_season_id { get; set; }
            public CarId car_id { get; set; }
            public TrackId track_id { get; set; }
            public CategoryIds category_ids { get; set; }
            public SeriesId series_id { get; set; }
            public RaceWeekNum race_week_num { get; set; }
            public OfficialOnly official_only { get; set; }
            public EventTypes event_types { get; set; }
            public SeasonId season_id { get; set; }
            public EventType event_type { get; set; }
            public From from { get; set; }
            public IncludeEndAfterFrom include_end_after_from { get; set; }
            public IncludeSeries include_series { get; set; }
            public Search search { get; set; }
            public Tag tag { get; set; }
            public RestrictToMember restrict_to_member { get; set; }
            public RestrictToRecruiting restrict_to_recruiting { get; set; }
            public RestrictToFriends restrict_to_friends { get; set; }
            public RestrictToWatched restrict_to_watched { get; set; }
            public MinimumRosterCount minimum_roster_count { get; set; }
            public MaximumRosterCount maximum_roster_count { get; set; }
            public Lowerbound lowerbound { get; set; }
            public Upperbound upperbound { get; set; }
            public Sort sort { get; set; }
            public Order order { get; set; }
            public CarClassId car_class_id { get; set; }
            public ClubId club_id { get; set; }
            public Division division { get; set; }
            public IncludeLeague include_league { get; set; }
            public Retired retired { get; set; }
            public ResultsOnly results_only { get; set; }
        }

        public class Profile
        {
            public string link { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class RaceGuide
        {
            public string link { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class RaceWeekNum
        {
            public string type { get; set; }
            public string note { get; set; }
            public bool required { get; set; }
        }

        public class RestrictToFriends
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class RestrictToMember
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class RestrictToRecruiting
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class RestrictToWatched
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class Results
        {
            public Get get { get; set; }
            public EventLog event_log { get; set; }
            public LapChartData lap_chart_data { get; set; }
            public LapData lap_data { get; set; }
            public SearchHosted search_hosted { get; set; }
            public SearchSeries search_series { get; set; }
            public SeasonResults season_results { get; set; }
        }

        public class ResultsOnly
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class Retired
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class Root
        {
            public Car car { get; set; }
            public Carclass carclass { get; set; }
            public Constants constants { get; set; }
            public Hosted hosted { get; set; }
            public League league { get; set; }
            public Lookup lookup { get; set; }
            public Member member { get; set; }
            public Results results { get; set; }
            public Season season { get; set; }
            public Series series { get; set; }
            public Stats stats { get; set; }
            public Team team { get; set; }
            public Track track { get; set; }
        }

        public class Search
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class SearchHosted
        {
            public string link { get; set; }
            public string note { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class SearchSeries
        {
            public string link { get; set; }
            public string note { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class SearchTerm
        {
            public string type { get; set; }
            public bool required { get; set; }
            public string note { get; set; }
        }

        public class Season
        {
            public List list { get; set; }
            public RaceGuide race_guide { get; set; }
        }

        public class SeasonDriverStandings
        {
            public string link { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class SeasonId
        {
            public string type { get; set; }
            public string note { get; set; }
            public bool required { get; set; }
        }

        public class SeasonQualifyResults
        {
            public string link { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class SeasonQuarter
        {
            public string type { get; set; }
            public bool required { get; set; }
            public string note { get; set; }
        }

        public class SeasonResults
        {
            public string link { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class Seasons
        {
            public string link { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class SeasonSessions
        {
            public string link { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class SeasonStandings
        {
            public string link { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class SeasonSupersessionStandings
        {
            public string link { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class SeasonTeamStandings
        {
            public string link { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class SeasonTtResults
        {
            public string link { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class SeasonTtStandings
        {
            public string link { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class SeasonYear
        {
            public string type { get; set; }
            public bool required { get; set; }
            public string note { get; set; }
        }

        public class Series
        {
            public Assets assets { get; set; }
            public Get get { get; set; }
            public Seasons seasons { get; set; }
            public StatsSeries stats_series { get; set; }
        }

        public class SeriesId
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class SessionName
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class Sessions
        {
            public string link { get; set; }
            public string note { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class SimsessionNumber
        {
            public string type { get; set; }
            public bool required { get; set; }
            public string note { get; set; }
        }

        public class Sort
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class StartRangeBegin
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class StartRangeEnd
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class Stats
        {
            public MemberBests member_bests { get; set; }
            public MemberCareer member_career { get; set; }
            public MemberDivision member_division { get; set; }
            public MemberRecentRaces member_recent_races { get; set; }
            public MemberSummary member_summary { get; set; }
            public MemberYearly member_yearly { get; set; }
            public SeasonDriverStandings season_driver_standings { get; set; }
            public SeasonSupersessionStandings season_supersession_standings { get; set; }
            public SeasonTeamStandings season_team_standings { get; set; }
            public SeasonTtStandings season_tt_standings { get; set; }
            public SeasonTtResults season_tt_results { get; set; }
            public SeasonQualifyResults season_qualify_results { get; set; }
            public WorldRecords world_records { get; set; }
        }

        public class StatsSeries
        {
            public string link { get; set; }
            public string note { get; set; }
            public int expirationSeconds { get; set; }
        }

        public class SubsessionId
        {
            public string type { get; set; }
            public bool required { get; set; }
        }

        public class Tag
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class Team
        {
            public Get get { get; set; }
        }

        public class TeamId
        {
            public string type { get; set; }
            public string note { get; set; }
            public bool required { get; set; }
        }

        public class Track
        {
            public Assets assets { get; set; }
            public Get get { get; set; }
        }

        public class TrackId
        {
            public string type { get; set; }
            public string note { get; set; }
            public bool required { get; set; }
        }

        public class Upperbound
        {
            public string type { get; set; }
            public string note { get; set; }
        }

        public class WorldRecords
        {
            public string link { get; set; }
            public Parameters parameters { get; set; }
            public int expirationSeconds { get; set; }
        }
    }
}
