using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class FavoritesPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    public FavoritesPage(ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }
}