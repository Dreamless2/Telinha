using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Telinha.Enums;

namespace Telinha.Entity
{
    public class DetectarTipo
    {
        public static MidiaTipo Detectar(JObject json)
        {
            // Verifica campos específicos para determinar o tipo
            if (json["first_air_date"] != null || json["name"] != null)
            {
                // Pode ser Série ou Anime, vamos diferenciar pelo gênero
                var genres = json["genres"]?.Select(g => g["name"]?.ToString().ToLower()).ToList() ?? [];
                if (genres.Contains("anime") || genres.Contains("animation"))
                {
                    return MidiaTipo.Anime;
                }
                else
                {
                    return MidiaTipo.Serie;
                }
            }
            else if (json["release_date"] != null || json["title"] != null)
            {
                return MidiaTipo.Filme;
            }
            // Tipo desconhecido, pode lançar exceção ou retornar um valor padrão
            throw new Exception("Tipo de mídia desconhecido");
        }
    }
}