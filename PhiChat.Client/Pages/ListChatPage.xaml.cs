namespace PhiChat.Client.Pages;

public partial class ListChatPage : ContentPage
{
	public ListChatPage(ListChatPageViewModel viewModel)
	{
		InitializeComponent();

		this.BindingContext = viewModel;

        viewModel.SearchEntry = searchEntry;
	}

    private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        (this.BindingContext as ListChatPageViewModel).Initialize();
    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        (this.BindingContext as ListChatPageViewModel).Search();
    }
}