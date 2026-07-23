using Autofac;
using Telinha.Views;

namespace Telinha.Forms.Views
{

    public partial class Principal : Form
    {
        private readonly ILifetimeScope _scope;
        public Principal(ILifetimeScope scope)
        {
            InitializeComponent();
            _scope = scope;
            HomeButton.Click += HomeButton_Click;
            SobreButton.Click += SobreButton_Click;
            SairButton.Click += SairButton_Click;
        }

        private void SairButton_Click(object? sender, EventArgs e)
        {
            Application.Exit();
        }

        private void SobreButton_Click(object? sender, EventArgs e)
        {
            var sobre = new Sobre();
            sobre.ShowDialog();
        }

        private void HomeButton_Click(object? sender, EventArgs e)
        {
            var home = _scope.Resolve<Home>();
            home.ShowDialog();
        }
    }
}
