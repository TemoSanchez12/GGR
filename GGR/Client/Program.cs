global using Microsoft.AspNetCore.Components.Authorization;
global using Blazored.LocalStorage;
global using CurrieTechnologies.Razor.SweetAlert2;
using GGR.Client;
using GGR.Client.Auth;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });


// DependencyInjection
GGR.Client.Areas.Users.DependencyInjection.Configure(builder.Services, builder.Configuration);
GGR.Client.Areas.Rewards.DependencyInjection.Configure(builder.Services, builder.Configuration);

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddSweetAlert2();

await builder.Build().RunAsync();
