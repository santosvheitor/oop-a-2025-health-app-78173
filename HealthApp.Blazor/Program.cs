using HealthApp.Blazor; 
using HealthApp.Blazor.Auth; 
using HealthApp.Blazor.Mappings; 
using Microsoft.AspNetCore.Components.Web; 
using Microsoft.AspNetCore.Components.WebAssembly.Hosting; 
using Microsoft.AspNetCore.Components.Authorization; 
using System.Net.Http;
using Microsoft.AspNetCore.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args); 
builder.RootComponents.Add<App>("#app"); 
builder.RootComponents.Add<HeadOutlet>("head::after"); 

//AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile)); 

// Adds support for JWT authentication and authorization
builder.Services.AddOptions(); 
builder.Services.AddAuthorizationCore(); 


builder.Services.AddScoped<CustomAuthStateProvider>(); 
builder.Services.AddScoped<AuthenticationStateProvider>(sp => 
    sp.GetRequiredService<CustomAuthStateProvider>()); 

// Configures HttpClient to point to your API
builder.Services.AddScoped(sp =>
{
    var navigationManager = sp.GetRequiredService<NavigationManager>();
    return new HttpClient
    {
        BaseAddress = new Uri("http://localhost:5101") // ✅ Use HTTPS!
    };
});

await builder.Build().RunAsync();