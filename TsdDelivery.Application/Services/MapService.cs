using System.Net.Http.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TsdDelivery.Application.Commons;
using TsdDelivery.Application.Interface;

namespace TsdDelivery.Application.Services;

public class MapService : IMapService
{
    private readonly AppConfiguration _configuration;
    public MapService(AppConfiguration appConfiguration)
    {
        _configuration = appConfiguration;
    }
    
    public async Task<double> CaculateDistanceBetweenTwoCoordinates(double originLat, double originLon,double destLat, double destLon)
    {
        var url = $"https://api.tomtom.com/routing/1/calculateRoute/{originLat},{originLon}:{destLat},{destLon}/json?key={_configuration.Tomtom.ApiKey}";
        Tomtom tomtom;
        double distance;
        using (var client = new HttpClient())
        {
            try
            {
                var response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    tomtom = JsonConvert.DeserializeObject<Tomtom>(result);
                    distance = tomtom.Routes.First().Summary.LengthInMeters;
                }
                else
                {
                    distance = 0;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
        return await Task.FromResult(distance);
    }
}


public class Tomtom
{
    public string FormatVersion { get; set; }
    public List<Route> Routes { get; set; }

    public class Route
    {
        public Summary Summary { get; set; }
        public List<Leg> Legs { get; set; }
        public List<Section> Sections { get; set; }
    }

    public class Summary 
    {
        public double LengthInMeters { get; set; }
        public double TravelTimeInSeconds { get; set; }
        public double TrafficDelayInSeconds { get; set; }
        public double TrafficLengthInMeters { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
    }

    public class Leg
    {
        public Summary Summary { get; set; }
        public List<Point> Points { get; set; }
    }

    public class Point
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class Section
    {
        public int StartPointIndex { get; set; }
        public int EndPointIndex { get; set; }
        public string SectionType { get; set; }
        public string TravelMode { get; set; }
    }
}