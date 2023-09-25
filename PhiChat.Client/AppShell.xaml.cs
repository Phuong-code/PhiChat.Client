namespace PhiChat.Client
{
    public partial class AppShell : Shell
    {
        public AppShell(LoginPage loginPage)
        //public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("ListChatPage", typeof(ListChatPage));
            Routing.RegisterRoute("ChatPage", typeof(ChatPage));

            this.CurrentItem = loginPage;
        }
    }
}