using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace VisionGlass.Fontes
{
    public class VG_Motor_Idiomas
    {
        // Memória para guardar as traduções e não pesar no PC
        private Dictionary<string, string> dicionarioMemoria = new Dictionary<string, string>();
        private string processoAtual = "";
        private string idiomaSistema;

        // Comandos para o Windows nos dizer qual programa está aberto
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public VG_Motor_Idiomas()
        {
            // Identifica o idioma do seu Windows (ex: pt_br)
            idiomaSistema = System.Globalization.CultureInfo.CurrentCulture.Name.ToLower().Replace("-", "_");
        }

        // Esta é a função que o Monitor vai chamar para traduzir
        public string TraduzirTexto(string textoOriginal)
        {
            AtualizarContextoDeJogo();

            // Tenta achar no dicionário do jogo primeiro
            string busca = textoOriginal.Trim().ToLower();
            if (dicionarioMemoria.ContainsKey(busca))
            {
                return dicionarioMemoria[busca];
            }

            // Se não for jogo ou não estiver no dicionário, retorna para tradução livre
            return "[VG]: " + textoOriginal;
        }

        private void AtualizarContextoDeJogo()
        {
            string nomeProcesso = ObterNomeProcessoAtivo();

            // Só carrega o dicionário se você mudar de jogo/janela
            if (nomeProcesso != processoAtual)
            {
                processoAtual = nomeProcesso;
                CarregarDicionarios(nomeProcesso);
            }
        }

        private void CarregarDicionarios(string nomeProcesso)
        {
            dicionarioMemoria.Clear();

            // Caminho para a pasta de dicionários que você criou
            string pastaRaiz = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dictionaries");
            string pastaDoJogo = Path.Combine(pastaRaiz, nomeProcesso);

            // Ajuste especial para identificar o Minecraft Java (javaw) ou Bedrock
            if (nomeProcesso.ToLower().Contains("javaw"))
                pastaDoJogo = Path.Combine(pastaRaiz, "Minecraft_Java");
            else if (nomeProcesso.ToLower().Contains("minecraft"))
                pastaDoJogo = Path.Combine(pastaRaiz, "Minecraft_Bedrock");

            if (Directory.Exists(pastaDoJogo))
            {
                // Procura arquivos pt_br.json ou pt_br.lang
                string[] arquivos = Directory.GetFiles(pastaDoJogo, idiomaSistema + ".*");

                foreach (string arquivo in arquivos)
                {
                    if (arquivo.EndsWith(".json")) LerJson(arquivo);
                    else if (arquivo.EndsWith(".lang")) LerLang(arquivo);
                }
            }
        }

        private void LerLang(string caminho)
        {
            foreach (string linha in File.ReadLines(caminho))
            {
                // Regra: ignora comentários e linhas vazias
                if (string.IsNullOrWhiteSpace(linha) || linha.StartsWith("#")) continue;

                int sinalIgual = linha.IndexOf('=');
                if (sinalIgual > 0)
                {
                    string chave = linha.Substring(0, sinalIgual).Trim().ToLower();
                    string valor = linha.Substring(sinalIgual + 1).Trim();
                    dicionarioMemoria[chave] = valor;
                }
            }
        }

        private void LerJson(string caminho)
        {
            try
            {
                string conteudo = File.ReadAllText(caminho);
                var dados = JsonSerializer.Deserialize<Dictionary<string, string>>(conteudo);
                if (dados != null)
                {
                    foreach (var item in dados)
                        dicionarioMemoria[item.Key.ToLower()] = item.Value;
                }
            }
            catch { /* Ignora erros de leitura */ }
        }

        private string ObterNomeProcessoAtivo()
        {
            IntPtr handle = GetForegroundWindow();
            uint pid;
            GetWindowThreadProcessId(handle, out pid);
            try
            {
                return Process.GetProcessById((int)pid).ProcessName;
            }
            catch { return ""; }
        }
    }
}