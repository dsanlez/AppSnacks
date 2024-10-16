using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class OrderDetailPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly FavoriteService _favoritesService;
    private bool _loginPageDisplayed = false;
    public OrderDetailPage(int orderId, decimal totalOrder, ApiService apiService, IValidator validator, FavoriteService favoritesService)
    {
        InitializeComponent();
        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        _validator = validator;
        _favoritesService = favoritesService;
        LblTotalPrice.Text = "$" + totalOrder.ToString();
        GetOrderDetail(orderId);
    }
    private async void GetOrderDetail(int orderId)
    {
        try
        {
            var (orderDetail, errorMessage) = await _apiService.GetOrderDetail(orderId);
            if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
            {
                await DisplayLoginPage();
                return;
            }
            if (orderDetail is null)
            {
                await DisplayAlert("Error", errorMessage ?? "The order detail could not be fetched", "OK");
                return;
            }
            else
            {
                CvOrderDetails.ItemsSource = orderDetail;
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Error fetching order detail. Try again later", "OK");
        }
    }
    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }
}