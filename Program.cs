using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.MSSqlServer.Sinks.MSSqlServer.Options;

namespace LogAcesso
{
    public class Program
    {        
        public static int Main(string[] args)
        {
            try
            {
                var hostBuilder = CreateHostBuilder(args).Build();
                Serilog.Log.Information("Iniciando Web Host");
                hostBuilder.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Serilog.Log.Fatal(ex, "Host encerrado inesperadamente");
                return 1;
            }
            finally
            {
                Serilog.Log.CloseAndFlush();
            }                        
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var settings = config.Build();
                    Serilog.Log.Logger = new LoggerConfiguration()
                        .Enrich.FromLogContext()
                        .WriteTo.MSSqlServer(settings.GetConnectionString("DefaultConnection"),
                            sinkOptions: new SinkOptions()
                            {
                                AutoCreateSqlTable = true,
                                TableName = "Log"
                            })                            
                        .CreateLogger();
                })
                // .UseSerilog()  // => Essa opção grava todos os eventos do sistema;
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
