using AppLanches.Validations;
using AppLanches.Services;

namespace AppLanches.Pages;

public partial class ProfilePage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;

    public ProfilePage(ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }
}