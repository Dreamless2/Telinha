using System.Reflection.Emit;
using Telinha.Card;
using Telinha.Controller;
using Telinha.Enums;
using Telinha.Helpers;
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
        private BindingSource _bs = new();
        public Home()
        {
            InitializeComponent();
            Load += Principal_Load!;
            _tmdb = new TMDBServices("eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiJhZTBkNmMxNWJmY2Q4MWIzYzE0MDAyM2RhOGRhNjRjOSIsIm5iZiI6MTc1NjYwODYzMC41NTAwMDAyLCJzdWIiOiI2OGIzYjg3NjcwMzc1YzcyZDYzOTdhMzciLCJzY29wZXMiOlsiYXBpX3JlYWQiXSwidmVyc2lvbiI6MX0.md8gEfeVlGepwG9GuT5I6tcBFZYy7F_A4TewbcEZDjU");
            _midiaService = new MidiaServices(_tmdb);
            SairButton.Click += SairButton_Click!;
            CopiarButton.Click += CopiarButton_Click!;
            CodigoBox.KeyPress += (s, e) => Functions.OnlyNumbers(s!, e);
            CodigoBox.KeyDown += BuscarMidia!;
            SalvarButton.Click += SalvarButton_Click;
            ConectarEventos();
        }

        private void SetupBindings()
        {
            _bs.DataSource = typeof(MidiaModel);
            CodigoBox.DataBindings.Add("Text", _bs, "Codigo", false, DataSourceUpdateMode.OnPropertyChanged);
            NomeBox.DataBindings.Add("Text", _bs, "Nome", false, DataSourceUpdateMode.OnPropertyChanged);
            AudioBox.DataBindings.Add("Text", _bs, "Audio", false, DataSourceUpdateMode.OnPropertyChanged);
            SinopseBox.DataBindings.Add("Text", _bs, "Sinopse", false, DataSourceUpdateMode.OnPropertyChanged);
            OriginalBox.DataBindings.Add("Text", _bs, "Original", false, DataSourceUpdateMode.OnPropertyChanged);
            LancamentoBox.DataBindings.Add("Text", _bs, "Lancamento", false, DataSourceUpdateMode.OnPropertyChanged);
            AlternativoBox.DataBindings.Add("Text", _bs, "Alternativo", false, DataSourceUpdateMode.OnPropertyChanged);
            TagsBox.DataBindings.Add("Text", _bs, "Tags", false, DataSourceUpdateMode.OnPropertyChanged);
            TipoBox.DataBindings.Add("Text", _bs, "Tipo", false, DataSourceUpdateMode.OnPropertyChanged);
            MCUBox.DataBindings.Add("Text", _bs, "MCU", false, DataSourceUpdateMode.OnPropertyChanged);
            PaisBox.DataBindings.Add("Text", _bs, "Pais", false, DataSourceUpdateMode.OnPropertyChanged);
            IdiomaBox.DataBindings.Add("Text", _bs, "Idioma", false, DataSourceUpdateMode.OnPropertyChanged);
            ObraBox.DataBindings.Add("Text", _bs, "Obra", false, DataSourceUpdateMode.OnPropertyChanged);
            AutoresBox.DataBindings.Add("Text", _bs, "Autores", false, DataSourceUpdateMode.OnPropertyChanged);
            FranquiaBox.DataBindings.Add("Text", _bs, "Franquia", false, DataSourceUpdateMode.OnPropertyChanged);
            CriadoresBox.DataBindings.Add("Text", _bs, "Criadores", false, DataSourceUpdateMode.OnPropertyChanged);
            GeneroBox.DataBindings.Add("Text", _bs, "Genero", false, DataSourceUpdateMode.OnPropertyChanged);
            DiretorBox.DataBindings.Add("Text", _bs, "Diretor", false, DataSourceUpdateMode.OnPropertyChanged);
            ArtistasBox.DataBindings.Add("Text", _bs, "Artistas", false, DataSourceUpdateMode.OnPropertyChanged);
            ProdutoraBox.DataBindings.Add("Text", _bs, "Produtora", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private async Task Carregar()
        {
            var item = await MidiaController.GetFirstAsync<MidiaModel>();

            if (item == null)
                item = new MidiaModel();

            _bs.DataSource = item;

            // UI dinâmica continua funcionando
            if (Enum.TryParse(item.Tipo, true, out MidiaTipo tipoReal))
            {
                label9.Text = TipoToDisplay(tipoReal);
                AtualizarUI(tipoReal);
            }
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
            if (TipoLabel.Text == "Filme")
            {
                PreencherMascara(MidiaTipo.Filme);
            }
            else if (TipoLabel.Text == "Série")
            {
                PreencherMascara(MidiaTipo.Serie);
            }
            else if (TipoLabel.Text == "Anime")
            {
                PreencherMascara(MidiaTipo.Anime);
            }
        }
        private void AtualizarUI(MidiaTipo tipo)
        {
            bool isFilme = tipo == MidiaTipo.Filme;
            bool isAnime = tipo == MidiaTipo.Anime;

            PaisLabel.Enabled = !isFilme;
            IdiomaLabel.Enabled = !isFilme;
            ObraLabel.Enabled = !isFilme;
            AutoresLabel.Enabled = !isFilme;
            CriadoresLabel.Enabled = !isFilme;
            PaisBox.Enabled = !isFilme;
            IdiomaBox.Enabled = !isFilme;
            ObraBox.Enabled = !isFilme;
            AutoresBox.Enabled = !isFilme;
            CriadoresBox.Enabled = !isFilme;
            MCUBox.Enabled = !isFilme && !isAnime;
            TipoBox.PlaceholderText = tipo.ToString();

            if (isAnime)
                MCUBox.PlaceholderText = "Fase MCU";
        }

        private void PreencherCampos(MidiaModel midia)
        {
            if (midia == null)
                return;

            currentId = midia.Id;

            if (!string.IsNullOrEmpty(midia.Codigo))
                CodigoBox.Text = midia.Codigo;

            NomeBox.Text = midia.Nome ?? string.Empty;
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
            TipoBox.Text = midia.Tipo ?? string.Empty;
            MCUBox.Text = midia.MCU ?? string.Empty;
            PaisBox.Text = midia.Pais ?? string.Empty;
            IdiomaBox.Text = midia.Idioma ?? string.Empty;
            AutoresBox.Text = midia.Autores ?? string.Empty;
            FranquiaBox.Text = midia.Franquia ?? string.Empty;
            CriadoresBox.Text = midia.Criadores ?? string.Empty;
            GeneroBox.Text = midia.Genero ?? string.Empty;
            DiretorBox.Text = midia.Diretor ?? string.Empty;
            ArtistasBox.Text = midia.Artistas ?? string.Empty;
            ProdutoraBox.Text = midia.Produtora ?? string.Empty;
        }

        private async Task PreencherCampos()
        {
            var item = await MidiaController.GetFirstAsync<MidiaModel>();

            if (item == null) return;

            currentId = item.Id;

            CodigoBox.Text = item.Codigo ?? string.Empty;
            NomeBox.Text = item.Nome ?? string.Empty;
            AudioBox.SelectedItem = item.Audio;
            SinopseBox.Text = item.Sinopse ?? string.Empty;
            OriginalBox.Text = item.Original ?? string.Empty;
            LancamentoBox.Text = item.Lancamento ?? string.Empty;
            AlternativoBox.Text = item.Alternativo ?? string.Empty;
            TagsBox.Text = item.Tags ?? string.Empty;
            TipoBox.Text = item.Tipo ?? string.Empty;
            MCUBox.Text = item.MCU ?? string.Empty;
            PaisBox.Text = item.Pais ?? string.Empty;
            IdiomaBox.Text = item.Idioma ?? string.Empty;
            AutoresBox.Text = item.Autores ?? string.Empty;
            FranquiaBox.Text = item.Franquia ?? string.Empty;
            CriadoresBox.Text = item.Criadores ?? string.Empty;
            GeneroBox.Text = item.Genero ?? string.Empty;
            DiretorBox.Text = item.Diretor ?? string.Empty;
            ArtistasBox.Text = item.Artistas ?? string.Empty;
            ProdutoraBox.Text = item.Produtora ?? string.Empty;
        }

        private void Principal_Load(object sender, EventArgs e)
        {
            //PreencherMascara(MidiaTipo.Filme);
            TipoLabel.Text = "Filme";
            TipoBox.PlaceholderText = "Filme";
            PaisLabel.Enabled = false;
            IdiomaLabel.Enabled = false;
            ObraLabel.Enabled = false;
            AutoresLabel.Enabled = false;
            CriadoresLabel.Enabled = false;
            PaisBox.Enabled = false;
            IdiomaBox.Enabled = false;
            ObraBox.Enabled = false;
            AutoresBox.Enabled = false;
            CriadoresBox.Enabled = false;

            try
            {
                if (MidiaController.Any<MidiaModel>())
                {
                    PreencherCampos().Wait();
                }
                else
                {
                    currentId = 0;
                    MessageBox.Show("Nenhuma mídia encontrada. Insira um código válido e pressione Enter para buscar.", "Bem-vindo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CopiarButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ResumoBox.Text))
            {
                Clipboard.SetText(ResumoBox.Text);
                MessageBox.Show("Copiado para a transferência!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Nada para ser copiado!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SalvarButton_Click(object? sender, EventArgs e)
        {
            try
            {
                var item = new MidiaModel
                {
                    Codigo = CodigoBox.Text.Trim(),
                    Nome = NomeBox.Text.Trim(),
                    Audio = AudioBox.SelectedItem?.ToString(),
                    Sinopse = SinopseBox.Text.Trim(),
                    Original = OriginalBox.Text.Trim(),
                    Lancamento = LancamentoBox.Text.Trim(),
                    Alternativo = AlternativoBox.Text.Trim(),
                    Tags = TagsBox.Text.Trim(),
                    Tipo = TipoBox.Text.Trim(),
                    MCU = MCUBox.Text.Trim(),
                    Pais = PaisBox.Text.Trim(),
                    Idioma = IdiomaBox.Text.Trim(),
                    Autores = AutoresBox.Text.Trim(),
                    Franquia = FranquiaBox.Text.Trim(),
                    Criadores = CriadoresBox.Text.Trim(),
                    Genero = GeneroBox.Text.Trim(),
                    Diretor = DiretorBox.Text.Trim(),
                    Artistas = ArtistasBox.Text.Trim(),
                    Produtora = ProdutoraBox.Text.Trim(),
                };

                if (string.IsNullOrEmpty(item.Codigo))
                {
                    MessageBox.Show("Informe um código.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Cursor = Cursors.WaitCursor;

                var (inserted, updated) = MidiaController.SaveAsync(item).Result;

                if (inserted)
                {
                    MessageBox.Show($"{item.Tipo} {item.Nome} inserido com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (updated)
                {
                    MessageBox.Show($"{item.Tipo} {item.Nome} atualizado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Nenhuma alteração detectada.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar:\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void SairButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }



        private async void BuscarMidia(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;

            if (!int.TryParse(CodigoBox.Text.Trim(), out int id) || id <= 0)
            {
                MessageBox.Show("Digite um código numérico válido.", "Inválido");
                return;
            }

            MidiaTipo tipoSolicitado = TipoLabel.Text.Contains("Filme", StringComparison.OrdinalIgnoreCase)
                ? MidiaTipo.Filme : MidiaTipo.Serie;

            try
            {
                var midia = await _midiaService.GetMidia(id, tipoSolicitado);

                TipoLabel.Text = GenericHelpers.GetDescription(tipoSolicitado);

                if (Enum.TryParse(midia!.Tipo, out MidiaTipo tipoReal))
                {
                    AtualizarUI(tipoReal);
                }

                PreencherCampos(midia);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao buscar:\n{ex.Message}", "Erro");
            }
        }
    }
}
