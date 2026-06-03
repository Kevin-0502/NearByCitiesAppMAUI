using NearByCities.Models;

namespace NearByCities.Services;

public interface ILocationService
{
    Task<LocationData> GetCurrentLocationAsync();
    void StartListening(Action<LocationData> onLocationChanged, double minDistanceMeters = 50);
    void StopListening();
}
