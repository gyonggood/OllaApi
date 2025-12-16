using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using OllaApi.Data;
using OllaApi.Models;
using System.Security.Claims;

namespace OllaApi.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly AppDbContext _db;

        public ChatHub(AppDbContext db) => _db = db;

        public async Task SendMessage(int adId, int toUserId, string text)
        {
            var fromUserId = int.Parse(Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var msg = new Message
            {
                AdId = adId,
                FromUserId = fromUserId,
                ToUserId = toUserId,
                Text = text
            };

            _db.Messages.Add(msg);
            await _db.SaveChangesAsync();

            await Clients.User(fromUserId.ToString()).SendAsync("ReceiveMessage", msg);
            await Clients.User(toUserId.ToString()).SendAsync("ReceiveMessage", msg);
        }
    }
}