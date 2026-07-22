using System.Runtime.InteropServices;
using Telinha.Core.Card;
using Telinha.Core.Controller;
using Telinha.Core.Enums;
using Telinha.Core.Models;
using Telinha.Core.Services;
using Telinha.Forms.Extras;
using Telinha.Views;

namespace Telinha
{
    public partial class Home : Form
    {
        private readonly MidiaServices? _midiaService;
        private long currentId = 0;
        private MidiaModel _current = new();
        private bool _buscando;
        private readonly Dictionary<string, TextBox> _mapeamentoCampos;

        private static readonly HashSet<string> CamposOpcionaisPorTipo = new()
        {
            nameof(MidiaModel.Referencia),
            nameof(MidiaModel.Autores),
            nameof(MidiaModel.Alternativo),
            nameof(MidiaModel.Franquia),
            nameof(MidiaModel.Showrunners),
            nameof(MidiaModel.MCU),
        };

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        public Home(MidiaServices midiaService)
        {
            InitializeComponent();
            _midiaService = midiaService;

            Load += Principal_Load!;
            ConfigurarTodosOsTextBoxes(this);

            SairButton.Click += SairButton_Click!;
            CopiarButton.Click += CopiarButton_Click!;
            CodigoBox.KeyPress += (s, e) => Functions.OnlyNumbers(s!, e);
            CodigoBox.KeyDown += BuscarMidia!;
            SalvarButton.Click += SalvarButton_Click!;
            AnteriorButton.Click += async (s, e) => await AnteriorButton_ClickAsync();
            ProximoButton.Click += async (s, e) => await ProximoButton_ClickAsync();
            PanelTopBar.MouseDown += PanelTopBar_MouseDown;

            RadioFilmes.CheckedChanged += TypeRadio_CheckedChanged!;
            RadioSeries.CheckedChanged += TypeRadio_CheckedChanged!;
            RadioAnimes.CheckedChanged += TypeRadio_CheckedChanged!;

            ConectarEventos();

            _mapeamentoCampos = new Dictionary<string, TextBox>
            {
                [nameof(MidiaModel.Codigo)] = CodigoBox,
                [nameof(MidiaModel.Nome)] = NomeBox,
                [nameof(MidiaModel.TituloResolvido)] = TipoBox,
                [nameof(MidiaModel.Sinopse)] = SinopseBox,
                [nameof(MidiaModel.Original)] = OriginalBox,
                [nameof(MidiaModel.Estreia)] = EstreiaBox,
                [nameof(MidiaModel.Alternativo)] = AlternativoBox,
                [nameof(MidiaModel.Tags)] = TagsBox,
                [nameof(MidiaModel.MCU)] = MCUBox,
                [nameof(MidiaModel.Local)] = LocalBox,
                [nameof(MidiaModel.Idioma)] = IdiomaBox,
                [nameof(MidiaModel.Referencia)] = ReferenciaBox,
                [nameof(MidiaModel.Autores)] = AutoresBox,
                [nameof(MidiaModel.Franquia)] = FranquiaBox,
                [nameof(MidiaModel.Showrunners)] = ShowrunnersBox,
                [nameof(MidiaModel.Genero)] = GeneroBox,
                [nameof(MidiaModel.Diretor)] = DiretorBox,
                [nameof(MidiaModel.Artistas)] = ArtistasBox,
                [nameof(MidiaModel.Produtora)] = ProdutoraBox,
            };
        }

        private void PanelTopBar_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private MidiaTipo GetSelectedType()
        {
            if (RadioFilmes.Checked) return MidiaTipo.Filme;
            if (RadioAnimes.Checked) return MidiaTipo.Anime;
            return MidiaTipo.Serie;
        }
        private void SetSelectedType(MidiaTipo tipo)
        {
            RadioFilmes.CheckedChanged -= TypeRadio_CheckedChanged!;
            RadioSeries.CheckedChanged -= TypeRadio_CheckedChanged!;
            RadioAnimes.CheckedChanged -= TypeRadio_CheckedChanged!;

            RadioFilmes.Checked = tipo == MidiaTipo.Filme;
            RadioSeries.Checked = tipo == MidiaTipo.Serie;
            RadioAnimes.Checked = tipo == MidiaTipo.Anime;

            if (RadioFilmes.Checked == true) TipoLabel.Text = "Filme";
            if (RadioSeries.Checked == true) TipoLabel.Text = "Série";
            if (RadioAnimes.Checked == true) TipoLabel.Text = "Anime";

            RadioFilmes.CheckedChanged += TypeRadio_CheckedChanged!;
            RadioSeries.CheckedChanged += TypeRadio_CheckedChanged!;
            RadioAnimes.CheckedChanged += TypeRadio_CheckedChanged!;
        }
        private void TypeRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (sender is RadioButton rb && !rb.Checked)
            {
                ConfigurarTodosOsTextBoxes(this);
                return;
            }

