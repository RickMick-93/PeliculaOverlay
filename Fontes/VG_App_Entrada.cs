using System;
using System.Windows.Forms;

namespace VisionGlass
{
    internal static class VG_App_Entrada
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            try
            {
                // --- PASSO 1: INICIAR O GERENCIADOR GERAL ---
                // O Gerenciador se encarrega de criar o Vidro, as Bordas e o Monitor OCR
                var gerenciador = new VG_Gerenciador_Geral();
                gerenciador.Start();

                // --- PASSO 2: MANTER APLICAÇÃO RODANDO ---
                Application.Run();

                // --- PASSO 3: LIMPEZA AO FECHAR ---
                gerenciador.Stop();
            }
            catch (Exception)
            {
                // O erro já está sendo logado no arquivo pelo LanguageDetector
            }
        }
    }
}