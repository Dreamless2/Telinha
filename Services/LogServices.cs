using Serilog;
using System.Runtime.CompilerServices;

namespace Telinha.Services
{
    public static class LogServices
    {
        private static readonly Lazy<Logger> _lazyLogger = new(InitLogger, isThreadSafe: true);

        private static Logger InitLogger()
        {
            var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "log-.txt");

            var logger = new LoggerConfiguration()
              .MinimumLevel.Debug()
              .Enrich.FromLogContext()
              .WriteTo.File(logPath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    fileSizeLimitBytes: 10_000_000,
                    rollOnFileSizeLimit: true,
                    shared: true, // permite múltiplos processos
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{Caller}] {Message:lj}{NewLine}{Exception}")
              .CreateLogger();

            logger.Information("Sistema de logs inicializado com sucesso.");
            return logger;
        }

        public static void ConfigurarLog() => _ = _lazyLogger.Value; // força init

        public static void LogarInformacao(
            string mensagem,
            [CallerMemberName] string member = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0,
            params object?[] args)
        {
            _lazyLogger.Value.ForContext("Caller", $"{Path.GetFileName(path)}:{member}:{line}")
               .Information(mensagem, args);
        }

        public static void LogarErro(
            string mensagem,
            [CallerMemberName] string member = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0,
            params object?[] args)
        {
            _lazyLogger.Value.ForContext("Caller", $"{Path.GetFileName(path)}:{member}:{line}")
               .Error(mensagem, args);
        }

        public static void LogarErroComException(
            Exception ex,
            string mensagem,
            [CallerMemberName] string member = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0,
            params object?[] args)
        {
            _lazyLogger.Value.ForContext("Caller", $"{Path.GetFileName(path)}:{member}:{line}")
               .Error(ex, mensagem, args);
        }

        public static void LogarAlerta(
            string mensagem,
            [CallerMemberName] string member = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0,
            params object?[] args)
        {
            _lazyLogger.Value.ForContext("Caller", $"{Path.GetFileName(path)}:{member}:{line}")
               .Warning(mensagem, args);
        }

        public static void LogarDebug(
            string mensagem,
            [CallerMemberName] string member = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0,
            params object?[] args)
        {
            _lazyLogger.Value.ForContext("Caller", $"{Path.GetFileName(path)}:{member}:{line}")
               .Debug(mensagem, args);
        }

        public static void LogarTrace(
            string mensagem,
            [CallerMemberName] string member = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0,
            params object?[] args)
        {
            _lazyLogger.Value.ForContext("Caller", $"{Path.GetFileName(path)}:{member}:{line}")
               .Verbose(mensagem, args);
        }

        public static void LogarCritico(
            string mensagem,
            [CallerMemberName] string member = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0,
            params object?[] args)
        {
            _lazyLogger.Value.ForContext("Caller", $"{Path.GetFileName(path)}:{member}:{line}")
               .Fatal(mensagem, args);
        }

        public static void EncerrarLog() => Log.CloseAndFlush();

        // Bônus: pra logar objeto inteiro
        public static void LogarDebugObj<T>(
            T obj,
            string mensagem = "Objeto: {@Obj}",
            [CallerMemberName] string member = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0)
        {
            _lazyLogger.Value.ForContext("Caller", $"{Path.GetFileName(path)}:{member}:{line}")
               .Debug(mensagem, obj);
        }
    }