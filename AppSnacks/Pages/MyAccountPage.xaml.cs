using AppLanches.Services;

namespace AppLanches.Pages;

public partial class MyAccoutPage : ContentPage
{
    private readonly ApiService _apiService;
    private const string UserNameKey = "username";
	private const string UserEmailKey = "useremail";
    private const string UserPhoneKey = "userphone";

    public MyAccoutPage(ApiService apiService)
	{
		InitializeComponent();
        _apiService = apiService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        LoadUserInfo();
        ImgBtnProfile.Source = await GetProfileImageAsync();
    }

    private void LoadUserInfo()
    {
        LblUserName.Text = Preferences.Get(UserNameKey, string.Empty);
        EntName.Text = LblUserName.Text;
        EntEmail.Text = Preferences.Get(UserEmailKey, string.Empty);
        EntPhone.Text = Preferences.Get(UserPhoneKey, string.Empty);
    }

    private async Task<string?> GetProfileImageAsync()
    {
        string defaultImage = AppConfig.StandardImage;

        var (response, errorMessage) = await _apiService.GetUserProfileImage();

        if (errorMessage is not null)
        {
            switch (errorMessage)
            {
                case "Unauthorized":
                    await DisplayAlert("Error", "Not authorized", "OK");
                    return defaultImage;
                default:
                    await DisplayAlert("Error", errorMessage ?? "Unable to get the image.", "OK");
                    return defaultImage;
            }
        }

        if (response?.UrlImage is not null)
        {
            return response.ImagePath;
        }
        return defaultImage;
    }
    private async void Button_Clicked(object sender, EventArgs e)
    {
        Preferences.Set(UserNameKey, EntName.Text);
        Preferences.Set(UserEmailKey, EntEmail.Text);
        Preferences.Set(UserPhoneKey, EntPhone.Text);
        await DisplayAlert("Information Saved", "Your information has been successfully saved!", "OK");
    }
}