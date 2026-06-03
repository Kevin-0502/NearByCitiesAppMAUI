using NearByCities.ViewModels;

namespace NearByCities.Views;

public partial class NearbyCitiesPage : ContentPage
{
    private readonly NearbyCitiesViewModel _viewModel;

    public NearbyCitiesPage(NearbyCitiesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_viewModel.Cities.Count == 0)
            await _viewModel.LoadCitiesCommand.ExecuteAsync(null);

        _viewModel.StartLocationTracking();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.StopLocationTracking();
    }
}
