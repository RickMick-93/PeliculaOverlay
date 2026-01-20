using System;
using System.Drawing; // Resolve erros de Point e Rectangle
using System.Windows.Forms; // Resolve erros de Timer e Cursor

namespace PeliculaOverlay // DEVE ser igual ao do OverlayManager
{
    public class ForeignTextMonitor
    {
        private Timer _mouseTimer;
        private Point _lastMousePosition;
        private int _secondsIdle = 0;

        public ForeignTextMonitor()
        {
            _mouseTimer = new Timer();
            _mouseTimer.Interval = 100;
            _mouseTimer.Tick += MouseTimer_Tick;
            _mouseTimer.Start();
        }

        private void MouseTimer_Tick(object sender, EventArgs e)
        {
            Point currentPosition = Cursor.Position;

            if (currentPosition != _lastMousePosition)
            {
                _lastMousePosition = currentPosition;
                _secondsIdle = 0;
            }
            else
            {
                _secondsIdle += 100;
            }

            if (_secondsIdle == 1000)
            {
                IniciarCapturaDeTexto();
            }
        }

        private void IniciarCapturaDeTexto()
        {
            // Apenas um aviso no console por enquanto
            Console.WriteLine("🎯 Mouse parado: Gatilho de 1 segundo ativado.");
        }
    }
}