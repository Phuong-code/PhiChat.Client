using PhiChat.Client.Services.ChatHub;
using PhiChat.Client.Services.ListChat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PhiChat.Client.ViewModels
{
    public partial class ListChatPageViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private User userInfo;

        [ObservableProperty]
        private ObservableCollection<User> userFriends;

        [ObservableProperty]
        private ObservableCollection<LastestMessage> lastestMessages;

        [ObservableProperty]
        private ObservableCollection<LastestMessage> allLastestMessages;

        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        private string searchText;

        [ObservableProperty]
        private Entry searchEntry;

        private readonly ServiceProvider _serviceProvider;
        private readonly ChatHub _chatHub;

        public ListChatPageViewModel(ServiceProvider serviceProvider, ChatHub chatHub)
        {
            _serviceProvider = serviceProvider;
            UserInfo = new User();
            UserFriends = new ObservableCollection<User>();
            LastestMessages = new ObservableCollection<LastestMessage>();

            _chatHub = chatHub;
            _chatHub.Connect();
            _chatHub.AddReceivedMessageHandler(OnReceivedMessage);

            //MessagingCenter.Send<string>("StartService", "MessageForegroundService");
            //MessagingCenter.Send<string, string[]>("StartService", "MessageNotificationService", new string[] { });

        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query == null || query.Count == 0) return;

            UserInfo.Id = int.Parse(HttpUtility.UrlDecode(query["userId"].ToString()));
        }
        public async void Initialize()
        {
            await GetListFriends();
        }

        private async Task GetListFriends()
        {
            IsRefreshing = true;
            try
            {
                var response = await _serviceProvider.CallWebApi<int, ListChatInitializeResponse>("/ListChat/Initialize", HttpMethod.Post, UserInfo.Id);

                if (response.StatusCode == 200)
                {
                    UserInfo = response.User;
                    UserFriends = new ObservableCollection<User>(response.UserFriends);
                    LastestMessages = new ObservableCollection<LastestMessage>(response.LastestMessages);
                    AllLastestMessages = new ObservableCollection<LastestMessage>(LastestMessages);
                }
                else
                {
                    await AppShell.Current.DisplayAlert("PhiChat", response.StatusMessage, "OK");
                }
            }
            catch (Exception ex)
            {
                await AppShell.Current.DisplayAlert("Can't get Friend Lists Data", $"{ex.Message}", "OK");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        public async Task OpenChatPage(int toUserId)
        {
            SearchEntry.IsEnabled = false;
            await Shell.Current.GoToAsync($"ChatPage?fromUserId={UserInfo.Id}&toUserId={toUserId}");
            SearchEntry.IsEnabled = true;

        }


        public void OnReceivedMessage(int fromUserId, string message)
        {
            var lastestMessage = LastestMessages.Where(x => x.UserFriendInfo.Id == fromUserId).FirstOrDefault();
            if (lastestMessage != null)
                LastestMessages.Remove(lastestMessage);

            var newLastestMessage = new LastestMessage
            {
                UserId = UserInfo.Id,
                Content = message,
                UserFriendInfo = UserFriends.Where(x => x.Id == fromUserId).FirstOrDefault()
            };

            LastestMessages.Insert(0, newLastestMessage);
            //OnPropertyChanged("LastestMessages");

            //MessagingCenter.Send<string, string[]>("Notify", "MessageNotificationService",
            //    new string[] { newLastestMessage.UserFriendInfo.UserName, newLastestMessage.Content });
        }

        [RelayCommand]
        public void Search()
        {
            LastestMessages = new ObservableCollection<LastestMessage>(AllLastestMessages);
            LastestMessages = new ObservableCollection<LastestMessage>(LastestMessages.Where(lm => lm.UserFriendInfo.UserName.ToLower().Contains(SearchText.ToLower())));

        }
    }
}
