using Server.Services.Tunnel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<TunnelRegistry>();
builder.Services.AddHostedService<TunnelCleanUp>();
builder.Services.AddSignalR(static cfg =>
{
#if DEBUG
    cfg.EnableDetailedErrors = true;
#endif
    // TODO: Expand this limit based on expected usage
    //cfg.MaximumReceiveMessageSize = 
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<Server.Hubs.TunnelHub>("/tunnel");

app.Run();
