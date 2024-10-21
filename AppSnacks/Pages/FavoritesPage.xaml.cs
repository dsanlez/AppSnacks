using AppLanches.Models;
using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class FavoritesPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly FavoriteService _favoritesService;
    public FavoritesPage(ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _favoritesService = ServiceFactory.CreateFavoriteService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetProductsFavorite();
    }
    private async Task GetProductsFavorite()
    {
        try
        {
            var productsFavorite = await _favoritesService.ReadAllAsync();
            if (productsFavorite is null || productsFavorite.Count == 0)
            {
                CvProducts.ItemsSource = null;
                LblWarning.IsVisible = true;
            }
            else
            {
                CvProducts.ItemsSource = productsFavorite;
                LblWarning.IsVisible = false;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"There was an unexpected error: {ex.Message}", "OK");
        }
    }
    private void CvProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection.FirstOrDefault() as FavoriteProduct;
        if (currentSelection == null) return;
        Navigation.PushAsync(new ProductDetailsPage(currentSelection.ProductId,
                                                     currentSelection.Name!,
                                                     _apiService, _validator));
        ((CollectionView)sender).SelectedItem = null;
    }
}
