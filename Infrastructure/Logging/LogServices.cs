using Serilog;

namespace Telinha.Infrastructure.Logging
{
    public class LogServices
    {
        public static void Configure()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/log-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .Enrich.FromLogContext()
                .CreateLogger();

            Log.Information("Log configurado com sucesso.");
        }

        // Método genérico para facilitar logs de erro com contexto
        public static void Error(Exception ex, string message, params object[] args)
        {
            Log.Error(ex, message, args);
        }
        public static void Info(string message, params object[] args)
        {
            Log.Information(message, args);
        }

        public static void Close()
        {
            Log.CloseAndFlush();
        }
    }
}
