using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace VisionGlass
{
    public class VG_Motor_Idiomas
    {
        // Memória para guardar as traduções e não pesar no PC
        private Dictionary<string, string> dicionarioMemoria = new Dictionary<string, string>();
        private string processoAtual = "";
        private string idiomaSistema;
        private bool contextoMinecraft = false;

        public bool EhContextoMinecraft => contextoMinecraft;

        // Comandos para o Windows nos dizer qual programa está aberto
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        public VG_Motor_Idiomas()
        {
            // Identifica o idioma do seu Windows (ex: pt-BR)
            idiomaSistema = System.Globalization.CultureInfo.CurrentUICulture.Name;
            Console.WriteLine($"VG [MOTOR]: Sistema em {idiomaSistema}. Textos em PT serão ignorados.");
        }

        // Dicionário de fallback para termos técnicos universais (Pilar Universalidade)
        private Dictionary<string, string> fallbackUniversal = new Dictionary<string, string>()
        {
            {"step id", "ID do Passo"},
            {"running", "Executando"},
            {"open", "Abrir"},
            {"close", "Fechar"},
            {"implementation plan", "Plano de Implementação"},
            {"proceed", "Prosseguir"},
            {"task", "Tarefa"},
            {"status", "Estado"},
            {"debugging", "Depurando"},
            {"logs", "Registros"},
            {"output", "Saída"},
            {"warning", "Atenção"},
            {"error", "Erro"},
            {"settings", "Configurações"},
            {"code", "Código"},
            {"file", "Arquivo"},
            {"edit", "Editar"},
            {"view", "Ver"},
            {"selection", "Seleção"},
            {"terminal", "Terminal"},
            {"help", "Ajuda"},
            {"about", "Sobre"},
            {"review", "Revisão"},
            {"plan", "Plano"},
            {"saneamento", "Saneamento"},
            {"c#", "C#"},
            {"component", "Componente"}
        };

        public string TraduzirTexto(string textoOriginal, out bool deveUsar80)
        {
            AtualizarContextoDeJogo();
            deveUsar80 = contextoMinecraft;

            string busca = textoOriginal.Trim();
            
            // 1. FILTRO DE RUÍDO: Ignora palavras muito curtas (menos de 3 letras), apenas números ou formatos de data/hora
            if (string.IsNullOrWhiteSpace(busca) || busca.Length < 3) return textoOriginal;
            if (System.Text.RegularExpressions.Regex.IsMatch(busca, @"^[0-9\W\:]+$")) return textoOriginal;

            // 2. FILTRO DE IDIOMA NATIVO (Ignora PT-BR óbvio)
            if (EhIdiomaNativo(busca)) return textoOriginal;

            string buscaLower = busca.ToLower();

            // 3. Busca no dicionário carregado (Jogo/Contexto)
            if (dicionarioMemoria.ContainsKey(buscaLower)) return dicionarioMemoria[buscaLower];

            // 4. Fallback Universal (Sistema/Comum)
            if (fallbackUniversal.ContainsKey(buscaLower)) return fallbackUniversal[buscaLower];

            // 5. Se chegou aqui e passou pelo filtro, e não está nos dicionários conhecidos, 
            // marcamos como estrangeiro para disparar o grifo.
            return "[FOREIGN]:" + textoOriginal; 
        }

        private bool EhIdiomaNativo(string texto)
        {
            // Se contém acentos típicos, é Português puro.
            if (System.Text.RegularExpressions.Regex.IsMatch(texto, @"[áàâãéèêíïóôõöúçÁÀÂÃÉÈÊÍÏÓÔÕÖÚÇ]")) return true;

            string t = texto.ToLower();
            // Lista de termos de interface/sistema (PT) para evitar grifos indevidos
            string[] proibidas = { 
                "arquivo", "editar", "seleção", "ajuda", "executar", "terminal", "janela", "plano", "tarefas", 
                "sistema", "detecção", "grifo", "vidro", "borda", "limpa", "abrir", "fechar", "novo", 
                "configurar", "configurações", "revisão", "alterações", "pesquisar", "iniciar", "explorador",
                "estrutura", "linha", "tempo", "ativa", "ativo", "pronto", "sucesso", "modo", "por", "ptb",
                "area", "central", "criando", "janela", "tamanho", "aplicando", "topo", "registrada", "visivel",
                "ciano", "mantenedor", "sucesso", "integração", "passivo", "cliques", "passam", "aguardando", "mudança",
                "main*o", "main", "gemini", "flash", "cancel", "collapse", "all", "review", "changes", "settings", "antigravity"
            };
            
            foreach(var p in proibidas) {
                if (t == p) return true;
                // Bloqueia prefixos comuns em PT para palavras longas
                if (t.Length > 4 && p.Length > 4 && p.StartsWith(t)) return true;
            }

            // Se contém caracteres não-ASCII (Japonês, etc), NÃO é nativo do teclado BR padrão.
            if (System.Text.RegularExpressions.Regex.IsMatch(texto, @"[^\x00-\x7F]")) return false;

            return false; 
        }

        private void AtualizarContextoDeJogo()
        {
            string nomeProcesso = ObterNomeProcessoAtivo();

            // Só carrega o dicionário se você mudar de jogo/janela
            if (nomeProcesso != processoAtual)
            {
                processoAtual = nomeProcesso;
                contextoMinecraft = nomeProcesso.ToLower().Contains("minecraft") || nomeProcesso.ToLower().Contains("javaw");
                CarregarDicionarios(nomeProcesso);
                Console.WriteLine($"VG [CONTEXTO]: {nomeProcesso} (Minecraft: {contextoMinecraft})");
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