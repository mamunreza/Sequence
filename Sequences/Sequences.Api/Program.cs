using FluentValidation;
using FluentValidation.AspNetCore;
using Sequences.Exception;
using Sequences.Service;
using Sequences.Validation;
using ConfigurationProvider = Sequences.Service.ConfigurationProvider;
using IConfigurationProvider = Sequences.Service.IConfigurationProvider;

namespace Sequences.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<FibonacciQueryValidator>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //TODO: Move dependency registration to separate class

            builder.Services.Configure<FibonacciMemoryCacheOptions>(
                builder.Configuration.GetSection(FibonacciMemoryCacheOptions.FibonacciMemoryCache));
            builder.Services.Configure<ArtificialProcessDelayOptions>(
                builder.Configuration.GetSection(ArtificialProcessDelayOptions.ArtificialProcessDelay));
            builder.Services.AddSingleton<IConfigurationProvider, ConfigurationProvider>();

            builder.Services.AddSingleton<IFibonacciCacheService, FibonacciCacheService>();
            builder.Services.AddTransient<IFibonacciService, FibonacciService>();
            builder.Services.AddSingleton<IMemoryChecker, MemoryChecker>();

            builder.Services.AddMemoryCache();

            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.MapControllers();

            app.Run();
        }
    }
}