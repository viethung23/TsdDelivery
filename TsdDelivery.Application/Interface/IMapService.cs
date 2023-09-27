namespace TsdDelivery.Application.Interface;

public interface IMapService
{
    Task<double> CaculateDistanceBetweenTwoCoordinates(double originLat, double originLon,double destLat, double destLon);
}