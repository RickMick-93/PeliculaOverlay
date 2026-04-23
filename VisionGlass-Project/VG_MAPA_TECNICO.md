# VISIONGLASS - MAPA TÉCNICO E ESTADO REAL

Este arquivo descreve como o projeto está organizado e qual é o progresso real de cada componente, corrigindo "alucinações" de documentações anteriores.

## 📁 ESTRUTURA DE ARQUIVOS (`/Fontes`)
1. **VG_App_Entrada.cs**: Ponto de partida. Inicializa o aplicativo silenciosamente. Namespace unificado: `VisionGlass`.
2. **VG_Gerenciador_Geral.cs**: O "maestro". Coordena as janelas, o monitor OCR e o motor de idiomas. Mantém o Z-order (sempre no topo).
3. **VG_Interface_Pelicula.cs**: A tela de "vidro". Alpha fixo em 15 (6%). Totalmente "click-through". Preparada para receber desenhos de tradução no `OnPaint`.
4. **VG_Interface_Borda.cs**: Renderiza a moldura ciano de 4px usando `TransparencyKey` (Magenta) e regiões de exclusão para cliques passarem.
5. **VG_Monitor_OCR.cs**: O sensor do "Pedal de 1 segundo". Detecta pausa do mouse e mudanças na tela (via amostragem 10x10). **ESTADO: Lógica de gatilho operacional, mas OCR interno ainda é um placeholder.**
6. **VG_Motor_Idiomas.cs**: Cérebro linguístico. Carrega dicionários locais (.json/.lang) baseados no processo ativo (ex: Minecraft). Operacional.
7. **VG_Sistema_Win32.cs**: Wrapper para APIs do Windows (User32.dll) para controle de janelas e estilos.
7.- **Gatilho de Repouso**: Detecta pausa do mouse e mudanças na tela. A amostragem agora deve considerar a detecção automática do idioma do sistema via `CultureInfo`.

## 🏗️ ARQUITETURA UNIVERSAL (VISÃO FUTURA)
O projeto será estruturado para separar a **Lógica de Processamento** (OCR e Tradução) da **Interface de Usuário** (Pelicula/Windows Forms). Isso permitirá que o "Cérebro" do VisionGlass seja futuramente portado para:
- **Android**: Para tablets (ex: Redmi Pad SE) usando .NET MAUI ou Nativo.
- **macOS**: Para MacMini usando .NET MAUI ou SwiftUI.

## 🛠️ PILARES TÉCNICOS ATUALIZADOS
- **Linguagem Principal**: C# (.NET 10).
- **Detecção de Idioma**: Uso de `System.Globalization.CultureInfo` para identificar o idioma nativo do usuário e tratar o resto como estrangeiro.
- **OCR**: `Windows.Media.Ocr` (Windows).

## 🩺 DIAGNÓSTICO ATUAL (AUDITORIA ANTIGRAVITY)
- **Namespace**: **CONCLUÍDO.** Todos os arquivos utilizam `VisionGlass`.
- **PROJETO**: Configurado para `.NET 10.0` e `Windows 10.0.19041.0`. Suporte para OCR nativo garantido.
- **OCR**: Gatilho de repouso (1s) e detecção de mudança de tela (amostragem) funcionais. Falta implementar a chamada real ao `Windows.Media.Ocr`.
- **Renderização**: Estrutura `OnPaint` pronta na Película para receber as tarjas/legendas.

## 🎯 PRÓXIMA TAREFA IMEDIATA
Implementar a captura de tela real em alta resolução e a integração com o `Windows.Media.Ocr` no arquivo `VG_Monitor_OCR.cs`.
