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

        bool savedAddress = Preferences.ContainsKey("address");
        if (savedAddress)
        {
            string name = Preferences.Get("name", string.Empty);
            string address = Preferences.Get("address", string.Empty);
            string phonenumber = Preferences.Get("phonenumber", string.Empty);
            LblAddress.Text = $"{name}\n{address}\n{phonenumber}";
        }
        else
        {
            LblAddress.Text = "Enter your address";
        }
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

    private async void BtnDelete_Clicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is ShoppingCartItem cartItem)
        {
            bool response = await DisplayAlert("Confirmation",
                          "Are you sure you want to remove this item from your cart?", "Yes", "No");
            if (response)
            {
                ItemsShoppingCart.Remove(cartItem);
                UpdateTotalPrice();
                await _apiService.UpdateItemCartQuantity(cartItem.ProductId, "delete");

            }
        }
    }

    private async void BtnIncrement_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is ShoppingCartItem cartItem)
        {
            cartItem.Quantity++;
            UpdateTotalPrice();
            await _apiService.UpdateItemCartQuantity(cartItem.ProductId, "increment");
        }
    }

    private async void BtnDecrement_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is ShoppingCartItem cartItem)
        {
            if (cartItem.Quantity == 1) return;
            else
            {
                cartItem.Quantity--;
                UpdateTotalPrice();
                await _apiService.UpdateItemCartQuantity(cartItem.ProductId, "decrement");
            }
        }
    }
}