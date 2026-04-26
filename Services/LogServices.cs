using Serilog;

namespace Telinha.Services
{
    public static class LogServices
    {
        private static readonly Lazy<Logger> _lazyLogger = new(InitLogger);

        public static Logger Logger => _lazyLogger.Value;

        private static Logger InitLogger()
        {
            var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "log-.txt");

            var logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .Enrich.FromLogContext() // permite ForContext
               .WriteTo.File(logPath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    fileSizeLimitBytes: 10_000_000,
                    rollOnFileSizeLimit: true,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
               .CreateLogger();

            logger.Information("Sistema de logs inicializado com sucesso.");
            return logger;
        }

        public static void EncerrarLog() => Log.CloseAndFlush();
    }
}