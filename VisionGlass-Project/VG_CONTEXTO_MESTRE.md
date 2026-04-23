# VisionGlass - Contexto e Propósito Mestre

Este documento registra a essência, a origem e as regras inegociáveis do projeto VisionGlass para garantir que toda evolução futura respeite a visão original do criador.

## 🌟 Origem e Propósito
O VisionGlass nasceu de uma necessidade familiar: permitir que o criador jogue **Minecraft Bedrock** com seus filhos e netas em diferentes dispositivos (PC, Mac, Android, iOS). 

Embora o jogo esteja em português, muitos "addons" (como o *Storage Drawer*) possuem partes bloqueadas em inglês (nomenclaturas internas). O VisionGlass atua como a ponte final para uma imersão completa em português, servindo primeiro à família e, futuramente, a todos os jogadores que enfrentam barreiras linguísticas.

## 🛠️ O Conceito de "Mimetismo Visual"
Diferente de tradutores comuns que apenas sobrepõe texto, o VisionGlass utiliza uma técnica de **Substituição Camaleônica**:
1. **Legenda de Filme**: O sistema deve cobrir o texto estrangeiro e escrever a tradução por cima.
2. **Tarja de Cobertura**: Deve ser desenhada uma tarja com a **mesma cor de fundo** do texto original.
3. **Regra dos 80%**: A tarja deve cobrir apenas **80% da largura** do elemento (ex: um botão). Isso permite ver a mudança de cor nas bordas (foco/hover) quando o cursor passa por cima, mantendo o feedback visual do jogo.
4. **Fidelidade Visual**: A tradução deve usar, sempre que possível, a **mesma cor e fonte** do texto original estrangeiro.

## 🧬 OCR Avançado e Repintura de Fundo (Inspiração: FrankYomik)
Além da substituição tradicional por tarjas sólidas, o VisionGlass tem como **Meta Arquitetural de Longo Prazo** implementar a técnica de *Inpainting* vista nos tradutores avançados de Manga do Fabio Akita (projeto de referência `FrankYomik`).
- Se o texto analisado estiver sobre uma **textura ou fundo transparente** (como um HUD de Minecraft complexo ou uma placa de madeira), o sistema deve ler o texto, **recriar/repintar a textura que estava atrás** dele usando IA (inpainting) e inserir no espaço original a tradução devidamente tipografada. Isso eleva o mimetismo de "básico" para "perfeição visual".

## 🕹️ Gatilhos e Lógica
- **O Cursor é um Cronômetro**: O mouse não indica "o que" traduzir. Ele serve apenas como um gatilho global. Se o cursor parar de se mover (repouso) por 1 segundo, o sistema inicia a varredura da tela toda.
- **Identificação de Idioma e App**: O sistema identifica o idioma do Sistema Operacional (SO) e trata qualquer texto diferente como "estrangeiro". Além disso, identifica o processo ativo para carregar o dicionário correspondente.
- **Prioridade .Lang e .Json**: Antes de usar tradução automática, o sistema busca na pasta `Dictionaries/[NomeDoProcesso]` o arquivo de idioma correspondente (ex: `pt_br`).
    - **Minecraft Bedrock**: Utiliza arquivos `.lang`.
    - **Minecraft Java**: Utiliza arquivos `.json`.
    - **Universalidade**: Para qualquer outro aplicativo, o sistema busca na pasta de dicionários pelo nome do executável, permitindo suporte a qualquer software através de mapeamento manual ou tradução OCR dinâmica.

## 🚀 Visão de Futuro
1. **Pilar Windows**: Consolidação da versão desktop.
2. **Multiplataforma**: Portabilidade para Android (Tablets), macOS e iOS.
3. **Universalidade**: De Minecraft para todos os jogos e, eventualmente, para qualquer aplicação.

---
*Este documento deve ser lido por qualquer IA ou desenvolvedor antes de sugerir alterações na lógica visual ou de gatilhos do projeto.*
