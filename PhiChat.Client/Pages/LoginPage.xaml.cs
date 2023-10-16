namespace PhiChat.Client.Pages;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginPageViewModel viewModel)
	{
		InitializeComponent();

        this.BindingContext = viewModel;

		viewModel.UsernameEntry = usernameEntry;

		viewModel.PasswordEntry = passwordEntry;

    }
}