using BncPayments.Middleware;
using BncPayments.Models;
using BncPayments.Services;
using ClassLibrary.BncModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// db config
var connectionString = builder.Configuration.GetConnectionString("ApplicationDBContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDBContextConnection' not found.");

builder.Services.AddDbContext<EpaymentsContext>(options =>
    options.UseSqlServer(connectionString, sqlServerOptionsAction: sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 100,
            maxRetryDelay: TimeSpan.FromSeconds(50),
            errorNumbersToAdd: null);
    }
));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ApiBncSettings>(builder.Configuration.GetSection("ApiBncSettings"));

// SINGLETON
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<ApiBncSettings>>().Value);
builder.Services.AddSingleton<IWorkingKeyServices, WorkingKeyServices>();

// SCOPED
builder.Services.AddScoped<IBncServices, BncServices>();
builder.Services.AddScoped<IRequestServices, RequestServices>();

// TRANSIENTS
builder.Services.AddTransient<IHashService, HashServices>();
builder.Services.AddTransient<IEncryptionServices, EncryptionServices>();

builder.Services.AddHostedService<WorkKeyUpdateServices>();

builder.Services.AddMemoryCache();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();