            var tipo = GetSelectedType();

            AtualizarUI(tipo, _current);
            PreencherMascara(tipo);
        }

        private static bool TryResolverTipo(string? tipoBruto, out MidiaTipo tipo)
        {
            var normalizado = tipoBruto
                ?.Replace("Série", "Serie")
                .Replace("Animé", "Anime");

            return Enum.TryParse(normalizado, true, out tipo);
        }

        private static string TipoToDisplay(MidiaTipo tipo)
        {
            return tipo switch
            {
                MidiaTipo.Filme => "Filme",
                MidiaTipo.Serie => "Série",
                MidiaTipo.Anime => "Anime",
                _ => tipo.ToString()
            };
        }

        private void CarregarNaTela(MidiaModel? midia)
        {
            _current = midia ?? new MidiaModel();
            currentId = _current.Id;

            MidiaTipo tipo = TryResolverTipo(_current.Tipo, out var tipoResolvido)
                ? tipoResolvido
                : MidiaTipo.Filme;

            bool ehSerieOuAnime = tipo == MidiaTipo.Serie || tipo == MidiaTipo.Anime;
            if (ehSerieOuAnime)
            {
                AplicarPadraoParaCamposOpcionaisVazios(_current);
            }

            PreencherTodosCampos(_current);
            SetSelectedType(tipo);
            AtualizarUI(tipo, _current);
            PreencherMascara(tipo);
        }

        private static void AplicarPadraoParaCamposOpcionaisVazios(MidiaModel midia)
        {
            foreach (var nomeCampo in CamposOpcionaisPorTipo)
            {
                var prop = midia.GetType().GetProperty(nomeCampo);
                var valorAtual = prop?.GetValue(midia) as string;
                if (string.IsNullOrWhiteSpace(valorAtual))
                {
                    prop?.SetValue(midia, "--");
                }
            }
        }

        private void PreencherTodosCampos(MidiaModel midia)
        {
            LogServices.LogarInformacao("VIEW: Preenchendo campos. ID: {id}, Nome: {nome}, Tipo: {tipo}", midia.Id, midia.Nome, midia.Tipo);

            foreach (var kvp in _mapeamentoCampos)
            {
                var valor = midia.GetType().GetProperty(kvp.Key)?.GetValue(midia) as string;
                kvp.Value.Text = valor ?? string.Empty;
            }

            string audioValue = string.IsNullOrWhiteSpace(midia.Audio) ? "Dublado" : midia.Audio;
            if (!AudioBox.Items.Contains(audioValue))
                AudioBox.Items.Add(audioValue);
            AudioBox.SelectedItem = audioValue;
        }

        private void PreencherMascara(MidiaTipo midiaTipo)
        {
            var card = new MidiaCard(
                midiaTipo,
                titulo: NomeBox.Text,
                audio: AudioBox.Text,
                sinopse: SinopseBox.Text,
                original: OriginalBox.Text,
                lancamento: EstreiaBox.Text,
                alternativo: AlternativoBox.Text,
                midia: _current.MidiaResolvida,
                local: LocalBox.Text,
                idioma: IdiomaBox.Text,
                franquia: FranquiaBox.Text,
                genero: GeneroBox.Text,
                tags: TagsBox.Text,
                diretor: DiretorBox.Text,
                artistas: ArtistasBox.Text,
                produtora: ProdutoraBox.Text,
                mcu: MCUBox.Text,
                autores: AutoresBox.Text,
                showrunners: ShowrunnersBox.Text,
                referencia: ReferenciaBox.Text
            );
            ResumoBox.Text = card.GetFormattedText();
        }

