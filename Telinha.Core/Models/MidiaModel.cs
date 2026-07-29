using FreeSql.DataAnnotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Telinha.Core.Utils;

namespace Telinha.Core.Models
{
    [Table(Name = "midia")]
    public class MidiaModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string? _codigo;
        private string? _nome;
        private string? _audio;
        private string? _tipo;
        private string? _classificacao;
        private string? _sinopse;
        private string? _original;
        private string? _lancamento;
        private string? _alternativo;
        private string? _midia;
        private string? _local;
        private string? _idioma;
        private string? _idiomaOriginal;
        private string? _franquia;
        private string? _serie;
        private string? _autores;
        private string? _showrunners;
        private string? _referencia;
        private string? _genero;
        private string? _tags;
        private string? _diretor;
        private string? _mcu;
        private string? _artistas;
        private string? _produtora;
        private string? _tipoSolicitado;
        private string? _tituloFinal;

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            return true;
        }

        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        public string? Codigo
        {
            get => _codigo;
            set => SetField(ref _codigo, value);
        }
        public string? Nome
        {
            get => _nome;
            set => SetField(ref _nome, value);
        }
        public string? Audio
        {
            get => _audio;
            set => SetField(ref _audio, value);
        }
        public string? Tipo
        {
            get => _tipo;
            set => SetField(ref _tipo, value);
        }

        [Column(IsIgnore = true)]
        public string? Classificacao
        {
            get => _classificacao;
            set => SetField(ref _classificacao, value);
        }
        public string? Sinopse
        {
            get => _sinopse;
            set => SetField(ref _sinopse, value);
        }
        public string? Original
        {
            get => _original;
            set => SetField(ref _original, value);
        }
        public string? Estreia
        {
            get => _lancamento;
            set => SetField(ref _lancamento, value);
        }
        public string? Alternativo
        {
            get => _alternativo;
            set => SetField(ref _alternativo, value);
        }

        [Column(IsIgnore = true)]
        public string? Midia
        {
            get => _midia;
            set => SetField(ref _midia, value);
        }
        public string? Local
        {
            get => _local;
            set => SetField(ref _local, value);
        }
        public string? Idioma
        {
            get => _idioma;
            set => SetField(ref _idioma, value);
        }

        [Column(IsIgnore = true)]
        public string? IdiomaOriginal
        {
            get => _idiomaOriginal;
            set => SetField(ref _idiomaOriginal, value);
        }

        [Column(IsIgnore = true)]
        public string? Serie
        {
            get => _serie;
            set => SetField(ref _serie, value);
        }
        public string? Franquia
        {
            get => _franquia;
            set => SetField(ref _franquia, value);
        }
        public string? Autores
        {
            get => _autores;
            set => SetField(ref _autores, value);
        }
        public string? Showrunners
        {
            get => _showrunners;
            set => SetField(ref _showrunners, value);
        }
        public string? Referencia
        {
            get => _referencia;
            set => SetField(ref _referencia, value);
        }
        public string? Genero
        {
            get => _genero;
            set => SetField(ref _genero, value);
        }
        public string? Tags
        {
            get => _tags;
            set => SetField(ref _tags, value);
        }
        public string? Diretor
        {
            get => _diretor;
            set => SetField(ref _diretor, value);
        }
        public string? MCU
        {
            get => _mcu;
            set => SetField(ref _mcu, value);
        }


        public string? Artistas
        {
            get => _artistas;
            set => SetField(ref _artistas, value);
        }


        public string? Produtora
        {
            get => _produtora;
            set => SetField(ref _produtora, value);
        }

        [Column(IsIgnore = true)]
        public string? TipoSolicitado
        {
            get => _tipoSolicitado;
            set => SetField(ref _tipoSolicitado, value);
        }

        [Column(IsIgnore = true)]
        public List<string>? GenerosLista { get; set; }

        [Column(IsIgnore = true)]
        public List<string>? PaisesOrigem { get; set; }

        [Column(IsIgnore = true)]
        public List<string>? ProdutorasLista { get; set; }

        [Column(IsIgnore = true)]
        public int Episodios { get; set; }

        [Column(IsIgnore = true)]
        public int DuracaoMedia { get; set; }

        [Column(IsIgnore = true)]
        public double Popularidade { get; set; }

        [Column(IsIgnore = true)]
        public int Votos { get; set; }

        [Column(IsIgnore = true)]
        public bool EhAnimacao =>
            GenerosLista?.Any(g =>
                g.Contains("Animation", StringComparison.OrdinalIgnoreCase) ||
                g.Contains("Animação", StringComparison.OrdinalIgnoreCase)) == true;

        [Column(IsIgnore = true)]
        public bool EhJapones => IdiomaOriginal?.Equals("ja", StringComparison.OrdinalIgnoreCase) == true;

        [Column(IsIgnore = true)]
        public bool EhCoreano => IdiomaOriginal?.Equals("ko", StringComparison.OrdinalIgnoreCase) == true;

        [Column(IsIgnore = true)]
        public bool EhChines => IdiomaOriginal?.Equals("zh", StringComparison.OrdinalIgnoreCase) == true;

        [Column(IsIgnore = true)]
        public string? TituloFinal
        {
            get => _tituloFinal;
            set => SetField(ref _tituloFinal, value);
        }
        public string NomeFormatado
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Nome)) return string.Empty;
                return TagEngine.FormatarTitulo(Nome);
            }
        }
        public string TituloResolvido =>
            string.IsNullOrWhiteSpace(TituloFinal) ? NomeFormatado : TituloFinal!;

        public string MidiaResolvida =>
            string.IsNullOrWhiteSpace(Midia) ? (Nome ?? "") : Midia!;
    }
}
