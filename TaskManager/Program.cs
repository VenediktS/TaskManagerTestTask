﻿
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.DB;
using TaskManager.Domain.Jobs;
using TaskManager.Domain.TaskDomain;
using TaskManager.Domain.TaskRequests;

using TaskManagerWeb.Handlers;

namespace TaskManager;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        // Add services to the container.
        builder.Services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(AddTaskRequest).Assembly);
        });

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<TaskManagerDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("LocalDbConnection"));
        });

       builder.Services.AddHostedService<TaskStatusUpdateJob>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}

