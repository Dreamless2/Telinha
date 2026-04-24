using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Telinha.Services
{
    public class LogServices
    {
        public static void ConfigurarLog()
        {
            // Define o caminho onde os logs serão salvos (Pasta 'Logs' na raiz da app)
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "log-.txt");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug() // Define o nível mínimo de log
                .WriteTo.File(logPath,
                    rollingInterval: RollingInterval.Day, // Cria um arquivo novo por dia
                    retainedFileCountLimit: 7,            // Mantém apenas os últimos 7 dias
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            Log.Information("Sistema de logs inicializado com sucesso.");
        }

        public static void EncerrarLog()
        {
            Log.CloseAndFlush(); // Garante que todos os logs no buffer sejam gravados antes de fechar
        }
    }
}