using AppLanches.Validations;
using AppLanches.Services;

namespace AppLanches.Pages;

public partial class ProfilePage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly IValidator _validator;
    private bool _loginPageDisplayed = false;

    public ProfilePage(ApiService apiService, IValidator validator)
	{
		InitializeComponent();
        _apiService = apiService;
        _validator = validator;
        LblUserName.Text = Preferences.Get("username", string.Empty);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        ImgBtnProfile.Source = await GetProfileImage();
    }

    private async Task<string?> GetProfileImage()
    {
        // Obtenha a imagem padr o do AppConfig
        string standardImage = AppConfig.StandardImage;

        var (response, errorMessage) = await _apiService.GetUserProfileImage();

        // Lida com casos de erro
        if (errorMessage is not null)
        {
            switch (errorMessage)
            {
                case "Unauthorized":
                    if (!_loginPageDisplayed)
                    {
                        await DisplayLoginPage();
                        return null;
                    }
                    break;
                default:
                    await DisplayAlert("Error", errorMessage ?? "It was not possible to get the profile image.", "OK");
                    return standardImage;
            }
        }

        if (response?.UrlImage is not null)
        {
            return response.ImagePath;
        }

        return standardImage;
    }

    private void MyAccount_Tapped(object sender, TappedEventArgs e)
    {

    }
    
    private void BtnLogout_Clicked(object sender, EventArgs e)
    {

    }
    private void Questions_Tapped(object sender, TappedEventArgs e)
    {

    }

    private async void ImgBtnProfile_Clicked(object sender, EventArgs e)
    {
        try
        {
            var imageArray = await SelectImageAsync();
            if (imageArray is null)
            {
                await DisplayAlert("Error", "It was not possible to get the profile image", "OK");
                return;
            }
            ImgBtnProfile.Source = ImageSource.FromStream(() => new MemoryStream(imageArray));

            var response = await _apiService.UploadUserImage(imageArray);
            if (response.Data)
            {
                await DisplayAlert("", "Image sent with success", "Ok");
            }
            else
            {
                await DisplayAlert("Error", response.ErrorMessage ?? "An error has ocurred", "Canceled");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"An error has ocurred: {ex.Message}", "Ok");
        }
    }

    private async Task<byte[]?> SelectImageAsync()
    {
        try
        {
            var file = await MediaPicker.PickPhotoAsync();

            if (file is null) return null;

            using (var stream = await file.OpenReadAsync())
            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert("Error", "This feature is not supported on the device", "Ok");
        }
        catch (PermissionException)
        {
            await DisplayAlert("Error", "Permissions not granted to access the camera or gallery", "Ok");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error selecting the image: {ex.Message}", "Ok");
        }
        return null;
    }

    private async Task DisplayLoginPage()
    {
        _loginPageDisplayed = true;
        await Navigation.PushAsync(new LoginPage(_apiService, _validator));
    }

   
}