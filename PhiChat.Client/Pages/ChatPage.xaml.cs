using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;

namespace PhiChat.Client.Pages;

public partial class ChatPage : ContentPage
{
    public ChatPage(ChatPageViewModel viewModel)
    {
        InitializeComponent();

        this.BindingContext = viewModel;

        viewModel.ChatCollectionView = chatCollectionView;

        viewModel.MessageEntryIsFocus = messageEntry.IsFocused;

        App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);

        SizeChanged += OnPageSizeChanged;
    }

    private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
    {
        (this.BindingContext as ChatPageViewModel).Initialize();

    }

    private void OnPageSizeChanged(object sender, EventArgs e)
    {
        (this.BindingContext as ChatPageViewModel).ScrollToBottom();
    }
}