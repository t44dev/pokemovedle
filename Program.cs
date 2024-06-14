using PokeMovedle.Models.Moves;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddMemoryCache();

var app = builder.Build();

// Setup MoveManager
MoveManager.moveFetcher = new PokeAPIMoveFetcher(app.Services.GetRequiredService<IMemoryCache>());
MoveManager manager = await MoveManager.Instance();
Console.WriteLine(manager.move?.name);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
