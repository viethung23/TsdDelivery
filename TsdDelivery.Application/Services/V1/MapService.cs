using Newtonsoft.Json;
using TsdDelivery.Application.Commons;
using TsdDelivery.Application.Interface.V1;
using TsdDelivery.Application.Models.Coordinates;

namespace TsdDelivery.Application.Services.V1;

public class MapService : IMapService
{
    private readonly AppConfiguration _configuration;
    public MapService(AppConfiguration appConfiguration)
    {
        _configuration = appConfiguration;
    }
    
    public async Task<double> CalculateDistanceBetweenTwoCoordinates(double originLat, double originLon,double destLat, double destLon)
    {
        var url = $"https://api.tomtom.com/routing/1/calculateRoute/{originLat},{originLon}:{destLat},{destLon}/json?key={_configuration.Tomtom.ApiKey}";
        double distance;
        using (var client = new HttpClient())
        {
            try
            {
                var response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    var tomtom = JsonConvert.DeserializeObject<Tomtom>(result);
                    distance = tomtom!.Routes.First().Summary.LengthInMeters;
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

    public async Task<CoordinateDistance?> CalculateDistanceBetweenFourCoordinates(CurrentCoordinates currentCoordinates, double originLat, double originLon,
        double destLat, double destLon, DestinationCoordinates coordinates)
    {
        var url = $"https://api.tomtom.com/routing/1/calculateRoute/{currentCoordinates.Latitude},{currentCoordinates.Longitude}:{originLat}," +
                  $"{originLon}:{destLat},{destLon}:{coordinates.LatitudeDes},{coordinates.LongitudeDes}:{currentCoordinates.Latitude},{currentCoordinates.Longitude}" +
                  $":{coordinates.LatitudeDes},{coordinates.LongitudeDes}/json?key={_configuration.Tomtom.ApiKey}";
        CoordinateDistance coordinateDistance;
        using (var client = new HttpClient())
        {
            try
            {
                var response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.Content.ReadAsStringAsync().Result;
                    var tomtom = JsonConvert.DeserializeObject<Tomtom>(result);
                    var listDistance = tomtom!.Routes.SelectMany(x => x.Legs)
                        .Select(leg => leg.Summary.LengthInMeters).ToArray();
                    coordinateDistance = new CoordinateDistance()
                    {
                        DistanceFromDriverToReservation = listDistance[0],
                        DistanceFromReservationToDestinationReservation = listDistance[1],
                        DistanceFromDestinationReservationToDriverTargetDestination = listDistance[2],
                        DistanceFromDriverToDriverTargetDestination = listDistance[4]
                    };
                }
                else
                {
                    coordinateDistance = null;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
        return await Task.FromResult(coordinateDistance);
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