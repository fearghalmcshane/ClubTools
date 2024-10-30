using ClubTools.UI.Shared.Services.ArticleService;
using ClubTools.UI.Shared.Services.AuthService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ClubTools.Mobile
{
    public static class MauiProgram
    {
        public static string Base = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5227" : "https://localhost:5001";
        public static string APIUrl = $"{Base}/";

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

            builder.Services.AddSingleton(sp => new HttpClient() { BaseAddress = new Uri(APIUrl) });

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IArticleService, ArticleService>();

            return builder.Build();
        }
    }
}
