using System.Text.RegularExpressions;

namespace IECE_WebApi.Helpers
{
    public class ManejoDeApostrofes
    {
        private static string VocalSinApostrofe(string vocalConApostrofe)
        {
            string vocalSimple = "";

            switch (vocalConApostrofe)
            {
                case "Á":
                    vocalSimple = "A";
                    break;
                case "É":
                    vocalSimple = "E";
                    break;
                case "Í":
                    vocalSimple = "I";
                    break;
                case "Ó":
                    vocalSimple = "O";
                    break;
                case "Ú":
                    vocalSimple = "U";
                    break;
                default:
                    break;
            }

            return vocalSimple;
        }
        public static string QuitarApostrofe(string textoConApostrofe)
        {
            // Crea la expresión regular
            string pattern = @"[ÁÉÍÓÚ]";
            Regex rg = new Regex(pattern);

            // Detecta la vocal acentuada
            string valueApostrofe = rg.Match(textoConApostrofe).Value;

            // Remplaza la vocal sin acento y retorna la cadena
            var resultado = rg.Replace(textoConApostrofe, VocalSinApostrofe(valueApostrofe));
            return resultado;
        }

        public static string QuitarApostrofe2(string textoConApostrofes)
        {
            char[] characters = textoConApostrofes.ToCharArray();
            string textoSinApostrofes = "";
            foreach (char letra in characters)
            {
                string let = "";
                switch (letra.ToString())
                {
                    case "Á":
                        let = "A";
                        break;
                    case "É":
                        let = "E";
                        break;
                    case "Í":
                        let = "I";
                        break;
                    case "Ó":
                        let = "O";
                        break;
                    case "Ú":
                        let = "U";
                        break;
                    default:
                        let = letra.ToString();
                        break;
                }
                textoSinApostrofes = textoSinApostrofes + let;
            }
            return textoSinApostrofes;
        }
    }
}
