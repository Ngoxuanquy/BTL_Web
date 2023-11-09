using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string idPhong, string user, string message)
        {
            if(idPhong == "2") {
            if (IsValidUser(user))
            {
                await Clients.All.SendAsync("ReceiveMessage", user, message);
            }
            else
            {
                await Clients.Caller.SendAsync("InvalidUser", "Your user is not valid.");
            }
            }
            else {
                await Clients.Caller.SendAsync("InvalidUser", "Sai phongf");

            }

        }

        private bool IsValidUser(string user)
        {
            // Your validation logic here, for example, checking in a database.
            return !string.IsNullOrEmpty(user);
        }
    }
}
