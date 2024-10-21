using AppLanches.Pages;
using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches
{
    public partial class AppShell : Shell
    {
        private readonly ApiService _apiService;
        private readonly IValidator _validator;
        private readonly FavoriteService _favoritesService;

        public AppShell(ApiService apiService, IValidator validator, FavoriteService favoritesService)
        {
            InitializeComponent();
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            _validator = validator;
            _favoritesService = favoritesService;

            ConfigureShell();
            _favoritesService = favoritesService;

        }

        private void ConfigureShell()
        {
            var homePage = new HomePage(_apiService, _validator, _favoritesService);
            var cartPage = new ShoppingcartPage(_apiService, _validator, _favoritesService);
            var favoritesPage = new FavoritesPage(_apiService, _validator);
            var profilePage = new ProfilePage(_apiService, _validator);

            Items.Add(new TabBar
            {
                Items =
                {
                    new ShellContent {Title = "Home", Icon = "home", Content = homePage},
                    new ShellContent {Title = "Cart", Icon = "cart", Content = cartPage},
                    new ShellContent {Title = "Favorites", Icon = "heart", Content = favoritesPage},
                    new ShellContent {Title = "Profile", Icon = "profile", Content = profilePage},
                }
            });
        }
    }
}