        private void TextBox_Enter(object sender, EventArgs e)
        {
            if (sender is TextBox txt && txt.Text == "--")
            {
                txt.Text = string.Empty;
            }
        }

        private void TextBox_Leave(object sender, EventArgs e)
        {
            if (sender is TextBox txt && string.IsNullOrWhiteSpace(txt.Text))
                txt.Text = "--";
        }

        private void ConfigurarTodosOsTextBoxes(Control container)
        {
            foreach (Control c in container.Controls)
            {
                if (c is TextBox txt)
                {
                    txt.Enter += TextBox_Enter!;
                    txt.Leave += TextBox_Leave!;
                    if (string.IsNullOrWhiteSpace(txt.Text)) txt.Text = "--";
                }
                else if (c.HasChildren) ConfigurarTodosOsTextBoxes(c);
            }
        }
        private void ConectarEventos()
        {
            var controles = new Control[] {
                TipoBox, NomeBox, AudioBox, SinopseBox, OriginalBox, EstreiaBox,
                AlternativoBox, LocalBox, IdiomaBox, FranquiaBox, GeneroBox,
                TagsBox, DiretorBox, ArtistasBox, ProdutoraBox, MCUBox,
                AutoresBox, ShowrunnersBox, ReferenciaBox
            };
            foreach (var ctrl in controles)
            {
                if (ctrl is TextBoxBase txt)
                    txt.TextChanged += QualquerAlteracao!;
                else if (ctrl is ComboBox combo)
                    combo.SelectedIndexChanged += QualquerAlteracao!;
            }
        }

        private void QualquerAlteracao(object sender, EventArgs e)
        {
            PreencherMascara(GetSelectedType());
        }

        private void AtualizarUI(MidiaTipo tipo, MidiaModel item)
        {
            item ??= new MidiaModel();
            bool isFilme = tipo == MidiaTipo.Filme;
            bool isAnime = tipo == MidiaTipo.Anime;

            item.Local = isFilme ? "--" : (item.Local ?? "--");
            item.Idioma = isFilme ? "--" : (item.Idioma ?? "--");
            item.Referencia = isFilme ? "--" : (item.Referencia ?? "--");
            item.Autores = isFilme ? "--" : (item.Autores ?? "--");
            item.Showrunners = isFilme ? "--" : (item.Showrunners ?? "--");
            item.Franquia = isFilme ? "--" : (item.Franquia ?? "--");
            item.MCU = isFilme || isAnime ? "--" : (item.MCU ?? "--");

            bool habilitarCamposGerais = !isFilme;
            LocalLabel.Enabled = LocalBox.Enabled = habilitarCamposGerais;
            IdiomaLabel.Enabled = IdiomaBox.Enabled = habilitarCamposGerais;
            ReferenciaLabel.Enabled = ReferenciaBox.Enabled = habilitarCamposGerais;
            AutoresLabel.Enabled = AutoresBox.Enabled = habilitarCamposGerais;
            ShowrunnersLabel.Enabled = ShowrunnersBox.Enabled = habilitarCamposGerais;
            MCUBox.Enabled = isFilme && !isAnime;

            SetSelectedType(tipo);
            TipoBox.PlaceholderText = TipoToDisplay(tipo);

            LocalBox.Text = item.Local;
            IdiomaBox.Text = item.Idioma;
            ReferenciaBox.Text = item.Referencia;
            AutoresBox.Text = item.Autores;
            ShowrunnersBox.Text = item.Showrunners;
            FranquiaBox.Text = item.Franquia;
            MCUBox.Text = item.MCU;
        }

