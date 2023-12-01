using Microsoft.AspNetCore.SignalR;

namespace SignalR.Hubs
{
    public class MessageHub:Hub
    {
        static Dictionary<string, string> users = new Dictionary<string, string>();
        private readonly IWebHostEnvironment env;
        public MessageHub(IWebHostEnvironment env)
        {
            this.env = env;
        }
        public async override Task OnConnectedAsync()//1
        {
            await Clients.Caller.SendAsync("connection", "You are connected");
        }
        public override Task OnDisconnectedAsync(Exception? exception)//2
        {
            return base.OnDisconnectedAsync(exception);
        }
        public async Task JoinAsync(string username)//3
        {
            users.Add(Context.ConnectionId, username);
            await Clients.All.SendAsync("userlist", users.Values.ToList());
        }
        public async Task SendAllAsync(string msg)//4
        {
            // await Clients.All.SendAsync("message", msg);

            await Clients.All.SendAsync("message", new { sender = users[Context.ConnectionId], message = msg });
        }
        public async Task UploadAsync(FileData data) //6
        {
            string ext = Path.GetExtension(data.Filename);
            string fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
            string savePath = Path.Combine(this.env.WebRootPath, "Uploads", fileName);
            data.Content = data.Content.Substring(data.Content.LastIndexOf(',') + 1);
            string converted = data.Content.Replace('-', '+').Replace('_', '/');

            byte[] buffer = Convert.FromBase64String(converted);
            FileStream fs = new FileStream(savePath, FileMode.Create);
            fs.Write(buffer, 0, buffer.Length);
            fs.Close();
            await Clients.All.SendAsync("uploaded", new { sender = users[Context.ConnectionId], File = fileName, Type = ext });

        }

    }
    public class FileData //5
    {
        public string Filename { get; set; } = default!;
        public string Content { get; set; } = default!;
    }
}
