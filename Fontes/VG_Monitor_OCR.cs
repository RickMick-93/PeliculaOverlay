using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using VisionGlass.Fontes;

namespace VisionGlass.Monitores
{
    public class VG_Monitor_OCR
    {
        // Correção do erro CS0104: Especificando que é o Timer do Windows Forms
        private System.Windows.Forms.Timer timerSistema;
        private Point ultimaPosicaoMouse;
        private Bitmap? ultimaCapturaTela; // Interrogação corrige o aviso CS8618
        private VG_Motor_Idiomas motorTradutor;
        private bool aguardandoMudancaDeTela = false;

        public VG_Monitor_OCR()
        {
            motorTradutor = new VG_Motor_Idiomas();
            timerSistema = new System.Windows.Forms.Timer();
            timerSistema.Interval = 1000; // 1 segundo
            timerSistema.Tick += GerenciadorCiclo;
            timerSistema.Start();
        }

        private void GerenciadorCiclo(object sender, EventArgs e)
        {
            Bitmap telaAtual = CapturarMiniaturaTela();
            bool telaMudou = VerificarSeTelaMudou(telaAtual);

            if (telaMudou)
            {
                // Se a tela mudou (ex: rolou o menu), o VG volta a vigiar o mouse
                aguardandoMudancaDeTela = false;
                ultimaCapturaTela = telaAtual;
            }

            // Se já traduzimos esta tela, ignoramos o mouse completamente
            if (aguardandoMudancaDeTela) return;

            Point posicaoMouseAgora = Cursor.Position;

            // Se o mouse parou por 1 segundo
            if (posicaoMouseAgora == ultimaPosicaoMouse)
            {
                ExecutarVarreduraETraducao();
                // REGRA: Após traduzir, o VG para de vigiar o mouse até a tela mudar
                aguardandoMudancaDeTela = true;
            }

            ultimaPosicaoMouse = posicaoMouseAgora;
        }

        private void ExecutarVarreduraETraducao()
        {
            // O comando para o OCR e para desenhar a película virá aqui
            // Por enquanto, aciona o motor de idiomas
            string resultado = motorTradutor.TraduzirTexto("Texto Detectado");
            Console.WriteLine("VG: " + resultado);
        }

        private bool VerificarSeTelaMudou(Bitmap novaTela)
        {
            if (ultimaCapturaTela == null) return true;

            for (int x = 0; x < novaTela.Width; x++)
            {
                for (int y = 0; y < novaTela.Height; y++)
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
                // Captura uma amostra da tela para detecção de movimento/rolagem
                g.CopyFromScreen(0, 0, 0, 0, bmp.Size);
            }
            return bmp;
        }
    }
}