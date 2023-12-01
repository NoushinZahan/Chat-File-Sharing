
using SignalR.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR(op =>
{
    op.MaximumReceiveMessageSize = 1024 * 1024;
});
var app = builder.Build();
app.UseStaticFiles();
app.MapDefaultControllerRoute();
app.MapHub<MessageHub>("/messageHub");
app.Run();
