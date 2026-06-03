using NearByCities.Views;

namespace NearByCities;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("CityDetails", typeof(CityDetailsPage));
    }
}
