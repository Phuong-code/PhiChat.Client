using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhiChat.Client.Services.ChatHub
{
    public class ChatHub
    {
        private readonly HubConnection hubConnection;
        private readonly List<Action<int, string>> onReceiveMessageHandler;
        private readonly ServiceProvider _serviceProvider;

        public ChatHub(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            var devSslHelper = new DevHttpsConnectionHelper(sslPort: 7105);
            hubConnection = new HubConnectionBuilder()
                .WithUrl(devSslHelper.DevServerRootUrl + "/ChatHub", options =>
                {
                    options.Headers.Add("ChatHubBearer", _serviceProvider._accessToken);
#if ANDROID
                    options.HttpMessageHandlerFactory = m => devSslHelper.GetPlatformMessageHandler();
#endif
                }).Build();

            onReceiveMessageHandler = new List<Action<int, string>>();
            hubConnection.On<int, string>("ReceiveMessage", OnReceiveMessage);
        }

        public async void Connect()
        {
            await hubConnection.StartAsync();
        }

        public async void Disconnect()
        {
            await hubConnection?.StopAsync();
        }

        public async void SendMessageToUser(int fromUserId, int toUserId, string message)
        {
            await hubConnection.InvokeAsync("SendMessageToUser", fromUserId, toUserId, message);
        }

        public void AddReceivedMessageHandler(Action<int, string> handler)
        {
            onReceiveMessageHandler.Add(handler);
        }

        void OnReceiveMessage(int userId, string message)
        {
            foreach (var handler in onReceiveMessageHandler)
            {
                handler(userId, message);
            }
        }
    }
}
