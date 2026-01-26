using System;
using System.Windows.Forms;

namespace PeliculaOverlay
{
    internal static class VG_App_Entrada
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            try
            {
                // --- PASSO 1: DETECTAR IDIOMA DO SISTEMA ---
                VG_Motor_Idiomas.Initialize();
                VG_Motor_Idiomas.CleanOldLogs();

                // --- PASSO 2: INICIAR O VIGIA DO MOUSE ---
                // Adicionamos esta linha para o sensor começar a contar o tempo de repouso
                var monitor = new VG_Monitor_OCR();

                // --- PASSO 3: INICIAR OVERLAY BASE ---
                var overlay = new VG_Gerenciador_Geral();
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