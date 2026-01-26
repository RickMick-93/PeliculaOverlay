PROXIMA-TAREFA - VISIONGLASS
STATUS ATUAL: DOCUMENTACAO CONCLUIDA
Regras (VG_RULES.md) OK.

Projeto (VG_MAPA_GERAL.md) OK.

Conquistas (VG_ESTADO_ATUAL.md) OK.

TAREFA IMEDIATA: PROGRAMACAO DO MONITOR DE TEXTO
O objetivo agora e ajustar o arquivo "VG_Monitor_OCR.cs" para seguir as novas regras de inteligencia.

PASSOS DA TAREFA:
IMPLEMENTAR O GATILHO GLOBAL (PEDAL)

Criar o cronometro de 1000ms (1 segundo).

Configurar para que, se o mouse parar em qualquer lugar, o app dispare a captura de tela (OCR).

CONFIGURAR O IDIOMA ANCORA

Adicionar o codigo que consulta o idioma do sistema (C# CultureInfo.CurrentCulture).

Definir que este sera o idioma "alvo" da traducao.

LOGICA DE TIPO DE TEXTO (AS TRES ROUPAS)

No codigo de desenho (Renderer), criar as tres condicoes:

Se for Botao: Criar Tarja Cinza (80% largura).

Se for Dialogo: Criar Legenda Amarela com Fundo Translucido.

Se for Bloco: Criar Box Camuflado (cor de fundo e cor de texto).

CRIAR O SISTEMA DE DICIONARIOS (NIVEL 1)

Criar a rotina que busca na pasta "Dictionaries/NomeDoJogo/idioma.lang" antes de chamar a API.

AJUSTAR O DESACOPLAMENTO DO MOUSE

Garantir que, apos o gatilho de 1 segundo, a rotina de OCR continue rodando baseada na imagem da tela e nao na posicao do mouse.

APLICAR O DELAY DE LEITURA

Implementar o temporizador de seguranca para legendas de dialogos.