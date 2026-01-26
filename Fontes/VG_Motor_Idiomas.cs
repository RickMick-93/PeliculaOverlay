using System;
using System.Globalization;
using System.IO;
using System.Threading;

namespace PeliculaOverlay
{
    /// <summary>
    /// Detecta e gerencia o idioma do sistema para uso no VisionGlass
    /// </summary>
    public static class VG_Motor_Idiomas
    {
        private static string _userLanguage = string.Empty;
        private static readonly string _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "visionglass.log");

        /// <summary>
        /// Idioma nativo do usuário detectado do sistema Windows
        /// </summary>
        public static string UserLanguage => _userLanguage;

        /// <summary>
        /// Inicializa o detector de idioma silenciosamente
        /// </summary>
        public static void Initialize()
        {
            try
            {
                // Criar diretório de logs se não existir
                Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath));

                // Detectar idioma do sistema Windows
                DetectSystemLanguage();

                // Log silencioso (apenas arquivo)
                Log($"VisionGlass iniciado. Idioma nativo detectado: {_userLanguage}");
                Log($"Data/Hora: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                Log($"Diretório: {AppDomain.CurrentDomain.BaseDirectory}");
            }
            catch (Exception ex)
            {
                Log($"Erro na inicialização do LanguageDetector: {ex.Message}");
                // Fallback para inglês em caso de erro
                _userLanguage = "en-US";
            }
        }

        /// <summary>
        /// Detecta o idioma do sistema Windows usando múltiplas fontes
        /// </summary>
        private static void DetectSystemLanguage()
        {
            try
            {
                // Prioridade 1: CultureInfo do thread atual (mais preciso para Windows)
                var currentCulture = CultureInfo.CurrentCulture;

                // Prioridade 2: CultureInfo da UI (para aplicativos com UI específica)
                var uiCulture = CultureInfo.CurrentUICulture;

                // Prioridade 3: Idioma do sistema do Windows
                var systemLanguage = CultureInfo.InstalledUICulture;

                // Log para debugging
                Log($"Cultura atual: {currentCulture.Name} ({currentCulture.EnglishName})");
                Log($"Cultura UI: {uiCulture.Name} ({uiCulture.EnglishName})");
                Log($"Cultura instalada: {systemLanguage.Name} ({systemLanguage.EnglishName})");

                // Heurística: Preferir a cultura atual, mas verificar consistência
                if (!string.IsNullOrEmpty(currentCulture.Name))
                {
                    _userLanguage = currentCulture.Name;
                    Log($"Idioma selecionado (current culture): {_userLanguage}");
                }
                else if (!string.IsNullOrEmpty(uiCulture.Name))
                {
                    _userLanguage = uiCulture.Name;
                    Log($"Idioma selecionado (UI culture): {_userLanguage}");
                }
                else
                {
                    _userLanguage = systemLanguage.Name;
                    Log($"Idioma selecionado (installed culture): {_userLanguage}");
                }

                // Normalizar o código do idioma (ex: pt-BR, en-US, es-ES)
                NormalizeLanguageCode();
            }
            catch (Exception ex)
            {
                Log($"Erro ao detectar idioma: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Normaliza o código do idioma para formato padrão
        /// </summary>
        private static void NormalizeLanguageCode()
        {
            try
            {
                // Exemplos de normalização:
                // "pt" -> "pt-BR" (assumindo Brasil como padrão para português)
                // "en" -> "en-US" (assumindo US como padrão para inglês)
                // "es" -> "es-ES" (assumindo Espanha como padrão para espanhol)

                if (_userLanguage.Length == 2) // Apenas código de idioma (ex: "pt", "en")
                {
                    switch (_userLanguage.ToLower())
                    {
                        case "pt":
                            _userLanguage = "pt-BR";
                            Log($"Idioma normalizado: {_userLanguage}");
                            break;
                        case "en":
                            _userLanguage = "en-US";
                            Log($"Idioma normalizado: {_userLanguage}");
                            break;
                        case "es":
                            _userLanguage = "es-ES";
                            Log($"Idioma normalizado: {_userLanguage}");
                            break;
                        case "fr":
                            _userLanguage = "fr-FR";
                            Log($"Idioma normalizado: {_userLanguage}");
                            break;
                        case "de":
                            _userLanguage = "de-DE";
                            Log($"Idioma normalizado: {_userLanguage}");
                            break;
                        case "it":
                            _userLanguage = "it-IT";
                            Log($"Idioma normalizado: {_userLanguage}");
                            break;
                        case "ja":
                            _userLanguage = "ja-JP";
                            Log($"Idioma normalizado: {_userLanguage}");
                            break;
                        case "ko":
                            _userLanguage = "ko-KR";
                            Log($"Idioma normalizado: {_userLanguage}");
                            break;
                        case "zh":
                            _userLanguage = "zh-CN";
                            Log($"Idioma normalizado: {_userLanguage}");
                            break;
                        case "ru":
                            _userLanguage = "ru-RU";
                            Log($"Idioma normalizado: {_userLanguage}");
                            break;
                            // Adicionar mais casos conforme necessário
                    }
                }
                else if (_userLanguage.Length > 2 && !_userLanguage.Contains("-"))
                {
                    // Formato inválido, tentar corrigir
                    var langCode = _userLanguage.Substring(0, 2).ToLower();
                    NormalizeLanguageCode(); // Recursão com código de 2 letras
                }

                Log($"Idioma final normalizado: {_userLanguage}");
            }
            catch (Exception ex)
            {
                Log($"Erro ao normalizar código de idioma: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifica se um idioma é diferente do idioma nativo do usuário
        /// </summary>
        /// <param name="detectedLanguage">Idioma detectado no texto</param>
        /// <returns>True se for um idioma estrangeiro</returns>
        public static bool IsForeignLanguage(string detectedLanguage)
        {
            if (string.IsNullOrEmpty(detectedLanguage) || string.IsNullOrEmpty(_userLanguage))
                return false;

            try
            {
                // Comparar códigos de idioma principal (ex: "pt" em "pt-BR")
                var userMainLang = _userLanguage.Split('-')[0].ToLower();
                var detectedMainLang = detectedLanguage.Split('-')[0].ToLower();

                bool isForeign = userMainLang != detectedMainLang;

                Log($"Comparação de idiomas: Usuário={_userLanguage}, Detectado={detectedLanguage}, É estrangeiro={isForeign}");

                return isForeign;
            }
            catch (Exception ex)
            {
                Log($"Erro ao comparar idiomas: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Obtém o código do idioma principal (sem região)
        /// </summary>
        public static string GetMainLanguageCode()
        {
            if (string.IsNullOrEmpty(_userLanguage))
                return string.Empty;

            var parts = _userLanguage.Split('-');
            return parts.Length > 0 ? parts[0].ToLower() : _userLanguage.ToLower();
        }

        /// <summary>
        /// Obtém o código da região (se disponível)
        /// </summary>
        public static string GetRegionCode()
        {
            if (string.IsNullOrEmpty(_userLanguage))
                return string.Empty;

            var parts = _userLanguage.Split('-');
            return parts.Length > 1 ? parts[1].ToUpper() : string.Empty;
        }

        /// <summary>
        /// Log silencioso em arquivo (não mostra console)
        /// </summary>
        private static void Log(string message)
        {
            try
            {
                string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] {message}";
                File.AppendAllText(_logFilePath, logMessage + Environment.NewLine);
            }
            catch
            {
                // Silenciosamente ignora erros de log
            }
        }

        /// <summary>
        /// Limpa logs antigos (mantém apenas últimos 7 dias)
        /// </summary>
        public static void CleanOldLogs(int daysToKeep = 7)
        {
            try
            {
                var logDir = Path.GetDirectoryName(_logFilePath);
                if (!Directory.Exists(logDir))
                    return;

                var cutoffDate = DateTime.Now.AddDays(-daysToKeep);
                var logFiles = Directory.GetFiles(logDir, "*.log");

                foreach (var file in logFiles)
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.LastWriteTime < cutoffDate)
                    {
                        File.Delete(file);
                        Log($"Log antigo removido: {file}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"Erro ao limpar logs antigos: {ex.Message}");
            }
        }
    }
}