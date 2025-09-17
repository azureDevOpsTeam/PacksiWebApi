using ApplicationLayer;
using AspNetCoreRateLimit;
using InfrastructureLayer;
using InfrastructureLayer.Seed;
using PresentationApp.Hubs;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Register(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add SignalR
builder.Services.AddSignalR();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var botToken = builder.Configuration["8109507045:AAG5iY_c1jLUSDeOOPL1N4bnXPWSvwVgx4A"];
var botClient = new TelegramBotClient(botToken);
await botClient.SetWebhook("https://api.packsi.net/api/miniapp/bot");

builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Configure(context.Configuration.GetSection("Kestrel"));
});
var app = builder.Build();

await ApplicationDbSeeder.SeedAsync(app.Services);

// Configure the HTTP request pipeline.
//In Produxtion UnXommewnt This
//if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/Identity/swagger.json", "Identity");
        options.SwaggerEndpoint("/swagger/Administrator/swagger.json", "Administrator");
        options.SwaggerEndpoint("/swagger/Managers/swagger.json", "Managers");
        options.SwaggerEndpoint("/swagger/Users/swagger.json", "Users");
        options.SwaggerEndpoint("/swagger/MiniApp/swagger.json", "MiniApp");
    });
}
// else
// {
//     app.UseHsts();
// }

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseIpRateLimiting();
app.UseClientRateLimiting();

app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.MapControllers();

// Map SignalR Hub
app.MapHub<ChatHub>("/chathub").RequireCors("AllowSpecificOrigin");

app.Run();