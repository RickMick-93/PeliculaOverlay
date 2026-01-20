using System;
using System.Windows.Forms;
using VisionGlass.Monitors;
using Timer = System.Windows.Forms.Timer;

namespace PeliculaOverlay
{
    public class OverlayManager : IDisposable
    {
        private TransparentWindow glassWindow;
        private BorderWindow borderWindow;
        private ForeignTextMonitor _textMonitor;
        private Timer zOrderTimer;

        public OverlayManager()
        {
            _textMonitor = new ForeignTextMonitor();
            Console.WriteLine("📦 OverlayManager criado (modo tradutor)");
        }

        public void Start()
        {
            Console.WriteLine("🚀 Iniciando sistema overlay...");

            try
            {
                // 1. JANELA PRINCIPAL TRANSPARENTE (VIDRO)
                Console.WriteLine("   Criando janela vidro...");
                glassWindow = new TransparentWindow();
                glassWindow.Show();
                Console.WriteLine("   ✅ Vidro transparente ativo (alpha=15)");

                // 2. JANELA DE BORDAS VISUAIS
                Console.WriteLine("   Criando bordas visuais...");
                borderWindow = new BorderWindow();
                borderWindow.Show();

                // Sincronizar com o vidro
                borderWindow.SyncWithParent(glassWindow);
                borderWindow.Location = glassWindow.Location;
                borderWindow.Size = glassWindow.Size;
                Console.WriteLine("   ✅ Bordas ciano ativas (4px, #00FFFF)");

                // 3. TIMER PARA MANTER SEMPRE NO TOPO
                Console.WriteLine("   Configurando mantenedor de Z-order...");
                zOrderTimer = new Timer();
                zOrderTimer.Interval = 1000; // Verifica a cada 1 segundo
                zOrderTimer.Tick += (s, e) => MaintainTopMost();
                zOrderTimer.Start();
                Console.WriteLine("   ✅ Mantenedor de topo ativo");

                Console.WriteLine("🎯 Sistema overlay base INICIADO com sucesso!");
                Console.WriteLine("   • Modo: 100% passivo (cliques passam)");
                Console.WriteLine("   • Pronto para integração com sistema OCR");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERRO CRÍTICO ao iniciar overlay: {ex.Message}");
                Stop(); // Garantir limpeza em caso de erro
                throw;
            }
        }

        private void MaintainTopMost()
        {
            try
            {
                // Garantir que ambas as janelas permaneçam sempre no topo
                if (glassWindow != null && !glassWindow.IsDisposed)
                {
                    Win32API.SetWindowPos(
                        glassWindow.Handle,
                        Win32API.HWND_TOPMOST,
                        0, 0, 0, 0,
                        Win32API.SWP_NOMOVE | Win32API.SWP_NOSIZE | Win32API.SWP_NOACTIVATE
                    );
                }

                if (borderWindow != null && !borderWindow.IsDisposed)
                {
                    Win32API.SetWindowPos(
                        borderWindow.Handle,
                        Win32API.HWND_TOPMOST,
                        0, 0, 0, 0,
                        Win32API.SWP_NOMOVE | Win32API.SWP_NOSIZE | Win32API.SWP_NOACTIVATE
                    );
                }
            }
            catch (Exception ex)
            {
                // Log silencioso - não interrompe o funcionamento
                Console.WriteLine($"[DEBUG] Manutenção de Z-order: {ex.Message}");
            }
        }

        public void Stop()
        {
            Console.WriteLine("🔄 Encerrando sistema overlay...");

            // 1. Parar timer
            if (zOrderTimer != null)
            {
                Console.WriteLine("   Parando mantenedor de Z-order...");
                zOrderTimer.Stop();
                zOrderTimer.Dispose();
                zOrderTimer = null;
                Console.WriteLine("   ✅ Timer parado");
            }

            // 2. Fechar bordas
            if (borderWindow != null)
            {
                Console.WriteLine("   Fechando bordas visuais...");
                borderWindow.Close();
                borderWindow.Dispose();
                borderWindow = null;
                Console.WriteLine("   ✅ Bordas fechadas");
            }

            // 3. Fechar vidro
            if (glassWindow != null)
            {
                Console.WriteLine("   Fechando janela vidro...");
                glassWindow.Close();
                glassWindow.Dispose();
                glassWindow = null;
                Console.WriteLine("   ✅ Vidro fechado");
            }

            Console.WriteLine("✅ Sistema overlay ENCERRADO com sucesso!");
        }

        public void Dispose()
        {
            Stop();
        }

        // MÉTODOS OBSOLETOS (REMOVER COMPLETAMENTE NO FUTURO)
        #region Métodos Obsoletos - Remover quando confirmado

        [Obsolete("Controle de transparência removido - valor fixo em alpha=15")]
        public void AdjustTransparency(int delta)
        {
            Console.WriteLine("⚠️ Método obsoleto chamado: AdjustTransparency");
            Console.WriteLine("   Transparência é FIXA em alpha=15 (6% visível)");
            Console.WriteLine("   Use a janela vidro diretamente se necessário");
        }

        [Obsolete("Propriedade mantida apenas para compatibilidade")]
        public int GetCurrentTransparency()
        {
            return 15; // Valor fixo
        }

        #endregion
    }
}