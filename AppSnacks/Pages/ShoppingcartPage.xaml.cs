using AppLanches.Validations;
using AppLanches.Services;
using System.Collections.ObjectModel;
using AppLanches.Models;

namespace AppLanches.Pages;

public partial class ShoppingcartPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;

    private bool _loginPageDisplayed = false;
    ObservableCollection<ShoppingCartItem> ItemsShoppingCart = new ObservableCollection<ShoppingCartItem>();

    public ShoppingcartPage(ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetItemsShoppingCart();
    }
    private async Task<IEnumerable<ShoppingCartItem>> GetItemsShoppingCart()
    {
        try
        {
            var userId = Preferences.Get("userid", 0);
            var (cartItems, errorMessage) = await _apiService.GetShoppingCartItems(userId);
            if (errorMessage == "Unauthorized" && _loginPageDisplayed)
            {
                await DisplayLoginPage();
                return Enumerable.Empty<ShoppingCartItem>();
            }
            if (cartItems == null)
            {
                await DisplayAlert("Error", errorMessage ?? "It was not possible to fetch the products", "OK");
                return Enumerable.Empty<ShoppingCartItem>();
            }
            ItemsShoppingCart.Clear();
            foreach (var item in cartItems)
            {
                ItemsShoppingCart.Add(item);
            }
            CvCart.ItemsSource = ItemsShoppingCart;
            UpdateTotalPrice();
            return cartItems;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"There was an unexpected error: {ex.Message}", "OK");
            return Enumerable.Empty<ShoppingCartItem>();
        }
    }
    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }
    private void UpdateTotalPrice()
    {
        try
        {
            var total = ItemsShoppingCart.Sum(item => item.Price * item.Quantity);
            LblTotalPrice.Text = total.ToString();
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"There was an error updating the price: {ex.Message}", "OK");
        }
    }

    private void TapConfirmOrder_Tapped(object sender, TappedEventArgs e)
    {

    }

    private void BtnEditAddress_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnDelete_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnIncrement_Clicked(object sender, EventArgs e)
    {

    }

    private void BtnDecrement_Clicked(object sender, EventArgs e)
    {

    }
}