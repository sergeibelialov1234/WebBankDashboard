using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using WebBankDashboard;
using WebBankDashboard.BankClient;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IBankClient, BankClient>();
builder.Services.AddHttpClient<IBankClient, BankClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7247");
});
builder.Services.AddRadzenComponents();
var app = builder.Build();

await app.RunAsync();