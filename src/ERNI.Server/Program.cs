using Autofac;
using Autofac.Extensions.DependencyInjection;
using ERNI.Core.DependencyInjection;
using ERNI.Infrastructure;
using ERNI.Server;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDatabase(builder.Configuration);

builder.Host.UseSerilog((ctx, configuration) =>
{
    configuration.ReadFrom.Configuration(ctx.Configuration);
});

builder.Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureContainer<ContainerBuilder>(builder =>
    {
        builder.RegisterModule<DatabaseModule>();
        builder.RegisterModule<CoreModule>();
        builder.RegisterHostedServices();
    });

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddlewares();
app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();

// Class exposure to integration testing fixtures.
public partial class Program
{
}
