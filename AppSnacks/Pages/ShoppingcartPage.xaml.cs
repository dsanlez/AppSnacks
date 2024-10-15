using AppLanches.Models;
using AppLanches.Services;
using AppLanches.Validations;
using System.Collections.ObjectModel;

namespace AppLanches.Pages;

public partial class ShoppingcartPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private readonly FavoriteService _favoritesService;
    private bool _loginPageDisplayed = false;
    private bool _isNavigatingToEmptyCartPage = false;
    ObservableCollection<ShoppingCartItem> ItemsShoppingCart = new ObservableCollection<ShoppingCartItem>();

    public ShoppingcartPage(ApiService apiService, IValidator validator, FavoriteService favoritesService)
    {
        InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _favoritesService = favoritesService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (IsNavigatingToEmptyCartPage()) return;

        bool hasItems = await GetItemsShoppingCart();
        if (hasItems)
        {
            ShowAddress();
        }
        else
        {
            await NavigateToEmptyCart();
        }
    }

    private async Task NavigateToEmptyCart()
    {
        LblAddress.Text = string.Empty;
        _isNavigatingToEmptyCartPage = true;
        await Navigation.PushAsync(new EmptyCartPage());
    }

    private bool IsNavigatingToEmptyCartPage()
    {
        if (_isNavigatingToEmptyCartPage)
        {
            _isNavigatingToEmptyCartPage = false;
            return true;
        }
        return false;
    }

    private void ShowAddress()
    {
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
            LblAddress.Text = "Inform your adress";
        }
    }
    private async Task<bool> GetItemsShoppingCart()
    {
        try
        {
            var userId = Preferences.Get("userid", 0);
            var (cartItems, errorMessage) = await _apiService.GetShoppingCartItems(userId);
            if (errorMessage == "Unauthorized" && _loginPageDisplayed)
            {
                await DisplayLoginPage();
                return false;
            }
            if (cartItems == null)
            {
                await DisplayAlert("Error", errorMessage ?? "It was not possible to fetch the products", "OK");
                return false;
            }
            ItemsShoppingCart.Clear();
            foreach (var item in cartItems)
            {
                ItemsShoppingCart.Add(item);
            }
            CvCart.ItemsSource = ItemsShoppingCart;
            UpdateTotalPrice();

            if (!ItemsShoppingCart.Any())
            {
                return false;
            }
            return true;

        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"There was an unexpected error: {ex.Message}", "OK");
            return false;
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

    private async void TapConfirmOrder_Tapped(object sender, TappedEventArgs e)
    {
        if (ItemsShoppingCart == null || !ItemsShoppingCart.Any())
        {
            await DisplayAlert("Warning", "Your shopping cart is empty!", "OK");
            return;
        }
        var order = new Order()
        {
            Address = LblAddress.Text,
            UserId = Preferences.Get("userid", 0),
            Total = Convert.ToDecimal(LblTotalPrice.Text),
        };
        var response = await _apiService.ConfirmOrder(order);
        if (response.HasError)
        {
            if (response.ErrorMessage == "Unauthorized")
            {
                await DisplayLoginPage();
                return;
            }
            await DisplayAlert("Error", $"Something went wrong: {response.ErrorMessage}", "Cancel");
            return;
        }
        ItemsShoppingCart.Clear();
        LblAddress.Text = "Enter your address";
        LblTotalPrice.Text = "0.00";

        await Navigation.PushAsync(new OrderConfirmedPage());
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