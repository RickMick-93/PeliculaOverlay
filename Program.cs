using System;
using System.Windows.Forms;

namespace PeliculaOverlay
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            try
            {
                // --- PASSO 1: DETECTAR IDIOMA DO SISTEMA (SILENCIOSO) ---
                // Inicializa o detector de idioma SILENCIOSAMENTE
                LanguageDetector.Initialize();

                // Limpa logs antigos (mantém apenas últimos 7 dias)
                LanguageDetector.CleanOldLogs();

                // 1. INICIAR OVERLAY BASE (SILENCIOSO)
                var overlay = new OverlayManager();
                overlay.Start();

                // 2. MANTER APLICAÇÃO RODANDO (SILENCIOSO)
                Application.Run();

                // 3. LIMPEZA AO FECHAR (SILENCIOSO)
                overlay.Stop();
            }
            catch (Exception)
            {
                // ERRO CRÍTICO - TAMBÉM SILENCIOSO
                // O erro já está sendo logado no arquivo pelo LanguageDetector
            }
        }
    }
}