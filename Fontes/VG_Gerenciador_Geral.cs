using System;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace VisionGlass
{
    public class VG_Gerenciador_Geral : IDisposable
    {
        private VG_Interface_Pelicula? glassWindow;
        private VG_Interface_Borda? borderWindow;
        private VG_Monitor_OCR _textMonitor;
        private Timer? zOrderTimer;
        private const int HOTKEY_ID = 9010;

        public VG_Gerenciador_Geral()
        {
            _textMonitor = new VG_Monitor_OCR();
            Console.WriteLine("📦 OverlayManager criado (modo tradutor)");
        }

        public void Start()
        {
            Console.WriteLine("🚀 Iniciando sistema overlay...");

            try
            {
                // 1. JANELA PRINCIPAL TRANSPARENTE (VIDRO)
                Console.WriteLine("   Criando janela vidro...");
                glassWindow = new VG_Interface_Pelicula();
                glassWindow.Show();
                
                // Conectar o monitor à janela para que ele possa enviar as traduções
                _textMonitor.ConectarInterface(glassWindow);
                _textMonitor.DefinirBorda(borderWindow); // Nova conexão

                // --- NOVO: REGISTRAR TECLA DE PÂNICO (Ctrl + Shift + X) ---
                bool hotkeyRegistrado = VG_Sistema_Win32.RegisterHotKey(
                    glassWindow.Handle, 
                    HOTKEY_ID, 
                    VG_Sistema_Win32.MOD_CONTROL | VG_Sistema_Win32.MOD_SHIFT, 
                    VG_Sistema_Win32.VK_X
                );
                
                if (hotkeyRegistrado)
                {
                    Console.WriteLine("   ✅ Tecla de PÂNICO (Ctrl+Shift+X) registrada.");
                    // Sobrescrever o WNDPROC da janela para capturar o hotkey
                    glassWindow.EscutarTeclaPanico(ProcessarTeclaPanico);
                }
                else
                {
                    Console.WriteLine("   ⚠️ Falha ao registrar Ctrl+Shift+X como pânico.");
                }
                
                Console.WriteLine("   ✅ Vidro transparente ativo (alpha=15)");

                // 2. JANELA DE BORDAS VISUAIS
                Console.WriteLine("   Criando bordas visuais...");
                borderWindow = new VG_Interface_Borda();
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
                    VG_Sistema_Win32.SetWindowPos(
                        glassWindow.Handle,
                        VG_Sistema_Win32.HWND_TOPMOST,
                        0, 0, 0, 0,
                        VG_Sistema_Win32.SWP_NOMOVE | VG_Sistema_Win32.SWP_NOSIZE | VG_Sistema_Win32.SWP_NOACTIVATE
                    );
                }

                if (borderWindow != null && !borderWindow.IsDisposed)
                {
                    VG_Sistema_Win32.SetWindowPos(
                        borderWindow.Handle,
                        VG_Sistema_Win32.HWND_TOPMOST,
                        0, 0, 0, 0,
                        VG_Sistema_Win32.SWP_NOMOVE | VG_Sistema_Win32.SWP_NOSIZE | VG_Sistema_Win32.SWP_NOACTIVATE
                    );
                }
            }
            catch (Exception ex)
            {
                // Log silencioso - não interrompe o funcionamento
                Console.WriteLine($"[DEBUG] Manutenção de Z-order: {ex.Message}");
            }
        }

        private void ProcessarTeclaPanico()
        {
            Console.WriteLine("\n🚨 PÂNICO DETECTADO! Encerrando VisionGlass IMEDIATAMENTE...");
            // Usamos Environment.Exit(0) para garantir que tudo morra na hora
            Environment.Exit(0);
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