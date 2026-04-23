# VISIONGLASS - MANUAL DE INSTRUÇÕES MESTRE

Este documento é a autoridade máxima de conduta e técnica. Deve ser lido integralmente por qualquer IA que interaja com este projeto.

## 🤝 DIRETRIZES DE TRABALHO (GLOBAIS)
1. **Idioma**: Utilizar exclusivamente Português do Brasil.
2. **Método "Um Passo por Vez"**: Nunca avançar para implementação sem confirmação explícita da lógica anterior.
3. **Comunicação Didática**: Explicar conceitos técnicos de forma simples, sem jargões complexos ou termos em inglês puro.
4. **Segurança de Código**: Explicar a teoria do que será feito ANTES de apresentar qualquer script.
5. **Registro de Conquistas**: Ao final de cada etapa bem-sucedida, atualizar o documento de Estado Atual.

## 🕶️ FILOSOFIA DO PROJETO (ESPECÍFICAS)
- **Visão Global**: Este não é apenas um mod de Minecraft, mas um tradutor universal multiplataforma (Windows, Android, macOS) e comercial.
- **O App é um Fantasma**: Deve ser invisível e silencioso (Alpha 15).
- **Interação Passiva**: O mouse não seleciona texto; ele funciona como um "gatilho de repouso" (Pedal de 1 segundo). **IMPORTANTE**: O repouso do mouse em QUALQUER lugar da tela dispara a tradução da tela inteira.
- **Detecção Automática de Idioma**: O app deve detectar o idioma principal do Sistema Operacional. Qualquer texto em idioma diferente deve ser tratado como "estrangeiro" e traduzido para o idioma do usuário.
- **Sem Interceção**: O sistema deve ser totalmente "click-through", permitindo jogar normalmente através do vidro.

## 🛠️ PILARES TÉCNICOS
- **Linguagem**: C# (.NET / Windows Forms).
- **OCR**: Uso obrigatório de `Windows.Media.Ocr` (Nativo do Windows).
- **Hierarquia de Idioma**: 1º Dicionários Locais (.lang/.json) -> 2º Tradutor Nativo.
- **Renderização Adaptativa**: 
    - Botoes: Tarja cinza (80% largura).
    - Diálogos: Legendas amarelas com tempo de leitura.
    - Blocos: Box camuflado (Cores originais).

---
*Este manual substitui e anula os arquivos 00-PROMPT_INICIAL.md e COPILOT_INIT.md.*
