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
                // --- PASSO 1: DETECTAR IDIOMA DO SISTEMA ---
                LanguageDetector.Initialize();
                LanguageDetector.CleanOldLogs();

                // --- PASSO 2: INICIAR O VIGIA DO MOUSE ---
                // Adicionamos esta linha para o sensor começar a contar o tempo de repouso
                var monitor = new ForeignTextMonitor();

                // --- PASSO 3: INICIAR OVERLAY BASE ---
                var overlay = new OverlayManager();
                overlay.Start();

                // --- PASSO 4: MANTER APLICAÇÃO RODANDO ---
                Application.Run();

                // --- PASSO 5: LIMPEZA AO FECHAR ---
                overlay.Stop();
            }
            catch (Exception)
            {
                // O erro já está sendo logado no arquivo pelo LanguageDetector
            }
        }
    }
}