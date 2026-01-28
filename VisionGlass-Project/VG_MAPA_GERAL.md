using System;
using System.Drawing;
using System.Windows.Forms;
using VisionGlass.Fontes;
using VisionGlass.Interface; // Assumindo que a Película está nesta pasta

namespace VisionGlass.Monitores
{
    public class VG_Monitor_OCR
    {
        private System.Windows.Forms.Timer timerSistema;
        private Point ultimaPosicaoMouse;
        private Bitmap? ultimaCapturaTela;
        private VG_Motor_Idiomas motorTradutor;
        private bool aguardandoMudancaDeTela = false;
        
        // Referência para a película existente
        private VG_Pelicula _pelicula; 

        public VG_Monitor_OCR(VG_Pelicula peliculaExistente)
        {
            _pelicula = peliculaExistente;
            motorTradutor = new VG_Motor_Idiomas();
            
            timerSistema = new System.Windows.Forms.Timer();
            timerSistema.Interval = 1000; 
            timerSistema.Tick += GerenciadorCiclo;
            timerSistema.Start();
        }

        private void GerenciadorCiclo(object sender, EventArgs e)
        {
            Bitmap telaAtual = CapturarMiniaturaTela();
            bool telaMudou = VerificarSeTelaMudou(telaAtual);

            if (telaMudou)
            {
                aguardandoMudancaDeTela = false;
                ultimaCapturaTela = telaAtual;
                // Se a tela mudou, limpamos a película para a próxima tradução
                _pelicula.LimparDesenhos(); 
            }

            if (aguardandoMudancaDeTela) return;

            Point posicaoMouseAgora = Cursor.Position;

            if (posicaoMouseAgora == ultimaPosicaoMouse)
            {
                ExecutarVarreduraETraducao(posicaoMouseAgora);
                aguardandoMudancaDeTela = true;
            }

            ultimaPosicaoMouse = posicaoMouseAgora;
        }

        private void ExecutarVarreduraETraducao(Point pontoReferencia)
        {
            // O motor busca no dicionário do jogo detectado ou faz tradução pura
            string resultado = motorTradutor.TraduzirTexto("Texto_Detectado");

            // Comandamos a Película a desenhar o texto na posição do mouse
            // Futuramente, aqui incluiremos a lógica da tarja
            _pelicula.DesenharTraducao(resultado, pontoReferencia);
        }

        private bool VerificarSeTelaMudou(Bitmap novaTela)
        {
            if (ultimaCapturaTela == null) return true;

            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    if (novaTela.GetPixel(x, y) != ultimaCapturaTela.GetPixel(x, y))
                        return true;
                }
            }
            return false;
        }

        private Bitmap CapturarMiniaturaTela()
        {
            Bitmap bmp = new Bitmap(10, 10);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(0, 0, 0, 0, bmp.Size);
            }
            return bmp;
        }
    }
}