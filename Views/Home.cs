using System.ComponentModel;
using Telinha.Card;
using Telinha.Controller;
using Telinha.Enums;
using Telinha.Helpers;
using Telinha.Models;
using Telinha.Services;
using Telinha.Utils;

namespace Telinha
{
    #region Form
    public partial class Home : Form
    {
        #region Variáveis
        private readonly MidiaServices? _midiaService;
        private readonly FileCacheServices _cacheService;

        private long currentId = 0;
        private readonly BindingSource _bs = [];
        private bool _buscando;
        private readonly Dictionary<string, TextBox> _mapeamentoCampos;

        #endregion

        #region Constructor
        public Home(FileCacheServices cacheServices, MidiaServices midiaService)
        {
            InitializeComponent();
            _cacheService = cacheServices;
            _midiaService = midiaService;
            Load += Principal_Load!;
            SairButton.Click += SairButton_Click!;
            CopiarButton.Click += CopiarButton_Click!;
            CodigoBox.KeyPress += (s, e) => Functions.OnlyNumbers(s!, e);
            CodigoBox.KeyDown += BuscarMidia!;
            SalvarButton.Click += SalvarButton_Click;
            AnteriorButton.Click += AnteriorButton_Click!;
            ProximoButton.Click += ProximoButton_Click!;
            ConectarEventos();
            _mapeamentoCampos = new Dictionary<string, TextBox>
            {
                [nameof(MidiaModel.Codigo)] = CodigoBox,
                [nameof(MidiaModel.Nome)] = NomeBox,
                [nameof(MidiaModel.Sinopse)] = SinopseBox,
                [nameof(MidiaModel.Original)] = OriginalBox,
                [nameof(MidiaModel.Estreia)] = EstreiaBox,
                [nameof(MidiaModel.Alternativo)] = AlternativoBox,
                [nameof(MidiaModel.Tags)] = TagsBox,
                [nameof(MidiaModel.Tipo)] = TipoBox,
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
        #endregion

        #region Setup Bindings
        private void SetupBindings()
        {
            _bs.DataSource = new MidiaModel();
            CodigoBox.DataBindings.Add("Text", _bs, "Codigo", false, DataSourceUpdateMode.OnPropertyChanged);
            NomeBox.DataBindings.Add("Text", _bs, "Nome", false, DataSourceUpdateMode.OnPropertyChanged);
            AudioBox.DataBindings.Add("Text", _bs, "Audio", false, DataSourceUpdateMode.OnPropertyChanged);
            SinopseBox.DataBindings.Add("Text", _bs, "Sinopse", false, DataSourceUpdateMode.OnPropertyChanged);
            OriginalBox.DataBindings.Add("Text", _bs, "Original", false, DataSourceUpdateMode.OnPropertyChanged);
            EstreiaBox.DataBindings.Add("Text", _bs, "Estreia", false, DataSourceUpdateMode.OnPropertyChanged);
            AlternativoBox.DataBindings.Add("Text", _bs, "Alternativo", false, DataSourceUpdateMode.OnPropertyChanged);
            TagsBox.DataBindings.Add("Text", _bs, "Tags", false, DataSourceUpdateMode.OnPropertyChanged);
            TipoBox.DataBindings.Add("Text", _bs, "NomeFormatado", false, DataSourceUpdateMode.OnPropertyChanged);
            MCUBox.DataBindings.Add("Text", _bs, "MCU", false, DataSourceUpdateMode.OnPropertyChanged);
            LocalBox.DataBindings.Add("Text", _bs, "Local", false, DataSourceUpdateMode.OnPropertyChanged);
            IdiomaBox.DataBindings.Add("Text", _bs, "Idioma", false, DataSourceUpdateMode.OnPropertyChanged);
            ReferenciaBox.DataBindings.Add("Text", _bs, "Referencia", false, DataSourceUpdateMode.OnPropertyChanged);
            AutoresBox.DataBindings.Add("Text", _bs, "Autores", false, DataSourceUpdateMode.OnPropertyChanged);
            FranquiaBox.DataBindings.Add("Text", _bs, "Franquia", false, DataSourceUpdateMode.OnPropertyChanged);
            ShowrunnersBox.DataBindings.Add("Text", _bs, "Showrunners", false, DataSourceUpdateMode.OnPropertyChanged);
            GeneroBox.DataBindings.Add("Text", _bs, "Genero", false, DataSourceUpdateMode.OnPropertyChanged);
            DiretorBox.DataBindings.Add("Text", _bs, "Diretor", false, DataSourceUpdateMode.OnPropertyChanged);
            ArtistasBox.DataBindings.Add("Text", _bs, "Artistas", false, DataSourceUpdateMode.OnPropertyChanged);
            ProdutoraBox.DataBindings.Add("Text", _bs, "Produtora", false, DataSourceUpdateMode.OnPropertyChanged);
        }
        #endregion

        #region Carregar Dados
        private async Task Carregar()
        {
            var item = await MidiaController.GetFirstAsync<MidiaModel>();

            LogServices.LogarInformacao("VIEW: Carregar inicial. Item null? {isNull}", item == null); // 🔥 LOG 8

            _bs.DataSource = item ?? new MidiaModel();

            if (item is MidiaModel midiaValida)
            {
                PreencherCampos(midiaValida);

                var tipoNormalizado = midiaValida.Tipo
                    ?.Replace("Série", "Serie")
                    .Replace("Animé", "Anime");

                if (Enum.TryParse(tipoNormalizado, true, out MidiaTipo tipoReal))
                {
                    TipoLabel.Text = TipoToDisplay(tipoReal);
                    AtualizarUI(tipoReal, midiaValida); // 🔥 sem warning
                }
            }
            else
            {
                TipoLabel.Text = "Tipo";
                LimparCampos();
            }
        }
        #endregion

        #region Tipo para Display
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
        #endregion

        #region Preencher Máscara
        private void PreencherMascara(MidiaTipo tipo)
        {
            var card = new MidiaCard(
                tipo,
                titulo: NomeBox.Text,
                audio: AudioBox.Text,
                sinopse: SinopseBox.Text,
                original: OriginalBox.Text,
                lancamento: EstreiaBox.Text,
                alternativo: AlternativoBox.Text,
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
        #endregion

        #region Conectar Eventos
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
                {
                    txt.TextChanged += QualquerAlteracao!;
                }
                else if (ctrl is ComboBox combo)
                {
                    combo.SelectedIndexChanged += QualquerAlteracao!;
                }
            }
        }
        #endregion

        #region Qualquer Alteração
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
        #endregion

        #region Atualizar UI
        private void AtualizarUI(MidiaTipo tipo, MidiaModel item)
        {
            item ??= new MidiaModel();

            bool isFilme = tipo == MidiaTipo.Filme;
            bool isAnime = tipo == MidiaTipo.Anime;

            item.Local = isFilme ? "--" : (item.Local ?? "");
            item.Idioma = isFilme ? "--" : (item.Idioma ?? "");
            item.Referencia = isFilme ? "--" : (item.Referencia ?? "");
            item.Autores = isFilme ? "--" : (item.Autores ?? "");
            item.Showrunners = isFilme ? "--" : (item.Showrunners ?? "");

            bool habilitarCamposGerais = !isFilme;

            LocalLabel.Enabled = LocalBox.Enabled = habilitarCamposGerais;
            IdiomaLabel.Enabled = IdiomaBox.Enabled = habilitarCamposGerais;
            ReferenciaLabel.Enabled = ReferenciaBox.Enabled = habilitarCamposGerais;
            AutoresLabel.Enabled = AutoresBox.Enabled = habilitarCamposGerais;
            ShowrunnersLabel.Enabled = ShowrunnersBox.Enabled = habilitarCamposGerais;
            MCUBox.Enabled = !isFilme && !isAnime;

            TipoLabel.Text = isFilme ? "Filme" : isAnime ? "Anime" : "Série";
            TipoBox.PlaceholderText = TipoLabel.Text;

            LocalBox.Text = item.Local;
            IdiomaBox.Text = item.Idioma;
            ReferenciaBox.Text = item.Referencia;
            AutoresBox.Text = item.Autores;
            ShowrunnersBox.Text = item.Showrunners;

            if (isAnime)
            {
                MCUBox.Enabled = false;
            }
            else
            {
                MCUBox.Enabled = true;
            }
        }
        #endregion

        #region Preencher Campos
        private void PreencherCampos(MidiaModel midia)
        {
            LogServices.LogarInformacao("VIEW: Preenchendo campos. ID: {id}, Nome: {nome}, Tipo: {tipo}", midia.Id, midia.Nome, midia.Tipo);

            if (midia == null)
            {
                LimparCampos();
                return;
            }

            currentId = midia.Id;

            // 🔥 Normaliza o tipo primeiro pra comparar
            var tipoNormalizado = midia.Tipo?
                .Replace("Série", "Serie")
                .Replace("Animé", "Anime");

            var ehSerie = tipoNormalizado?.Equals("Serie", StringComparison.OrdinalIgnoreCase) == true
                       || tipoNormalizado?.Equals("Anime", StringComparison.OrdinalIgnoreCase) == true;

            // 🔥 Campos vazios viram "--"
            var camposOpcionaisPorTipo = new HashSet<string>
            {
                nameof(midia.Referencia),
                nameof(midia.Autores),
                nameof(midia.Diretor),
                nameof(midia.Showrunners),
                nameof(midia.Alternativo),
                nameof(midia.Franquia),
                nameof(midia.MCU),
            };

            foreach (var kvp in _mapeamentoCampos)
            {
                var valor = midia.GetType().GetProperty(kvp.Key)?.GetValue(midia) as string;

                if (string.IsNullOrWhiteSpace(valor))
                {
                    if (camposOpcionaisPorTipo.Contains(kvp.Key) && ehSerie)
                    {
                        kvp.Value.Text = "--";
                    }
                }
                else
                {
                    kvp.Value.Text = valor;
                }
            }

            string audioValue = string.IsNullOrWhiteSpace(midia.Audio) ? "Dublado" : midia.Audio;
            if (!AudioBox.Items.Contains(audioValue))
                AudioBox.Items.Add(audioValue);

            AudioBox.SelectedItem = audioValue;

            if (Enum.TryParse(tipoNormalizado, true, out MidiaTipo tipoReal))
            {
                PreencherMascara(tipoReal);
                TipoLabel.Text = TipoToDisplay(tipoReal);
            }
            else
            {
                TipoLabel.Text = "Tipo";
                ResumoBox.Clear();
            }
        }
        #endregion

        #region Limpar Campos
        private void LimparCampos()
        {
            // 🔥 Uma linha pra limpar todos
            foreach (var tb in _mapeamentoCampos.Values)
                tb.Clear();

            AudioBox.SelectedIndex = -1;
            TipoLabel.Text = "Tipo";
            currentId = 0;
        }
        #endregion

        #region Form Load
        private async void Principal_Load(object sender, EventArgs e)
        {
            var lista = new BindingList<MidiaModel>();
            _bs.DataSource = lista;

            SetupBindings();

            _cacheService.LimparExpirados();

            try
            {
                if (await MidiaController.AnyAsync<MidiaModel>())
                {
                    await Carregar();
                }
                else
                {
                    currentId = 0;
                    MessageBox.Show("Insira um novo registro para começar.", "Bem-vindo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Navegação
        private async Task AtualizarBotoesNavegacao()
        {
            if (_bs.Current is MidiaModel item && item.Id == 0)
            {
                AnteriorButton.Enabled = await MidiaController.GetPrevious<MidiaModel>(0) != null;
                ProximoButton.Enabled = false;
                return;
            }

            if (currentId <= 0)
            {
                AnteriorButton.Enabled = false;
                ProximoButton.Enabled = await MidiaController.GetNext<MidiaModel>(0) != null;
                return;
            }

            AnteriorButton.Enabled = await MidiaController.ExistsPrevious<MidiaModel>(currentId);
            ProximoButton.Enabled = await MidiaController.ExistsNext<MidiaModel>(currentId);
        }
        #endregion

        #region Buscar por Código
        private async void BuscarMidia(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            e.SuppressKeyPress = true;

            var codigoDigitado = CodigoBox.Text.Trim();

            LogServices.LogarInformacao("VIEW: Enter pressionado. Código: {codigo}, TipoLabel: {tipo}", codigoDigitado, TipoLabel.Text); // 🔥 LOG 1

            if (!int.TryParse(codigoDigitado, out int id) || id <= 0)
            {
                MessageBox.Show("Informe o código do TMDB.", "Código Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_buscando)
            {
                LogServices.LogarInformacao("VIEW: Busca ignorada - já está buscando"); // 🔥 LOG 2
                return;
            }

            _buscando = true;

            try
            {
                MidiaTipo tipoSolicitado = TipoLabel.Text.Contains("Filme", StringComparison.OrdinalIgnoreCase)
                    ? MidiaTipo.Filme
                    : MidiaTipo.Serie;


                if (_midiaService == null)
                {
                    MessageBox.Show("Serviço ainda não inicializado.");
                    return;
                }

                var midia = await _midiaService.GetMidia(id);

                LogServices.LogarInformacao("VIEW: Busca ignorada - já está buscando"); // 🔥 LOG 2

                if (midia == null)
                {
                    MessageBox.Show($"Nenhuma mídia encontrada com o ID {id}.", "Não Encontrada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                TipoLabel.Text = GenericHelpers.GetDescription(tipoSolicitado);

                if (Enum.TryParse(midia.Tipo, true, out MidiaTipo tipoRetornado))
                {
                    LogServices.LogarInformacao("VIEW: Tipo parseado: {tipoRetornado}", tipoRetornado); // 🔥 LOG 4
                    TipoLabel.Text = TipoToDisplay(tipoRetornado);
                    AtualizarUI(tipoRetornado, midia);
                }

                PreencherCampos(midia);

                CodigoBox.Text = codigoDigitado;
                CodigoBox.SelectionStart = CodigoBox.Text.Length;
                CodigoBox.SelectionLength = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao buscar a mídia:\n{ex.Message}", "Erro na Busca", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogServices.LogarErroComException(ex, "VIEW: Erro ao buscar mídia ID {id}", id); // 🔥 LOG 6
            }
            finally
            {
                _buscando = false;
            }
        }
        #endregion

        #region Copiar Button
        private void CopiarButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ResumoBox.Text))
            {
                Clipboard.SetText(ResumoBox.Text);
                MessageBox.Show("Copiado para a área de transferência!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Nada para ser copiado!", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion

        #region Salvar Button
        private async void SalvarButton_Click(object? sender, EventArgs e)
        {
            try
            {
                _bs.EndEdit();

                var tipo = (MidiaModel)_bs.Current!;

                tipo.Tipo = TipoLabel.Text;

                if (_bs.Current is not MidiaModel item)
                {
                    MessageBox.Show("Nenhum registro para salvar.", "Aviso");
                    return;
                }

                var (inserted, updated) = await MidiaController.SaveAsync(item);

                MessageBox.Show(inserted
                    ? $"{item.Nome} inserido com sucesso!"
                    : $"{item.Nome} atualizado com sucesso!");

                if (inserted && item.Id != 0)
                {
                    currentId = item.Id;

                    if (_bs.DataSource is BindingList<MidiaModel> lista && !lista.Contains(item))
                        lista.Add(item);

                    _bs.Position = _bs.IndexOf(item);
                }
                else
                {
                    _bs.ResetCurrentItem();
                }

                await AtualizarBotoesNavegacao();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Anterior/Próximo Buttons
        private async void AnteriorButton_Click(object sender, EventArgs e)
        {
            try
            {
                var item = await MidiaController.GetPrevious<MidiaModel>(currentId);

                if (item == null)
                {
                    MessageBox.Show("Você chegou ao primeiro registro.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    AnteriorButton.Enabled = false;
                    return;
                }

                currentId = item.Id;
                PreencherCampos(item);
                _bs.Position = _bs.IndexOf(item);
                await AtualizarBotoesNavegacao();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao navegar: {ex.Message}");
            }
        }
        private async void ProximoButton_Click(object sender, EventArgs e)
        {
            try
            {
                var item = await MidiaController.GetNext<MidiaModel>(currentId);

                if (item == null)
                {
                    MessageBox.Show("Você chegou ao último registro.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ProximoButton.Enabled = false;
                    return;
                }

                currentId = item.Id;
                PreencherCampos(item);
                _bs.Position = _bs.IndexOf(item);
                await AtualizarBotoesNavegacao();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao navegar: {ex.Message}");
            }
        }
        #endregion

        #region Form Closing
        private void SairButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        #endregion
    }
    #endregion
}