        private async void Principal_Load(object sender, EventArgs e)
        {
            CodigoBox.Focus();
            ConfigurarTodosOsTextBoxes(this);

            try
            {
                if (await MidiaController.AnyAsync<MidiaModel>())
                {
                    var primeiro = await MidiaController.GetFirstAsync<MidiaModel>();
                    CarregarNaTela(primeiro);
                }
                else
                {
                    CarregarNaTela(null);
                    MessageBox.Show("Insira um novo registro para começar.", "Bem-vindo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CodigoBox.Focus();
                }

                await AtualizarBotoesNavegacao();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task AtualizarBotoesNavegacao()
        {
            if (currentId <= 0)
            {
                AnteriorButton.Enabled = false;
                ProximoButton.Enabled = await MidiaController.GetNext<MidiaModel>(0) != null;
                return;
            }

            AnteriorButton.Enabled = await MidiaController.ExistsPrevious<MidiaModel>(currentId);
            ProximoButton.Enabled = await MidiaController.ExistsNext<MidiaModel>(currentId);
        }

        private async Task NavegarAsync(Func<long, Task<MidiaModel?>> buscar, string mensagemFim, Button botaoDesabilitar)
        {
            try
            {
                var item = await buscar(currentId);
                if (item == null)
                {
                    MessageBox.Show(mensagemFim, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    botaoDesabilitar.Enabled = false;
                    return;
                }

                CarregarNaTela(item);
                await AtualizarBotoesNavegacao();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao navegar: {ex.Message}");
            }
        }

        private Task AnteriorButton_ClickAsync() =>
            NavegarAsync(id => MidiaController.GetPrevious<MidiaModel>(id), "Você chegou ao primeiro registro.", AnteriorButton);

        private Task ProximoButton_ClickAsync() =>
            NavegarAsync(id => MidiaController.GetNext<MidiaModel>(id), "Você chegou ao último registro.", ProximoButton);

        private async void BuscarMidia(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            e.SuppressKeyPress = true;

            var codigoDigitado = CodigoBox.Text.Trim();

            if (!int.TryParse(codigoDigitado, out int id) || id <= 0)
            {
                MessageBox.Show("Informe o código do TMDB.", "Código Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_buscando)
                return;

            _buscando = true;

            try
            {
                if (_midiaService == null)
                {
                    MessageBox.Show("Serviço ainda não inicializado.");
                    return;
                }

                var midia = await _midiaService.GetMidia(id);

                if (midia == null)
                {
                    MessageBox.Show($"Nenhuma mídia encontrada com o ID {id}.", "Não Encontrada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (!TryResolverTipo(midia.Tipo, out _))
                {
                    MessageBox.Show($"Tipo de mídia não reconhecido: {midia.Tipo}", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                CarregarNaTela(midia);

                CodigoBox.Text = codigoDigitado;
                CodigoBox.SelectionStart = CodigoBox.Text.Length;
                CodigoBox.SelectionLength = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao buscar a mídia:\n{ex.Message}", "Erro na Busca", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogServices.LogarErroComException(ex, "VIEW: Erro ao buscar mídia ID {id}", id);
            }
            finally
            {
                _buscando = false;
            }
        }

        private void CopiarButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ResumoBox.Text))
                Clipboard.SetText(ResumoBox.Text);
        }

        private async void SalvarButton_Click(object? sender, EventArgs e)
        {
            try
            {
                _current.Tipo = TipoToDisplay(GetSelectedType());
                _current.TituloFinal = TipoBox.Text;
                _current.Nome = NomeBox.Text;
                _current.Sinopse = SinopseBox.Text;
                _current.Original = OriginalBox.Text;
                _current.Estreia = EstreiaBox.Text;
                _current.Alternativo = AlternativoBox.Text;
                _current.Tags = TagsBox.Text;
                _current.MCU = MCUBox.Text;
                _current.Local = LocalBox.Text;
                _current.Idioma = IdiomaBox.Text;
                _current.Referencia = ReferenciaBox.Text;
                _current.Autores = AutoresBox.Text;
                _current.Franquia = FranquiaBox.Text;
                _current.Showrunners = ShowrunnersBox.Text;
                _current.Genero = GeneroBox.Text;
                _current.Diretor = DiretorBox.Text;
                _current.Artistas = ArtistasBox.Text;
                _current.Produtora = ProdutoraBox.Text;
                _current.Audio = AudioBox.Text;

                var (inserted, updated) = await MidiaController.SaveAsync(_current);

                MessageBox.Show(inserted
                    ? $"{_current.Nome} inserido com sucesso!"
                    : $"{_current.Nome} atualizado com sucesso!");

                CarregarNaTela(_current);
                await AtualizarBotoesNavegacao();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SobreButton_Click(object sender, EventArgs e)
        {
            var sobreForm = new Sobre();
            sobreForm.Show();
        }

        private void SairButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}