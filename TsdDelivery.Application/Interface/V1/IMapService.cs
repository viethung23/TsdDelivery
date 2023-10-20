using TsdDelivery.Application.Models.Coordinates;

namespace TsdDelivery.Application.Interface.V1;

public interface IMapService
{
    Task<double> CalculateDistanceBetweenTwoCoordinates(double originLat, double originLon,double destLat, double destLon);

    Task<CoordinateDistance?> CalculateDistanceBetweenFourCoordinates(CurrentCoordinates currentCoordinates,double originLat, double originLon,double destLat, double destLon,
        DestinationCoordinates coordinates);
}