using SquadMasterWeb.Components;
using SquadMasterWeb.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Catalog is shared; game state is per browser circuit/session.
builder.Services.AddSingleton<HeroDataService>();
builder.Services.AddScoped<GameStateService>();

var app = builder.Build();

// Warm the card catalog at startup so the first request is fast.
_ = app.Services.GetRequiredService<HeroDataService>().EnsureLoadedAsync();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
