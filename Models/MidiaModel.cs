using FreeSql.DataAnnotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Telinha.Models
{
    [Table(Name = "midia")]
    public class MidiaModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            return true;
        }

        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }

        private string? _codigo;
        public string? Codigo
        {
            get => _codigo;
            set => SetField(ref _codigo, value);
        }
        private string? _nome;
        public string? Nome
        {
            get => _nome;
            set => SetField(ref _nome, value);
        }
        private string? _audio;
        public string? Audio
        {
            get => _audio;
            set => SetField(ref _audio, value);
        }
        private string? _tipo;
        public string? Tipo
        {
            get => _tipo;
            set => SetField(ref _tipo, value);
        }
        private string? _sinopse;
        public string? Sinopse
        {
            get => _sinopse;
            set => SetField(ref _sinopse, value);
        }
        private string? _original;
        public string? Original
        {
            get => _original;
            set => SetField(ref _original, value);
        }
        private string? _lancamento;
        public string? Lancamento
        {
            get => _lancamento;
            set => SetField(ref _lancamento, value);
        }
        private string? _alternativo;
        public string? Alternativo
        {
            get => _alternativo;
            set => SetField(ref _alternativo, value);
        }
        private string? _pais;
        public string? Pais
        {
            get => _pais;
            set => SetField(ref _pais, value);
        }
        private string? _idioma;
        public string? Idioma
        {
            get => _idioma;
            set => SetField(ref _idioma, value);
        }
        private string? _serie;
        public string? Serie
        {
            get => _serie;
            set => SetField(ref _serie, value);
        }
        private string? _franquia;
        public string? Franquia
        {
            get => _franquia;
            set => SetField(ref _franquia, value);
        }
        private string? _autores;
        public string? Autores
        {
            get => _autores;
            set => SetField(ref _autores, value);
        }
        private string? _criadores;
        public string? Criadores
        {
            get => _criadores;
            set => SetField(ref _criadores, value);
        }
        private string? _obra;
        public string? Obra
        {
            get => _obra;
            set => SetField(ref _obra, value);
        }
        private string? _genero;
        public string? Genero
        {
            get => _genero;
            set => SetField(ref _genero, value);
        }
        private string? _tags;
        public string? Tags
        {
            get => _tags;
            set => SetField(ref _tags, value);
        }
        private string? _diretor;
        public string? Diretor
        {
            get => _diretor;
            set => SetField(ref _diretor, value);
        }
        private string? _mcu;
        public string? MCU
        {
            get => _mcu;
            set => SetField(ref _mcu, value);
        }
        private string? _artistas;
        public string? Artistas
        {
            get => _artistas;
            set => SetField(ref _artistas, value);
        }
        private string? _produtora;
        public string? Produtora
        {
            get => _produtora;
            set => SetField(ref _produtora, value);
        }
        private string? _tipoSolicitado;
        public string? TipoSolicitado
        {
            get => _tipoSolicitado;
            set => SetField(ref _tipoSolicitado, value);
        }
    }
}
