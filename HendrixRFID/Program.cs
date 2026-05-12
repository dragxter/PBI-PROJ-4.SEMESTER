using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using HendrixRFID.Data;
using HendrixRFID.DTOs;
using HendrixRFID.Services;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=hendrix.db"));

// Channel — køen mellem MQTT-lytter og processor
var channel = Channel.CreateBounded<MqttScanMessage>(500);
builder.Services.AddSingleton(channel);

// Services
builder.Services.AddSingleton<EartagDecoder>();
builder.Services.AddScoped<PigService>();
builder.Services.AddScoped<PigLocationService>();
builder.Services.AddScoped<LocationDecisionService>();

// Background services
builder.Services.AddHostedService<MqttListenerService>();
builder.Services.AddHostedService<ScanProcessorService>();
builder.Services.AddHostedService<JobService>();

// REST API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowDashboard",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

app.UseCors("AllowDashboard");

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();