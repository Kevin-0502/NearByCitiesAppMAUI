using NearByCities.Models;

namespace NearByCities.Services;

public class LocationService : ILocationService
{
    private CancellationTokenSource? _cts;
    private Location? _lastKnownLocation;
    private double _minDistanceMeters = 500;
    private Action<LocationData>? _onLocationChanged;

    public async Task<LocationData> GetCurrentLocationAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
                throw new PermissionException("Permiso de ubicación denegado.");
        }

        var location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest
        {
            DesiredAccuracy = GeolocationAccuracy.Medium,
            Timeout = TimeSpan.FromSeconds(10)
        });

        if (location is null)
            throw new Exception("No se pudo obtener la ubicación actual.");

        _lastKnownLocation = location;

        return new LocationData
        {
            Latitude = location.Latitude,
            Longitude = location.Longitude
        };
    }

    public void StartListening(Action<LocationData> onLocationChanged, double minDistanceMeters = 50)
    {
        StopListening();
        _onLocationChanged = onLocationChanged;
        _minDistanceMeters = minDistanceMeters;
        _cts = new CancellationTokenSource();
        _ = PollLocationAsync(_cts.Token);
    }

    public void StopListening()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    private async Task PollLocationAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(30), ct);

                var location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Medium,
                    Timeout = TimeSpan.FromSeconds(10)
                });

                if (location is null || ct.IsCancellationRequested) continue;

                if (_lastKnownLocation is not null)
                {
                    var distance = Location.CalculateDistance(
                        _lastKnownLocation.Latitude, _lastKnownLocation.Longitude,
                        location.Latitude, location.Longitude, DistanceUnits.Kilometers);

                    if (distance * 1000 < _minDistanceMeters) continue;
                }

                _lastKnownLocation = location;
                _onLocationChanged?.Invoke(new LocationData
                {
                    Latitude = location.Latitude,
                    Longitude = location.Longitude
                });
            }
            catch (TaskCanceledException)
            {
                break;
            }
            catch
            {
                // Ignore transient errors, retry next cycle
            }
        }
    }
}
