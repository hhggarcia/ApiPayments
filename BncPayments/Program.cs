using BncPayments.Jobs;
using BncPayments.Middleware;
using BncPayments.Models;
using BncPayments.Services;
using ClassLibrary.BncModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz.Impl;
using Quartz.Spi;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// db config
var connectionString = builder.Configuration.GetConnectionString("ApplicationDBContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDBContextConnection' not found.");

builder.Services.AddDbContext<DbEpaymentsContext>(options =>
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


// SCOPED
builder.Services.AddScoped<IBncServices, BncServices>();
builder.Services.AddScoped<IRequestServices, RequestServices>();
builder.Services.AddScoped<IResponseServices, ResponseServices>();
builder.Services.AddScoped<WorkingKeyServices>();


// TRANSIENTS
builder.Services.AddTransient<IHashService, HashServices>();
builder.Services.AddTransient<IEncryptionServices, EncryptionServices>();


// SINGLETON
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<ApiBncSettings>>().Value);
builder.Services.AddSingleton<WorkKeyUpdateJob>();
builder.Services.AddSingleton<IJobFactory, SingletonJobFactory>();
builder.Services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

var jobSchedules = new[]
{
    new JobSchedule(
        jobType: typeof(WorkKeyUpdateJob),
        cronExpression: "0 10 1 * * ?" // Todos los días a la 1:10 AM
    )
};

builder.Services.AddSingleton(jobSchedules);

builder.Services.AddHostedService<QuartzHostedService>();
//builder.Services.AddHostedService<WorkKeyUpdateServices>();

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