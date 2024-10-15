using AppLanches.Validations;
using AppLanches.Pages;
using AppLanches.Services;

namespace AppLanches;

public partial class App : Application
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly FavoriteService _favoritesService;

    public App(ApiService apiService, IValidator validator, FavoriteService favoritesService)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _favoritesService = favoritesService;
        //MainPage = new NavigationPage(new RegisterPage(_apiService, _validator));
        SetMainPage();
    }

    private void SetMainPage()
    {
        var accessToken = Preferences.Get("accesstoken", string.Empty);
        if (string.IsNullOrEmpty(accessToken))
        {
            MainPage = new NavigationPage(new RegisterPage(_apiService, _validator));
            return;
        }
        MainPage = new AppShell(_apiService, _validator, _favoritesService);
    }
}
