using FreeSql.DataAnnotations;

namespace Telinha.Models
{
    [Table(Name = "midia")]
    public class MidiaModel
    {
        [Column(IsPrimary = true, IsIdentity = true)]
        public long Id { get; set; }
        public string? Codigo { get; set; }
        public string? Titulo { get; set; }
        public string? Audio { get; set; }
        public string? Tipo { get; set; }
        public string? Sinopse { get; set; }
        public string? Original { get; set; }
        public string? Lancamento { get; set; }
        public string? Alternativo { get; set; }
        public string? Pais { get; set; }
        public string? Idioma { get; set; }
        public string? Serie { get; set; }
        public string? Franquia { get; set; }
        public string? Autores { get; set; }
        public string? Criadores { get; set; }
        public string? Obra { get; set; }
        public string? Genero { get; set; }
        public string? Tags { get; set; }
        public string? Diretor { get; set; }
        public string? MCU { get; set; }
        public string? Artistas { get; set; }
        public string? Estudio { get; set; }
        public string? TipoSolicitado { get; set; }   // "Filme" ou "Serie"
    }
}
