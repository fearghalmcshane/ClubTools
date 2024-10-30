using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ClubTools.UI.Shared;
using ClubTools.UI.Shared.Services.ArticleService;
using ClubTools.UI.Shared.Services.AuthService;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var clubToolsApiBaseUri = builder.Configuration.GetSection("ClubTools").GetValue<String>("ApiBaseUri") ?? throw new InvalidOperationException("API Base Uri string 'ApiBaseUri' not found.");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(clubToolsApiBaseUri) });

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IArticleService, ArticleService>();

await builder.Build().RunAsync();
