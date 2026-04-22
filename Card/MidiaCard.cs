using System.Text;
using Telinha.Enums;

namespace Telinha.Card
{
    public class MidiaCard(
        MidiaTipo tipo,
        string titulo,
        string audio = "",
        string sinopse = "",
        string original = "",
        string lancamento = "",
        string alternativo = "",
        string pais = "",
        string idioma = "",
        string franquia = "",
        string genero = "",
        string tags = "",
        string diretor = "",
        string artistas = "",
        string produtora = "",
        string mcu = "",
        string autores = "",
        string criadores = "",
        string obra = "")
    {
        private readonly MidiaTipo _tipo = tipo;
        private readonly string _titulo = titulo ?? "";
        private readonly string _audio = audio ?? "";
        private readonly string _sinopse = sinopse ?? "";
        private readonly string _original = original ?? "";
        private readonly string _lancamento = lancamento ?? "";
        private readonly string _alternativo = alternativo ?? "";
        private readonly string _pais = pais ?? "";
        private readonly string _idioma = idioma ?? "";
        private readonly string _franquia = franquia ?? "";
        private readonly string _genero = genero ?? "";
        private readonly string _tags = tags ?? "";
        private readonly string _diretor = diretor ?? "";
        private readonly string _artistas = artistas ?? "";
        private readonly string _produtora = produtora ?? "";
        private readonly string _mcu = mcu ?? "";
        private readonly string _autores = autores ?? "";
        private readonly string _criadores = criadores ?? "";
        private readonly string _obra = obra ?? "";

        public string GetFormattedText()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"**{_titulo} - {_audio}**" + Environment.NewLine);

            sb.AppendLine("**HD** - __720p__");
            sb.AppendLine("**SD** - __480p__");
            sb.AppendLine("(__Os vídeos estão em ordem crescente, ou seja, de cima para baixo, tal como na descrição das resoluções.__)" + Environment.NewLine);

            sb.AppendLine($"**Sinopse:** __{_sinopse}__" + Environment.NewLine);

            // Bloco de Identidade
            sb.AppendLine($"**Nome Original:** __{_original}__");
            sb.AppendLine($"**Nome alternativo:** __{_alternativo}__");
            sb.AppendLine($"**Data de lançamento:** __{_lancamento}__");

            // Bloco de Origem (Só para Séries e Animes)
            if (_tipo != MidiaTipo.Filme)
            {
                sb.AppendLine($"**Países de Origem:** {_pais}");
                sb.AppendLine($"**Idioma Original:** {_idioma}");
            }

            // Bloco Dinâmico
            string labelTipo = _tipo switch
            {
                MidiaTipo.Filme => "Filme",
                MidiaTipo.Serie => "Série",
                _ => "Anime"
            };

            sb.AppendLine($"**{labelTipo}:** __{_titulo}__");
            sb.AppendLine($"**Franquia:** {_franquia}");

            // Bloco de Autoria (Só Séries e Animes)
            if (_tipo != MidiaTipo.Filme)
            {
                sb.AppendLine($"**Autores:** {_autores}");
                sb.AppendLine($"**Criadores:** {_criadores}");
                sb.AppendLine($"**Obra Original:** __{_obra}__");
            }

            // Bloco Final Comum
            sb.AppendLine($"**Gênero:** {_genero}");
            sb.AppendLine($"**Tags:** {_tags}");
            sb.AppendLine($"**Diretor:** {_diretor}");

            // Bloco MCU (Filme e Série)
            if (_tipo != MidiaTipo.Anime)
            {
                sb.AppendLine($"**Fase MCU:** {_mcu}");
            }

            sb.AppendLine($"**Artistas:** {_artistas}");
            sb.AppendLine($"**Produtora:** {_produtora}");

            return sb.ToString();
        }
    }
}