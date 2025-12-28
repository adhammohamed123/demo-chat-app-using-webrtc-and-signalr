using Microsoft.AspNetCore.SignalR;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(_ => true)
            .AllowCredentials();
    });
});
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseCors();
//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapHub<CallHub>("/callHub");
app.MapControllers();

app.Run();


public class CallHub : Hub
{
    // (Offer, Answer, Candidates)
    public async Task SendSignal(string signal, string targetId)
    {
        await Clients.Client(targetId).SendAsync("ReceiveSignal", Context.ConnectionId, signal);
    }

    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("UserJoined", Context.ConnectionId);
        await base.OnConnectedAsync();
    }
}