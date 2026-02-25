using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TradeUp.Client;
using TradeUp.Client.Components;
using TradeUp.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddTransient<CacheBustingHandler>();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

builder.Services.AddHttpClient("TradeUp.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<CacheBustingHandler>();


// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("TradeUp.ServerAPI"));

builder.Services.AddHttpClient("Public", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<CacheBustingHandler>();

//services registration
builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<AssetService>();
builder.Services.AddScoped<ToolBarService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TranslationService>();

builder.Services.AddScoped<StateContainerService>();
builder.Services.AddScoped<UserLoggedService>();
builder.Services.AddScoped<UserOptionsService>();

builder.Services.AddScoped<FeatureManagerService>();
builder.Services.AddScoped<ThemeManagerService>();

builder.Services.AddViewModels();

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<AuthenticationStateProvider, HostAuthenticationStateProvider>();

builder.Services.AddScoped<
    AccountClaimsPrincipalFactory<RemoteUserAccount>,
    CustomAccountClaimsPrincipalFactory>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var translationService = scope.ServiceProvider.GetRequiredService<TranslationService>();
    await translationService.Initialize();
}


await host.RunAsync();
