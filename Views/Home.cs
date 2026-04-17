using Telinha.Card;
using Telinha.Enums;
using Telinha.Models;
using Telinha.Services;
using Telinha.Utils;

namespace Telinha
{
    public partial class Home : Form
    {
        private readonly TMDBServices _tmdb;
        private readonly MidiaServices _midiaService;
        private long currentId = 0;
        public Home()
        {
            InitializeComponent();
            Load += Principal_Load!;
            _tmdb = new TMDBServices("eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiJhZTBkNmMxNWJmY2Q4MWIzYzE0MDAyM2RhOGRhNjRjOSIsIm5iZiI6MTc1NjYwODYzMC41NTAwMDAyLCJzdWIiOiI2OGIzYjg3NjcwMzc1YzcyZDYzOTdhMzciLCJzY29wZXMiOlsiYXBpX3JlYWQiXSwidmVyc2lvbiI6MX0.md8gEfeVlGepwG9GuT5I6tcBFZYy7F_A4TewbcEZDjU");
            _midiaService = new MidiaServices(_tmdb);
            SairButton.Click += SairButton_Click!;
            FilmesButton.Click += FilmesButton_Click!;
            SeriesButton.Click += SeriesButton_Click!;
            AnimesButton.Click += AnimesButton_Click!;
            CopiarButton.Click += CopiarButton_Click!;
            CodigoBox.KeyPress += (s, e) => Functions.OnlyNumbers(s!, e);
            CodigoBox.KeyDown += BuscarMidia!;
            ConectarEventos();
        }
        private void PreencherMascara(MidiaTipo tipo)
        {
            var card = new MidiaCard(
                tipo,
                titulo: NomeBox.Text,
                audio: AudioBox.Text,
                sinopse: SinopseBox.Text,
                original: OriginalBox.Text,
                lancamento: LancamentoBox.Text,
                alternativo: AlternativoBox.Text,
                pais: PaisBox.Text,
                idioma: IdiomaBox.Text,
                franquia: FranquiaBox.Text,
                genero: GeneroBox.Text,
                tags: TagsBox.Text,
                diretor: DiretorBox.Text,
                artistas: ArtistasBox.Text,
                produtora: ProdutoraBox.Text,
                mcu: MCUBox.Text,
                autores: AutoresBox.Text,
                criadores: CriadoresBox.Text,
                obra: ObraBox.Text
            );
            ResumoBox.Text = card.GetFormattedText();
        }
        private void CopiarButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ResumoBox.Text))
            {
                Clipboard.SetText(ResumoBox.Text);
                MessageBox.Show("Resumo copiado para a área de transferência!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Não há resumo para copiar!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void SairButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void ConectarEventos()
        {
            var controles = new Control[] {
                TipoBox, NomeBox, AudioBox, SinopseBox, OriginalBox, LancamentoBox,
                AlternativoBox, PaisBox, IdiomaBox, FranquiaBox, GeneroBox,
                TagsBox, DiretorBox, ArtistasBox, ProdutoraBox, MCUBox,
                AutoresBox, CriadoresBox, ObraBox
            };

            foreach (var ctrl in controles)
            {
                if (ctrl is TextBoxBase txt)
                {
                    txt.TextChanged += QualquerAlteracao!;
                }
                else if (ctrl is ComboBox combo)
                {
                    combo.SelectedIndexChanged += QualquerAlteracao!;
                }
            }
        }
        private void QualquerAlteracao(object sender, EventArgs e)
        {
            if (label9.Text == "Filme")
            {
                PreencherMascara(MidiaTipo.Filme);
            }
            else if (label9.Text == "Série")
            {
                PreencherMascara(MidiaTipo.Serie);
            }
            else if (label9.Text == "Anime")
            {
                PreencherMascara(MidiaTipo.Anime);
            }
        }
        private void Principal_Load(object sender, EventArgs e)
        {
            //Functions.SetCamposEnabled(this, false, typeof(TextBox), typeof(Label), typeof(ComboBox));
            PreencherMascara(MidiaTipo.Filme);
            label9.Text = "Filme";
            TipoBox.PlaceholderText = "Filme";
            label11.Enabled = false;
            label12.Enabled = false;
            label13.Enabled = false;
            label14.Enabled = false;
            label16.Enabled = false;
            PaisBox.Enabled = false;
            IdiomaBox.Enabled = false;
            ObraBox.Enabled = false;
            AutoresBox.Enabled = false;
            CriadoresBox.Enabled = false;
        }
        private void FilmesButton_Click(object sender, EventArgs e)
        {
            label9.Text = "Filme";
            TipoBox.PlaceholderText = "Filme";
            MCUBox.PlaceholderText = "Fase MCU";
            MCUBox.Text = string.Empty;
            PreencherMascara(MidiaTipo.Filme);
        }
        private void SeriesButton_Click(object sender, EventArgs e)
        {
            Functions.SetCamposEnabled(this, true, typeof(TextBox), typeof(Label), typeof(ComboBox));
            label9.Text = "Série";
            TipoBox.PlaceholderText = "Série";
            MCUBox.PlaceholderText = "Fase MCU";
            MCUBox.Text = string.Empty;
            PreencherMascara(MidiaTipo.Serie);
        }
        private void AnimesButton_Click(object sender, EventArgs e)
        {
            Functions.SetCamposEnabled(this, true, typeof(TextBox), typeof(Label), typeof(ComboBox));
            label9.Text = "Anime";
            TipoBox.PlaceholderText = "Anime";
            label10.Enabled = false;
            MCUBox.Enabled = false;
            MCUBox.PlaceholderText = "Fase MCU";
            MCUBox.Text = string.Empty; PreencherMascara(MidiaTipo.Anime);
        }

        private void PreencherCampos(MidiaModel midia)
        {
            if (midia == null)
                return;

            currentId = midia.Id;

            if (!string.IsNullOrEmpty(midia.Codigo))
                CodigoBox.Text = midia.Codigo;

            NomeBox.Text = midia.Titulo ?? string.Empty;
            var audioValue = string.IsNullOrWhiteSpace(midia.Audio) ? "Dublado" : midia.Audio;
            if (!AudioBox.Items.Contains(audioValue))
            {
                AudioBox.Items.Add(audioValue);
            }

            AudioBox.SelectedItem = audioValue;
            SinopseBox.Text = midia.Sinopse ?? string.Empty;
            OriginalBox.Text = midia.Original ?? string.Empty;
            LancamentoBox.Text = midia.Lancamento ?? string.Empty;
            AlternativoBox.Text = midia.Alternativo ?? string.Empty;
            TagsBox.Text = midia.Tags ?? string.Empty;
            PaisBox.Text = midia.Pais ?? string.Empty;
            IdiomaBox.Text = midia.Idioma ?? string.Empty;
            AutoresBox.Text = midia.Autores ?? string.Empty;
            FranquiaBox.Text = midia.Franquia ?? string.Empty;
            CriadoresBox.Text = midia.Criadores ?? string.Empty;
            GeneroBox.Text = midia.Genero ?? string.Empty;
            DiretorBox.Text = midia.Diretor ?? string.Empty;
            ArtistasBox.Text = midia.Artistas ?? string.Empty;
            ProdutoraBox.Text = midia.Estudio ?? string.Empty;
        }

        private async void BuscarMidia(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;

            if (!int.TryParse(CodigoBox.Text.Trim(), out int id)) return;

            // Tipo inicial baseado no Label
            MidiaTipo tipoBusca = label9.Text == "Filme" ? MidiaTipo.Filme : MidiaTipo.Serie;

            try
            {
                var midia = await _midiaService.GetMidia(id, tipoBusca);

                if (midia != null)
                {
                    // Se o Service achou algo, ele já traz o Tipo certo (Filme, Serie ou Anime)
                    // Atualizamos o Label para o usuário não ficar confuso
                    label9.Text = midia.Tipo;

                    PreencherCampos(midia);
                }
                else
                {
                    MessageBox.Show("ID não encontrado em nenhuma categoria.", "TMDB", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro crítico: {ex.Message}");
            }
        }
    }
}


