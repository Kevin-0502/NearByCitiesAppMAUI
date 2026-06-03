using NearByCities.ViewModels;

namespace NearByCities.Views;

public partial class CityDetailsPage : ContentPage
{
    public CityDetailsPage(CityDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
