using AppLanches.Models;
using AppLanches.Services;
using AppLanches.Validations;

namespace AppLanches.Pages;

public partial class ProductDetailsPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private int _productId;
    private bool _loginPageDisplayed = false;
    private readonly FavoriteService _favoritesService = new FavoriteService();
    private string? _imageUrl;
    public ProductDetailsPage(int productId, string productName, ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        _productId = productId;
        
    }

    // Método chamado quando a página aparece
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await GetProductDetails(_productId);
    }

    private async Task<Product?> GetProductDetails(int productId)
    {
        var (productDetail, errorMessage) = await _apiService.GetProductDetail(productId);

        if (errorMessage == "Unauthorized" && !_loginPageDisplayed)
        {
            await DisplayLoginPage();
            return null;
        }

        // Verificar se houve algum erro na obtenção das produtos
        if (productDetail == null)
        {
            // Lidar com o erro, exibir mensagem ou logar
            await DisplayAlert("Erro", errorMessage ?? "Não foi possível obter o produto.", "OK");
            return null;
        }

        if (productDetail != null)
        {
            // Atualizar as propriedades dos controles com os dados do produto
            ProductImage.Source = productDetail.ImagePath;
            LblProductName.Text = productDetail.Name;
            LblProductPrice.Text = productDetail.Price.ToString();
            LblProductDescription.Text = productDetail.Detail;
            LblTotalPrice.Text = productDetail.Price.ToString();
            _imageUrl = productDetail.ImagePath;
        }
        else
        {
            await DisplayAlert("Erro", errorMessage ?? "Não foi possível obter os detalhes do produto.", "OK");
            return null;
        }
        return productDetail;
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

    private async void ImageBtnFavorite_Clicked(object sender, EventArgs e)
    {
        try
        {
            var favoriteExists = await _favoritesService.ReadAsync(_productId);
            if (favoriteExists is not null)
            {
                await _favoritesService.DeleteAsync(favoriteExists);
            }
            else
            {
                var favoriteProduct = new FavoriteProduct()
                {
                    ProductId = _productId,
                    IsFavorite = true,
                    Details = LblProductDescription.Text,
                    Name = LblProductName.Text,
                    Price = Convert.ToDecimal(LblProductPrice.Text),
                    ImageUrl = _imageUrl
                };
                await _favoritesService.CreateAsync(favoriteProduct);
            }
            UpdateFavoritesButton();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"There was an unexpected error: {ex.Message}", "OK");
        }
    }

    private async void UpdateFavoritesButton()
    {
        var favoriteExists = await _favoritesService.ReadAsync(_productId);

        if (favoriteExists is not null)
        {
            ImageBtnFavorite.Source = "heartfill";
        }
        else
        {
            ImageBtnFavorite.Source = "heart";
        }
    }

    private void BtnRemove_Clicked(object sender, EventArgs e)
    {

        if (int.TryParse(LblQuantity.Text, out int quantity) &&
        decimal.TryParse(LblProductPrice.Text, out decimal unitPrice))
        {
            quantity = Math.Max(1, quantity - 1);
            LblQuantity.Text = quantity.ToString();

            var totalPrice = quantity + unitPrice;
            LblTotalPrice.Text = totalPrice.ToString();
        }
        else
        {
            DisplayAlert("Error", "Invalid Values", "Ok");
        }
    }

    private void BtnAdd_Clicked(object sender, EventArgs e)
    {
        if(int.TryParse(LblQuantity.Text, out int quantity) &&
        decimal.TryParse(LblProductPrice.Text, out decimal unitPrice))
        {
            quantity++;
            LblQuantity.Text = quantity.ToString();

            var totalPrice = quantity + unitPrice;
            LblTotalPrice.Text = totalPrice.ToString();
        }
        else
        {
            DisplayAlert("Error", "Invalid Values", "Ok");
        }
    }

    private async void BtnAddToTheCart_Clicked(object sender, EventArgs e)
    {
        try
        {
            var cart = new ShoppingCart()
            {
                Quantity = Convert.ToInt32(LblQuantity.Text),
                UnitPrice = Convert.ToDecimal(LblProductPrice.Text),
                TotalValue = Convert.ToDecimal(LblTotalPrice.Text),
                ProductId = _productId,
                ClientId = Preferences.Get("userid", 0),
            };
            var response = await _apiService.AddItemToCart(cart);
            if (response.Data)
            {
                await DisplayAlert("Success", "Item added to cart!", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", $"Failure adding item to cart: {response.ErrorMessage}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"There was an unexpected error: {ex.Message}", "OK");
        }
    }
}