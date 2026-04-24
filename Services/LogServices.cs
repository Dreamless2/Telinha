using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Telinha.Services
{
    public static class LogService
    {
        public static void ConfigurarLog()
        {
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "log-.txt");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(logPath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            Log.Information("Sistema de logs inicializado com sucesso.");
        }

        // O segredo está no 'params object[] args'
        // O Serilog aceita o template da mensagem e os objetos que preenchem esse template.

        public static void LogarInformacao(string mensagem, params object[] args)
        {
            Log.Information(mensagem, args);
        }

        public static void LogarErro(string mensagem, params object[] args)
        {
            Log.Error(mensagem, args);
        }

        public static void LogarErroComException(Exception ex, string mensagem, params object[] args)
        {
            Log.Error(ex, mensagem, args);
        }

        public static void LogarAlerta(string mensagem, params object[] args)
        {
            Log.Warning(mensagem, args);
        }

        public static void LogarDebug(string mensagem, params object[] args)
        {
            Log.Debug(mensagem, args);
        }

        public static void LogarTrace(string mensagem, params object[] args)
        {
            Log.Verbose(mensagem, args);
        }

        public static void LogarCritico(string mensagem, params object[] args)
        {
            Log.Fatal(mensagem, args);
        }

        public static void EncerrarLog()
        {
            Log.CloseAndFlush();
        }
    }
}