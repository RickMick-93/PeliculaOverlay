using System.Drawing;

namespace VisionGlass
{
    /// <summary>
    /// Estrutura que representa um bloco de texto traduzido e sua posição na tela.
    /// </summary>
    public class VG_Texto_Traduzido
    {
        public string TextoOriginal { get; set; } = string.Empty;
        public string TextoTraduzido { get; set; } = string.Empty;
        public Rectangle Regiao { get; set; }
        public bool UsarRegra80PorCento { get; set; } // Ativado apenas em contextos específicos (Ex: Minecraft)
        public string IdiomaDetectado { get; set; } = ""; // Para filtragem posterior

        public VG_Texto_Traduzido(string original, string traduzido, Rectangle regiao, bool regra80 = false)
        {
            TextoOriginal = original;
            TextoTraduzido = traduzido;
            Regiao = regiao;
            UsarRegra80PorCento = regra80;
        }
    }
}
