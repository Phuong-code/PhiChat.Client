using Microsoft.Extensions.Logging;
using PhiChat.Client.Services.ChatHub;

namespace PhiChat.Client
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("MaterialIcons-Regular.ttf", "IconFontTypes");
                });

            //AppShell
            builder.Services.AddSingleton<AppShell>();

            //ChatHub
            builder.Services.AddSingleton<ChatHub>();

            //services
            builder.Services.AddSingleton<ServiceProvider>();

            //Pages
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<ListChatPage>();
            //builder.Services.AddSingleton<ChatPage>();
            builder.Services.AddTransient<ChatPage>();

            //View Models
            builder.Services.AddSingleton<LoginPageViewModel>();
            builder.Services.AddSingleton<ListChatPageViewModel>();
            builder.Services.AddSingleton<ChatPageViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif
            // Removing underline from entry
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) => {
#if ANDROID
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#endif
            });

            // Removing underline from editor
            Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping(nameof(Editor), (handler, view) => {
#if ANDROID
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#endif
            });
            return builder.Build();
        }
    }
}