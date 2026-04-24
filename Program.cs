using Telinha.Services;

namespace Telinha
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            try
            {
                var configService = new AppConfigServices();
                var config = configService.Load();

                // 🔴 Se não tem config → abre tela de configuração
                if (config == null ||
                    string.IsNullOrWhiteSpace(config.Host) ||
                    string.IsNullOrWhiteSpace(config.Porta) ||
                    string.IsNullOrWhiteSpace(config.Usuario) ||
                    string.IsNullOrWhiteSpace(config.Senha) ||
                    string.IsNullOrWhiteSpace(config.TMDB) ||
                    string.IsNullOrWhiteSpace(config.DEEPL))
                {
                    Application.Run(new Token());
                    return;
                }

                // ✅ Já tem tudo → vai direto pra Home
                //Application.Run(new Home());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro crítico ao iniciar:\n" + ex.Message);
            }
        }
    }
}