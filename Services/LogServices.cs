using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Telinha.Services
{
    public static class LogServices
    {
        private static bool _configurado = false;

        public static void ConfigurarLog()
        {
            if (_configurado) return;

            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "log-.txt");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(logPath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    fileSizeLimitBytes: 10_000_000,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            _configurado = true;
            Log.Information("Sistema de logs inicializado com sucesso.");
        }

        public static void LogarInformacao(string mensagem, params object?[] args)
        {
            Log.Information(mensagem, args);
        }

        public static void LogarErro(string mensagem, params object?[] args)
        {
            Log.Error(mensagem, args);
        }

        public static void LogarErroComException(Exception ex, string mensagem, params object?[] args)
        {
            Log.Error(ex, mensagem, args);
        }

        public static void LogarAlerta(string mensagem, params object?[] args)
        {
            Log.Warning(mensagem, args);
        }

        public static void LogarDebug(string mensagem, params object?[] args)
        {
            Log.Debug(mensagem, args);
        }

        public static void LogarTrace(string mensagem, params object?[] args)
        {
            Log.Verbose(mensagem, args);
        }

        public static void LogarCritico(string mensagem, params object?[] args)
        {
            Log.Fatal(mensagem, args);
        }

        public static void EncerrarLog()
        {
            Log.CloseAndFlush();
        }
    }
}