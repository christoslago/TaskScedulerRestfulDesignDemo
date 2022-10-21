using Logic.Helpers;
using Logic.Services;
using Logic.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;
using Repository.DBContext;
using Repository.Infrastructure;
using System.Text.Json.Serialization;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


IConfiguration configuration = builder.Configuration;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(options =>
    {
        configuration.Bind("AzureOptions", options);
        options.TokenValidationParameters.NameClaimType = "name";
    },
    options => { configuration.Bind("AzureOptions", options); });
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddDbContext<DataContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IPersonsService,PersonsService>();
builder.Services.AddScoped<ISupervisorsService, SupervisorsService>();
var smtpOptions = new SmtpParams();
configuration.Bind(nameof(smtpOptions), smtpOptions);
builder.Services.AddScoped<IEmailsService,EmailsService>(x => new EmailsService(smtpOptions));
var azureOptions = new AzureParams();
configuration.Bind(nameof(azureOptions), azureOptions);
builder.Services.AddScoped<IAzureService,AzureService>(x => new AzureService(azureOptions));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    db.Database.Migrate();
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
IdentityModelEventSource.ShowPII = true;
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
