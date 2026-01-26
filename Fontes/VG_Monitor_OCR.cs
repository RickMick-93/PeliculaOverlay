using System;
using System.Drawing; // Resolve erros de Point e Rectangle
using System.Windows.Forms; // Resolve erros de Timer e Cursor

namespace PeliculaOverlay // DEVE ser igual ao do OverlayManager
{
    public class VG_Monitor_OCR
    {
        private System.Windows.Forms.Timer _mouseTimer;
        private Point _lastMousePosition;
        private int _secondsIdle = 0;

        public VG_Monitor_OCR()
        {
            _mouseTimer = new System.Windows.Forms.Timer();
            _mouseTimer.Interval = 100;
            _mouseTimer.Tick += MouseTimer_Tick;
            _mouseTimer.Start();
        }

        private void MouseTimer_Tick(object sender, EventArgs e)
        {
            Point currentPosition = Cursor.Position;

            if (currentPosition != _lastMousePosition)
            {
                // Se moveu o mouse, reseta tudo e "limpa" a tela se necessário
                _lastMousePosition = currentPosition;
                _secondsIdle = 0;
            }
            else
            {
                // Se está parado, aumenta o tempo
                _secondsIdle += 100;

                // Quando atingir EXATAMENTE 1 segundo
                if (_secondsIdle == 1000)
                {
                    IniciarCapturaDeTexto();
                }

                // Opcional: Aqui poderíamos colocar uma regra para 
                // re-verificar a cada 5 segundos se o mouse continuar parado
            }
        }

        private void IniciarCapturaDeTexto()
        {
            // Isso faz o aviso pular na tela
            MessageBox.Show("🎯 O VisionGlass detectou que o mouse parou! O sensor está funcionando.", "Teste de Sensor");

            // Zera o tempo para não repetir o aviso sem parar
            _secondsIdle = 0;
        }
    } // Esta chave fecha a Classe
} // Esta chave fecha o Namespace