global using Microsoft.AspNetCore.Components;
global using Microsoft.AspNetCore.Components.Web;
global using Microsoft.AspNetCore.Hosting.StaticWebAssets;
global using Microsoft.AspNetCore.ResponseCompression;
global using Microsoft.AspNetCore.SignalR;
global using Microsoft.AspNetCore.HttpOverrides;

global using System.Text;
global using System.Text.Json;
global using System.Text.Json.Serialization;


global using MudBlazor.Services;
global using MudBlazorServer.Data;
global using MudBlazorServer.Pages;

global using MudBlazorServer.Hubs;
global using MudBlazorServer.Services;

global using SkiaSharp;
global using Markdig;

var builder = WebApplication.CreateBuilder(args);

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddSingleton<WeatherForecastService>();

builder.Services.AddSingleton<ThemeService>();

builder.Services.AddSignalR();
builder.Services.AddSignalR().AddHubOptions<AIVaryHub>(options =>
{
    options.MaximumReceiveMessageSize = 5 * 1024 * 1024; // 5MB
});


builder.Services.AddMudServices();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseAuthentication();

app.UseResponseCompression();

//app.UseHttpsRedirection();

app.UseStaticFiles();



app.UseRouting();

app.MapBlazorHub(); 

app.MapHub<AIChatHub>("/AIChatHub");
app.MapHub<AIGenerateHub>("/AIGenerateHub");
app.MapHub<AIVaryHub>("/AIVaryHub");

app.MapFallbackToPage("/_Host");

app.Run();