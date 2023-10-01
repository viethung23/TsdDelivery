namespace TsdDelivery.Application.Models.Coordinates;

public class CoordinateDistance
{
    public double DistanceFromDriverToReservation { get; set; }                             // 1
    public double DistanceFromReservationToDestinationReservation { get; set; }             // 2
    public double DistanceFromDestinationReservationToDriverTargetDestination { get; set; } // 3
    public double DistanceFromDriverToDriverTargetDestination { get; set; }                 // 5
    
}