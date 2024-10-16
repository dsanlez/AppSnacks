using AppLanches.Models;
using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class OrdersPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly FavoriteService _favoritesService;
    private bool _loginPageDisplayed = false;

    public OrdersPage(ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetOrderList();
    }

    
        private async Task GetOrderList()
        {
            try
            {
                var (orders, errorMessage) = await _apiService.GetOrdersByUser(Preferences.Get("userid", 0));

                if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
                {
                    await DisplayLoginPage();
                    return;
                }
                if (errorMessage == "NotFound")
                {
                    await DisplayAlert("Warning", "There are no orders for the customer.", "OK");
                    return;
                }
                if (orders is null)
                {
                    await DisplayAlert("Error", errorMessage ?? "Unable to retrieve orders.", "OK");
                    return;
                }
                else
                {
                    CvOrders.ItemsSource = orders;
                }
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "An error occurred while retrieving the orders. Please try again later.", "OK");
            }
        }
    

    private void CvOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedItem = e.CurrentSelection.FirstOrDefault() as OrdersByUser;

        if (selectedItem == null) return;

        Navigation.PushAsync(new OrderDetailPage(selectedItem.Id,
                                                    selectedItem.TotalPrice,
                                                    _apiService,
                                                    _validator, _favoritesService));

        ((CollectionView)sender).SelectedItem = null;
    }
    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

}