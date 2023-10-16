using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhiChat.Client.ViewModels
{
    public partial class LoginPageViewModel : ObservableObject
    {

        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool isProcessing;

        [ObservableProperty]
        private Entry usernameEntry;

        [ObservableProperty]
        private Entry passwordEntry;


        private readonly ServiceProvider _serviceProvider;


        public LoginPageViewModel(ServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        [RelayCommand]
        public async Task Login()
        {
            IsProcessing = true;
            UsernameEntry.IsEnabled = false;
            PasswordEntry.IsEnabled = false;
            try
            {
                var request = new AuthenticateRequest
                {
                    LoginId = Username,
                    Password = Password,
                };
                var response = await _serviceProvider.Authenticate(request);
                if (response.StatusCode == 200)
                {
                    await Shell.Current.GoToAsync($"ListChatPage?userId={response.Id}");
                }
                else
                {
                    await AppShell.Current.DisplayAlert("PhiChat", response.StatusMessage, "OK");
                }
            }
            catch (Exception ex)
            {
                await AppShell.Current.DisplayAlert("PhiChat ", ex.Message, "OK");
            }
            finally
            {
                IsProcessing = false;
                UsernameEntry.IsEnabled = true;
                PasswordEntry.IsEnabled = true;

            }
        }


    }
}
