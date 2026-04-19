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
        private readonly BindingSource _bs = [];
        private bool _buscando;


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
            SalvarButton.Click += SalvarButton_ClickAsync;
            ConectarEventos();
        }
        private void SetupBindings()
        {
            _bs.DataSource = new MidiaModel();
            CodigoBox.DataBindings.Add("Text", _bs, "Codigo", false, DataSourceUpdateMode.OnPropertyChanged);
            NomeBox.DataBindings.Add("Text", _bs, "Nome", false, DataSourceUpdateMode.OnPropertyChanged);
            AudioBox.DataBindings.Add("Text", _bs, "Audio", false, DataSourceUpdateMode.OnPropertyChanged);
            SinopseBox.DataBindings.Add("Text", _bs, "Sinopse", false, DataSourceUpdateMode.OnPropertyChanged);
            OriginalBox.DataBindings.Add("Text", _bs, "Original", false, DataSourceUpdateMode.OnPropertyChanged);
            LancamentoBox.DataBindings.Add("Text", _bs, "Lancamento", false, DataSourceUpdateMode.OnPropertyChanged);
            AlternativoBox.DataBindings.Add("Text", _bs, "Alternativo", false, DataSourceUpdateMode.OnPropertyChanged);
            TagsBox.DataBindings.Add("Text", _bs, "Tags", false, DataSourceUpdateMode.OnPropertyChanged);
            TipoBox.DataBindings.Add("Text", _bs, "NomeFormatado", false, DataSourceUpdateMode.OnPropertyChanged);
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

            _bs.DataSource = item ?? new MidiaModel();

            if (Enum.TryParse(item?.Tipo, true, out MidiaTipo tipoReal))
            {
                TipoLabel.Text = TipoToDisplay(tipoReal);
                AtualizarUI(tipoReal, item);
            }
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
        private void AtualizarUI(MidiaTipo tipo, MidiaModel item)
        {
            bool isFilme = tipo == MidiaTipo.Filme;
            bool isAnime = tipo == MidiaTipo.Anime;

            // 1. TRATAMENTO DOS DADOS
            // Se for filme, força "--". Se não for, só mexe se estiver nulo (coalescência).
            item.Pais = isFilme ? "--" : (item.Pais ?? "");
            item.Idioma = isFilme ? "--" : (item.Idioma ?? "");
            item.Obra = isFilme ? "--" : (item.Obra ?? "");
            item.Autores = isFilme ? "--" : (item.Autores ?? "");
            item.Criadores = isFilme ? "--" : (item.Criadores ?? "");

            // 2. ESTADO DOS CONTROLES (ENABLE/DISABLE)
            bool habilitarCamposGerais = !isFilme;

            PaisLabel.Enabled = PaisBox.Enabled = habilitarCamposGerais;
            IdiomaLabel.Enabled = IdiomaBox.Enabled = habilitarCamposGerais;
            ObraLabel.Enabled = ObraBox.Enabled = habilitarCamposGerais;
            AutoresLabel.Enabled = AutoresBox.Enabled = habilitarCamposGerais;
            CriadoresLabel.Enabled = CriadoresBox.Enabled = habilitarCamposGerais;

            // MCU só habilita se não for Filme nem Anime (ou seja, apenas Série)
            MCUBox.Enabled = !isFilme && !isAnime;

            // 3. TIPO VISUAL
            TipoLabel.Text = isFilme ? "Filme" : isAnime ? "Anime" : "Série";
            TipoBox.PlaceholderText = TipoLabel.Text;

            // 4. EXIBIÇÃO NA UI (VALORES FINAIS)
            PaisBox.Text = item.Pais;
            IdiomaBox.Text = item.Idioma;
            ObraBox.Text = item.Obra;
            AutoresBox.Text = item.Autores;
            CriadoresBox.Text = item.Criadores;

            if (isAnime)
            {
                MCUBox.Enabled = false;
            }
            else
            {
                MCUBox.Enabled = true;
            }
        }
        private void PreencherCampos(MidiaModel midia)
        {
            if (midia == null)
            {
                LimparCampos();
                return;
            }

            currentId = midia.Id;

            // Mapeamento de propriedade → Control
            var mapeamento = new Dictionary<string, TextBox>
            {
                [nameof(midia.Codigo)] = CodigoBox,
                [nameof(midia.Nome)] = NomeBox,
                [nameof(midia.Sinopse)] = SinopseBox,
                [nameof(midia.Original)] = OriginalBox,
                [nameof(midia.Lancamento)] = LancamentoBox,
                [nameof(midia.Alternativo)] = AlternativoBox,
                [nameof(midia.Tags)] = TagsBox,
                [nameof(midia.Tipo)] = TipoBox,
                [nameof(midia.MCU)] = MCUBox,
                [nameof(midia.Pais)] = PaisBox,
                [nameof(midia.Idioma)] = IdiomaBox,
                [nameof(midia.Autores)] = AutoresBox,
                [nameof(midia.Franquia)] = FranquiaBox,
                [nameof(midia.Criadores)] = CriadoresBox,
                [nameof(midia.Genero)] = GeneroBox,
                [nameof(midia.Diretor)] = DiretorBox,
                [nameof(midia.Artistas)] = ArtistasBox,
                [nameof(midia.Produtora)] = ProdutoraBox,
            };

            foreach (var kvp in mapeamento)
            {
                var valor = midia.GetType().GetProperty(kvp.Key)?.GetValue(midia) as string;
                kvp.Value.Text = valor ?? string.Empty;
            }

            // Áudio continua com lógica especial
            string audioValue = string.IsNullOrWhiteSpace(midia.Audio) ? "Dublado" : midia.Audio;

            if (!AudioBox.Items.Contains(audioValue))
                AudioBox.Items.Add(audioValue);

            AudioBox.SelectedItem = audioValue;
        }
        private void LimparCampos()
        {
            currentId = 0;

            // Dicionário de campos (fácil de adicionar/remover no futuro)
            var camposTexto = new Dictionary<string, TextBox>
            {
                { nameof(CodigoBox),      CodigoBox },
                { nameof(NomeBox),        NomeBox },
                { nameof(SinopseBox),     SinopseBox },
                { nameof(OriginalBox),    OriginalBox },
                { nameof(LancamentoBox),  LancamentoBox },
                { nameof(AlternativoBox), AlternativoBox },
                { nameof(TagsBox),        TagsBox },
                { nameof(TipoBox),        TipoBox },
                { nameof(MCUBox),         MCUBox },
                { nameof(PaisBox),        PaisBox },
                { nameof(IdiomaBox),      IdiomaBox },
                { nameof(AutoresBox),     AutoresBox },
                { nameof(FranquiaBox),    FranquiaBox },
                { nameof(CriadoresBox),   CriadoresBox },
                { nameof(GeneroBox),      GeneroBox },
                { nameof(DiretorBox),     DiretorBox },
                { nameof(ArtistasBox),    ArtistasBox },
                { nameof(ProdutoraBox),   ProdutoraBox }
            };

            foreach (var campo in camposTexto.Values)
            {
                campo.Text = string.Empty;
            }

            // Áudio
            AudioBox.SelectedIndex = -1;
        }
        private async void Principal_Load(object sender, EventArgs e)
        {
            SetupBindings();

            try
            {
                if (await MidiaController.AnyAsync<MidiaModel>())
                {
                    await Carregar();
                }
                else
                {
                    currentId = 0;
                    MessageBox.Show("Nenhuma mídia encontrada. Insira um código válido e pressione Enter para buscar.",
                                    "Bem-vindo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados: {ex.Message}",
                                "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private async void SalvarButton_ClickAsync(object? sender, EventArgs e)
        {
            try
            {
                var item = (MidiaModel)_bs.Current!;

                var (inserted, updated) = await MidiaController.SaveAsync(item);

                MessageBox.Show(inserted ? $"{item.Nome} inserido com sucesso!" : $"{item.Nome} atualizado com sucesso!");

                _bs.EndEdit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void SairButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private async void BuscarMidia(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            e.SuppressKeyPress = true;

            string codigoDigitado = CodigoBox.Text.Trim();

            if (!int.TryParse(codigoDigitado, out int id) || id <= 0)
            {
                MessageBox.Show("Digite um código numérico válido.", "Código Inválido",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_buscando)
                return;

            _buscando = true;

            try
            {
                MidiaTipo tipoSolicitado = TipoLabel.Text.Contains("Filme", StringComparison.OrdinalIgnoreCase)
                    ? MidiaTipo.Filme
                    : MidiaTipo.Serie;

                var midia = await _midiaService.GetMidia(id, tipoSolicitado);

                if (midia == null)
                {
                    MessageBox.Show($"Nenhuma mídia encontrada com o código {id}.",
                                    "Não Encontrada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // ✅ IMPORTANTE: Atualiza o Label primeiro
                TipoLabel.Text = GenericHelpers.GetDescription(tipoSolicitado);

                // Atualiza a UI de acordo com o tipo real
                if (Enum.TryParse(midia.Tipo, true, out MidiaTipo tipoReal))
                {
                    AtualizarUI(tipoReal, midia);
                }

                // Preenche os campos, mas mantendo o código que o usuário digitou
                PreencherCampos(midia);

                // Força o código digitado a permanecer no TextBox (solução principal)
                CodigoBox.Text = codigoDigitado;

                // Coloca o cursor no final do texto (melhor UX)
                CodigoBox.SelectionStart = CodigoBox.Text.Length;
                CodigoBox.SelectionLength = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao buscar a mídia:\n{ex.Message}",
                                "Erro na Busca", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _buscando = false;
            }
        }
    }
}