using PhiChat.Client.Services.ChatHub;
using PhiChat.Client.Services.Message;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PhiChat.Client.ViewModels
{
    public partial class ChatPageViewModel : ObservableObject, IQueryAttributable
    {
        [ObservableProperty]
        private int fromUserId;

        [ObservableProperty]
        private int toUserId;

        [ObservableProperty]
        private User friendInfo;

        [ObservableProperty]
        private ObservableCollection<Message> messages;

        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        private bool isVisible = true;

        [ObservableProperty]
        private string message;

        [ObservableProperty]
        private bool messageEntryIsFocus;

        [ObservableProperty]
        private CollectionView chatCollectionView;

        private readonly ServiceProvider _serviceProvider;

        private readonly ChatHub _chatHub;

        public ChatPageViewModel(ServiceProvider serviceProvider, ChatHub chatHub)
        {
            Messages = new ObservableCollection<Message>();
            _serviceProvider = serviceProvider;
            _chatHub = chatHub;
            _chatHub.AddReceivedMessageHandler(OnReceiveMessage);
            //_chatHub.Connect();
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query == null || query.Count == 0) return;

            FromUserId = int.Parse(HttpUtility.UrlDecode(query["fromUserId"].ToString()));
            ToUserId = int.Parse(HttpUtility.UrlDecode(query["toUserId"].ToString()));

        }

        public async void Initialize()
        {
            await GetMessages();
            ScrollToBottom();

        }

        [RelayCommand]
        public async Task SendMessage()
        {
            try
            {
                if (Message == null || Message.Trim() == "")
                {
                    return;
                }
                else
                {
                    _chatHub.SendMessageToUser(FromUserId, ToUserId, Message);

                    Messages.Add(new Message
                    {
                        Content = Message.Trim(),
                        FromUserId = FromUserId,
                        ToUserId = ToUserId,
                        SendDateTime = DateTime.Now
                    });

                    Message = "";

                    ScrollToBottom();
                }
            }
            catch (Exception ex)
            {
                await AppShell.Current.DisplayAlert("PhiChat", ex.Message, "OK");
            }
        }

        [RelayCommand]
        private async Task Refresh()
        {
            IsVisible = false;
            await GetMessages();
            IsVisible = true;
        }

        private async Task GetMessages()
        {
            IsRefreshing = true;
            try
            {
                var request = new MessageInitializeRequest
                {
                    FromUserId = FromUserId,
                    ToUserId = ToUserId,
                };

                var response = await _serviceProvider.CallWebApi<MessageInitializeRequest, MessageInitializeReponse>
                    ("/Message/Initialize", HttpMethod.Post, request);

                if (response.StatusCode == 200)
                {
                    FriendInfo = response.FriendInfo;
                    Messages = new ObservableCollection<Message>(response.Messages);

                    //var message = new Message
                    //{
                    //    FromUserId = 0,
                    //    SendDateTime = DateTime.Now,
                    //    Content = null,
                    //};

                    //Messages.Insert(2, message);
                }
                else
                {
                    await AppShell.Current.DisplayAlert("ChatApp", response.StatusMessage, "OK");
                }
            }
            catch (Exception ex)
            {
                await AppShell.Current.DisplayAlert("Can't get previous messages", $"{ex.Message}", "OK");
            }
            finally
            {
                IsRefreshing = false; 
            }
        }

        private void OnReceiveMessage(int fromUserId, string message)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                Messages.Add(new Models.Message
                {
                    Content = message,
                    FromUserId = ToUserId,
                    ToUserId = FromUserId,
                    SendDateTime = DateTime.Now,
                });


                ScrollToBottom();

                var request = new MessageInitializeRequest
                {
                    FromUserId = FromUserId,
                    ToUserId = ToUserId,
                };

                await _serviceProvider.CallWebApi<MessageInitializeRequest, MessageInitializeReponse>("/Message/ReadMessage", HttpMethod.Post, request);
            });
        }

        public void ScrollToBottom()
        {
            if (Messages.Count != 0 )
            {
                ChatCollectionView.ScrollTo(Messages.Last(), position: ScrollToPosition.End, animate: true);
            }
        }
    }
}
